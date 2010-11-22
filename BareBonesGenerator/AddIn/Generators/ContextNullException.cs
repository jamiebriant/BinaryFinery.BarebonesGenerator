using System;
using System.Runtime.Serialization;

namespace FileGenerator.AddIn.Generators
{
	[Serializable]
	public sealed class ContextNullException : Exception
	{
		public ContextNullException()
			: base()
		{
		}

		public ContextNullException(string message)
			: base(message)
		{
		}

		public ContextNullException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		private ContextNullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

