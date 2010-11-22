using System;
using System.Runtime.Serialization;

namespace FileGenerator.AddIn
{
	[Serializable]
	public sealed class FileGenerationException : Exception
	{
		public FileGenerationException()
			: base()
		{
		}

		public FileGenerationException(string message)
			: base(message)
		{
		}

		public FileGenerationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		private FileGenerationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
