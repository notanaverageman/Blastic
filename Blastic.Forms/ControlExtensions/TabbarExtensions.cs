using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Blastic.Forms.UserInterface;
using Blastic.Reactive;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Blastic.Forms.ControlExtensions
{
	public class TabbarExtensions
	{
		public static readonly BindableProperty ShellTabsProperty = BindableProperty.CreateAttached(
			nameof(ShellTabsProperty).Replace("Property", ""),
			typeof(IEnumerable<IShellTab>),
			typeof(TabbarExtensions),
			default(IEnumerable<IShellTab>),
			propertyChanged: OnItemsSourceChanged);
		public static IEnumerable<IShellTab> GetShellTabs(BindableObject obj) => (IEnumerable<IShellTab>)obj.GetValue(ShellTabsProperty);
		public static void SetShellTabs(BindableObject obj, IEnumerable<IShellTab> value) => obj.SetValue(ShellTabsProperty, value);

		public static readonly BindableProperty TabTemplateProperty = BindableProperty.CreateAttached(
			nameof(TabTemplateProperty).Replace("Property", ""),
			typeof(DataTemplate),
			typeof(TabbarExtensions),
			default(DataTemplate),
			propertyChanged: OnTabTemplateChanged);
		public static DataTemplate GetTabTemplate(BindableObject obj) => (DataTemplate)obj.GetValue(TabTemplateProperty);
		public static void SetTabTemplate(BindableObject obj, DataTemplate value) => obj.SetValue(TabTemplateProperty, value);

		public static readonly BindableProperty SelectedItemProperty = BindableProperty.CreateAttached(
			nameof(SelectedItemProperty).Replace("Property", ""),
			typeof(IShellTab),
			typeof(TabbarExtensions),
			default(IShellTab),
			BindingMode.TwoWay,
			propertyChanged: OnSelectedItemChanged);
		public static IShellTab GetSelectedItem(BindableObject obj) => (IShellTab)obj.GetValue(SelectedItemProperty);
		public static void SetSelectedItem(BindableObject obj, IShellTab value) => obj.SetValue(SelectedItemProperty, value);

		private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
		{
			DataTemplate tabTemplate = GetTabTemplate(bindable);
			ResetContent(bindable, (IEnumerable<IShellTab>)newValue, tabTemplate);
		}

		private static void OnTabTemplateChanged(BindableObject bindable, object oldValue, object newValue)
		{
			IEnumerable<IShellTab> tabs = GetShellTabs(bindable);
			ResetContent(bindable, tabs, (DataTemplate)newValue);
		}

		private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
		{
			TabBar tabBar = (TabBar)bindable;

			if (newValue == null)
			{
				tabBar.CurrentItem = null;
				return;
			}

			IEnumerable<IShellTab> tabs = GetShellTabs(tabBar);

			int index = tabs.IndexOf(newValue);

			if (index < 0)
			{
				return;
			}

			tabBar.CurrentItem = tabBar.Items[index];
		}

		private static void ResetContent(
			BindableObject bindable,
			IEnumerable<IShellTab> tabs,
			DataTemplate tabTemplate)
		{
			if (tabs == null || tabTemplate == null)
			{
				return;
			}

			TabBar tabBar = (TabBar)bindable;

			tabBar.Items.Clear();

			void OnTabBarOnPropertyChanged(object sender, PropertyChangedEventArgs args)
			{
				if (args.PropertyName == nameof(ShellSection.CurrentItem))
				{
					int index = tabBar.Items.IndexOf(tabBar.CurrentItem);
					IShellTab tab = tabs.ElementAtOrDefault(index);

					SetSelectedItem(tabBar, tab);
				}
			}

			tabBar.PropertyChanged -= OnTabBarOnPropertyChanged;
			tabBar.PropertyChanged += OnTabBarOnPropertyChanged;

			foreach (IShellTab shellTab in tabs)
			{
				ShellSection tab = (ShellSection)tabTemplate.CreateContent();
				tab.BindingContext = shellTab;

				tabBar.Items.Add(tab);
			}

			if (tabs is INotifyCollectionChanged notifier)
			{
				void AddChildren(IEnumerable<IShellTab> items)
				{
					foreach (IShellTab item in items)
					{
						// TODO:
					}
				}

				void RemoveChildren(IEnumerable<IShellTab> items)
				{
					foreach (IShellTab item in items)
					{
						// TODO:
					}
				}

				void ClearChildren()
				{
					tabBar.Items.Clear();
				}

				void ReplaceChildren((IShellTab[] OldItems, IShellTab[] NewItems) items)
				{
					RemoveChildren(items.OldItems);
					AddChildren(items.NewItems);
				}

				// TODO: Dispose subscriptions.
				notifier
					.ObserveAdd<IShellTab>()
					.Subscribe(AddChildren);

				notifier
					.ObserveRemove<IShellTab>()
					.Subscribe(RemoveChildren);

				notifier
					.ObserveReplace<IShellTab>()
					.Subscribe(ReplaceChildren);

				notifier
					.ObserveReset()
					.Subscribe(_ => ClearChildren());
			}
		}
	}
}