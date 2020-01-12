namespace Blastic.LifetimeManagement
{
	public class LifetimeChainOptions
	{
		public bool InitializeChildrenOnSelfInitialization { get; set; }
		public bool CloseChildrenOnSelfClose { get; set; }
		public bool ActivateChildrenOnSelfActivation { get; set; }
		public bool DeactivateChildrenOnSelfDeactivation { get; set; }

		public LifetimeChainOptions()
		{
			InitializeChildrenOnSelfInitialization = true;
			CloseChildrenOnSelfClose = true;
			ActivateChildrenOnSelfActivation = true;
			DeactivateChildrenOnSelfDeactivation = true;
		}
	}
}