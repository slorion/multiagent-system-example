using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace DLC.Multiagent
{
	[DataContract]
	[Serializable]
	public class PeerNode
	{
		internal static async Task<PeerNode> Create(string host, int port, int rxPort, string description = null)
		{
			if (string.IsNullOrEmpty(host)) throw new ArgumentNullException("host");

			IPAddress ip;
			if (string.Equals(host, "auto", StringComparison.OrdinalIgnoreCase))
			{
				ip = (await Dns.GetHostAddressesAsync(Dns.GetHostName()).ConfigureAwait(false)).First(a => a.AddressFamily == AddressFamily.InterNetwork);
				host = ip.ToString();
			}
			else if (!IPAddress.TryParse(host, out ip))
			{
				ip = (await Dns.GetHostAddressesAsync(host).ConfigureAwait(false)).First(a => a.AddressFamily == AddressFamily.InterNetwork);
			}

			if (string.IsNullOrEmpty(description))
				description = string.Format("{0}:{1}", host, port);

			return Create(ip, port, rxPort, description);
		}

		internal static PeerNode Create(IPAddress ip, int port, int rxPort, string description = null)
		{
			if (ip == null) throw new ArgumentNullException("ip");

			return new PeerNode(ip, port, rxPort, description);
		}

		private PeerNode(IPAddress host, int port, int rxPort, string description)
		{
			if (host == null) throw new ArgumentNullException("host");
			if (port <= 0 || port > 65535) throw new ArgumentOutOfRangeException("port");
			if (rxPort <= 0 || rxPort > 65535) throw new ArgumentOutOfRangeException("rxPort");

			this.Host = host;
			this.Port = port;
			this.RxPort = rxPort;
			this.Description = description;
		}

		[DataMember]
		public IPAddress Host { get; private set; }

		[DataMember]
		public int Port { get; private set; }

		[DataMember]
		public int RxPort { get; private set; }

		[DataMember]
		public string Description { get; private set; }

		public override string ToString()
		{
			return this.Description;
		}

		public bool Equals(PeerNode other)
		{
			if (other == null)
				return false;
			else
				return this.Host == other.Host && this.Port == other.Port;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as PeerNode);
		}

		public override int GetHashCode()
		{
			int hash = 17;
			unchecked
			{
				hash = hash * 23 + this.Host.GetHashCode();
				hash = hash * 23 + this.Port.GetHashCode();
			}

			return hash;
		}

		public static bool operator ==(PeerNode x, PeerNode y)
		{
			if (object.ReferenceEquals(x, y))
				return true;
			else if ((object) x == null || (object) y == null)
				return false;
			else
				return x.Equals(y);
		}

		public static bool operator !=(PeerNode x, PeerNode y)
		{
			return !(x == y);
		}
	}
}