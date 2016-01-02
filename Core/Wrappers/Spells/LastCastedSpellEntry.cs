// <copyright file="LastCastedSpellEntry.cs" company="LeagueSharp">
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
    /// <summary>
    ///     Holds information about the last casted spell a unit did.
    /// </summary>
    public class LastCastedSpellEntry
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LastCastedSpellEntry" /> class.
        /// </summary>
        /// <param name="args">
        ///     Processed Casted Spell Data
        /// </param>
        internal LastCastedSpellEntry(GameObjectProcessSpellCastEventArgs args)
        {
            this.Name = args.SData.Name;
            this.Target = args.Target as Obj_AI_Base;
            this.StartTime = Variables.TickCount;
            this.EndTime = Variables.TickCount + args.SData.SpellCastTime;
            this.SpellData = args.SData;
            this.IsValid = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LastCastedSpellEntry" /> class.
        /// </summary>
        internal LastCastedSpellEntry()
        {
            this.Name = string.Empty;
            this.Target = null;
            this.StartTime = 0;
            this.EndTime = 0;
            this.SpellData = null;
            this.IsValid = false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the end time of the cast.
        /// </summary>
        public float EndTime { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is the spell is valid and not empty.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        ///     Gets or sets the name of the spell last casted.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="SpellData" /> of the spell casted.
        /// </summary>
        public SpellData SpellData { get; set; }

        /// <summary>
        ///     Gets or sets the Start time of the cast.
        /// </summary>
        public float StartTime { get; set; }

        /// <summary>
        ///     Gets or sets the Target
        /// </summary>
        public Obj_AI_Base Target { get; set; }

        #endregion
    }
}