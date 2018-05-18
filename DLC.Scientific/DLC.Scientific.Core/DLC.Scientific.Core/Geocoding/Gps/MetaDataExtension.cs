using DLC.Scientific.Core.Geocoding.Gps;
using System.Runtime.Serialization;

namespace DLC.Scientific.Core.Geocoding.Gps
{
    [DataContract]
    public class MetaDataExtension
    {
        public GpsDeviceType DeviceType { get; set; }
        public string Sequenceur { get; set; }
        public string FileStructVersion { get; set; }
        public string CreationSourceType { get; set; }
        public string CreationSourceVersion { get; set; }
        public string CreationSource { get; set; }
    }
}
