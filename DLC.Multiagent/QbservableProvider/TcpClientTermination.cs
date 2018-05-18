using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;

namespace QbservableProvider
{
	[Serializable]
	public sealed class TcpClientTermination : ISerializable
	{
		public EndPoint LocalEndPoint
		{
			get
			{
				return localEndPoint;
			}
		}

		public EndPoint RemoteEndPoint
		{
			get
			{
				return remoteEndPoint;
			}
		}

		public TimeSpan Duration
		{
			get
			{
				return duration;
			}
		}

		public QbservableProtocolShutDownReason Reason
		{
			get
			{
				return reason;
			}
		}

		public ICollection<ExceptionDispatchInfo> Exceptions
		{
			get
			{
				return exceptions;
			}
		}

		private readonly EndPoint localEndPoint, remoteEndPoint;
		private readonly TimeSpan duration;
		private readonly QbservableProtocolShutDownReason reason;
		private readonly ICollection<ExceptionDispatchInfo> exceptions;

		public TcpClientTermination(
			EndPoint localEndPoint,
			EndPoint remoteEndPoint,
			TimeSpan duration,
			QbservableProtocolShutDownReason reason,
			IEnumerable<ExceptionDispatchInfo> exceptions)
		{
			this.localEndPoint = localEndPoint;
			this.remoteEndPoint = remoteEndPoint;
			this.duration = duration;
			this.reason = reason;
			this.exceptions = (exceptions ?? Enumerable.Empty<ExceptionDispatchInfo>())
				.Distinct(ExceptionDispatchInfoEqualityComparer.Instance)
				.ToList()
				.AsReadOnly();
		}

		private TcpClientTermination(SerializationInfo info, StreamingContext context)
		{
			localEndPoint = (EndPoint) info.GetValue("localEndPoint", typeof(EndPoint));
			remoteEndPoint = (EndPoint) info.GetValue("remoteEndPoint", typeof(EndPoint));
			duration = (TimeSpan) info.GetValue("duration", typeof(TimeSpan));
			reason = (QbservableProtocolShutDownReason) info.GetValue("reason", typeof(QbservableProtocolShutDownReason));
			exceptions = ((List<Exception>) info.GetValue("rawExceptions", typeof(List<Exception>)))
				.Select(ExceptionDispatchInfo.Capture)
				.ToList()
				.AsReadOnly();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("localEndPoint", localEndPoint);
			info.AddValue("remoteEndPoint", remoteEndPoint);
			info.AddValue("duration", duration);
			info.AddValue("reason", reason);

			// ExceptionDispatchInfo is not serializable.
			info.AddValue("rawExceptions", exceptions.Select(ex => ex.SourceException).ToList());

			/* The following line is required; otherwise, the rawExceptions list contains only null 
			 * references when deserialized.  The count remains correct, but the exceptions are null.
			 * Only the first exception needs to be explicitly serialized in order for the entire list
			 * to contain non-null references for all exceptions.  I have no idea why this behavior 
			 * exists and whether it's a bug in .NET.
			 */
			info.AddValue("ignored", exceptions.Select(ex => ex.SourceException).FirstOrDefault());
		}
	}
}