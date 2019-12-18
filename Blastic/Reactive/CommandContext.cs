namespace Blastic.Reactive
{
	public class CommandContext
	{
		public bool ContinueExecution { get; set; }

		public CommandContext()
		{
			ContinueExecution = true;
		}
	}

	public class CommandContext<T> : CommandContext
	{
		public T Parameter { get; }

		public CommandContext(T parameter)
		{
			Parameter = parameter;
		}
	}
}