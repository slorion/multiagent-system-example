using Newtonsoft.Json;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	public sealed class ImageSaturationParameters
	{
		/// <summary>
		/// Indique le pourcentage maximal toléré de pixel dits saturés dans l'image.
		/// <example>5 indiquerait qu'on tolère que l'image ait un maximum de 5% de pixels dits saturés.</example>
		/// </summary>
		[JsonProperty]
		public int MaximumToleratedPercentageOfSaturatedPixels { get; private set; }

		/// <summary>
		/// Indique le seuil en % de saturation pour qu'un pixel soit considéré comme étant saturé.
		/// <example>
		/// Avec des images 8 bits (0 étant un pixel noir et 255 étant un pixel blanc), si on a un seuil
		/// de 95%, un pixel serait considéré comme saturé si sa valeur est de 255 * 0.95 = 242.5 ou 243.
		/// </example>
		/// <remarks>
		/// Un pixel saturé est un pixel trop clair pour être capable d'extraire un minimum d'information.
		/// </remarks>
		/// </summary>
		[JsonProperty]
		public int PixelSaturationThreshold { get; private set; }

		/// <summary>
		/// Indique la cible en % de saturation de l'image que le monitoring de la saturation de l'image cherchera à atteindre.
		/// </summary>
		[JsonProperty]
		public double SaturationTarget { get; private set; }

		/// <summary>
		/// Indique la variation tolérée en % par rapport à la <see cref="SaturationTarget" />.
		/// <remarks>
		/// Si le % de saturation de l'image n'est pas sur la <see cref="SaturationTarget" />, mais est situé à l'intérieur
		/// de la variation tolérée (cible ± variation), aucun changement ne sera apporté aux paramètres de la caméra.
		/// </remarks>
		/// </summary>
		[JsonProperty]
		public double SaturationTargetVariation { get; private set; }

		/// <summary>
		/// Indique le % de saturation maximum toléré pour l'image.
		/// <example>
		/// Avec un MaximumSaturationScore à 20, cela voudrait dire que l'on tolère qu'un maximum de 20% des pixels
		/// de l'image soient saturés.
		/// </example>
		/// </summary>
		[JsonProperty]
		public double MaximumSaturationScore { get; private set; }

		/// <summary>
		/// Indique le nombre de frame sur lequel le taux de saturation est calculé.
		/// <remarks>La saturation des images est constamment surveillée en calculant le taux de saturation d'un groupe d'image 
		/// afin d'en établir la moyenne. Le nombre d'image que doit avoir le groupe est déterminé avec cette config.</remarks>
		/// </summary>
		[JsonProperty]
		public int NumberOfFramesToCalculateSaturationFrom { get; private set; }

		#region Ctor

		/// <summary>
		/// Default ctor
		/// </summary>
		public ImageSaturationParameters() { }

		/// <summary>
		/// Instancie ImageSaturationParameters avec des valeurs par défaut, si nécessaire.
		/// </summary>
		/// <param name="withDefaultValues">Détermine si l'on veut utiliser des valeurs par défaut ou pas.</param>
		public ImageSaturationParameters(bool withDefaultValues)
			: this()
		{
			if (withDefaultValues)
			{
				MaximumSaturationScore = 20.0;
				MaximumToleratedPercentageOfSaturatedPixels = 5;
				NumberOfFramesToCalculateSaturationFrom = 100;
				PixelSaturationThreshold = 95;
				SaturationTarget = 3.5;
				SaturationTargetVariation = 0.25;
			}
		}

		#endregion
	}
}