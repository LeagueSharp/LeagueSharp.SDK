using System;
using System.Collections.Generic;
using System.IO;
using LeagueSharp.CommonEx.Core.Utils;

namespace LeagueSharp.CommonEx.Core.UI.Settings
{
    /// <summary>
    ///     SavedSettings class used to manage the saved Setting
    /// </summary>
    [Serializable]
    public static class SavedSettings
    {
        /// <summary>
        ///     Dictionary of loaded Files
        /// </summary>
        public static Dictionary<string, Dictionary<string, byte[]>> LoadedFiles =
            new Dictionary<string, Dictionary<string, byte[]>>();

        /// <summary>
        ///     Byte Array of the saved Data
        /// </summary>
        /// <param name="name">Name of the saved data</param>
        /// <param name="key">Key of the saved data</param>
        /// <returns></returns>
        public static byte[] GetSavedData(string name, string key)
        {
            var dic = LoadedFiles.ContainsKey(name) ? LoadedFiles[name] : Load(name);

            if (dic == null)
            {
                return null;
            }
            return dic.ContainsKey(key) ? dic[key] : null;
        }

        /// <summary>
        ///     Loads the Settings
        /// </summary>
        /// <param name="name">Name of the Settings</param>
        /// <returns></returns>
        public static Dictionary<string, byte[]> Load(string name)
        {
            try
            {
                var fileName = Path.Combine(MenuSettings.MenuConfigPath, name + ".bin");
                if (File.Exists(fileName))
                {
                    return Serialization.Deserialize<Dictionary<string, byte[]>>(File.ReadAllBytes(fileName));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        /// <summary>
        ///     Saves the Settings
        /// </summary>
        /// <param name="name">Name of the Settings</param>
        /// <param name="entries">Byte Array of the entries</param>
        public static void Save(string name, Dictionary<string, byte[]> entries)
        {
            try
            {
                Directory.CreateDirectory(MenuSettings.MenuConfigPath);
                var fileName = Path.Combine(MenuSettings.MenuConfigPath, name + ".bin");
                File.WriteAllBytes(fileName, Serialization.Serialize(entries));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}