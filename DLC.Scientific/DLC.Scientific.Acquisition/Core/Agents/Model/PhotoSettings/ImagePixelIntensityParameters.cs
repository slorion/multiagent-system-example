using Newtonsoft.Json;

namespace DLC.Scientific.Acquisition.Core.Agents.Model.PhotoSettings
{
	public sealed class ImagePixelIntensityParameters
	{
		/// <summary>
		/// Indique le seuil minimum d'intensité moyenne des pixels toléré pour l'image.
		/// <remarks>
		/// Si la valeur d'intensité moyenne des pixels de l'image courante est sous ce seuil, des actions seront
		/// entreprises auprès des paramètres de la caméra afin de faire augmenter cette valeur.
		/// </remarks>
		/// </summary>
		[JsonProperty]
		public float MinimumAveragePixelValue { get; private set; }

		/// <summary>
		/// Indique le seuil maximum d'intensité moyenne des pixels toléré pour l'image.
		/// <remarks>
		/// Si la valeur d'intensité moyenne des pixels de l'image courante est au-dessus ce seuil, des actions seront
		/// entreprises auprès des paramètres de la caméra afin de faire diminuer cette valeur.
		/// </remarks>
		/// </summary>
		[JsonProperty]
		public float MaximumAveragePixelValue { get; private set; }

		/// <summary>
		/// Indique la variation tolérée en valeur d'intensité de pixels par rapport aux seuils minimum (<see cref="MinimumAveragePixelValue"/>) 
		/// et maximum (<see cref="MaximumAveragePixelValue"/>) tolérés.
		/// 
		/// <remarks>
		/// Si la valeur d'intensité moyenne de l'image est en-dessous du seuil minimum ou au-dessus du seuil maximum,
		/// mais située à l'intérieur de la variation tolérée, de légères modifications aux paramètres de la caméra
		/// seront entreprises afin de ramener la valeur de l'intensité moyenne des pixels de l'image entre les seuils
		/// minimum et maximum.
		/// 
		/// Toutefois, si sa valeur est située au-delà de la variation tolérée, c'est que nous sommes en présence
		/// d'un cas limite où l'image est beaucoup trop sombre ou beaucoup trop claire. Des actions drastiques et immédiates
		/// sont déclanchées auprès des paramètres de la caméra afin de corriger la situation.
		/// </remarks>
		/// 
		/// <example>
		/// 
		/// Seuil minimum = 100
		/// Seuil maximum = 175
		/// Variation tolérée = 15
		/// 
		/// Avec une valeur d'intensité moyenne à 125, une image serait ok, donc aucune action corrective serait entreprise.
		/// Avec une valeur à 180, l'image serait légèrement trop claire, donc de légères modifs aux params de la cam seraient faites.
		/// Avec une valeur à 200, l'image serait nettement trop claire, donc des actions drastiques seraient entreprises.
		/// 
		/// </example>
		/// </summary>
		[JsonProperty]
		public float VariationAveragePixelValue { get; private set; }

		#region Ctor

		/// <summary>
		/// Default ctor
		/// </summary>
		public ImagePixelIntensityParameters() { }

		/// <summary>
		/// Instancie ImagePixelIntensityParameters avec des valeurs par défaut, si nécessaire.
		/// </summary>
		/// <param name="withDefaultValues">Détermine si l'on veut utiliser des valeurs par défaut ou pas.</param>
		public ImagePixelIntensityParameters(bool withDefaultValues)
			: this()
		{
			if (withDefaultValues)
			{
				MinimumAveragePixelValue = 100;
				MaximumAveragePixelValue = 175;
				VariationAveragePixelValue = 15;
			}
		}

		#endregion
	}
}