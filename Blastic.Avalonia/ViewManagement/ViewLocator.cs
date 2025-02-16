using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Blastic.ViewManagement;
using Blastic.ViewManagement.TypeMappers;

namespace Blastic.Avalonia.ViewManagement;

/// <summary>
/// Default implementation of <see cref="IViewLocator{T}"/> for .NET Maui.
/// </summary>
public class ViewLocator : ViewLocatorBase<StyledElement>
{
	private readonly IServiceProvider _serviceProvider;

	/// <summary>
	/// This value should be set on initialization.
	/// </summary>
	[field: AllowNull, MaybeNull]
	internal static IViewLocator<StyledElement> Current
	{
		get => field ?? throw new InvalidOperationException();
		set => field = value ?? throw new InvalidOperationException();
	}

	/// <inheritdoc />
	public ViewLocator(IServiceProvider serviceProvider, IEnumerable<ITypeMapper> typeMappers) : base(typeMappers)
	{
		_serviceProvider = serviceProvider;
	}

	protected override StyledElement CreateViewOverride(Type viewType)
	{
		if (_serviceProvider.GetService(viewType) is StyledElement element)
		{
			return element;
		}

		return base.CreateViewOverride(viewType);
	}

	/// <inheritdoc />
	protected override void SubscribeViewUnloadEvent(StyledElement view, IViewAware viewAware)
	{
		void OnViewOnUnloaded(object? obj, LogicalTreeAttachmentEventArgs args)
		{
			viewAware.View.Value = null;
			view.DetachedFromLogicalTree -= OnViewOnUnloaded;
		}

		view.DetachedFromLogicalTree += OnViewOnUnloaded;
	}

	/// <inheritdoc />
	protected override StyledElement PostProcessCachedView(StyledElement view)
	{
		return view;
	}

	/// <inheritdoc />
	protected override void PostProcessCreatedView(StyledElement view, object model)
	{
		view.DataContext = model;
	}

	/// <inheritdoc />
	protected override Control CreateNotFoundView(Type type, string message)
	{
		return new TextBlock
		{
			Text = message
		};
	}
}