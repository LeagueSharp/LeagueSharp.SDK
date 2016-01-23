// <copyright file="Extensions.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDK
{
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;

    public static partial class Extensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     From a JSON-Type string to the type given.
        /// </summary>
        /// <param name="json">
        ///     The JSON string.
        /// </param>
        /// <typeparam name="T">
        ///     The type to convert the JSON to
        /// </typeparam>
        /// <returns>
        ///     The converted type of the JSON.
        /// </returns>
        public static T FromJson<T>(this string json)
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(json)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        /// <summary>
        ///     Returns if the spell is ready to use.
        /// </summary>
        /// <param name="spell">Spell Data Instance</param>
        /// <param name="t">Time Left</param>
        /// <returns>Is Spell Ready to use</returns>
        public static bool IsReady(this SpellDataInst spell, int t = 0)
        {
            return (spell != null)
                   && (spell.Slot != SpellSlot.Unknown && t == 0
                           ? spell.State == SpellState.Ready
                           : (spell.State == SpellState.Ready
                              || (spell.State == SpellState.Cooldown && (spell.CooldownExpires - Game.Time) <= t / 1000f)));
        }

        /// <summary>
        ///     Returns if the spell is ready to use.
        /// </summary>
        /// <param name="slot">The SpellSlot</param>
        /// <param name="t">Time Left</param>
        /// <returns>Is Spell Ready to use</returns>
        public static bool IsReady(this SpellSlot slot, int t = 0)
        {
            var s = GameObjects.Player.Spellbook.GetSpell(slot);
            return s != null && IsReady(s, t);
        }

        /// <summary>
        ///     To a JSON-Type string from an object.
        /// </summary>
        /// <param name="obj">
        ///     The object
        /// </param>
        /// <returns>
        ///     The JSON string.
        /// </returns>
        public static string ToJson(this object obj)
        {
            using (var ms = new MemoryStream())
            {
                var js = new DataContractJsonSerializer(obj.GetType());
                js.WriteObject(ms, obj);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        #endregion
    }
}