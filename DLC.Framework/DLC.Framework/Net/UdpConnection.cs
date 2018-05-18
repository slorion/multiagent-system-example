using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;

namespace DLC.Framework.Net
{
	public static class UdpConnection
	{
		public static IObservable<UdpReceiveResult> CreateListener(IPAddress remoteAddress, int port)
		{
			if (remoteAddress == null) throw new ArgumentNullException("remoteAddress");
			if (port < 0 || port > 65535) throw new ArgumentOutOfRangeException("port");

			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

			var remoteIPBytes = remoteAddress.GetAddressBytes();

			var localAddress =
				host.AddressList.FirstOrDefault(ip =>
				ip.AddressFamily == remoteAddress.AddressFamily
				&& ip.GetAddressBytes().Take(3).SequenceEqual(remoteIPBytes.Take(3)));

			if (localAddress == null)
			{
				throw new InvalidOperationException(
					string.Format("There is no interface in the same network of the target IP address for UDP listening. Target IP : {0} on port {1}",
					remoteAddress, port));
			}

			var localEndPoint = new IPEndPoint(localAddress, port);
			var remoteEndPoint = new IPEndPoint(remoteAddress, port);

			return Observable.Using(
				() =>
				{
					var client = new UdpClient { ExclusiveAddressUse = false };
					client.Client.Bind(localEndPoint);
					client.Connect(remoteEndPoint.Address, 0);
					return client;
				},
				client => Observable.While(() => client.Client.Connected, Observable.FromAsync(client.ReceiveAsync)));
		}
	}
}