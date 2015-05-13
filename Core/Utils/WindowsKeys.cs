// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsKeys.cs" company="LeagueSharp">
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
//   Utility class to translate Windows Messages into keys or into <see cref="WindowsMessages" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Utils
{
    using System.Windows.Forms;

    using LeagueSharp.SDK.Core.Enumerations;

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
        ///     Gets the Cursor Position
        /// </summary>
        public Vector2 Cursor { get; private set; }

        /// <summary>
        ///     Gets the full name of the mapped key.
        /// </summary>
        public Keys Key
        {
            get
            {
                Keys keyData;
                if ((Keys)((int)this.args.WParam) != Control.ModifierKeys)
                {
                    keyData = (Keys)((int)this.args.WParam) | Control.ModifierKeys;
                }
                else
                {
                    keyData = (Keys)((int)this.args.WParam);
                }

                return keyData;
            }
        }

        /// <summary>
        ///     Gets the message of the key.
        /// </summary>
        public WindowsMessages Msg
        {
            get
            {
                return (WindowsMessages)this.args.Msg;
            }
        }

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
        ///     Gets the single name of the mapped key.
        /// </summary>
        public Keys SingleKey
        {
            get
            {
                return (Keys)((int)this.args.WParam);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the key from the <see cref="WndEventArgs" />.
        /// </summary>
        /// <param name="args"><see cref="WndEventArgs" /> data</param>
        /// <returns><see cref="System.Windows.Forms.Keys" /> data</returns>
        public static Keys GetKey(WndEventArgs args)
        {
            return (Keys)((int)args.WParam);
        }

        /// <summary>
        ///     Returns the set collection of keys from the <see cref="WndEventArgs" />.
        /// </summary>
        /// <param name="args"><see cref="WndEventArgs" /> data</param>
        /// <returns><see cref="System.Windows.Forms.Keys" /> data</returns>
        public static Keys GetKeys(WndEventArgs args)
        {
            Keys keyData;
            if ((Keys)((int)args.WParam) != Control.ModifierKeys)
            {
                keyData = (Keys)((int)args.WParam) | Control.ModifierKeys;
            }
            else
            {
                keyData = (Keys)((int)args.WParam);
            }

            return keyData;
        }

        /// <summary>
        ///     Returns the translated windows message from the <see cref="WndEventArgs" />
        /// </summary>
        /// <param name="args"><see cref="WndEventArgs" /> data</param>
        /// <returns>
        ///     <see cref="WindowsMessages" />
        /// </returns>
        public static WindowsMessages GetWindowsMessage(WndEventArgs args)
        {
            return (WindowsMessages)args.Msg;
        }

        #endregion
    }
}