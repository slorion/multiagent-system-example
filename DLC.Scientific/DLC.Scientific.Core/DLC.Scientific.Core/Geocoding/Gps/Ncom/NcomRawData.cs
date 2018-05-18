using DLC.Scientific.Core.Geocoding.Gps.Ncom.Channels;
using System;

namespace DLC.Scientific.Core.Geocoding.Gps.Ncom
{
	public class NcomRawData
	{
		public static readonly DateTime GpsEpoch = new DateTime(1980, 01, 06, 0, 0, 0, DateTimeKind.Unspecified);

		public NcomRawData(byte[] packet, ref DateTime lastGpsTime)
		{
			if (packet == null) throw new ArgumentNullException("packet");
			if (packet.Length < 72) throw new ArgumentException("The packet size must contains at least 72 bytes.");

			this.Packet = packet;
			this.Channel = Channel.Empty;
			this.NavStatus = packet[21];

			if (this.NavStatus != (int) NavigationStatus.InternalUse)
			{
				this.ParseBatch1(packet);
				this.ParseBatch2(packet);
				this.ParseBatch3(packet);

				if (this.Channel.ChannelNumber == 0)
					this.GpsTime = UpdateGpsTime(lastGpsTime, ((Channel0) this.Channel).Minutes, this.Milliseconds);
				else
					this.GpsTime = UpdateGpsTime(lastGpsTime, this.Milliseconds);

				lastGpsTime = this.GpsTime;
			}
		}

		private static DateTime UpdateGpsTime(DateTime lastGpsTime, long minutes, int milliseconds)
		{
			var updated = GpsEpoch.AddMinutes(minutes);
			updated = updated.AddMilliseconds(milliseconds);
			return updated;
		}

		private static DateTime UpdateGpsTime(DateTime lastGpsTime, int milliseconds)
		{
			//NOTE: if not GPS signal, milliseconds will be equal to 65,535 and elapsed time will be superior to one minute

			int finalMilliseconds;
			int finalSeconds = Math.DivRem(milliseconds, 1000, out finalMilliseconds);
			int finalMinutesToAdd = Math.DivRem(finalSeconds, 60, out finalSeconds);

			var updated = new DateTime(lastGpsTime.Year, lastGpsTime.Month, lastGpsTime.Day, lastGpsTime.Hour, lastGpsTime.Minute + finalMinutesToAdd, finalSeconds, finalMilliseconds);

			// assume that if current milliseconds value is below last value, then we are in the next minute
			if (milliseconds < (lastGpsTime.Second * 1000 + lastGpsTime.Millisecond))
				updated = updated.AddMinutes(1);

			return updated;
		}

		internal byte[] Packet { get; private set; }

		public DateTime GpsTime { get; private set; }

		public int Milliseconds { get; private set; }
		public double AccelerationX { get; private set; }
		public double AccelerationY { get; private set; }
		public double AccelerationZ { get; private set; }
		public double AngularRateX { get; private set; }
		public double AngularRateY { get; private set; }
		public double AngularRateZ { get; private set; }
		public int NavStatus { get; private set; } // enum (table 5)

		public double Latitude { get; private set; }
		public double Longitude { get; private set; }
		public double Altitude { get; private set; }
		public double NorthVelocity { get; private set; }
		public double EastVelocity { get; private set; }
		public double DownVelocity { get; private set; }
		public double Heading { get; private set; }
		public double Pitch { get; private set; }
		public double Roll { get; private set; }

		public Channel Channel { get; private set; }

		public void FillGeoData(GeoData geoData)
		{
			if (geoData == null) throw new ArgumentNullException("geoData");

			geoData.DeviceType = GpsDeviceType.Inertial;

			if (geoData.PositionData == null) geoData.PositionData = new PositionData();
			if (geoData.PrecisionData == null) geoData.PrecisionData = new PrecisionData();
			if (geoData.VelocityData == null) geoData.VelocityData = new VelocityData();

			FillPositionData(geoData.PositionData);
			FillPrecisionData(geoData.PrecisionData);
			FillVelocityData(geoData.VelocityData);
		}

