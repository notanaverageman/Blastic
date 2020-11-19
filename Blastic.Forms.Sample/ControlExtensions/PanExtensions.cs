using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Blastic.Forms.Sample.ControlExtensions
{
	public class PanExtensions
	{
		private enum Direction
		{
			Up,
			Down
		}

		private class PanStateInternal
		{
			public bool IsUserPanning { get; set; }
			public double PreviousTotalY { get; set; }
			public PanState InternalState { get; set; }
			public Direction Direction { get; set; }
		}

		private static readonly BindableProperty GestureRecognizerProperty = BindableProperty.CreateAttached(
			nameof(GestureRecognizerProperty).Replace("Property", ""),
			typeof(IGestureRecognizer),
			typeof(PanExtensions),
			null);

		private static IGestureRecognizer GetGestureRecognizer(BindableObject view)
		{
			return (IGestureRecognizer)view.GetValue(GestureRecognizerProperty);
		}

		private static void SetGestureRecognizer(BindableObject view, IGestureRecognizer value)
		{
			view.SetValue(GestureRecognizerProperty, value);
		}

		private static readonly BindableProperty PanStateProperty = BindableProperty.CreateAttached(
			nameof(PanStateProperty).Replace("Property", ""),
			typeof(PanStateInternal),
			typeof(PanExtensions),
			null);

		private static PanStateInternal GetPanState(BindableObject view)
		{
			return (PanStateInternal)view.GetValue(PanStateProperty);
		}

		private static void SetPanState(BindableObject view, PanStateInternal value)
		{
			view.SetValue(PanStateProperty, value);
		}

		public static readonly BindableProperty EnableMediaPlayerPanProperty = BindableProperty.CreateAttached(
			nameof(EnableMediaPlayerPanProperty).Replace("Property", ""),
			typeof(bool),
			typeof(PanExtensions),
			false,
			propertyChanged: EnableMediaPlayerPanChanged);

		public static bool GetEnableMediaPlayerPan(BindableObject view)
		{
			return (bool) view.GetValue(EnableMediaPlayerPanProperty);
		}

		public static void SetEnableMediaPlayerPan(BindableObject view, bool value)
		{
			view.SetValue(EnableMediaPlayerPanProperty, value);
		}

		public static readonly BindableProperty StateProperty = BindableProperty.CreateAttached(
			nameof(StateProperty).Replace("Property", ""),
			typeof(PanState),
			typeof(PanExtensions),
			PanState.Invisible,
			BindingMode.TwoWay,
			propertyChanged: StateChanged);

		public static PanState GetState(BindableObject view)
		{
			return (PanState) view.GetValue(StateProperty);
		}

		public static void SetState(BindableObject view, PanState value)
		{
			view.SetValue(StateProperty, value);
		}

		public static readonly BindableProperty ExpandedViewProperty = BindableProperty.CreateAttached(
			nameof(ExpandedViewProperty).Replace("Property", ""),
			typeof(View),
			typeof(PanExtensions),
			null,
			propertyChanged: ViewsChanged);

		public static View GetExpandedView(BindableObject view)
		{
			return (View) view.GetValue(ExpandedViewProperty);
		}

		public static void SetExpandedView(BindableObject view, View value)
		{
			view.SetValue(ExpandedViewProperty, value);
		}

		public static readonly BindableProperty CollapsedViewProperty = BindableProperty.CreateAttached(
			nameof(CollapsedViewProperty).Replace("Property", ""),
			typeof(View),
			typeof(PanExtensions),
			null,
			propertyChanged: ViewsChanged);

		public static View GetCollapsedView(BindableObject view)
		{
			return (View)view.GetValue(CollapsedViewProperty);
		}

		public static void SetCollapsedView(BindableObject view, View value)
		{
			view.SetValue(CollapsedViewProperty, value);
		}

		public static readonly BindableProperty BarViewProperty = BindableProperty.CreateAttached(
			nameof(BarViewProperty).Replace("Property", ""),
			typeof(View),
			typeof(PanExtensions),
			null,
			propertyChanged: ViewsChanged);

		public static View GetBarView(BindableObject view)
		{
			return (View)view.GetValue(BarViewProperty);
		}

		public static void SetBarView(BindableObject view, View value)
		{
			view.SetValue(BarViewProperty, value);
		}

		public static readonly BindableProperty ContentViewProperty = BindableProperty.CreateAttached(
			nameof(ContentViewProperty).Replace("Property", ""),
			typeof(View),
			typeof(PanExtensions),
			null,
			propertyChanged: ViewsChanged);

		public static View GetContentView(BindableObject view)
		{
			return (View)view.GetValue(ContentViewProperty);
		}

		public static void SetContentView(BindableObject view, View value)
		{
			view.SetValue(ContentViewProperty, value);
		}

		public static readonly BindableProperty EasingProperty = BindableProperty.CreateAttached(
			nameof(Easing),
			typeof(Easing),
			typeof(PanExtensions),
			Easing.SinOut);

		public static Easing GetEasing(BindableObject view)
		{
			return (Easing)view.GetValue(EasingProperty);
		}

		public static void SetEasing(BindableObject view, Easing value)
		{
			view.SetValue(EasingProperty, value);
		}

		public static readonly BindableProperty AnimationDurationProperty = BindableProperty.CreateAttached(
			nameof(AnimationDurationProperty).Replace("Property", ""),
			typeof(int),
			typeof(PanExtensions),
			200);

		public static int GetAnimationDuration(BindableObject view)
		{
			return (int)view.GetValue(AnimationDurationProperty);
		}

		public static void SetAnimationDuration(BindableObject view, int value)
		{
			view.SetValue(AnimationDurationProperty, value);
		}

		public static readonly BindableProperty ExpandBoundaryProperty = BindableProperty.CreateAttached(
			nameof(ExpandBoundaryProperty).Replace("Property", ""),
			typeof(double),
			typeof(PanExtensions),
			0.1);

		public static double GetExpandBoundary(BindableObject view)
		{
			return (double)view.GetValue(ExpandBoundaryProperty);
		}

		public static void SetExpandBoundary(BindableObject view, double value)
		{
			view.SetValue(ExpandBoundaryProperty, value);
		}

		public static readonly BindableProperty CollapseBoundaryProperty = BindableProperty.CreateAttached(
			nameof(CollapseBoundaryProperty).Replace("Property", ""),
			typeof(double),
			typeof(PanExtensions),
			0.9);

		public static double GetCollapseBoundary(BindableObject view)
		{
			return (double)view.GetValue(CollapseBoundaryProperty);
		}

		public static void SetCollapseBoundary(BindableObject view, double value)
		{
			view.SetValue(CollapseBoundaryProperty, value);
		}

		public static readonly BindableProperty ExpandedOpacityBoundaryProperty = BindableProperty.CreateAttached(
			nameof(ExpandedOpacityBoundaryProperty).Replace("Property", ""),
			typeof(double),
			typeof(PanExtensions),
			0.9);

		public static double GetExpandedOpacityBoundary(BindableObject view)
		{
			return (double)view.GetValue(ExpandedOpacityBoundaryProperty);
		}

		public static void SetExpandedOpacityBoundary(BindableObject view, double value)
		{
			view.SetValue(ExpandedOpacityBoundaryProperty, value);
		}

		public static readonly BindableProperty CollapsedOpacityBoundaryProperty = BindableProperty.CreateAttached(
			nameof(CollapsedOpacityBoundaryProperty).Replace("Property", ""),
			typeof(double),
			typeof(PanExtensions),
			0.95);

		public static double GetCollapsedOpacityBoundary(BindableObject view)
		{
			return (double)view.GetValue(CollapsedOpacityBoundaryProperty);
		}

		public static void SetCollapsedOpacityBoundary(BindableObject view, double value)
		{
			view.SetValue(CollapsedOpacityBoundaryProperty, value);
		}

		private static void EnableMediaPlayerPanChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (!(bindable is View view))
			{
				return;
			}

			if (newValue is false)
			{
				IGestureRecognizer gestureRecognizer = GetGestureRecognizer(view);

				if (gestureRecognizer != null)
				{
					view.GestureRecognizers.Remove(gestureRecognizer);
				}

				SetPanState(view, null);
			}
			else if (newValue is true)
			{
				PanGestureRecognizer gestureRecognizer = new PanGestureRecognizer();
				gestureRecognizer.PanUpdated += PanUpdated;

				view.GestureRecognizers.Add(gestureRecognizer);
				SetGestureRecognizer(view, gestureRecognizer);

				PanStateInternal panState = new PanStateInternal();
				SetPanState(view, panState);
			}
		}

		private static async void ViewsChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			if (!(bindable is View view))
			{
				return;
			}

			if (!GetEnableMediaPlayerPan(view))
			{
				return;
			}

			View expandedView = GetExpandedView(view);
			View collapsedView = GetCollapsedView(view);

			if (expandedView == null || collapsedView == null)
			{
				return;
			}

			PanState targetState = GetState(view);
			PanStateInternal panState = GetPanState(view);

			await ChangeState(view, panState, targetState, forceRun: true);
		}

		private static async void StateChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (!(bindable is View view))
			{
				return;
			}

			if (!GetEnableMediaPlayerPan(view))
			{
				return;
			}

			PanStateInternal panState = GetPanState(view);
			PanState targetState = (PanState) newValue;

			await ChangeState(view, panState, targetState);
		}

		private static async void PanUpdated(object sender, PanUpdatedEventArgs e)
		{
			View view = (View) sender;
			PanStateInternal panState = GetPanState(view);

			View expandedView = GetExpandedView(view);
			View collapsedView = GetCollapsedView(view);
			View barView = GetBarView(view);

			if (expandedView == null || collapsedView == null)
			{
				return;
			}

			if (e.StatusType == GestureStatus.Running)
			{
				panState.IsUserPanning = true;
				Pan(
					view,
					expandedView,
					collapsedView,
					barView,
					panState,
					e.TotalY);
			}
			else if (e.StatusType == GestureStatus.Completed)
			{
				panState.IsUserPanning = false;
				await CompletePan(view, expandedView, collapsedView, barView, panState);
			}
		}

		private static void Pan(
			View view,
			View expandedView,
			View collapsedView,
			View barView,
			PanStateInternal panState,
			double totalY)
		{
			UpdateMembers(panState, totalY);

			if (panState.InternalState == PanState.Expanded)
			{
				HandlePan(view, expandedView, collapsedView, barView, totalY);
			}
			else
			{
				double normalizedTotalY = NormalizeTotalY(view, collapsedView, barView, totalY);
				HandlePan(view, expandedView, collapsedView, barView, normalizedTotalY);
			}
		}

		private static void HandlePan(
			View view,
			View expandedView,
			View collapsedView,
			View barView,
			double normalizedTotalY)
		{
			SetTranslation(view, collapsedView, barView, normalizedTotalY);
			SetOpacity(view, expandedView, collapsedView, barView);
		}

		private static void SetTranslation(
			View view,
			View collapsedView,
			View barView,
			double normalizedTotalY)
		{
			double heightWithoutCollapsedView = GetHeightWithoutCollapsedView(view, collapsedView, barView);

			if (normalizedTotalY <= 0)
			{
				view.TranslationY = 0;

				if (barView != null)
				{
					barView.TranslationY = barView.Height;
				}

				return;
			}

			if (normalizedTotalY >= heightWithoutCollapsedView)
			{
				view.TranslationY = heightWithoutCollapsedView;

				if (barView != null)
				{
					barView.TranslationY = 0;
				}

				return;
			}

			view.TranslationY = normalizedTotalY;

			if (barView != null)
			{
				barView.TranslationY = barView.Height * (1 - normalizedTotalY / heightWithoutCollapsedView);
			}
		}

		private static void SetOpacity(
			View view,
			View expandedView,
			View collapsedView,
			View barView)
		{
			double heightWithoutCollapsedView = GetHeightWithoutCollapsedView(view, collapsedView, barView);
			double currentTranslation = view.TranslationY;

			double expandedOpacityBoundaryHeight = heightWithoutCollapsedView * GetExpandedOpacityBoundary(view);
			double collapsedOpacityBoundaryHeight = heightWithoutCollapsedView * GetCollapsedOpacityBoundary(view);

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

			expandedView.Opacity = expandedOpacity;
			collapsedView.Opacity = collapsedOpacity;
		}

		private static async Task CompletePan(
			View view,
			View expandedView,
			View collapsedView,
			View barView,
			PanStateInternal panState)
		{
			if (panState.InternalState == PanState.Expanded && panState.PreviousTotalY < 0)
			{
				return;
			}

			if (panState.InternalState == PanState.Collapsed && panState.PreviousTotalY > 0)
			{
				return;
			}

			double heightWithoutCollapsedView = GetHeightWithoutCollapsedView(view, collapsedView, barView);
			double currentTranslation = view.TranslationY;

			double expandBoundaryHeight = heightWithoutCollapsedView * GetExpandBoundary(view);
			double collapseBoundaryHeight = heightWithoutCollapsedView * GetCollapseBoundary(view);

			if (currentTranslation > collapseBoundaryHeight && panState.IsUserPanning)
			{
				panState.Direction = Direction.Down;
			}
			else if (currentTranslation < expandBoundaryHeight && panState.IsUserPanning)
			{
				panState.Direction = Direction.Up;
			}

			PanState targetState = panState.Direction == Direction.Down
				? PanState.Collapsed
				: PanState.Expanded;

			await CommitExpandCollapseAnimation(
				view,
				expandedView,
				collapsedView,
				barView,
				panState,
				targetState);
		}

		private static async Task ChangeState(
			View view,
			PanStateInternal panState,
			PanState targetState,
			bool forceRun = false)
		{
			if (panState.InternalState == targetState && !forceRun)
			{
				return;
			}

			View expandedView = GetExpandedView(view);
			View collapsedView = GetCollapsedView(view);
			View barView = GetBarView(view);
			View contentView = GetContentView(view);

			if (targetState == PanState.Expanded)
			{
				if (panState.InternalState == PanState.Invisible)
				{
					await CommitVisibilityAnimation(
						view,
						expandedView,
						collapsedView,
						contentView,
						barView,
						panState,
						PanState.Collapsed);
				}

				panState.Direction = Direction.Up;
				panState.PreviousTotalY = 0;

				await CommitExpandCollapseAnimation(
					view,
					expandedView,
					collapsedView,
					barView,
					panState,
					PanState.Expanded);
			}
			else if (targetState == PanState.Collapsed)
			{
				if (panState.InternalState == PanState.Invisible)
				{
					await CommitVisibilityAnimation(
						view,
						expandedView,
						collapsedView,
						contentView,
						barView,
						panState,
						PanState.Collapsed);
				}
				else
				{
					panState.Direction = Direction.Down;
					panState.PreviousTotalY = 0;

					await CommitExpandCollapseAnimation(
						view,
						expandedView,
						collapsedView,
						barView,
						panState,
						PanState.Collapsed);
				}
			}
			else
			{
				if (panState.InternalState == PanState.Expanded)
				{
					await CommitExpandCollapseAnimation(
						view,
						expandedView,
						collapsedView,
						barView,
						panState,
						PanState.Collapsed);
				}

				await CommitVisibilityAnimation(
					view,
					expandedView,
					collapsedView,
					contentView,
					barView,
					panState,
					PanState.Invisible);
			}
		}

		private static Task CommitVisibilityAnimation(
			View view,
			View expandedView,
			View collapsedView,
			View contentView,
			View barView,
			PanStateInternal panState,
			PanState targetState)
		{
			Animation consistencyAnimation = GetVisibilityAnimation(
				view,
				expandedView,
				collapsedView,
				contentView,
				barView,
				panState.InternalState);

			consistencyAnimation.GetCallback()(1);

			Animation animation = GetVisibilityAnimation(
				view,
				expandedView,
				collapsedView,
				contentView,
				barView,
				targetState);

			string animationName = targetState == PanState.Invisible
				? "invisible"
				: "visible";

			double animationDuration = GetAnimationDuration(view);

			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

			animation.Commit(
				view,
				animationName,
				length: (uint)animationDuration,
				easing: GetEasing(view),
				finished: (x, y) =>
				{
					panState.InternalState = targetState;
					SetState(view, panState.InternalState);

					taskCompletionSource.SetResult(y);
				});

			return taskCompletionSource.Task;
		}

		private static Task CommitExpandCollapseAnimation(
			View view,
			View expandedView,
			View collapsedView,
			View barView,
			PanStateInternal panState,
			PanState targetState)
		{
			Animation consistencyAnimation = GetExpandCollapseAnimation(
				view,
				expandedView,
				collapsedView,
				barView,
				panState,
				panState.InternalState);

			consistencyAnimation.GetCallback()(1);

			Animation animation = GetExpandCollapseAnimation(
				view,
				expandedView,
				collapsedView,
				barView,
				panState,
				targetState);

			double heightWithoutCollapsedView = GetHeightWithoutCollapsedView(view, collapsedView, barView);
			double animationEnd = GetExpandCollapseAnimationEnd(panState, targetState, heightWithoutCollapsedView);

			string animationName = targetState == PanState.Collapsed
				? "collapse"
				: "expand";

			double animationDuration = GetAnimationDuration(view);
			animationDuration *= Math.Abs(panState.PreviousTotalY - animationEnd) / heightWithoutCollapsedView;

			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

			animation.Commit(
				view,
				animationName,
				length: (uint) animationDuration,
				easing: GetEasing(view),
				finished: (x, y) =>
				{
					panState.PreviousTotalY = 0;
					panState.InternalState = targetState;

					SetState(view, panState.InternalState);
					taskCompletionSource.SetResult(y);
				});

			return taskCompletionSource.Task;
		}

		private static Animation GetVisibilityAnimation(
			View view,
			View expandedView,
			View collapsedView,
			View contentView,
			View barView,
			PanState targetState)
		{
			double height = GetHeight(view, barView);
			double heightWithoutCollapsedView = GetHeightWithoutCollapsedView(view, collapsedView, barView);
			double currentTranslation = view.TranslationY;

			double animationEnd = targetState == PanState.Invisible
				? height
				: heightWithoutCollapsedView;

			Animation animation = new Animation(
				x =>
				{
					double opacityOffset = height - view.TranslationY;
					double opacityBoundaryDifference = height - heightWithoutCollapsedView;

					double collapsedOpacity = opacityOffset / opacityBoundaryDifference;

					if (contentView != null)
					{
						Thickness margin = contentView.Margin;
						margin.Bottom = collapsedOpacity * collapsedView.Height;

						contentView.Margin = margin;
					}

					view.TranslationY = x;
					collapsedView.Opacity = collapsedOpacity;

					expandedView.Opacity = 0;
				},
				currentTranslation,
				animationEnd);

			return animation;
		}

		private static Animation GetExpandCollapseAnimation(
			View view,
			View expandedView,
			View collapsedView,
			View barView,
			PanStateInternal panState,
			PanState targetState)
		{
			double heightWithoutCollapsedView = GetHeightWithoutCollapsedView(view, collapsedView, barView);
			double animationEnd = GetExpandCollapseAnimationEnd(panState, targetState, heightWithoutCollapsedView);

			return new Animation(
				x => Pan(view, expandedView, collapsedView, barView, panState, x),
				panState.PreviousTotalY,
				animationEnd);
		}

		private static double GetExpandCollapseAnimationEnd(
			PanStateInternal panState,
			PanState targetState,
			double heightWithoutCollapsedView)
		{
			double animationEnd =
				panState.InternalState == PanState.Expanded
					? targetState == PanState.Expanded
						? 0
						: heightWithoutCollapsedView
					: targetState == PanState.Collapsed
						? 0
						: -heightWithoutCollapsedView;

			return animationEnd;
		}

		private static void UpdateMembers(PanStateInternal panState, double totalY)
		{
			if (!panState.IsUserPanning)
			{
				return;
			}

			double difference = panState.PreviousTotalY - totalY;

			if (difference > 0)
			{
				panState.Direction = Direction.Up;
			}
			else if (difference < 0)
			{
				panState.Direction = Direction.Down;
			}

			panState.PreviousTotalY = totalY;
		}

		private static double GetHeight(View view, View barView)
		{
			double height = view.Height;

			if (barView != null)
			{
				height -= barView.Height;
			}

			return height;
		}

		private static double GetHeightWithoutCollapsedView(View view, View collapsedView, View barView)
		{
			return GetHeight(view, barView) - collapsedView.Height;
		}

		private static double NormalizeTotalY(View view, View collapsedView, View barView, double totalY)
		{
			return GetHeight(view, barView) - collapsedView.Height + totalY;
		}
	}
}