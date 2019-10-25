using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace Blastic.Caliburn
{
	public class KeyTrigger : TriggerBase<UIElement>
	{
		public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
			nameof(KeyProperty).Replace("Property", ""),
			typeof(Key),
			typeof(KeyTrigger),
			new PropertyMetadata(default));
		public Key Key
		{
			get => (Key)GetValue(KeyProperty);
			set => SetValue(KeyProperty, value);
		}

		public static readonly DependencyProperty ModifiersProperty = DependencyProperty.Register(
			nameof(ModifiersProperty).Replace("Property", ""),
			typeof(ModifierKeys),
			typeof(KeyTrigger),
			new PropertyMetadata(default));
		public ModifierKeys Modifiers
		{
			get => (ModifierKeys)GetValue(ModifiersProperty);
			set => SetValue(ModifiersProperty, value);
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.KeyDown += OnAssociatedObjectKeyDown;
			
			if (AssociatedObject is ButtonBase buttonBase)
			{
				buttonBase.Click += OnAssociatedObjectClick;
			}
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.KeyDown -= OnAssociatedObjectKeyDown;
			
			if (AssociatedObject is ButtonBase buttonBase)
			{
				buttonBase.Click -= OnAssociatedObjectClick;
			}
		}

		private void OnAssociatedObjectClick(object sender, RoutedEventArgs e)
		{
			InvokeActions(e);
		}

		private void OnAssociatedObjectKeyDown(object sender, KeyEventArgs e)
		{
			if ((e.Key == Key) && (Keyboard.Modifiers == GetActualModifiers(e.Key, Modifiers)))
			{
				InvokeActions(e);
			}
		}

		private static ModifierKeys GetActualModifiers(Key key, ModifierKeys modifiers)
		{
			switch (key)
			{
				case Key.LeftCtrl:
				case Key.RightCtrl:
					modifiers |= ModifierKeys.Control;
					break;

				case Key.LeftAlt:
				case Key.RightAlt:
					modifiers |= ModifierKeys.Alt;
					break;

				case Key.LeftShift:
				case Key.RightShift:
					modifiers |= ModifierKeys.Shift;
					break;
			}

			return modifiers;
		}
	}
}