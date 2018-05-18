using DLC.Scientific.Acquisition.Core.AcquisitionProviders.Model;
using DLC.Scientific.Core.Geocoding;
using DLC.Scientific.Core.Geocoding.Bgr;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DLC.Scientific.Acquisition.Core.Agents
{
	[ServiceContract]
	[ServiceKnownType(typeof(Rtssc))]
	public interface IBgrDirectionalAgent
		: IProviderAgent<BgrData>, IAcquisitionableAgent
	{
		string ItineraryLogRelativeFilePath { [OperationContract] get; }

		BgrDataTypes AllowedBgrDataTypes { [OperationContract] get; [OperationContract] set; }
		int AutoCorrectDelta { [OperationContract] get; }
		int ManualSearchRadiusInMeters { [OperationContract] get; }
		int AutoSearchRadiusInMeters { [OperationContract] get; }
		int AutoSearchIntervalInMs { [OperationContract] get; }

		[OperationContract]
		IRtssc GetNextRtsscSameDirection(GeoCoordinate coord);

		[OperationContract]
		GeoCoordinate GeoCodage(IRtssc rtssc);

		[OperationContract]
		double GetSectionLength(IRtssc rtssc);

		[OperationContract]
		IEnumerable<IRtssc> GetRtssFromRoute(string route);

		[OperationContract]
		IEnumerable<string> SelectRoutes(GeoCoordinate coord, double searchRadiusInMeters, int? maxRouteNumber);
	}
}