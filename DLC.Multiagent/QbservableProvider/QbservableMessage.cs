namespace QbservableProvider
{
	internal class QbservableMessage : IProtocolMessage
	{
		public QbservableProtocolMessageKind Kind
		{
			get
			{
				return kind;
			}
		}

		public byte[] Data
		{
			get
			{
				return data;
			}
		}

		public long Length
		{
			get
			{
				return length;
			}
		}

		public bool Handled
		{
			get;
			set;
		}

		private readonly QbservableProtocolMessageKind kind;
		private readonly byte[] data;
		private readonly long length;

		public QbservableMessage(QbservableProtocolMessageKind kind, params byte[] data)
			: this(kind, data, data == null ? 0 : data.Length)
		{
		}

		public QbservableMessage(QbservableProtocolMessageKind kind, byte[] data, long length)
		{
			this.kind = kind;
			this.data = data;
			this.length = length;
		}

		public override string ToString()
		{
			return "{" + kind + ", Length = " + length + "}";
		}
	}
}