// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuSeparator.cs" company="LeagueSharp">
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
//   A menu separator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Values
{
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.UI.IMenu.Skins;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     A menu separator.
    /// </summary>
    public class MenuSeparator : AMenuValue
    {
        #region Public Properties

        /// <summary>
        ///     Value Width.
        /// </summary>
        public override int Width
        {
            get
            {
                return 0;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Extracts the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public override void Extract(AMenuValue component)
        {
        }

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        public override void OnDraw()
        {
            ThemeManager.Current.Separator.Draw(this);
        }

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            // not needed                        
        }

        #endregion
    }
}