		public void FillPositionData(PositionData positionData)
		{
			if (positionData == null) throw new ArgumentNullException("positionData");

			positionData.Latitude = this.Latitude;
			positionData.Longitude = this.Longitude;
			positionData.Altitude = this.Altitude;
			positionData.Utc = this.GpsTime;

			switch (this.Channel.ChannelNumber)
			{
				case 0:
					var channel0 = (Channel0) this.Channel;

					positionData.NbSatellites = channel0.NumberOfSatellites;

					switch (channel0.PositionMode)
					{
						case 2:
						case 3:
							positionData.Quality = FixType.Fix;
							break;
						case 4:
						case 8:
						case 18:
							positionData.Quality = FixType.Diff;
							break;
						case 5:
						case 9:
						case 17:
							positionData.Quality = FixType.RTKfloating;
							break;
						case 6:
							positionData.Quality = FixType.RTKfixed;
							break;
						case 7:
							positionData.Quality = FixType.WAAS;
							break;
						case 12:
						case 13:
						case 14:
						case 15:
						case 16:
							positionData.Quality = FixType.PostProcess;
							break;
						default:
							positionData.Quality = FixType.None;
							break;
					}
					break;
				case 20:
					var channel20 = (Channel20) this.Channel;
					positionData.DifferentialDataAge = channel20.AgeDiffRefStationUpdate;
					positionData.DifferentialStationId = channel20.DifferentialStationId.ToString();
					break;
				case 48:
					var channel48 = (Channel48) this.Channel;
					positionData.GeoIdHeight = channel48.Undulation;
					break;
			}

			positionData.InsData = new InsData {
				AccelerationX = this.AccelerationX,
				AccelerationY = this.AccelerationY,
				AccelerationZ = this.AccelerationZ,
				AngularRateX = this.AngularRateX,
				AngularRateY = this.AngularRateY,
				AngularRateZ = this.AngularRateZ,
				NorthVelocity = this.NorthVelocity,
				EastVelocity = this.EastVelocity,
				DownVelocity = this.DownVelocity,
				Heading = this.Heading,
				Pitch = this.Pitch,
				Roll = this.Roll,
			};

			if (Enum.IsDefined(typeof(NavigationStatus), this.NavStatus))
				positionData.InsData.Status = (NavigationStatus) this.NavStatus;
			else
				positionData.InsData.Status = NavigationStatus.Others;
		}

		public void FillVelocityData(VelocityData velocityData)
		{
			if (velocityData == null) throw new ArgumentNullException("velocityData");

			double speed = this.NorthVelocity * this.NorthVelocity + this.EastVelocity * this.EastVelocity;
			velocityData.SpeedMs = Math.Sqrt(speed);
		}

		public void FillPrecisionData(PrecisionData precisionData)
		{
			if (precisionData == null) throw new ArgumentNullException("precisionData");

			switch (this.Channel.ChannelNumber)
			{
				case 3:
					var channel3 = (Channel3) this.Channel;
					precisionData.NorthPositionAccuracy = channel3.NorthPositionAccuracy;
					precisionData.EastPositionAccuracy = channel3.EastPositionAccuracy;
					precisionData.DownPositionAccuracy = channel3.DownPositionAccurady;
					break;
				case 4:
					var channel4 = (Channel4) this.Channel;
					precisionData.NorthVelocityAccuracy = channel4.NorthVelocityAccuracy;
					precisionData.EastVelocityAccuracy = channel4.EastVelocityAccuracy;
					precisionData.DownVelocityAccuracy = channel4.DownVelocityAccuracy;
					break;
				case 5:
					var channel5 = (Channel5) this.Channel;
					precisionData.HeadingAccuracy = channel5.HeadingAccuracy;
					precisionData.PitchAccuracy = channel5.PitchAccuracy;
					precisionData.RollAccuracy = channel5.RollAccuracy;
					break;
				case 48:
					var channel48 = (Channel48) this.Channel;
					precisionData.Hdop = channel48.Hdop;
					precisionData.Pdop = channel48.Pdop;
					precisionData.Vdop = channel48.Vdop;
					break;
			}
		}

		private static void ValidateChecksum(byte checksum, byte[] bytes, int endIndex)
		{
			if (bytes == null) throw new ArgumentNullException("bytes");
			if (endIndex >= bytes.Length) throw new ArgumentOutOfRangeException("endIndex");

			byte actual = 0;
			unchecked
			{
				// always skip the first byte (Sync byte)
				for (int i = 1; i <= endIndex; i++)
					actual += bytes[i];
			}

			if (actual != checksum)
				throw new ArgumentException("The checksum check failed.", "bytes");
		}

		private void ParseBatch1(byte[] bytes)
		{
			if (bytes == null) throw new ArgumentNullException("bytes");

			ValidateChecksum(bytes[22], bytes, 21);

			// Time ( range from 0 to 59 999 ms)
			this.Milliseconds = bytes[1] + (bytes[2] << 8);

			// Accelerations : vehicle body-frame accelerations in the x, y, z-directions
			// signed words in units of 10^-4 m/s^2
			int accX = bytes[3] + (bytes[4] << 8) + (bytes[5] << 16);
			if (bytes[5] >= 128)
				accX = accX.TwosComplementFromInt24();
			this.AccelerationX = accX * Constants.Acc2Mps2;

			int accY = bytes[6] + (bytes[7] << 8) + (bytes[8] << 16);
			if (bytes[8] >= 128)
				accY = accY.TwosComplementFromInt24();
			this.AccelerationY = accY * Constants.Acc2Mps2;

			int accZ = bytes[9] + (bytes[10] << 8) + (bytes[11] << 16);
			if (bytes[11] >= 128)
				accZ = accZ.TwosComplementFromInt24();
			this.AccelerationZ = accZ * Constants.Acc2Mps2;

			// Angular rates : vehicle body-frame angular rates in the x, y, z-directions
			// signed words in units of 10^-5 radians/s
			int rateX = bytes[12] + (bytes[13] << 8) + (bytes[14] << 16);
			if (bytes[14] >= 128)
				rateX = rateX.TwosComplementFromInt24();
			this.AngularRateX = rateX * Constants.Rate2Rps * Constants.Rad2Deg;

			int rateY = bytes[15] + (bytes[16] << 8) + (bytes[17] << 16);
			if (bytes[17] >= 128)
				rateY = rateY.TwosComplementFromInt24();
			this.AngularRateY = rateY * Constants.Rate2Rps * Constants.Rad2Deg;

			int rateZ = bytes[18] + (bytes[19] << 8) + (bytes[20] << 16);
			if (bytes[20] >= 128)
				rateZ = rateZ.TwosComplementFromInt24();
			this.AngularRateZ = rateZ * Constants.Rate2Rps * Constants.Rad2Deg;
		}

