using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Acquisition.Core.AcquisitionProviders
{
	[Serializable]
	public class InvalidStateTransitionException
		: Exception
	{
		public InvalidStateTransitionException() : base() { }
		public InvalidStateTransitionException(string message) : base(message) { }
		public InvalidStateTransitionException(string message, Exception inner) : base(message, inner) { }

		protected InvalidStateTransitionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}