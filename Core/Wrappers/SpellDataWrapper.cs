// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpellDataWrapper.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   SpellData Wrapper
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Wrappers
{
    /// <summary>
    ///     SpellData Wrapper
    /// </summary>
    public class SpellDataWrapper
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellDataWrapper" /> class.
        /// </summary>
        /// <param name="slot">
        ///     The SpellSlot
        /// </param>
        public SpellDataWrapper(SpellSlot slot)
        {
            this.Slot = slot;
            this.LoadSpellData(ObjectManager.Player.Spellbook.GetSpell(slot).SData);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellDataWrapper" /> class.
        /// </summary>
        /// <param name="base">
        ///     <see cref="Obj_AI_Base" /> object
        /// </param>
        /// <param name="slot">
        ///     The SpellSlot
        /// </param>
        public SpellDataWrapper(Obj_AI_Base @base, SpellSlot slot)
        {
            this.Slot = slot;
            this.LoadSpellData(@base.Spellbook.GetSpell(slot).SData);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Spell Delay
        /// </summary>
        public float Delay { get; set; }

        /// <summary>
        ///     Gets or sets the Spell Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the Spell Range
        /// </summary>
        public float Range { get; set; }

        /// <summary>
        ///     Gets or sets the Spell Slot
        /// </summary>
        public SpellSlot Slot { get; set; }

        /// <summary>
        ///     Gets or sets the Spell Speed
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        ///     Gets or sets the Spell Width
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this spell hass collision.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this spell has collision; otherwise, <c>false</c>.
        /// </value>
        public bool Collision { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Load the spell data
        /// </summary>
        /// <param name="spellData">
        ///     <see cref="SpellData" /> instance
        /// </param>
        private void LoadSpellData(SpellData spellData)
        {
            this.Range = spellData.CastRange;
            this.Width = spellData.LineWidth.Equals(0) ? spellData.CastRadius : spellData.LineWidth;
            this.Speed = spellData.MissileSpeed;
            this.Delay = spellData.CastFrame / 30;
            this.Name = spellData.Name;
            this.Collision = spellData.HaveHitBone;
        }

        #endregion
    }
}