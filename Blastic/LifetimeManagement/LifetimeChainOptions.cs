namespace Blastic.LifetimeManagement
{
	/// <summary>
	/// Class to manage the lifecycles of children of a <see cref="ConductorBase{T}"/>.
	/// </summary>
	public class LifetimeChainOptions
	{
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
	}
}