// <copyright file="LessCastsToKill.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.TSModes
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.UI;

    /// <summary>
    ///     The less casts to kill Mode.
    /// </summary>
    public class LessCastsToKill : ITargetSelectorMode
    {
        #region Public Properties

        /// <inheritdoc />
        public string DisplayName => "Less Casts To Kill";

        /// <inheritdoc />
        public string Name => "less-casts-to-kill";

        #endregion

        #region Public Methods and Operators

        /// <inheritdoc />
        public void AddToMenu(Menu menu)
        {
        }

        /// <inheritdoc />
        public List<Obj_AI_Hero> OrderChampions(List<Obj_AI_Hero> heroes)
        {
            return heroes.OrderBy(x => x.Health / GameObjects.Player.TotalMagicalDamage).ToList();
        }

        #endregion
    }
}