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
    using LeagueSharp.SDK.Core.UI.IMenu.Skins;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     A menu separator.
    /// </summary>
    public class MenuSeparator : MenuItem
    {
        /// <summary>
        /// A new instance of MenuSeperator
        /// </summary>
        /// <param name="name">The internal name of this menu component</param>
        /// <param name="displayName">The display name of this menu component</param>
        /// <param name="uniqueString">String used in saving settings</param>
        public MenuSeparator(string name, string displayName, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            
        }

        #region Public Properties

        /// <summary>
        ///     Value Width.
        /// </summary>
        public override int Width
        {
            get
            {
                return Handler.Width();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Extracts the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public override void Extract(MenuItem component)
        {
            //do nothing
        }

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        public override void Draw()
        {
            Handler.Draw();
        }

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public override void WndProc(WindowsKeys args)
        {
            Handler.OnWndProc(args);      
        }

        #endregion

        /// <summary>
        /// Resets the MenuItem back to his default values.
        /// </summary>
        public override void RestoreDefault()
        {
            //do nothing
        }

        /// <summary>
        /// Builds an <see cref="ADrawable"/> for this component.
        /// </summary>
        /// <returns></returns>
        protected override ADrawable BuildHandler(ITheme theme)
        {
            return theme.BuildSeparatorHandler(this);
        }
    }
}