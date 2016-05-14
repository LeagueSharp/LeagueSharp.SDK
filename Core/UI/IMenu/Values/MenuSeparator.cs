// <copyright file="MenuSeparator.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx.UI
{
    using LeagueSharp.SDKEx.UI.Skins;
    using LeagueSharp.SDKEx.Utils;

    /// <summary>
    ///     A menu separator.
    /// </summary>
    public class MenuSeparator : MenuItem
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuSeparator" /> class.
        /// </summary>
        /// <param name="name">
        ///     The internal name of this menu component
        /// </param>
        /// <param name="displayName">
        ///     The display name of this menu component
        /// </param>
        /// <param name="uniqueString">
        ///     String used in saving settings
        /// </param>
        public MenuSeparator(string name, string displayName, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Value Width.
        /// </summary>
        public override int Width => this.Handler.Width();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        public override void Draw()
        {
            this.Handler.Draw();
        }

        /// <summary>
        ///     Extracts the specified component.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        public override void Extract(MenuItem component)
        {
            // Do nothing.
        }

        /// <summary>
        ///     Resets the MenuItem back to his default values.
        /// </summary>
        public override void RestoreDefault()
        {
            // Do nothing.
        }

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public override void WndProc(WindowsKeys args)
        {
            this.Handler.OnWndProc(args);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Builds an <see cref="ADrawable" /> for this component.
        /// </summary>
        /// <param name="theme">
        ///     The theme.
        /// </param>
        /// <returns>
        ///     The <see cref="ADrawable" /> instance.
        /// </returns>
        protected override ADrawable BuildHandler(ITheme theme)
        {
            return theme.BuildSeparatorHandler(this);
        }

        #endregion
    }
}