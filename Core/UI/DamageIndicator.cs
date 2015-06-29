// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DamageIndicator.cs" company="LeagueSharp">
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
//   Utility which displays the damage done to a unit with a Line + Damage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI
{
    using System;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    ///     Utility which displays the damage done to a unit with a Line + Damage.
    /// </summary>
    public static class DamageIndicator
    {
        #region Constants

        /// <summary>
        ///     Height of the HP Bar
        /// </summary>
        private const int Height = 8;

        /// <summary>
        ///     Width of the HP Bar
        /// </summary>
        private const int Width = 103;

        /// <summary>
        ///     X-Axis Offset of the HP Bar
        /// </summary>
        private const int XOffset = 10;

        /// <summary>
        ///     Y-Axis Offset of the HP Bar
        /// </summary>
        private const int YOffset = 20;

        #endregion

        #region Static Fields

        /// <summary>
        ///     Color of the line drawing on the HP Bar.
        /// </summary>
        private static Color color = Color.Lime;

        /// <summary>
        ///     Damage dealt to unit delegate.
        /// </summary>
        private static DamageToUnitDelegate damageToUnit;

        /// <summary>
        ///     Local HP Bar Damage Indicator
        /// </summary>
        private static bool enabled = true;

        #endregion

        #region Delegates

        /// <summary>
        ///     Delegate for <see cref="DamageToUnit" />
        /// </summary>
        /// <param name="hero">The hero we are drawing on.</param>
        /// <returns>Amount of damage done.</returns>
        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the color of the line drawing on the HP Bar.
        /// </summary>
        public static Color Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
            }
        }

        /// <summary>
        ///     Gets or sets the damage to the unit.
        /// </summary>
        public static DamageToUnitDelegate DamageToUnit
        {
            get
            {
                return damageToUnit;
            }

            set
            {
                if (damageToUnit == null)
                {
                    Drawing.OnDraw += Drawing_OnDraw;
                }

                damageToUnit = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the HP bar damage indicator is enabled.
        /// </summary>
        public static bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                enabled = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Drawing event.
        /// </summary>
        /// <param name="args">
        ///     Event data
        /// </param>
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled || damageToUnit == null)
            {
                return;
            }

            foreach (var unit in
                GameObjects.EnemyHeroes.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var xPos = barPos.X + XOffset + Width * percentHealthAfterDamage;

                Drawing.DrawText(
                    barPos.X + XOffset, 
                    barPos.Y + YOffset - 13, 
                    Color.Red, 
                    ((int)(unit.Health - damage)).ToString());
                Drawing.DrawLine(xPos, barPos.Y + YOffset, xPos, barPos.Y + YOffset + Height, 2, Color);
            }
        }

        #endregion
    }
}