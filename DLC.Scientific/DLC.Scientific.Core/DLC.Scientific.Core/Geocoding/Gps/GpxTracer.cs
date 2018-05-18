using DLC.Framework;
using NLog.Fluent;
using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace DLC.Scientific.Core.Geocoding.Gps
{
	public sealed class GpxTracer
		: IDisposable
	{
		private bool _capture;
		private XmlWriter _output;

		/// <summary>
		/// Initializes a new instance of the <see cref="GpxTracer"/> class.
		/// </summary>
		public GpxTracer(string fileExtension, bool append)
		{
			this.FileExtension = fileExtension ?? string.Empty;
			this.Append = append;
		}

		public string FileExtension { get; private set; }
		public bool Append { get; private set; }

		public bool IsClosed
		{
			get { return _output == null || _output.WriteState == WriteState.Closed; }
		}

		public string StartTracing(string essaiId, string savePath, GpsDeviceType gpsDeviceType)
		{
			var extension = new MetaDataExtension { DeviceType = gpsDeviceType };
			return StartTracing(essaiId, savePath, extension);
		}

		/// <summary>
		/// Starts the tracing.
		/// </summary>
		public string StartTracing(string essaiId, string savePath, MetaDataExtension extension)
		{
			if (string.IsNullOrEmpty(savePath)) throw new ArgumentNullException("savePath");
			if (extension == null) throw new ArgumentNullException("extension");

			string outputFullPath;

			Log.Debug().Message("Démarrage de la trace GPS. AcquisitionId: '{0}'.", essaiId).Write();

			try
			{
				string prefix = this.Append ? null : essaiId;
				outputFullPath = Path.Combine(savePath, prefix + "." + this.FileExtension);

				try
				{
					if (!Directory.Exists(savePath))
						Directory.CreateDirectory(savePath);
				}
				catch (IOException ex)
				{
					Log.Warn().Exception(ex).Write();
				}

				_output = XmlWriter.Create(outputFullPath, new XmlWriterSettings { Indent = true });

				_output.WriteStartDocument();
				{
					_output.WriteStartElement("gpx", "http://www.topografix.com/GPX/1/1");

					_output.WriteAttributeString("version", "2.0");
					_output.WriteAttributeString("creator", "Transports Québec");
					_output.WriteAttributeString("xsi", "schemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd");
					_output.WriteAttributeString("xmlns", "ins", null, "http://www.w3.org/2001/XMLSchema-instance/ins");
					_output.WriteAttributeString("xmlns", "dlc", null, "http://www.w3.org/2001/XMLSchema-instance/dlc");

					{
						_output.WriteStartElement("metadata");

						_output.WriteElementString("name", essaiId);
						_output.WriteElementString("time", DateTimePrecise.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"));

						{
							_output.WriteStartElement("extensions");
							{
								if (!String.IsNullOrEmpty(extension.CreationSource))
									_output.WriteElementString("CreationSource", extension.CreationSource);
								if (!String.IsNullOrEmpty(extension.CreationSourceType))
									_output.WriteElementString("CreationSourceType", extension.CreationSourceType);
								if (!String.IsNullOrEmpty(extension.CreationSourceVersion))
									_output.WriteElementString("CreationSourceVersion", extension.CreationSourceVersion);
								if (!String.IsNullOrEmpty(extension.FileStructVersion))
									_output.WriteElementString("FileStructVersion", extension.FileStructVersion);
								_output.WriteElementString("gpsDeviceType", Convert.ToString(extension.DeviceType));
							}
							_output.WriteEndElement();
						}

						_output.WriteEndElement();
					}
				}

				_output.WriteStartElement("rte");
				_output.Flush();
				_capture = true;
			}
			catch (Exception ex)
			{
				//Console.WriteLine(ex.Message);
				outputFullPath = string.Empty;
				Log.Warn().Exception(ex).Write();
			}

			return outputFullPath;
		}

		/// <summary>
		/// Stops the tracing.
		/// </summary>
		public void StopTracing()
		{
			Log.Debug().Message("Arrêt de la trace GPS.").Write();

			try
			{
				if (!_capture)
					return;

				_capture = false;

				if (_output != null)
				{
					_output.WriteEndElement();
					_output.WriteEndElement();

					_output.Close();
				}
			}
			catch (Exception ex)
			{
				Log.Warn().Exception(ex).Write();
			}
		}

		/// <summary>
		/// Traces the position data.
		/// </summary>
		/// <remarks>Writes the output in GPX format.</remarks>
		public void Trace(GeoData raw, GeoData corrected, GpsStatus gpsStatus, DateTime computerDateTime)
		{
			Trace(raw, corrected, gpsStatus, computerDateTime, null);
		}

		public void Trace(GeoData raw, GeoData corrected, GpsStatus gpsStatus, DateTime computerDateTime, RoutepointExtension rteptExtension)
		{
			if (raw == null) { Log.Warn().Message("raw est null").Write(); return; }
			if (corrected == null) { Log.Warn().Message("corrected est null").Write(); return; }

			if (raw.PositionData == null) { Log.Warn().Message("raw.PositionData est null").Write(); return; }
			if (raw.VelocityData == null) { Log.Warn().Message("raw.VelocityData est null").Write(); return; }

			if (corrected.PositionData == null) { Log.Warn().Message("corrected.PositionData est null").Write(); return; }
			if (corrected.VelocityData == null) { Log.Warn().Message("corrected.VelocityData est null").Write(); return; }

			if (_output != null && (_output.WriteState == WriteState.Closed || _output.WriteState == WriteState.Error)) { Log.Warn().Message("Le fichier de trace est fermé ou en erreur.").Write(); return; }

			_output.WriteStartElement("rtept");
			{
				_output.WriteAttributeString("lat", Convert.ToString(raw.PositionData.Latitude, CultureInfo.InvariantCulture));
				_output.WriteAttributeString("lon", Convert.ToString(raw.PositionData.Longitude, CultureInfo.InvariantCulture));

				_output.WriteElementString("time", raw.PositionData.Utc.ToString("yyyy-MM-ddTHH:mm:ss.ffK"));
				_output.WriteElementString("ele", Convert.ToString(raw.PositionData.Altitude, CultureInfo.InvariantCulture));
				_output.WriteElementString("geoidheight", Convert.ToString(raw.PositionData.GeoIdHeight, CultureInfo.InvariantCulture));
				_output.WriteElementString("fix", GpxConvert.FromFixType(raw.PositionData.Quality));
				_output.WriteElementString("sat", Convert.ToString(raw.PositionData.NbSatellites, CultureInfo.InvariantCulture));

				if (raw.PrecisionData != null)
				{
					if (raw.PrecisionData.Hdop != null)
						_output.WriteElementString("hdop", Convert.ToString(raw.PrecisionData.Hdop, CultureInfo.InvariantCulture));

					if (raw.PrecisionData.Vdop != null)
						_output.WriteElementString("vdop", Convert.ToString(raw.PrecisionData.Vdop, CultureInfo.InvariantCulture));

					if (raw.PrecisionData.Pdop != null)
						_output.WriteElementString("pdop", Convert.ToString(raw.PrecisionData.Pdop, CultureInfo.InvariantCulture));
				}

				_output.WriteElementString("ageofdgpsdata", Convert.ToString(raw.PositionData.DifferentialDataAge, CultureInfo.InvariantCulture));
				_output.WriteElementString("dgpsid", Convert.ToString(raw.PositionData.DifferentialStationId));

				_output.WriteStartElement("extensions");
				{
					_output.WriteElementString("computerDateTime", computerDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK"));

					_output.WriteStartElement("CorrectedPositionValue");
					{
						_output.WriteElementString("lat", Convert.ToString(corrected.PositionData.Latitude, CultureInfo.InvariantCulture));
						_output.WriteElementString("lon", Convert.ToString(corrected.PositionData.Longitude, CultureInfo.InvariantCulture));
					}
					_output.WriteEndElement();

					_output.WriteElementString("gpsStatus", Convert.ToString(gpsStatus));
					_output.WriteElementString("speedKmh", Convert.ToString(raw.VelocityData.SpeedKmh, CultureInfo.InvariantCulture));

					if (rteptExtension != null)
					{
						if (rteptExtension.Progress.HasValue)
							_output.WriteElementString("progress", Convert.ToString(rteptExtension.Progress.Value, CultureInfo.InvariantCulture));
					}

					if (raw.PositionData.InsData != null)
					{
						_output.WriteStartElement("ins", "inertial", null);
						{
							_output.WriteElementString("navigationStatus", Convert.ToString(raw.PositionData.InsData.Status, CultureInfo.InvariantCulture));
							_output.WriteElementString("accx", Convert.ToString(raw.PositionData.InsData.AccelerationX, CultureInfo.InvariantCulture));
							_output.WriteElementString("accy", Convert.ToString(raw.PositionData.InsData.AccelerationY, CultureInfo.InvariantCulture));
							_output.WriteElementString("accz", Convert.ToString(raw.PositionData.InsData.AccelerationZ, CultureInfo.InvariantCulture));
							_output.WriteElementString("angrtx", Convert.ToString(raw.PositionData.InsData.AngularRateX, CultureInfo.InvariantCulture));
							_output.WriteElementString("angrty", Convert.ToString(raw.PositionData.InsData.AngularRateY, CultureInfo.InvariantCulture));
							_output.WriteElementString("angrtz", Convert.ToString(raw.PositionData.InsData.AngularRateZ, CultureInfo.InvariantCulture));
							_output.WriteElementString("nvelo", Convert.ToString(raw.PositionData.InsData.NorthVelocity, CultureInfo.InvariantCulture));
							_output.WriteElementString("evelo", Convert.ToString(raw.PositionData.InsData.EastVelocity, CultureInfo.InvariantCulture));
							_output.WriteElementString("dvelo", Convert.ToString(raw.PositionData.InsData.DownVelocity, CultureInfo.InvariantCulture));
							_output.WriteElementString("heading", Convert.ToString(raw.PositionData.InsData.Heading, CultureInfo.InvariantCulture));
							_output.WriteElementString("pitch", Convert.ToString(raw.PositionData.InsData.Pitch, CultureInfo.InvariantCulture));
							_output.WriteElementString("roll", Convert.ToString(raw.PositionData.InsData.Roll, CultureInfo.InvariantCulture));

							if (raw.PrecisionData != null)
							{
								if (raw.PrecisionData.EastPositionAccuracy != null)
									_output.WriteElementString("eastPositionAccuracy", Convert.ToString(raw.PrecisionData.EastPositionAccuracy, CultureInfo.InvariantCulture));

								if (raw.PrecisionData.DownPositionAccuracy != null)
									_output.WriteElementString("downPositionAccuracy", Convert.ToString(raw.PrecisionData.DownPositionAccuracy, CultureInfo.InvariantCulture));

								if (raw.PrecisionData.NorthPositionAccuracy != null)
									_output.WriteElementString("northPositionAccuracy", Convert.ToString(raw.PrecisionData.NorthPositionAccuracy, CultureInfo.InvariantCulture));

								if (raw.PrecisionData.DownVelocityAccuracy != null)
									_output.WriteElementString("downVelocityAccuracy", Convert.ToString(raw.PrecisionData.DownVelocityAccuracy, CultureInfo.InvariantCulture));

								if (raw.PrecisionData.EastVelocityAccuracy != null)
									_output.WriteElementString("eastVelocityAccuracy", Convert.ToString(raw.PrecisionData.EastVelocityAccuracy, CultureInfo.InvariantCulture));

								if (raw.PrecisionData.NorthVelocityAccuracy != null)
									_output.WriteElementString("northVelocityAccuracy", Convert.ToString(raw.PrecisionData.NorthVelocityAccuracy, CultureInfo.InvariantCulture));

								if (raw.PrecisionData.HeadingAccuracy != null)
									_output.WriteElementString("headingAccuracy", Convert.ToString(raw.PrecisionData.HeadingAccuracy, CultureInfo.InvariantCulture));

								if (raw.PrecisionData.PitchAccuracy != null)
									_output.WriteElementString("pitchAccuracy", Convert.ToString(raw.PrecisionData.PitchAccuracy, CultureInfo.InvariantCulture));

								if (raw.PrecisionData.RollAccuracy != null)
									_output.WriteElementString("rollAccuracy", Convert.ToString(raw.PrecisionData.RollAccuracy, CultureInfo.InvariantCulture));
							}
						}
						_output.WriteEndElement();
					}
				}
				_output.WriteEndElement();
			}
			_output.WriteEndElement();
		}

		#region IDisposable members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			this.StopTracing();
		}

		~GpxTracer()
		{
			Log.Warn().Message("Object was not disposed correctly.").Write();
			this.Dispose(false);
		}

		#endregion
	}
}