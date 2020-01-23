using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Blastic.Forms.UserInterface;
using Blastic.Reactive;
using Xamarin.Forms;

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

		private static void ResetContent(
			BindableObject bindable,
			IEnumerable<IShellTab> tabs,
			DataTemplate tabTemplate)
		{
			TabBar tabBar = (TabBar)bindable;

			tabBar.Items.Clear();

			if (tabs == null || tabTemplate == null)
			{
				return;
			}

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