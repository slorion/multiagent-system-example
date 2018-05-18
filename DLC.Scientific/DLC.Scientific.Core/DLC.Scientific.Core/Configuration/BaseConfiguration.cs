using System;

namespace DLC.Scientific.Core.Configuration
{
	public abstract class BaseConfiguration
	{
		public static string MissingProperty(string propertyName) { throw new ConfigurationException(string.Format("The property '{0}' is mandatory.", propertyName)); }
		public static string OutOfRange(string propertyName, object min, object max) { throw new ConfigurationException(string.Format("The property '{0}' must be between '{1}' and '{2}' inclusively.", propertyName, min, max)); }
		public static string OutOfRangeMin(string propertyName, object min) { throw new ConfigurationException(string.Format("The property '{0}' must be greater than or equal to '{1}'.", propertyName, min)); }
		public static string OutOfRangeMax(string propertyName, object max) { throw new ConfigurationException(string.Format("The property '{0}' must be less than or equal to '{1}'.", propertyName, max)); }
		public static string InvalidDataFormat(string propertyName, string type) { throw new ConfigurationException(string.Format("The value for the property '{0}' must be of type '{1}'.", propertyName, type)); }
		public virtual void Validate() { }
	}
}