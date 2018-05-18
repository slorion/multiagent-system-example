using System;
using System.Runtime.Serialization;

namespace DLC.Scientific.Core.Geocoding.Gps
{
    [DataContract]
    public class MetaData
    {
        public string Name { get; set; }
        public DateTime time { get; set; }
        public MetaDataExtension Extension { get; set; }
    }
}
