// <copyright file="ShortDistancePlayer.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Core.Wrappers.TargetSelector.Modes.Weights
{
    using LeagueSharp.SDK.Core.Extensions;

    /// <summary>
    ///     Short Distance to Player
    /// </summary>
    public class ShortDistancePlayer : IWeightItem
    {
        #region Public Properties

        public int DefaultWeight => 5;

        public string DisplayName => "Short Distance to Player";

        public bool Inverted => true;

        public string Name => "short-distance-player";

        #endregion

        #region Public Methods and Operators

        public float GetValue(Obj_AI_Hero hero) => hero.Distance(GameObjects.Player);

        #endregion
    }
}