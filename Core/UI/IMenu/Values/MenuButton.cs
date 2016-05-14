// <copyright file="MenuButton.cs" company="LeagueSharp">
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
    ///     A Button designed to perform an action when clicked
    /// </summary>
    public class MenuButton : MenuItem
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuButton" /> class, with a specified button text.
        /// </summary>
        /// <param name="name">
        ///     The internal name of this component
        /// </param>
        /// <param name="displayName">
        ///     The display name of this component
        /// </param>
        /// <param name="buttonText">
        ///     The button text
        /// </param>
        /// <param name="uniqueString">
        ///     String used in saving settings
        /// </param>
        public MenuButton(string name, string displayName, string buttonText, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            this.ButtonText = buttonText;
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     The action
        /// </summary>
        public delegate void ButtonAction();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the action to be performed when the button is clicked
        /// </summary>
        public ButtonAction Action { get; set; }

        /// <summary>
        ///     Gets or sets the text on the button
        /// </summary>
        public string ButtonText { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the item is being hovered on.
        /// </summary>
        public bool Hovering { get; set; }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        public override int Width => this.Handler.Width();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     On Draw event.
        /// </summary>
        public override void Draw()
        {
            this.Handler.Draw();
        }

        /// <summary>
        ///     Extracts the component.
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
        ///     On Windows Process Message event.
        /// </summary>
        /// <param name="args">
        ///     The event data.
        /// </param>
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
            return theme.BuildButtonHandler(this);
        }

        #endregion
    }
}