		private void ParseBatch2(byte[] bytes)
		{
			if (bytes == null) throw new ArgumentNullException("bytes");

			ValidateChecksum(bytes[61], bytes, 60);

			// Latitude and Longitude
			// double in units of radians
			byte[] latitudeByte =
				{
					bytes[30], bytes[29], bytes[28], bytes[27], bytes[26], bytes[25], bytes[24],
					bytes[23]
				};
			this.Latitude = latitudeByte.ToDouble() * Constants.Rad2Deg;

			byte[] longitudeByte =
				{
					bytes[38], bytes[37], bytes[36], bytes[35], bytes[34], bytes[33], bytes[32],
					bytes[31]
				};
			this.Longitude = longitudeByte.ToDouble() * Constants.Rad2Deg;

			// Altitude
			// float in units of meters
			byte[] altitude = { 0, 0, 0, 0, bytes[42], bytes[41], bytes[40], bytes[39] };
			this.Altitude = altitude.ToSingle();

			// North, East and Down velocities
			// in units of 10^-4 m/s
			int northVelocity = bytes[43] + (bytes[44] << 8) + (bytes[45] << 16);
			if (bytes[45] >= 128)
				northVelocity = northVelocity.TwosComplementFromInt24();
			this.NorthVelocity = northVelocity * Constants.Vel2Mps;

			int eastVelocity = bytes[46] + (bytes[47] << 8) + (bytes[48] << 16);
			if (bytes[48] >= 128)
				eastVelocity = eastVelocity.TwosComplementFromInt24();
			this.EastVelocity = eastVelocity * Constants.Vel2Mps;

			int downVelocity = bytes[49] + (bytes[50] << 8) + (bytes[51] << 16);
			if (bytes[51] >= 128)
				downVelocity = downVelocity.TwosComplementFromInt24();
			this.DownVelocity = downVelocity * Constants.Vel2Mps;

			// Heading
			// in units of 10^-6 radians (range from -pi to + pi)
			int heading = bytes[52] + (bytes[53] << 8) + (bytes[54] << 16);
			if (bytes[54] >= 128)
				heading = heading.TwosComplementFromInt24();
			this.Heading = heading * Constants.Ang2Rad * Constants.Rad2Deg;

			if (this.Heading < 0)
				this.Heading = this.Heading + 360;

			// Pitch
			// in units of 10^-6 radians (range from -pi/2 to + pi/2)
			int pitch = bytes[55] + (bytes[56] << 8) + (bytes[57] << 16);
			if (bytes[57] >= 128)
				pitch = pitch.TwosComplementFromInt24();
			this.Pitch = pitch * Constants.Ang2Rad * Constants.Rad2Deg;

			// Roll
			// in units of 10^-6 radians (range from -pi to + pi)
			int roll = bytes[58] + (bytes[59] << 8) + (bytes[60] << 16);
			if (bytes[60] >= 128)
				roll = roll.TwosComplementFromInt24();
			this.Roll = roll * Constants.Ang2Rad * Constants.Rad2Deg;
		}

		private void ParseBatch3(byte[] bytes)
		{
			if (bytes == null) throw new ArgumentNullException("bytes");

			ValidateChecksum(bytes[71], bytes, 70);

			int channelNumber = bytes[62];

			switch (channelNumber)
			{
				case 0:
					this.Channel = new Channel0(this);
					break;
				case 3:
					this.Channel = new Channel3(this);
					break;
				case 4:
					this.Channel = new Channel4(this);
					break;
				case 5:
					this.Channel = new Channel5(this);
					break;
				case 13:
					this.Channel = new Channel13(this);
					break;
				case 14:
					this.Channel = new Channel14(this);
					break;
				case 20:
					this.Channel = new Channel20(this);
					break;
				case 48:
					this.Channel = new Channel48(this);
					break;
			}
		}
	}
}