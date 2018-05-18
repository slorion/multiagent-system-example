using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace DLC.Scientific.Core.Geocoding.Gps
{
	public static class GpxReader
	{
		/// <summary>
		/// When passed a file, open it and parse all routes and route segments from it.
		/// </summary>
		/// <param name="filePath">Fully qualified file name (local)</param>
		/// <returns>
		/// A List of PositionData
		/// </returns>
		public static IEnumerable<PositionData> LoadPositionData(string filePath)
		{
			XDocument gpxDoc = XDocument.Load(filePath);

			return LoadPositionData(gpxDoc);
		}

		/// <summary>
		/// When passed a file, open it and parse all routes and route segments from it.
		/// </summary>
		/// <param name="gpxDoc">Fully qualified file name (local)</param>
		/// <returns>
		/// A List of PositionData
		/// </returns>
		public static IEnumerable<PositionData> LoadPositionData(XDocument gpxDoc)
		{
			XNamespace gpx = XNamespace.Get("http://www.topografix.com/GPX/1/1");
			XNamespace ins = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance/ins");

			return from routepoint in gpxDoc.Descendants(gpx + "rtept")
				   let extensions = routepoint.Element(gpx + "extensions")
				   let inertial = extensions != null ? extensions.Element(ins + "inertial") : null
				   select new PositionData {
					   Latitude = XmlConvert.ToDouble(routepoint.Attribute("lat").Value),
					   Longitude = XmlConvert.ToDouble(routepoint.Attribute("lon").Value),
					   Utc = routepoint.Element(gpx + "time") != null
							? XmlConvert.ToDateTime(routepoint.Element(gpx + "time").Value, XmlDateTimeSerializationMode.Utc)
							: DateTime.MinValue,
					   Altitude = routepoint.Element(gpx + "geoidheight") != null
							? XmlConvert.ToDouble(routepoint.Element(gpx + "geoidheight").Value)
							: routepoint.Element(gpx + "ele") != null
								? XmlConvert.ToDouble(routepoint.Element(gpx + "ele").Value)
								: 0d,
					   Quality = routepoint.Element(gpx + "fix") != null
							? GpxConvert.ToFixType(routepoint.Element(gpx + "fix").Value)
							: FixType.None,
					   NbSatellites = routepoint.Element(gpx + "sat") != null
							? XmlConvert.ToInt32(routepoint.Element(gpx + "sat").Value)
							: 0,
					   InsData = inertial != null
							? new InsData {
								AccelerationX = XmlConvert.ToDouble(inertial.Element(gpx + "accx").Value),
								AccelerationY = XmlConvert.ToDouble(inertial.Element(gpx + "accy").Value),
								AccelerationZ = XmlConvert.ToDouble(inertial.Element(gpx + "accz").Value),
								AngularRateX = XmlConvert.ToDouble(inertial.Element(gpx + "angrtx").Value),
								AngularRateY = XmlConvert.ToDouble(inertial.Element(gpx + "angrty").Value),
								AngularRateZ = XmlConvert.ToDouble(inertial.Element(gpx + "angrtz").Value),
								DownVelocity = XmlConvert.ToDouble(inertial.Element(gpx + "dvelo").Value),
								EastVelocity = XmlConvert.ToDouble(inertial.Element(gpx + "evelo").Value),
								NorthVelocity = XmlConvert.ToDouble(inertial.Element(gpx + "nvelo").Value),
								Heading = XmlConvert.ToDouble(inertial.Element(gpx + "heading").Value),
								Pitch = XmlConvert.ToDouble(inertial.Element(gpx + "pitch").Value),
								Roll = XmlConvert.ToDouble(inertial.Element(gpx + "roll").Value),
							}
							: null
				   };
		}

		/// <summary>
		/// When passed a file, open it and parse all routes and route segments from it.
		/// </summary>
		/// <param name="filePath">Fully qualified file name (local)</param>
		/// <returns>
		/// A List of PositionData
		/// </returns>
		public static IEnumerable<GpxData> LoadGpxData(string filePath, bool useCorrectedValue)
		{
			XDocument gpxDoc = XDocument.Load(filePath);
			XNamespace gpx = XNamespace.Get("http://www.topografix.com/GPX/1/1");
			XNamespace ins = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance/ins");
			string version = gpxDoc.Root.FirstAttribute.Value;

			if (version == "1.1")
				useCorrectedValue = false;

			return from routepoint in gpxDoc.Descendants(gpx + "rtept")
				   let extensions = routepoint.Element(gpx + "extensions")
				   let correctedPositionValue = extensions.Element(gpx + "CorrectedPositionValue")
				   let inertial = extensions != null ? extensions.Element(ins + "inertial") : null
				   select new GpxData {
					   PositionData = new PositionData {
						   Latitude = useCorrectedValue
							   ? XmlConvert.ToDouble(correctedPositionValue.Element(gpx + "lat").Value)
							   : XmlConvert.ToDouble(routepoint.Attribute("lat").Value),
						   Longitude = useCorrectedValue
								   ? XmlConvert.ToDouble(correctedPositionValue.Element(gpx + "lon").Value)
								   : XmlConvert.ToDouble(routepoint.Attribute("lon").Value),
						   Utc = routepoint.Element(gpx + "time") != null
							   ? XmlConvert.ToDateTime(routepoint.Element(gpx + "time").Value, XmlDateTimeSerializationMode.Utc)
							   : DateTime.MinValue,
						   Altitude = routepoint.Element(gpx + "ele") != null
							   ? XmlConvert.ToDouble(routepoint.Element(gpx + "ele").Value)
							   : 0d,
						   GeoIdHeight = routepoint.Element(gpx + "geoidheight") != null
							   ? XmlConvert.ToDouble(routepoint.Element(gpx + "geoidheight").Value)
							   : 0d,
						   Quality = routepoint.Element(gpx + "fix") != null
							   ? GpxConvert.ToFixType(routepoint.Element(gpx + "fix").Value)
							   : FixType.None,
						   NbSatellites = routepoint.Element(gpx + "sat") != null
							   ? XmlConvert.ToInt32(routepoint.Element(gpx + "sat").Value)
							   : 0,
						   InsData = inertial != null
							   ? new InsData {
								   AccelerationX = XmlConvert.ToDouble(inertial.Element(gpx + "accx").Value),
								   AccelerationY = XmlConvert.ToDouble(inertial.Element(gpx + "accy").Value),
								   AccelerationZ = XmlConvert.ToDouble(inertial.Element(gpx + "accz").Value),
								   AngularRateX = XmlConvert.ToDouble(inertial.Element(gpx + "angrtx").Value),
								   AngularRateY = XmlConvert.ToDouble(inertial.Element(gpx + "angrty").Value),
								   AngularRateZ = XmlConvert.ToDouble(inertial.Element(gpx + "angrtz").Value),
								   DownVelocity = XmlConvert.ToDouble(inertial.Element(gpx + "dvelo").Value),
								   EastVelocity = XmlConvert.ToDouble(inertial.Element(gpx + "evelo").Value),
								   NorthVelocity = XmlConvert.ToDouble(inertial.Element(gpx + "nvelo").Value),
								   Heading = XmlConvert.ToDouble(inertial.Element(gpx + "heading").Value),
								   Pitch = XmlConvert.ToDouble(inertial.Element(gpx + "pitch").Value),
								   Roll = XmlConvert.ToDouble(inertial.Element(gpx + "roll").Value),
							   }
							   : null
					   },
					   VelocityData = new VelocityData {
						   SpeedMs = extensions.Element(gpx + "speedKmh") != null
							   ? XmlConvert.ToDouble(extensions.Element(gpx + "speedKmh").Value) / 3.6
							   : 0,
					   },
					   PrecisionData = new PrecisionData {
						   Hdop = routepoint.Element(gpx + "hdop") != null
							   ? XmlConvert.ToDouble(routepoint.Element(gpx + "hdop").Value)
							   : 0d,
						   Vdop = routepoint.Element(gpx + "vdop") != null
							   ? XmlConvert.ToDouble(routepoint.Element(gpx + "vdop").Value)
							   : 0d,
						   Pdop = routepoint.Element(gpx + "pdop") != null
							   ? XmlConvert.ToDouble(routepoint.Element(gpx + "pdop").Value)
							   : 0d,
					   },
					   ExtensionData = new ExtensionData {
						   Speed = extensions.Element(gpx + "speedKmh") != null
							   ? XmlConvert.ToDouble(extensions.Element(gpx + "speedKmh").Value) / 3.6
							   : 0,
						   CorrectedLatitude = extensions.Element(gpx + "lat") != null ? XmlConvert.ToDouble(correctedPositionValue.Element(gpx + "lat").Value) : 0,
						   CorrectedLongitude = extensions.Element(gpx + "lon") != null ? XmlConvert.ToDouble(correctedPositionValue.Element(gpx + "lon").Value) : 0,
						   GpsStatus = extensions.Element(gpx + "gpsStatus") != null
							   ? extensions.Element(gpx + "gpsStatus").Value
							   : "",
						   ComputerDateTime = extensions.Element(gpx + "computerDateTime") != null
							   ? XmlConvert.ToDateTime(extensions.Element(gpx + "computerDateTime").Value, XmlDateTimeSerializationMode.Utc)
							   : DateTime.MinValue,
						   Progress = extensions.Element(gpx + "progress") != null
							   ? XmlConvert.ToDouble(extensions.Element(gpx + "progress").Value)
							   : 0,
					   }
				   };
		}

		public static MetaData LoadMetaData(string filePath)
		{
			MetaData result = new MetaData();
			XDocument gpxDoc = XDocument.Load(filePath);
			XNamespace gpx = XNamespace.Get("http://www.topografix.com/GPX/1/1");
			XNamespace ins = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance/ins");
			string version = gpxDoc.Root.FirstAttribute.Value;

			var elem = (
				from metadata in gpxDoc.Descendants(gpx + "metadata")
				let extension = metadata.Element(gpx + "extensions")
				select new MetaData {
					time = metadata.Element(gpx + "time") != null
						? XmlConvert.ToDateTime(metadata.Element(gpx + "time").Value, XmlDateTimeSerializationMode.Utc)
						: DateTime.MinValue,
					Name = metadata.Element(gpx + "name") != null
						? metadata.Element(gpx + "name").Value
						: String.Empty,
					Extension = extension != null
						? new MetaDataExtension {
							DeviceType = (GpsDeviceType) Enum.Parse(typeof(GpsDeviceType), extension.Element(gpx + "gpsDeviceType") != null
								? extension.Element(gpx + "gpsDeviceType").Value
								: "Gps"),
							Sequenceur = extension.Element(gpx + "sequenceur") != null
								? extension.Element(gpx + "sequenceur").Value
								: String.Empty,
							FileStructVersion = extension.Element(gpx + "FileVersion") != null
								? extension.Element(gpx + "FileVersion").Value
								: String.Empty,
							CreationSource = extension.Element(gpx + "CreationSource") != null
								? extension.Element(gpx + "CreationSource").Value
								: String.Empty,
							CreationSourceVersion = extension.Element(gpx + "CreationSourceVersion") != null
								? extension.Element(gpx + "CreationSourceVersion").Value
								: String.Empty,
							CreationSourceType = extension.Element(gpx + "CreationSourceType") != null
								? extension.Element(gpx + "CreationSourceType").Value
								: String.Empty
						}
						: null,
				});

			return elem.First();
		}
	}
}