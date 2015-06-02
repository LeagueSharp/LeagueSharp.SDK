// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuInputText.cs" company="LeagueSharp">
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
//   InputText Menu Item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Values
{
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     InputText Menu Item.
    /// </summary>
    public class MenuInputText : AMenuValue
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuInputText" /> class.
        ///     InputText Constructor.
        /// </summary>
        /// <param name="text">
        ///     text string
        /// </param>
        public MenuInputText(string text = null)
        {
            this.Text = text ?? string.Empty;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="MenuInputText" /> has interaction.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this <see cref="MenuInputText" /> has interaction; otherwise, <c>false</c>.
        /// </value>
        public bool Interaction { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the InputText is focused.
        /// </summary>
        public bool IsFocused { get; set; }

        /// <summary>
        ///     Gets or sets the InputText's text string.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     InputText Item Width.
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
        ///     InputText Draw callback.
        /// </summary>
        public override void OnDraw()
        {
        }

        /// <summary>
        ///     InputText Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!MenuGUI.IsChatOpen && args.Msg == WindowsMessages.LBUTTONDOWN)
            {
            }
        }

        #endregion
    }
}