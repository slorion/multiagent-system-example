// System.IO.Ports.SerialPort is not used because of problems highlighted in these articles:
// http://www.sparxeng.com/blog/software/must-use-net-system-io-ports-serialport
// http://zachsaw.blogspot.ca/2010/07/net-serialport-woes.html

using System;
using System.IO;
using System.IO.Ports;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using RJCPIO = RJCP.IO.Ports;

namespace DLC.Framework.IO.Ports
{
	public static class SerialConnection
	{
		public static IObservable<byte> CreateByteListener(string portName, BaudRate baudRate, Parity parity, int dataBits, StopBits stopBits, int? bufferSize = null)
		{
			return CreateListener<byte>(portName, baudRate, parity, dataBits, stopBits, Encoding.ASCII, bufferSize ?? (int) baudRate,
				(port, buffer) => port.ReadAsync(buffer, 0, buffer.Length));
		}

		public static IObservable<char> CreateCharListener(string portName, BaudRate baudRate, Parity parity, int dataBits, StopBits stopBits, int? bufferSize = null)
		{
			return CreateCharListener(portName, baudRate, parity, dataBits, stopBits, Encoding.ASCII, bufferSize);
		}

		public static IObservable<char> CreateCharListener(string portName, BaudRate baudRate, Parity parity, int dataBits, StopBits stopBits, Encoding encoding, int? bufferSize = null)
		{
			return CreateListener<char>(portName, baudRate, parity, dataBits, stopBits, encoding, bufferSize ?? (int) baudRate,
				(port, buffer) =>
				{
					using (var sr = new StreamReader(port, encoding, true, buffer.Length, true))
					{
						return sr.ReadAsync(buffer, 0, buffer.Length);
					}
				});
		}

		private static IObservable<T> CreateListener<T>(string portName, BaudRate baudRate, Parity parity, int dataBits, StopBits stopBits, Encoding encoding, int bufferSize, Func<RJCPIO.SerialPortStream, T[], Task<int>> readFromPort)
		{
			if (string.IsNullOrEmpty(portName)) throw new ArgumentNullException("portName");
			if (encoding == null) throw new ArgumentNullException("encoding");
			if (bufferSize < 1) throw new ArgumentOutOfRangeException("bufferSize", bufferSize, string.Empty);

			var buffer = new T[bufferSize];

			return Observable.Using(
				() =>
				{
					RJCPIO.Parity rjcpParity;
					switch (parity)
					{
						case Parity.Even: rjcpParity = RJCPIO.Parity.Even; break;
						case Parity.Mark: rjcpParity = RJCPIO.Parity.Mark; break;
						case Parity.None: rjcpParity = RJCPIO.Parity.None; break;
						case Parity.Odd: rjcpParity = RJCPIO.Parity.Odd; break;
						case Parity.Space: rjcpParity = RJCPIO.Parity.Space; break;
						default: throw new NotSupportedException(string.Format("'{0}' is not supported.", parity));
					}

					RJCPIO.StopBits rjcpStopBits;
					switch (stopBits)
					{
						case StopBits.One: rjcpStopBits = RJCPIO.StopBits.One; break;
						case StopBits.OnePointFive: rjcpStopBits = RJCPIO.StopBits.One5; break;
						case StopBits.Two: rjcpStopBits = RJCPIO.StopBits.Two; break;
						default: throw new NotSupportedException(string.Format("'{0}' is not supported.", stopBits));
					}

					var port = new RJCPIO.SerialPortStream(portName, (int) baudRate, dataBits, rjcpParity, rjcpStopBits) { NewLine = "\r\n", Encoding = encoding };
					port.Open();

					return port;
				},
				port =>
				{
					int? count = null;

					return Observable.While(() => port.IsOpen && (count == null || count > 0),
						Observable.Defer(
							async () =>
							{
								try
								{
									count = await readFromPort(port, buffer).ConfigureAwait(false);

									if (count <= 0)
									{
										return Notification.CreateOnCompleted<T>().ToObservable();
									}
									else
									{
										var received = new T[count.Value];
										Array.Copy(buffer, 0, received, 0, count.Value);
										return received.ToObservable();
									}
								}
								catch (Exception ex)
								{
									return Notification.CreateOnError<T>(ex).ToObservable();
								}
							}));
				});
		}
	}
}