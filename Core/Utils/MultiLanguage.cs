namespace LeagueSharp.SDK.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Resources;
    using System.Security.Cryptography;
    using System.Text;

    using LeagueSharp.Sandbox;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Properties;

    using Newtonsoft.Json;

    /// <summary>
    ///     Provides multi-lingual strings.
    /// </summary>
    public static class MultiLanguage
    {
        #region Static Fields

        /// <summary>
        ///     The translations
        /// </summary>
        private static Dictionary<string, string> translations = new Dictionary<string, string>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Loads the translation.
        /// </summary>
        /// <param name="languageName">Name of the language.</param>
        /// <returns><c>true</c> if the operation succeeded, <c>false</c> otherwise false.</returns>
        public static bool LoadLanguage(string languageName)
        {
            try
            {
                var languageStrings =
                    new ResourceManager("LeagueSharp.SDK.Properties.Resources", typeof(Resources).Assembly).GetString(
                        languageName + "Json");

                if (string.IsNullOrEmpty(languageStrings))
                {
                    return false;
                }

                translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(languageStrings);
                return true;
            }
            catch (Exception ex)
            {
                Logging.Write()(LogLevel.Error, $"[MultiLanguage] Load Language Catch Exception：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        ///     judge the select language
        /// </summary>
        public static void LoadTranslation()
        {
            try
            {
                var selectLanguage = SandboxConfig.SelectedLanguage;

                if (selectLanguage == "Chinese")
                {
                    LoadLanguage("Chinese");
                }
                else
                {
                    // ignore
                }
            }
            catch (Exception ex)
            {
                Logging.Write()(LogLevel.Error, $"[MultiLanguage] Load Translation Catch Exception：{ex.Message}");
            }
        }

        /// <summary>
        ///     Translates the text into the loaded language.
        /// </summary>
        /// <param name="textToTranslate">The text to translate.</param>
        /// <returns>System.String.</returns>
        public static string Translation(string textToTranslate)
        {
            var textToTranslateToLower = textToTranslate.ToLower();

            return translations.ContainsKey(textToTranslateToLower)
                       ? translations[textToTranslateToLower]
                       : (translations.ContainsKey(textToTranslate) ? translations[textToTranslate] : textToTranslate);
        }

        #endregion

        #region Methods

        private static string DesDecrypt(string decryptString, string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            var keyIV = keyBytes;
            var inputByteArray = Convert.FromBase64String(decryptString);
            var provider = new DESCryptoServiceProvider();
            var mStream = new MemoryStream();
            var cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        #endregion
    }
}