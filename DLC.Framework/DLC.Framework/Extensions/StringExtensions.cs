using System.Globalization;
using System.Text;

namespace DLC.Framework.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Returns provided text without diacritics (e.g. accents).
		/// </summary>
		/// <param name="text">The text to process.</param>
		/// <returns>The provided text without diacritics.</returns>
		public static string RemoveDiacritics(this string text)
		{
			if (text == null)
				return null;

			string normalizedText = text.Normalize(NormalizationForm.FormD);

			var sb = new StringBuilder(normalizedText.Length);

			for (int i = 0; i < normalizedText.Length; i++)
			{
				UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(normalizedText[i]);

				if (uc != UnicodeCategory.NonSpacingMark)
					sb.Append(normalizedText[i]);
			}

			return sb.ToString().Normalize(NormalizationForm.FormC);
		}
	}
}