using System;

namespace DLC.Scientific.Core.Geocoding.Bgr.Services.InformationService
{
	partial class InformationClient
	{
		public void FillRtss(IRtssc rtssc, DateTime date)
		{
			var request = new ObtenirInformationSousRouteRequete {
				RTSS = rtssc.NumeroRTSS,
				DateConsultation = date
			};

			ObtenirInformationSousRouteReponse response = this.ObtenirInformationSousRoute(request);

			int length;
			if (response.Longueur != null && int.TryParse(response.Longueur, out length))
				rtssc.Longueur = length;

			int ide;
			if (int.TryParse(response.Identifiant, out ide))
				rtssc.Ide = ide;
			else
				rtssc.Ide = 0;

			rtssc.Statut = response.Statut;
		}
	}
}