// <copyright file="WindowsKeys.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx.Utils
{
    using System;
    using System.Windows.Forms;

    using LeagueSharp.SDKEx.Enumerations;

    using SharpDX;

    /// <summary>
    ///     Utility class to translate Windows Messages into keys or into <see cref="WindowsMessages" />
    /// </summary>
    public class WindowsKeys
    {
        #region Fields

        /// <summary>
        ///     The arguments
        /// </summary>
        private readonly WndEventArgs args;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WindowsKeys" /> class.
        ///     WindowsKeys constructor
        /// </summary>
        /// <param name="args">
        ///     <see cref="WndEventArgs" /> event data
        /// </param>
        public WindowsKeys(WndEventArgs args)
        {
            this.args = args;
            this.Cursor = Utils.Cursor.Position;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the textual representation of the input.
        /// </summary>
        public char Char => Convert.ToChar(this.args.WParam);

        /// <summary>
        ///     Gets the Cursor Position
        /// </summary>
        public Vector2 Cursor { get; private set; }

        /// <summary>
        ///     Gets the full name of the mapped key.
        /// </summary>
        public Keys Key
            =>
                (Keys)((int)this.args.WParam) != Control.ModifierKeys
                    ? (Keys)((int)this.args.WParam) | Control.ModifierKeys
                    : (Keys)((int)this.args.WParam);

        /// <summary>
        ///     Gets the message of the key.
        /// </summary>
        public WindowsMessages Msg => (WindowsMessages)this.args.Msg;

        /// <summary>
        ///     Gets or sets a value indicating whether to process the command.
        /// </summary>
        public bool Process
        {
            get
            {
                return this.args.Process;
            }

            set
            {
                this.args.Process = value;
            }
        }

        /// <summary>
        ///     Gets the side button.
        /// </summary>
        public Keys SideButton
        {
            get
            {
                var bytes = BitConverter.GetBytes(this.args.WParam);

                if (bytes.Length > 2)
                {
                    int buttonId = bytes[2];
                    var sideButton = Keys.None;

                    if (buttonId == 1)
                    {
                        sideButton = Keys.XButton1;
                    }
                    else if (buttonId == 2)
                    {
                        sideButton = Keys.XButton2;
                    }

                    return sideButton;
                }

                return Keys.None;
            }
        }

        /// <summary>
        ///     Gets the single name of the mapped key.
        /// </summary>
        public Keys SingleKey => (Keys)((int)this.args.WParam);

        /// <summary>
        ///     Gets the <c>WParam</c>
        /// </summary>
        public uint WParam => this.args.WParam;

        #endregion
    }
}