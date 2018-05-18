using DLC.Framework.IO;
using DLC.Framework.IO.Ports;
using DLC.Framework.Net;
using DLC.Framework.Runtime;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders;
using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Core.Geocoding.Gps;
using DLC.Scientific.Core.Geocoding.Gps.Ncom;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.FtpClient;
using System.Net.FtpClient.Async;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLC.Scientific.Acquisition.Modules.LocalisationModule
{
	public class NcomProvider
		: LocalisationProvider
	{
		private const byte SyncMarker = 0xE7;
		private const int MaxPacketSize = 72;

		public NcomProvider()
			: base()
		{
			this.Frequency = 10;
		}

		public ConnectionType ConnectionType { get; set; }
		public IPAddress RemoteAddress { get; set; }
		public int UdpListenerPort { get; set; }
		public bool ActivateLogRetrieval { get; set; }
		public int UdpSenderPort { get; set; }
		public string SerialPortName { get; set; }
		public BaudRate SerialBaudRate { get; set; }

		protected override void ValidateConfigurationCore()
		{
			base.ValidateConfigurationCore();

			switch (this.ConnectionType)
			{
				case ConnectionType.Serial:
					if (string.IsNullOrEmpty(this.SerialPortName)) throw new InvalidOperationException("SerialPortName is mandatory.");
					break;
				case ConnectionType.Udp:
					if (this.UdpListenerPort < 0 || this.UdpListenerPort > 65535) throw new InvalidOperationException("UdpListenerPort value must be between 0 and 65535.");
					if (this.RemoteAddress == null) throw new InvalidOperationException("RemoteAddress is mandatory.");
					if (this.ActivateLogRetrieval && (this.UdpSenderPort < 0 || this.UdpSenderPort > 65535)) throw new InvalidOperationException("UdpSenderPort value must be between 0 and 65535.");
					break;
				default:
					throw new NotSupportedException(string.Format("Connection type '{0}' is not supported.", this.ConnectionType));
			}
		}

		protected override Task<IObservable<LocalisationData>> InitializeCore()
		{
			return Task.Run(
				() =>
				{
					IObservable<byte[]> packets;

					switch (this.ConnectionType)
					{
						case ConnectionType.Serial:
							packets = ToNcomPacket(SerialConnection.CreateByteListener(this.SerialPortName, this.SerialBaudRate, Parity.None, 8, StopBits.One));
							break;
						case ConnectionType.Udp:
							packets = UdpConnection.CreateListener(this.RemoteAddress, this.UdpListenerPort).Select(udpResult => udpResult.Buffer);
							break;
						default:
							throw new NotSupportedException(string.Format("Connection type '{0}' is not supported.", this.ConnectionType));
					}

					return (
						from geoData in ToGeoData(packets)
						select new LocalisationData {
							RawData = geoData,
							GpsStatus = (IsInitializing(geoData) ? GpsStatus.Initializing : geoData.PositionData.Longitude == 0 && geoData.PositionData.Latitude == 0 ? GpsStatus.SignalLost : GpsStatus.Reliable)
						}).Publish().RefCount();
				});
		}

		private bool IsInitializing(GeoData geoData)
		{
			return geoData.PositionData.InsData.Status != NavigationStatus.Locked
				|| geoData.PrecisionData.NorthPositionAccuracy > 0.6
				|| geoData.PrecisionData.EastPositionAccuracy > 0.6
				|| geoData.PrecisionData.DownPositionAccuracy > 0.9
				|| geoData.PrecisionData.NorthVelocityAccuracy > 0.5
				|| geoData.PrecisionData.EastVelocityAccuracy > 0.5
				|| geoData.PrecisionData.DownVelocityAccuracy > 0.75
				|| geoData.PrecisionData.HeadingAccuracy > 0.35
				|| geoData.PrecisionData.PitchAccuracy > 0.09
				|| geoData.PrecisionData.RollAccuracy > 0.09;
		}

		protected override async Task InitializeRecordCore()
		{
			this.LastTransferredFile = null;

			if (this.ActivateLogRetrieval) //Create new RD file
			{
				using (var updclient = new UdpClient(this.RemoteAddress.ToString(), this.UdpSenderPort))
				{
					byte[] sendOffCommand = Encoding.ASCII.GetBytes("!LOG RD OFF \r\n");
					var udpResult = await updclient.SendAsync(sendOffCommand, sendOffCommand.Length).ConfigureAwait(false);

					byte[] sendOnCommand = Encoding.ASCII.GetBytes("!LOG RD ON \r\n");
					udpResult = await updclient.SendAsync(sendOnCommand, sendOnCommand.Length).ConfigureAwait(false);
				}
			}

			await base.InitializeRecordCore().ConfigureAwait(false);
		}

		protected override Task StopRecordCore()
		{
			if (ActivateLogRetrieval)
				StartDownloadLastRdFile();

			return base.StopRecordCore();
		}

		protected override async Task UninitializeRecordCore()
		{
			if (this.ActivateLogRetrieval)
			{
				var currentTransfer = this.CurrentFileTransfer;
				if (currentTransfer != null && !currentTransfer.IsCanceled && !currentTransfer.IsCompleted && !currentTransfer.IsFaulted)
					await currentTransfer.ConfigureAwait(false);
			}

			await base.UninitializeRecordCore().ConfigureAwait(false);
		}

		private void StartDownloadLastRdFile()
		{
			if (string.IsNullOrEmpty(this.SequenceId)) throw new InvalidOperationException("SequenceId is missing. RD file not downloaded.");
			if (string.IsNullOrEmpty(this.SaveFolderAbsolutePath)) throw new InvalidOperationException("SaveFolderPath is missing. RD file not downloaded.");

			this.IsTransferring = true;

			this.CurrentFileTransfer = Task.Run(
				async () =>
				{
					try
					{
						using (var client = new UdpClient(new IPEndPoint(this.RemoteAddress, this.UdpSenderPort)))
						{
							byte[] sendBytes = Encoding.ASCII.GetBytes("!LOG RD OFF \r\n");
							await client.SendAsync(sendBytes, sendBytes.Length).ConfigureAwait(false);
						}

						using (var ftp = new FtpClient())
						{
							ftp.Host = this.RemoteAddress.ToString();

							await ftp.ConnectAsync().ConfigureAwait(false);

							var configFiles = await ftp.GetNameListingAsync().ConfigureAwait(false);
							string lastRdFileName = configFiles.OrderByDescending(_ => _).FirstOrDefault(filename => filename.EndsWith(".rd"));

							if (lastRdFileName == null)
								throw new InvalidOperationException("No RD file found.");

							lastRdFileName = lastRdFileName.Replace('\\', '/');

							var transferData = new FileTransferData {
								MonitoredFolderPath = string.Format(@"\\{0}", this.RemoteAddress),
								FileName = this.SequenceId + ".rd",
								MachineName = Environment.MachineName,
								DestinationFolderPath = this.SaveFolderAbsolutePath
							};

							this.LastTransferredFile = transferData;

							using (var source = await ftp.OpenReadAsync(lastRdFileName).ConfigureAwait(false))
							using (var destination = File.Open(Path.Combine(transferData.DestinationFolderPath, transferData.FileName), FileMode.OpenOrCreate))
							{
								await IOHelper.Copy(source, destination,
									progressCallback: (copied, total) =>
									{
										this.LastTransferredFile = new FileTransferData(transferData) {
											CopiedBytes = copied,
											TotalBytes = total
										};
									})
									.ConfigureAwait(false);
							}
						}
					}
					finally
					{
						using (var client = new UdpClient(new IPEndPoint(this.RemoteAddress, this.UdpSenderPort)))
						{
							byte[] sendBytes = Encoding.ASCII.GetBytes("!LOG RD ON \r\n");
							client.Send(sendBytes, sendBytes.Length);
						}

						this.IsTransferring = false;
					}
				});
		}

		private static IObservable<byte[]> ToNcomPacket(IObservable<byte> stream)
		{
			if (stream == null) throw new ArgumentNullException("stream");

			return Observable.Create<byte[]>(
				observer =>
				{
					var packet = new List<byte>(MaxPacketSize);

					return stream.Subscribe(
						b =>
						{
							if (b == SyncMarker && packet.Count > 0)
							{
								observer.OnNext(packet.ToArray());
								packet.Clear();
							}
							packet.Add(b);
						});
				});
		}

		private static IObservable<GeoData> ToGeoData(IObservable<byte[]> packets)
		{
			if (packets == null) throw new ArgumentNullException("packets");

			return Observable.Create<GeoData>(
				observer =>
				{
					var gpsTime = NcomRawData.GpsEpoch;
					var data = new GeoData();

					return packets.Subscribe(
						packet =>
						{
							var ncomRawData = new NcomRawData(packet, ref gpsTime);
							if (ncomRawData.NavStatus != (int) NavigationStatus.InternalUse)
							{
								ncomRawData.FillGeoData(data);

								observer.OnNext(data);
								data =CloneHelper.DeepClone(data);
							}
						});
				});
		}
	}
}