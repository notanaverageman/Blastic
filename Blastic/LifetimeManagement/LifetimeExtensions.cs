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