using System;

namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// Class to manage the lifecycle interactions of parent and children.
	/// </summary>
	public class LifetimeChainOptions : IEquatable<LifetimeChainOptions>
	{
		public static readonly LifetimeChainOptions All = new();
		public static readonly LifetimeChainOptions None = new(false, false, false, false);

		public static readonly LifetimeChainOptions InitializationClosureOnly = new(
			activateChildrenOnSelfActivation: false,
			deactivateChildrenOnSelfDeactivation: false);

		public static readonly LifetimeChainOptions ActivationDeactivationOnly = new(
			initializeChildrenOnSelfInitialization: false,
			closeChildrenOnSelfClose: false);

		/// <summary>
		/// Initialize the children when the parent is initialized.
		/// </summary>
		public bool InitializeChildrenOnSelfInitialization { get; }

		/// <summary>
		/// Deinitialize the children when the parent is deinitialized.
		/// </summary>
		public bool CloseChildrenOnSelfClose { get; }

		/// <summary>
		/// Activate the children when the parent is activated.
		/// </summary>
		public bool ActivateChildrenOnSelfActivation { get; }

		/// <summary>
		/// Deactivate the children when the parent is deactivated.
		/// </summary>
		public bool DeactivateChildrenOnSelfDeactivation { get; }

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="initializeChildrenOnSelfInitialization">Initialize the children when the parent is initialized.</param>
		/// <param name="closeChildrenOnSelfClose">Deinitialize the children when the parent is deinitialized.</param>
		/// <param name="activateChildrenOnSelfActivation">Activate the children when the parent is activated.</param>
		/// <param name="deactivateChildrenOnSelfDeactivation">Deactivate the children when the parent is deactivated.</param>
		public LifetimeChainOptions(
			bool initializeChildrenOnSelfInitialization = true,
			bool closeChildrenOnSelfClose = true,
			bool activateChildrenOnSelfActivation = true,
			bool deactivateChildrenOnSelfDeactivation = true)
		{
			InitializeChildrenOnSelfInitialization = initializeChildrenOnSelfInitialization;
			CloseChildrenOnSelfClose = closeChildrenOnSelfClose;
			ActivateChildrenOnSelfActivation = activateChildrenOnSelfActivation;
			DeactivateChildrenOnSelfDeactivation = deactivateChildrenOnSelfDeactivation;
		}

		/// <inheritdoc />
		public bool Equals(LifetimeChainOptions? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return 
				InitializeChildrenOnSelfInitialization == other.InitializeChildrenOnSelfInitialization &&
				CloseChildrenOnSelfClose == other.CloseChildrenOnSelfClose &&
				ActivateChildrenOnSelfActivation == other.ActivateChildrenOnSelfActivation &&
				DeactivateChildrenOnSelfDeactivation == other.DeactivateChildrenOnSelfDeactivation;
		}

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((LifetimeChainOptions)obj);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = InitializeChildrenOnSelfInitialization.GetHashCode();
				hashCode = (hashCode * 397) ^ CloseChildrenOnSelfClose.GetHashCode();
				hashCode = (hashCode * 397) ^ ActivateChildrenOnSelfActivation.GetHashCode();
				hashCode = (hashCode * 397) ^ DeactivateChildrenOnSelfDeactivation.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(LifetimeChainOptions? left, LifetimeChainOptions? right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(LifetimeChainOptions? left, LifetimeChainOptions? right)
		{
			return !Equals(left, right);
		}
	}
}