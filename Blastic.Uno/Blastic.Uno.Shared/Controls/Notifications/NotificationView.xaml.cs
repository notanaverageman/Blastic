using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Blastic.Services.Notifications;

namespace Blastic.Controls.Notifications
{
	public partial class NotificationView
	{
		public static readonly DependencyProperty NotificationProperty = DependencyProperty.Register(
			nameof(NotificationProperty).Replace("Property", ""),
			typeof(Notification),
			typeof(NotificationView),
			new PropertyMetadata(default(Notification)));
		public Notification Notification
		{
			get => (Notification)GetValue(NotificationProperty);
			set => SetValue(NotificationProperty, value);
		}

		private bool _isPointerOver;
		private bool _hasFocus;

		public NotificationView()
		{
			InitializeComponent();

			DataContextChanged += (sender, args) =>
			{
				Console.WriteLine($"Blastic: DataContextChanged: {args.NewValue}");
				Console.WriteLine(new StackTrace().ToString());
			};
		}

		protected override void OnPointerEntered(PointerRoutedEventArgs e)
		{
			_isPointerOver = true;
			Notification?.StopTimeout();
		}

		protected override void OnPointerExited(PointerRoutedEventArgs e)
		{
			_isPointerOver = false;

			if (!_hasFocus)
			{
				Notification?.StartTimeout();
			}
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			_hasFocus = true;
			Notification?.StopTimeout();
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			_hasFocus = false;

			if (!_isPointerOver)
			{
				Notification?.StartTimeout();
			}
		}
	}
}