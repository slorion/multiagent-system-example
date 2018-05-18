using Newtonsoft.Json;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	public sealed class RegionsOfInterestParameters
	{
		/// <summary>
		/// Retourne le margin top de la Region of Interest de la caméra.
		/// </summary>
		[JsonProperty]
		public int RegionOfInterestOfCamera_MarginTop { get; private set; }

		/// <summary>
		/// Retourne le margin bottom de la Region of Interest de la caméra.
		/// </summary>
		[JsonProperty]
		public int RegionOfInterestOfCamera_MarginBottom { get; private set; }

		/// <summary>
		/// Retourne le margin left de la Region of Interest de la caméra.
		/// </summary>
		[JsonProperty]
		public int RegionOfInterestOfCamera_MarginLeft { get; private set; }

		/// <summary>
		/// Retourne le margin right de la Region of Interest de la caméra.
		/// </summary>
		[JsonProperty]
		public int RegionOfInterestOfCamera_MarginRight { get; private set; }

		/// <summary>
		/// Retourne le margin top de la Region of Interest réservée aux calculs de saturation et d'intensité moyenne des pixels de l'image.
		/// </summary>
		[JsonProperty]
		public int RegionOfInterestForCalculation_MarginTop { get; private set; }

		/// <summary>
		/// Retourne le margin bottom de la Region of Interest réservée aux calculs de saturation et d'intensité moyenne des pixels de l'image.
		/// </summary>
		[JsonProperty]
		public int RegionOfInterestForCalculation_MarginBottom { get; private set; }

		/// <summary>
		/// Retourne le margin left de la Region of Interest réservée aux calculs de saturation et d'intensité moyenne des pixels de l'image.
		/// </summary>
		[JsonProperty]
		public int RegionOfInterestForCalculation_MarginLeft { get; private set; }

		/// <summary>
		/// Retourne le margin right de la Region of Interest réservée aux calculs de saturation et d'intensité moyenne des pixels de l'image.
		/// </summary>
		[JsonProperty]
		public int RegionOfInterestForCalculation_MarginRight { get; private set; }

		#region Ctor

		/// <summary>
		/// Default ctor
		/// </summary>
		public RegionsOfInterestParameters() { }

		/// <summary>
		/// Instancie RegionsOfInterestParameters avec des valeurs par défaut, si nécessaire.
		/// </summary>
		/// <param name="withDefaultValues">Détermine si l'on veut utiliser des valeurs par défaut ou pas.</param>
		public RegionsOfInterestParameters(bool withDefaultValues)
			: this()
		{
			if (withDefaultValues)
			{ 
				this.RegionOfInterestOfCamera_MarginTop = 300;
				this.RegionOfInterestOfCamera_MarginBottom = 200;
				this.RegionOfInterestOfCamera_MarginLeft = 200;
				this.RegionOfInterestOfCamera_MarginRight = 200;
				this.RegionOfInterestForCalculation_MarginTop = 450;
				this.RegionOfInterestForCalculation_MarginBottom = 200;
				this.RegionOfInterestForCalculation_MarginLeft = 550;
				this.RegionOfInterestForCalculation_MarginRight = 550;
			}
		}

		#endregion
	}
}