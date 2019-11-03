using System;
using System.Reactive.Disposables;
using Blastic.Common;
using Blastic.Reactive;
using Reactive.Bindings.Extensions;

namespace Blastic.LifetimeManagement
{
	public static class LifetimeExtensions
	{
		public static IDisposable AddChildLifetime(
			this ILifetime lifetime,
			ILifetime childLifetime,
			LifetimeChainOptions lifetimeChainOptions,
			Order order = null)
		{
			CompositeDisposable disposable = new CompositeDisposable();

			void Subscribe<T>(AsyncCommand<T> parent, AsyncCommand<T> child)
			{
				parent
					.Subscribe(async x =>
					{
						await child.Execute(x);
					}, order)
					.AddTo(disposable);
			}

			if (lifetimeChainOptions.InitializeChildrenOnSelfInitialization)
			{
				Subscribe(lifetime.Initialize, childLifetime.Initialize);
			}

			if (lifetimeChainOptions.CloseChildrenOnSelfClose)
			{
				Subscribe(lifetime.CanClose, childLifetime.CanClose);
				Subscribe(lifetime.Close, childLifetime.Close);
			}

			if (lifetimeChainOptions.ActivateChildrenOnSelfActivation)
			{
				Subscribe(lifetime.Activate, childLifetime.Activate);
			}

			if (lifetimeChainOptions.DeactivateChildrenOnSelfDeactivation)
			{
				Subscribe(lifetime.Deactivate, childLifetime.Deactivate);
			}

			return disposable;
		}
	}
}