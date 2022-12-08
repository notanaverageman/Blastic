using System;
using System.Reactive.Disposables;
using Blastic.Commanding;

namespace Blastic.LifetimeManagement
{
	public static class LifetimeExtensions
	{
		/// <summary>
		/// Activate lifetime if it is deactive or vice versa.
		/// </summary>
		/// <param name="lifetime">Lifetime to toggle its activation.</param>
		public static void ToggleActivation(this ILifetime lifetime)
		{
			if (lifetime.IsActive.Value)
			{
				lifetime.Deactivate();
			}
			else
			{
				lifetime.Activate();
			}
		}

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
			LifetimeChainOptions? lifetimeChainOptions = null)
		{
			CompositeDisposable disposable = new();
			lifetimeChainOptions ??= LifetimeChainOptions.All;

			if (lifetimeChainOptions == LifetimeChainOptions.None)
			{
				return Disposable.Empty;
			}

			void Subscribe<T>(Command<T> parent, Command<T> child)
			{
				IDisposable subscription = parent
					.Subscribe(x =>
					{
						child.Execute(x);
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

		/// <summary>
		/// Bind the lifetime of an object to its parent's lifetime.
		/// </summary>
		/// <param name="lifetime">Parent's lifetime.</param>
		/// <param name="childLifetime">Child's lifetime.</param>
		/// <param name="lifetimeChainOptions">Options to manage the child lifetime.</param>
		/// <returns>An <see cref="IDisposable"/> that removes the link between lifetimes when disposed.</returns>
		public static IDisposable AddChildLifetime(
			this IAsyncLifetime lifetime,
			IAsyncLifetime childLifetime,
			LifetimeChainOptions? lifetimeChainOptions = null)
		{
			CompositeDisposable disposable = new();
			lifetimeChainOptions ??= new LifetimeChainOptions();

			void Subscribe<T>(AsyncCommand<T> parent, AsyncCommand<T> child)
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