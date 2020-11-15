using System;
using System.Reactive.Disposables;
using Blastic.Commanding;
using Blastic.Ordering;

namespace Blastic.LifetimeManagement
{
	public static class LifetimeExtensions
	{
		public static IDisposable AddChildLifetime(
			this ILifetime lifetime,
			ILifetime childLifetime,
			LifetimeChainOptions lifetimeChainOptions,
			Order? order = null)
		{
			CompositeDisposable disposable = new CompositeDisposable();

			void Subscribe<T>(Command<T> parent, Command<T> child)
			{
				IDisposable subscription = parent
					.Subscribe(async x =>
					{
						await child.Execute(x);
					}, order);

				disposable.Add(subscription);
			}

			if (lifetimeChainOptions.InitializeChildrenOnSelfInitialization)
			{
				Subscribe(lifetime.Initialization, childLifetime.Initialization);
			}

			if (lifetimeChainOptions.CloseChildrenOnSelfClose)
			{
				Subscribe(lifetime.CanClose, childLifetime.CanClose);
				Subscribe(lifetime.Closure, childLifetime.Closure);
			}

			if (lifetimeChainOptions.ActivateChildrenOnSelfActivation)
			{
				Subscribe(lifetime.Activation, childLifetime.Activation);
			}

			if (lifetimeChainOptions.DeactivateChildrenOnSelfDeactivation)
			{
				Subscribe(lifetime.Deactivation, childLifetime.Deactivation);
			}

			return disposable;
		}
	}
}