using System.Collections.Specialized;
using System.Linq;
using AiForms.Renderers.iOS;
using Blastic.Forms.Sample.Controls;
using Blastic.Forms.Sample.iOS.Renderers.PickerCellFix;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PickerCellSelectionColorFix), typeof(PickerCellRendererSelectionColorFix))]

namespace Blastic.Forms.Sample.iOS.Renderers.PickerCellFix
{
	public class PickerCellRendererSelectionColorFix : CellBaseRenderer<PickerCellViewSelectionColorFix>
	{
	}

	[Preserve(AllMembers = true)]
	public sealed class PickerCellViewSelectionColorFix : LabelCellView
	{
		private PickerTableViewControllerSelectionColorFix _viewController;
		private INotifyCollectionChanged _notifyCollection;
		private INotifyCollectionChanged _selectedCollection;

		private PickerCellSelectionColorFix PickerCell => (PickerCellSelectionColorFix) Cell;
		
		public PickerCellViewSelectionColorFix(Cell formsCell) : base(formsCell)
		{
			Accessory = UITableViewCellAccessory.DisclosureIndicator;
			EditingAccessory = UITableViewCellAccessory.DisclosureIndicator;
			SelectionStyle = UITableViewCellSelectionStyle.Default;
			
			SetRightMarginZero();
		}
		
		public override void CellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.CellPropertyChanged(sender, e);
			
			if (e.PropertyName == AiForms.Renderers.PickerCell.SelectedItemsProperty.PropertyName ||
				e.PropertyName == AiForms.Renderers.PickerCell.SelectedItemProperty.PropertyName ||
				e.PropertyName == AiForms.Renderers.PickerCell.DisplayMemberProperty.PropertyName ||
				e.PropertyName == AiForms.Renderers.PickerCell.UseNaturalSortProperty.PropertyName ||
				e.PropertyName == AiForms.Renderers.PickerCell.SelectedItemsOrderKeyProperty.PropertyName)
			{
				UpdateSelectedItems();
			}
			
			if (e.PropertyName == AiForms.Renderers.PickerCell.UseAutoValueTextProperty.PropertyName)
			{
				if (PickerCell.UseAutoValueText)
				{
					UpdateSelectedItems();
				}
				else
				{
					UpdateValueText();
				}
			}
			
			if (e.PropertyName == AiForms.Renderers.PickerCell.ItemsSourceProperty.PropertyName)
			{
				UpdateCollectionChanged();
				UpdateSelectedItems();
			}
		}
		
		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			if (PickerCell.ItemsSource == null)
			{
				tableView.DeselectRow(indexPath, true);
				return;
			}

			_viewController?.Dispose();

			UINavigationController naviCtrl = GetUINavigationController(UIApplication.SharedApplication.KeyWindow.RootViewController);

			if (naviCtrl is ShellSectionRenderer shell)
			{
				// When use Shell, the NativeView is wrapped in a Forms.ContentPage.
				_viewController = new PickerTableViewControllerSelectionColorFix(
					this,
					tableView,
					shell.ShellSection.Navigation)
				{
					TableView =
					{
						ContentInset = new UIEdgeInsets(44, 0, 44, 0)
					}
				};

				// Fix height broken. For some reason, TableView ContentSize is broken.
				ContentPage page = new()
				{
					Content = _viewController.TableView.ToView(),
					Title = PickerCell.PageTitle
				};

				// Fire manually because INavigation.PushAsync does not work ViewDidAppear and ViewWillAppear.
				_viewController.ViewDidAppear(false);
				_viewController.InitializeView();
				
				BeginInvokeOnMainThread(async () => {
					await shell.ShellSection.Navigation.PushAsync(page, true);
					_viewController.InitializeScroll();
				});
			}
			else
			{
				// When use traditional navigation.
				_viewController = new PickerTableViewControllerSelectionColorFix(this, tableView);
				BeginInvokeOnMainThread(() => naviCtrl.PushViewController(_viewController, true));
			}

			if (!PickerCell.KeepSelectedUntilBack)
			{
				tableView.DeselectRow(indexPath, true);
			}
		}
		
		public override void UpdateCell(UITableView tableView)
		{
			base.UpdateCell(tableView);
			
			UpdateSelectedItems();
			UpdateCollectionChanged();
		}
		
		public void UpdateSelectedItems()
		{
			if (!PickerCell.UseAutoValueText)
			{
				return;
			}

			if (_selectedCollection != null)
			{
				_selectedCollection.CollectionChanged -= SelectedItems_CollectionChanged;
			}

			_selectedCollection = PickerCell.SelectedItems as INotifyCollectionChanged;

			if (_selectedCollection != null)
			{
				_selectedCollection.CollectionChanged += SelectedItems_CollectionChanged;
			}

			ValueLabel.Text = PickerCell.GetSelectedItemsTextPublic();
		}

		private void UpdateCollectionChanged()
		{
			if (_notifyCollection != null)
			{
				_notifyCollection.CollectionChanged -= ItemsSourceCollectionChanged;
			}

			_notifyCollection = PickerCell.ItemsSource as INotifyCollectionChanged;

			if (_notifyCollection != null)
			{
				_notifyCollection.CollectionChanged += ItemsSourceCollectionChanged;
				ItemsSourceCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		/// <summary>
		/// Updates the is enabled.
		/// </summary>
		protected override void UpdateIsEnabled()
		{
			if (PickerCell.ItemsSource != null && PickerCell.ItemsSource.Count == 0)
			{
				return;
			}
			base.UpdateIsEnabled();
		}

		private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!CellBase.IsEnabled)
			{
				return;
			}

			SetEnabledAppearance(PickerCell.ItemsSource.Count > 0);
		}

		private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdateSelectedItems();
		}

		/// <summary>
		/// Dispose the specified disposing.
		/// </summary>
		/// <returns>The dispose.</returns>
		/// <param name="disposing">If set to <c>true</c> disposing.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_viewController?.Dispose();
				_viewController = null;

				if (_notifyCollection != null)
				{
					_notifyCollection.CollectionChanged -= ItemsSourceCollectionChanged;
					_notifyCollection = null;
				}
				if (_selectedCollection != null)
				{
					_selectedCollection.CollectionChanged -= SelectedItems_CollectionChanged;
					_selectedCollection = null;
				}
			}
			base.Dispose(disposing);
		}

		// Refer to https://forums.xamarin.com/discussion/comment/294088/#Comment_294088
		private UINavigationController GetUINavigationController(UIViewController controller)
		{
			if (controller == null)
			{
				return null;
			}

			if (controller.PresentedViewController != null)
			{
				// on modal page
				return GetUINavigationController(controller.PresentedViewController);
			}
			
			if (controller is UINavigationController navigationController)
			{
				return navigationController;
			}
			
			if (controller is UITabBarController tabCtrl)
			{
				//in case Root->Tab->Navi->Page
				return GetUINavigationController(tabCtrl.SelectedViewController);
			}
			
			if (controller.ChildViewControllers.Count() != 0)
			{
				int count = controller.ChildViewControllers.Count();

				for (int c = 0; c < count; c++)
				{
					UINavigationController child = GetUINavigationController(controller.ChildViewControllers[c]);

					if (child == null)
					{
						//TODO: Analytics...
					}
					else if (child is UINavigationController)
					{
						return (child as UINavigationController);
					}
				}
			}

			return null;
		}
	}
}