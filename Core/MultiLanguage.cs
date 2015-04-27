using System.Collections.Generic;
using System.Xml.Serialization;

namespace LeagueSharp.CommonEx.Core
{
    /// <summary>
    /// MultiLanguage class used to translate textes easily.
    /// </summary>
    public static class MultiLanguage
    {
        /// <summary>
        /// Dictionary of Translated textes.
        /// </summary>
        public static Dictionary<string, string> Translations = new Dictionary<string, string>();

        /// <summary>
        /// Translates the text
        /// </summary>
        /// <param name="textToTranslate">Text that will be translated</param>
        /// <returns></returns>
        public static string _(string textToTranslate)
        {
            return Translations.ContainsKey(textToTranslate) ? Translations[textToTranslate] : textToTranslate;
        }

        /// <summary>
        /// Entries of Textes that are translated or will be translated.
        /// </summary>
        public class TranslatedEntry
        {
            /// <summary>
            /// Text that will be translated.
            /// </summary>
            [XmlAttribute]
            public string TextToTranslate;
            /// <summary>
            /// Translated text.
            /// </summary>
            [XmlAttribute]
            public string TranslatedText;
        }
    }
}
