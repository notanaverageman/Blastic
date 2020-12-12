using System;
using System.Reactive.Disposables;
using Blastic.Commanding;

namespace Blastic.LifetimeManagement
{
	public static class LifetimeExtensions
	{
		/// <summary>
		/// Bind the lifetime of an object to its parent's lifetime.
		/// </summary>
		/// <param name="lifetime">Parent's lifetime.</param>
		/// <param name="childLifetime">Child's lifetime.</param>
		/// <param name="lifetimeChainOptions">Options to manage the child lifetime.</param>
		/// <returns>An <see cref="IDisposable"/> that removes the link between lifetimes when disposed.</returns>
		public static IDisposable AddChildLifetime(
			this ILifetime lifetime,
			ILifetime childLifetime,
			LifetimeChainOptions lifetimeChainOptions)
		{
			CompositeDisposable disposable = new CompositeDisposable();

			void Subscribe<T>(Command<T> parent, Command<T> child)
			{
				IDisposable subscription = parent
					.Subscribe(async x =>
					{
						await child.Execute(x);
					});

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