using System;
using System.Runtime.Serialization;
using System.Security;

namespace QbservableProvider
{
	[Serializable]
	internal sealed class ExpressionSecurityException : SecurityException
	{
		public ExpressionSecurityException()
		{
		}

		public ExpressionSecurityException(string message)
			: base(message)
		{
		}

		public ExpressionSecurityException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		private ExpressionSecurityException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}