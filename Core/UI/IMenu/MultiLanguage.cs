using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.SDK.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSharp.SDK.Core.UI.IMenu {
	/// <summary>
	/// Provides multi-lingual strings.
	/// </summary>
	public static class MultiLanguage {
		/// <summary>
		/// The translations
		/// </summary>
		private static Dictionary<string, string> Translations = new Dictionary<string, string>();

		/// <summary>
		/// Translates the text into the loaded language.
		/// </summary>
		/// <param name="textToTranslate">The text to translate.</param>
		/// <returns>System.String.</returns>
		public static string _(string textToTranslate) {

			var textToTranslateToLower = textToTranslate.ToLower();
			if (Translations.ContainsKey(textToTranslateToLower))
			{
				return Translations[textToTranslateToLower];
			}
			else if (Translations.ContainsKey(textToTranslate))
			{
				return Translations[textToTranslate];
			}
			else
			{
				return textToTranslate;
			}
		}

		/// <summary>
		/// Loads the language.
		/// </summary>
		/// <param name="languageName">Name of the language.</param>
		/// <returns><c>true</c> if the operation succeeded, <c>false</c> otherwise false.</returns>
		public static bool LoadLanguage(string languageName) {
			try
			{
				var languageStrings = new ResourceManager("LeagueSharp.SDK.Properties.Resources", typeof(Resources).Assembly).GetString(languageName+"Json");
				if (string.IsNullOrEmpty(languageStrings))
				{
					return false;
				}

				Translations = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(languageStrings);
				return true;
			}
			catch (Exception ex)
			{
				Logging.Write()(LogLevel.Error, $"[MultiLanguage] Load Language Catch Exception：{ex.Message}");
				return false;
			}
		}
	}
}
