using System.Threading;

namespace Blastic.Commanding
{
	public class CommandContext
	{
		public CancellationToken CancellationToken { get; }

		public CommandContext(CancellationToken cancellationToken = default)
		{
			CancellationToken = cancellationToken;
		}
	}

	public class CommandContext<T> : CommandContext
	{
		public T Parameter { get; }

		public CommandContext(T parameter, CancellationToken cancellationToken = default) : base(cancellationToken)
		{
			Parameter = parameter;
		}
	}
}