using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace Blastic.Forms.Sample.Controls.Overlay
{
	public partial class OverlayView
	{
		private enum Direction
		{
			Up,
			Down
		}

		public static readonly BindableProperty StateProperty = BindableProperty.Create(
			nameof(StateProperty).Replace("Property", ""),
			typeof(OverlayState),
			typeof(OverlayView),
			propertyChanged: StateChanged,
			defaultBindingMode: BindingMode.TwoWay);
		public OverlayState State
		{
			get => (OverlayState)GetValue(StateProperty);
			set => SetValue(StateProperty, value);
		}

		public static readonly BindableProperty ExpandedProperty = BindableProperty.Create(
			nameof(ExpandedProperty).Replace("Property", ""),
			typeof(View),
			typeof(OverlayView),
			propertyChanged: ViewChanged);
		public View? Expanded
		{
			get => (View)GetValue(ExpandedProperty);
			set => SetValue(ExpandedProperty, value);
		}

		public static readonly BindableProperty CollapsedProperty = BindableProperty.Create(
			nameof(CollapsedProperty).Replace("Property", ""),
			typeof(View),
			typeof(OverlayView),
			propertyChanged: ViewChanged);
		public View? Collapsed
		{
			get => (View)GetValue(CollapsedProperty);
			set => SetValue(CollapsedProperty, value);
		}

		public static readonly BindableProperty TabBarProperty = BindableProperty.Create(
			nameof(TabBarProperty).Replace("Property", ""),
			typeof(ExtendedTabbedPage),
			typeof(OverlayView),
			propertyChanged: TabBarChanged);
		public ExtendedTabbedPage? TabBar
		{
			get => (ExtendedTabbedPage)GetValue(TabBarProperty);
			set => SetValue(TabBarProperty, value);
		}

		public static readonly BindableProperty EasingProperty = BindableProperty.Create(
			nameof(EasingProperty).Replace("Property", ""),
			typeof(Easing),
			typeof(OverlayView),
			Easing.SinOut);
		public Easing Easing
		{
			get => (Easing)GetValue(EasingProperty);
			set => SetValue(EasingProperty, value);
		}

		public static readonly BindableProperty AnimationDurationProperty = BindableProperty.Create(
			nameof(AnimationDurationProperty).Replace("Property", ""),
			typeof(int),
			typeof(OverlayView),
			200);
		public int AnimationDuration
		{
			get => (int)GetValue(AnimationDurationProperty);
			set => SetValue(AnimationDurationProperty, value);
		}

		public static readonly BindableProperty ExpandBoundaryProperty = BindableProperty.Create(
			nameof(ExpandBoundaryProperty).Replace("Property", ""),
			typeof(double),
			typeof(OverlayView),
			0.1);
		public double ExpandBoundary
		{
			get => (double)GetValue(ExpandBoundaryProperty);
			set => SetValue(ExpandBoundaryProperty, value);
		}

		public static readonly BindableProperty CollapseBoundaryProperty = BindableProperty.Create(
			nameof(CollapseBoundaryProperty).Replace("Property", ""),
			typeof(double),
			typeof(OverlayView),
			0.9);
		public double CollapseBoundary
		{
			get => (double)GetValue(CollapseBoundaryProperty);
			set => SetValue(CollapseBoundaryProperty, value);
		}

		public static readonly BindableProperty ExpandOpacityBoundaryProperty = BindableProperty.Create(
			nameof(ExpandOpacityBoundaryProperty).Replace("Property", ""),
			typeof(double),
			typeof(OverlayView),
			0.9);
		public double ExpandOpacityBoundary
		{
			get => (double)GetValue(ExpandOpacityBoundaryProperty);
			set => SetValue(ExpandOpacityBoundaryProperty, value);
		}

		public static readonly BindableProperty CollapseOpacityBoundaryProperty = BindableProperty.Create(
			nameof(CollapseOpacityBoundaryProperty).Replace("Property", ""),
			typeof(double),
			typeof(OverlayView),
			0.95);
		public double CollapseOpacityBoundary
		{
			get => (double)GetValue(CollapseOpacityBoundaryProperty);
			set => SetValue(CollapseOpacityBoundaryProperty, value);
		}

		private bool _isUserPanning;
		private double _previousTotalY;
		private Direction _direction;
		private OverlayState _overlayState;

		public OverlayView()
		{
			InitializeComponent();

			PanGestureRecognizer gestureRecognizer = new PanGestureRecognizer();
			gestureRecognizer.PanUpdated += (sender, args) =>
			{
				PanUpdated(args);
			};

			GestureRecognizers.Add(gestureRecognizer);
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (Expanded != null)
			{
				SetInheritedBindingContext(Expanded, BindingContext);
			}

			if (Collapsed != null)
			{
				SetInheritedBindingContext(Collapsed, BindingContext);
			}
		}

		private static void StateChanged(BindableObject bindable, object oldValue, object newValue)
		{
			OverlayView overlayView = (OverlayView)bindable;
			OverlayState newState = (OverlayState)newValue;

			overlayView.ChangeState(newState);
		}

		private static void ViewChanged(BindableObject bindable, object oldValue, object newValue)
		{
			OverlayView overlayView = (OverlayView)bindable;
			BindableObject newView = (BindableObject)newValue;

			SetInheritedBindingContext(newView, overlayView.BindingContext);
			overlayView.ChangeState(overlayView.State, forceRun: true);
		}

		private static void TabBarChanged(BindableObject bindable, object oldValue, object newValue)
		{
			void TabBarPropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName != nameof(ExtendedTabbedPage.TabBarHeight))
				{
					return;
				}

				OverlayView overlayView = (OverlayView)bindable;
				overlayView.ChangeState(overlayView.State, forceRun: true);
			}

			if (oldValue is ExtendedTabbedPage oldPage)
			{
				oldPage.PropertyChanged -= TabBarPropertyChanged;
			}

			if (newValue is ExtendedTabbedPage newPage)
			{
				newPage.PropertyChanged += TabBarPropertyChanged;
			}
		}

		private async void PanUpdated(PanUpdatedEventArgs e)
		{
			if (e.StatusType == GestureStatus.Running)
			{
				_isUserPanning = true;

				Pan(e.TotalY);
			}
			else if (e.StatusType == GestureStatus.Completed)
			{
				_isUserPanning = false;
				await CompletePan();
			}
		}

		private void Pan(double totalY)
		{
			UpdateMembers(totalY);

			if (_overlayState == OverlayState.Expanded)
			{
				HandlePan(totalY);
			}
			else
			{
				double normalizedTotalY = NormalizeTotalY(totalY);
				HandlePan(normalizedTotalY);
			}
		}

		private void HandlePan(double normalizedTotalY)
		{
			SetTranslation(normalizedTotalY);
			SetOpacity();
		}

		private void SetTranslation(double normalizedTotalY)
		{
			double heightWithoutCollapsedView = GetHeightWithoutCollapsedView();
			ExtendedTabbedPage? tabBar = TabBar;

			if (normalizedTotalY <= 0)
			{
				TranslationY = 0;

				if (tabBar != null)
				{
					tabBar.TabBarOffset = 1;
				}

				return;
			}

			if (normalizedTotalY >= heightWithoutCollapsedView)
			{
				TranslationY = heightWithoutCollapsedView;

				if (tabBar != null)
				{
					tabBar.TabBarOffset = 0;
				}

				return;
			}

			TranslationY = normalizedTotalY;

			if (tabBar != null)
			{
				double offset = heightWithoutCollapsedView - normalizedTotalY;
				double ratio = offset / heightWithoutCollapsedView;

				if (ratio >= 0 && ratio <= 1)
				{
					tabBar.TabBarOffset = (float)(ratio);
				}
			}
		}

		private void SetOpacity()
		{
			double heightWithoutCollapsedView = GetHeightWithoutCollapsedView();
			double currentTranslation = TranslationY;

			double expandedOpacityBoundaryHeight = heightWithoutCollapsedView * ExpandOpacityBoundary;
			double collapsedOpacityBoundaryHeight = heightWithoutCollapsedView * CollapseOpacityBoundary;

			double expandedOpacity = 0;
			double collapsedOpacity = 0;

			if (currentTranslation < expandedOpacityBoundaryHeight)
			{
				expandedOpacity = 1;
			}
			else if (currentTranslation >= expandedOpacityBoundaryHeight && currentTranslation <= collapsedOpacityBoundaryHeight)
			{
				double opacityBoundaryDifference = collapsedOpacityBoundaryHeight - expandedOpacityBoundaryHeight;
				double opacityOffset = currentTranslation - expandedOpacityBoundaryHeight;

				expandedOpacity = 1 - (opacityOffset / opacityBoundaryDifference);
			}
			else if (currentTranslation > collapsedOpacityBoundaryHeight - 1)
			{
				double opacityBoundaryDifference = heightWithoutCollapsedView - collapsedOpacityBoundaryHeight;
				double opacityOffset = currentTranslation - collapsedOpacityBoundaryHeight;

				collapsedOpacity = opacityOffset / opacityBoundaryDifference;
			}
			else
			{
				collapsedOpacity = 1;
			}

			View? expanded = Expanded;

			if (expanded != null)
			{
				expanded.Opacity = expandedOpacity;
			}

			View? collapsed = Collapsed;

			if (collapsed != null)
			{
				collapsed.Opacity = collapsedOpacity;
			}
		}

		private async Task CompletePan()
		{
			if (_overlayState == OverlayState.Expanded && _previousTotalY < 0)
			{
				return;
			}

			if (_overlayState == OverlayState.Collapsed && _previousTotalY > 0)
			{
				return;
			}

			double heightWithoutCollapsedView = GetHeightWithoutCollapsedView();
			double currentTranslation = TranslationY;

			double expandBoundaryHeight = heightWithoutCollapsedView * ExpandBoundary;
			double collapseBoundaryHeight = heightWithoutCollapsedView * CollapseBoundary;

			if (currentTranslation > collapseBoundaryHeight && _isUserPanning)
			{
				_direction = Direction.Down;
			}
			else if (currentTranslation < expandBoundaryHeight && _isUserPanning)
			{
				_direction = Direction.Up;
			}

			OverlayState targetState = _direction == Direction.Down
				? OverlayState.Collapsed
				: OverlayState.Expanded;

			await CommitExpandCollapseAnimation(targetState);
		}

		private async void ChangeState(OverlayState targetState, bool forceRun = false)
		{
			if (_overlayState == targetState && !forceRun)
			{
				return;
			}

			if (targetState == OverlayState.Expanded)
			{
				if (_overlayState == OverlayState.Invisible)
				{
					await CommitVisibilityAnimation(OverlayState.Collapsed);
				}

				_direction = Direction.Up;
				_previousTotalY = 0;

				await CommitExpandCollapseAnimation(OverlayState.Expanded);
			}
			else if (targetState == OverlayState.Collapsed)
			{
				if (_overlayState == OverlayState.Invisible)
				{
					await CommitVisibilityAnimation(OverlayState.Collapsed);
				}
				else
				{
					_direction = Direction.Down;
					_previousTotalY = 0;

					await CommitExpandCollapseAnimation(OverlayState.Collapsed);
				}
			}
			else
			{
				if (_overlayState == OverlayState.Expanded)
				{
					await CommitExpandCollapseAnimation(OverlayState.Collapsed);
				}

				await CommitVisibilityAnimation(OverlayState.Invisible);
			}
		}

		private Task CommitVisibilityAnimation(OverlayState targetState)
		{
			Animation consistencyAnimation = GetVisibilityAnimation(_overlayState);
			consistencyAnimation.GetCallback()(1);

			Animation animation = GetVisibilityAnimation(targetState);

			string animationName = targetState == OverlayState.Invisible
				? "invisible"
				: "visible";

			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

			animation.Commit(
				this,
				animationName,
				length: (uint)AnimationDuration,
				easing: Easing,
				finished: (x, y) =>
				{
					_overlayState = targetState;
					State = _overlayState;

					taskCompletionSource.SetResult(y);
				});

			return taskCompletionSource.Task;
		}

		private Task CommitExpandCollapseAnimation(OverlayState targetState)
		{
			Animation consistencyAnimation = GetExpandCollapseAnimation(_overlayState);
			consistencyAnimation.GetCallback()(1);

			Animation animation = GetExpandCollapseAnimation(targetState);

			double heightWithoutCollapsedView = GetHeightWithoutCollapsedView();
			double animationEnd = GetExpandCollapseAnimationEnd(targetState);

			string animationName = targetState == OverlayState.Collapsed
				? "collapse"
				: "expand";

			double animationDuration = AnimationDuration;
			animationDuration *= Math.Abs(_previousTotalY - animationEnd) / heightWithoutCollapsedView;

			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

			animation.Commit(
				this,
				animationName,
				length: (uint)animationDuration,
				easing: Easing,
				finished: (x, y) =>
				{
					_previousTotalY = 0;
					_overlayState = targetState;

					State = _overlayState;
					taskCompletionSource.SetResult(y);
				});

			return taskCompletionSource.Task;
		}

		private Animation GetVisibilityAnimation(OverlayState targetState)
		{
			double height = GetHeight();
			double heightWithoutCollapsedView = GetHeightWithoutCollapsedView();
			double currentTranslation = TranslationY;

			double animationEnd = targetState == OverlayState.Invisible
				? height
				: heightWithoutCollapsedView;

			Animation animation = new Animation(
				x =>
				{
					double opacityOffset = height - TranslationY;
					double opacityBoundaryDifference = height - heightWithoutCollapsedView;

					double collapsedOpacity = opacityOffset / opacityBoundaryDifference;

					View? collapsed = Collapsed;
					View? expanded = Expanded;

					if (TabBar != null)
					{
						float margin = (float)(collapsedOpacity * (collapsed?.Height ?? expanded?.Height ?? 0));
						TabBar.ContainerMargin = margin;
					}

					TranslationY = x;

					if (collapsed != null)
					{
						collapsed.Opacity = collapsedOpacity;

						collapsed.Clip = new RectangleGeometry(new Rect(
							0,
							0,
							collapsed.Width,
							collapsedOpacity * collapsed.Height));

					}

					if (expanded != null)
					{
						expanded.Opacity = 0;
					}
				},
				currentTranslation,
				animationEnd);

			return animation;
		}

		private Animation GetExpandCollapseAnimation(OverlayState targetState)
		{
			double animationEnd = GetExpandCollapseAnimationEnd(targetState);

			return new Animation(
				Pan,
				_previousTotalY,
				animationEnd);
		}

		private double GetExpandCollapseAnimationEnd(OverlayState targetState)
		{
			double heightWithoutCollapsedView = GetHeightWithoutCollapsedView();

			double animationEnd =
				_overlayState == OverlayState.Expanded
					? targetState == OverlayState.Expanded
						? 0
						: heightWithoutCollapsedView
					: targetState == OverlayState.Collapsed
						? 0
						: -heightWithoutCollapsedView;

			return animationEnd;
		}

		private void UpdateMembers(double totalY)
		{
			if (!_isUserPanning)
			{
				return;
			}

			double difference = _previousTotalY - totalY;

			if (difference > 0)
			{
				_direction = Direction.Up;
			}
			else if (difference < 0)
			{
				_direction = Direction.Down;
			}

			_previousTotalY = totalY;
		}

		private double GetHeightWithoutCollapsedView()
		{
			return GetHeight() - (Collapsed?.Height ?? 0);
		}

		private double GetHeight()
		{
			double height = Height;

			if (TabBar != null)
			{
				height -= TabBar.TabBarHeight;
			}

			return height;
		}

		private double NormalizeTotalY(double totalY)
		{
			return GetHeightWithoutCollapsedView() + totalY;
		}
	}
}