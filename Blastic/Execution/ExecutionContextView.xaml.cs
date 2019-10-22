using Bindables;

namespace Blastic.Execution
{
	public partial class ExecutionContextView
	{
		[DependencyProperty]
		public ExecutionContext ExecutionContext { get; set; }

		public ExecutionContextView()
		{
			InitializeComponent();
		}
	}
}