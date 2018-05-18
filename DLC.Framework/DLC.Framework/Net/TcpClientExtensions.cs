using System;
using System.IO;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;

namespace DLC.Framework.Net
{
	public static class TcpClientExtensions
	{
		public static IObservable<string> ToObservable(this TcpClient tcpClient, Encoding encoding)
		{
			if (tcpClient == null) throw new ArgumentNullException("tcpClient");
			if (encoding == null) throw new ArgumentNullException("encoding");

			return Observable.While(() => tcpClient.Connected, Observable.FromAsync(new StreamReader(tcpClient.GetStream(), encoding).ReadLineAsync));
		}
	}
}