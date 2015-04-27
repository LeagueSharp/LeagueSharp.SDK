using System;
using LeagueSharp.CommonEx.Core.Enumerations;

namespace LeagueSharp.CommonEx.Core.UI
{
    /// <summary>
    ///     Menu Keybinds
    /// </summary>
    [Serializable]
    public struct KeyBind
    {
        /// <summary>
        ///     Boolean if the Keybind is active
        /// </summary>
        public bool Active;

        /// <summary>
        ///     Key of the Keybind
        /// </summary>
        public uint Key;

        /// <summary>
        ///     Keybindtype of the Key
        /// </summary>
        public KeyBindType Type;

        /// <summary>
        ///     Creates the Keybind
        /// </summary>
        /// <param name="key">Key of the Keybind</param>
        /// <param name="type">Keybind Type</param>
        /// <param name="defaultValue">Default value of the Keybind</param>
        public KeyBind(uint key, KeyBindType type, bool defaultValue = false)
        {
            Key = key;
            Active = defaultValue;
            Type = type;
        }
    }
}