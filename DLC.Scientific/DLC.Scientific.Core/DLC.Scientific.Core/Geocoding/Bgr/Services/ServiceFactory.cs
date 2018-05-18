using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;

namespace DLC.Scientific.Core.Geocoding.Bgr.Services
{
	public static class ServiceFactory
	{
		public static InformationService.InformationClient CreateInformationService(string url)
		{
			return new InformationService.InformationClient(CreateServiceBinding(WSMessageEncoding.Mtom), new EndpointAddress(url));
		}

		public static GeocodageService.GeocodageSoapClient CreateGeocodageService(string url)
		{
			return new GeocodageService.GeocodageSoapClient(CreateServiceBinding(WSMessageEncoding.Text), new EndpointAddress(url));
		}

		public static SpatialService.ServiceSpatialSoapClient CreateSpatialService(string url)
		{
			return new SpatialService.ServiceSpatialSoapClient(CreateServiceBinding(WSMessageEncoding.Text), new EndpointAddress(url));
		}

		private static Binding CreateServiceBinding(WSMessageEncoding messageEncoding)
		{
			var binding = new BasicHttpBinding {
				CloseTimeout = TimeSpan.FromMinutes(5),
				OpenTimeout = TimeSpan.FromMinutes(5),
				ReceiveTimeout = TimeSpan.FromHours(5),
				SendTimeout = TimeSpan.FromHours(5),

				MaxBufferSize = 4194304,
				MaxBufferPoolSize = 4194304,
				MaxReceivedMessageSize = 4194304,

				MessageEncoding = messageEncoding,
				TextEncoding = Encoding.UTF8
			};

			binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
			binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
			binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
			binding.Security.Transport.Realm = "";
			binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
			binding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Default;

			return binding;
		}
	}
}