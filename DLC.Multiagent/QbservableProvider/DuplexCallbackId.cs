using System;

namespace QbservableProvider
{
	public struct DuplexCallbackId : IEquatable<DuplexCallbackId>
	{
		internal const int Size = 8;

		public int ClientId
		{
			get
			{
				return (int) id;
			}
		}

		public int ServerId
		{
			get
			{
				return (int) (id >> 32);
			}
		}

		private readonly long id;

		public DuplexCallbackId(long id)
		{
			this.id = id;
		}

		public DuplexCallbackId(int clientId, int serverId)
			: this(clientId + ((long) serverId << 32))
		{
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is DuplexCallbackId && Equals((DuplexCallbackId) obj);
		}

		public bool Equals(DuplexCallbackId other)
		{
			return id == other.id;
		}

		public DuplexCallbackId WithClientId(int clientId)
		{
			return new DuplexCallbackId(clientId, this.ServerId);
		}

		public static bool operator ==(DuplexCallbackId first, DuplexCallbackId second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(DuplexCallbackId first, DuplexCallbackId second)
		{
			return !first.Equals(second);
		}

		public static implicit operator DuplexCallbackId(long id)
		{
			return new DuplexCallbackId(id);
		}

		public static implicit operator long(DuplexCallbackId id)
		{
			return id.id;
		}

		public override string ToString()
		{
			return "{ClientId = " + ClientId + ", ServerId = " + ServerId + '}';
		}
	}
}