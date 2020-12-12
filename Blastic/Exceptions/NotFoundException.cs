using System;
using System.Runtime.Serialization;

namespace Blastic.Exceptions
{
	/// <summary>
	/// An exception that is thrown when a requested value or information not found.
	/// </summary>
	public class NotFoundException : Exception
	{
		/// <inheritdoc />
		public NotFoundException()
		{
		}

		/// <inheritdoc />
		protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		/// <inheritdoc />
		public NotFoundException(string message) : base(message)
		{
		}

		/// <inheritdoc />
		public NotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}