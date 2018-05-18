using Newtonsoft.Json;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	public sealed class ImageAnalysisParameters
	{
		[JsonProperty]
		public ImagePixelIntensityParameters ImagePixelIntensityParameters { get; private set; }

		[JsonProperty]
		public ImageSaturationParameters ImageSaturationParameters { get; private set; }

		/// <summary>
		/// Indique le nombre de frame qui sont skippés volontairement lorsque l'on ajuste les paramètres de la caméra
		/// en situation d'image trop claire ou trop sombre.
		/// <remarks>
		/// C'est une limitation de la caméra. Celle-ci peut prendre 3-4 frames à s'ajuster à un changement de valeur
		/// dans ses paramètres.
		/// </remarks>
		/// </summary>
		[JsonProperty]
		public int FrameSkipForAdjustment { get; private set; }

		/// <summary>
		/// Indique le nombre de frame réservé à l'initialisation de la caméra.
		/// </summary>
		[JsonProperty]
		public int NumberOfFramesForInitialization { get; private set; }

		/// <summary>
		/// Permet de déterminer l'échantillonnage de pixels évalués.
		/// <example>
		/// 4 voudrait dire qu'on évalue un pixel sur 4 sur l'axe des X, idem sur l'axe des Y. 
		/// Donc pour une zone de 4 x 4, on a 16 pixels, 1 seul pixel serait évalué.
		/// </example>
		/// </summary>
		[JsonProperty]
		public int PixelDownSamplingFactor { get; private set; }

		#region Ctor

		/// <summary>
		/// Default ctor
		/// </summary>
		public ImageAnalysisParameters() { }

		/// <summary>
		/// Instancie ImageAnalysisParameters avec des valeurs par défaut, si nécessaire.
		/// </summary>
		/// <param name="withDefaultValues">Détermine si l'on veut utiliser des valeurs par défaut ou pas.</param>
		public ImageAnalysisParameters(bool withDefaultValues)
			: this()
		{
			if (withDefaultValues)
			{
				FrameSkipForAdjustment = 4;
				NumberOfFramesForInitialization = 100;
				PixelDownSamplingFactor = 4;

				ImagePixelIntensityParameters = new ImagePixelIntensityParameters(true);
				ImageSaturationParameters = new ImageSaturationParameters(true);
			}
		}

		#endregion
	}
}
