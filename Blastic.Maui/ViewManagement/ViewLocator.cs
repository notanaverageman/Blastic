using System;
using System.Collections.Generic;
using Blastic.DynamicControls;
using Blastic.Maui.DynamicControls;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;
using Microsoft.Maui.Controls;

namespace Blastic.Maui.ViewManagement;

/// <summary>
/// Default implementation of <see cref="IViewLocator{T}"/> for .NET Maui.
/// </summary>
public class ViewLocator : ViewLocatorBase<VisualElement>
{
	private static IViewLocator<VisualElement>? _current;

	/// <summary>
	/// This value should be set on initialization.
	/// </summary>
	internal static IViewLocator<VisualElement> Current
	{
		get => _current ?? throw new InvalidOperationException();
		set => _current = value ?? throw new InvalidOperationException();
	}

	/// <inheritdoc />
	public ViewLocator(IEnumerable<ITypeMapper> typeMappers) : base(typeMappers)
	{
	}

	/// <inheritdoc />
	protected override void SubscribeViewUnloadEvent(VisualElement view, IViewAware viewAware)
	{
		// TODO: Use parent, navigation? https://forums.xamarin.com/discussion/80435/loaded-unloaded-events-for-views
		//view. += (sender, args) =>
		//{
		//	viewAware.View.Value = null;
		//};
	}

	/// <inheritdoc />
	protected override VisualElement PostProcessCachedView(VisualElement view)
	{
		return view;
	}

	/// <inheritdoc />
	protected override void PostProcessCreatedView(VisualElement view, object model)
	{
		view.BindingContext = model;

		if (view is DynamicControl dynamicControl && model is DynamicModel dynamicModel)
		{
			dynamicControl.Model = dynamicModel;
		}
	}

	/// <inheritdoc />
	protected override VisualElement CreateNotFoundView(Type type, string message)
	{
		return new Label
		{
			Text = message
		};
	}
}