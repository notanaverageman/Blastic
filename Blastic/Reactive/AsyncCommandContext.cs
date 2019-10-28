namespace Blastic.Reactive
{
	public class AsyncCommandContext
	{
		public bool ContinueExecution { get; set; }

		public AsyncCommandContext()
		{
			ContinueExecution = true;
		}
	}

	public class AsyncCommandContext<T> : AsyncCommandContext
	{
		public T Parameter { get; }

		public AsyncCommandContext(T parameter)
		{
			Parameter = parameter;
		}
	}
}