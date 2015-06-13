// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuKeyBind.cs" company="LeagueSharp">
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
//   Menu KeyBind.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Values
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Windows.Forms;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.UI.IMenu.Skins;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     Menu KeyBind.
    /// </summary>
    [Serializable]
    public class MenuKeyBind : AMenuValue, ISerializable
    {
        #region Fields

        /// <summary>
        ///     The local active value.
        /// </summary>
        private bool active;

        /// <summary>
        ///     Local Interacting value.
        /// </summary>
        private bool interacting;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuKeyBind" /> class.
        ///     Menu KeyBind Constructor
        /// </summary>
        /// <param name="key">
        ///     The Key to bind
        /// </param>
        /// <param name="type">
        ///     Key bind type
        /// </param>
        public MenuKeyBind(Keys key, KeyBindType type)
        {
            this.Key = key;
            this.Type = type;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuKeyBind" /> class.
        ///     Menu KeyBind Constructor
        /// </summary>
        public MenuKeyBind()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuKeyBind" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected MenuKeyBind(SerializationInfo info, StreamingContext context)
        {
            this.Key = (Keys)info.GetValue("key", typeof(Keys));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the key is active.
        /// </summary>
        public bool Active
        {
            get
            {
                return this.active;
            }

            set
            {
                if (this.active != value)
                {
                    this.active = value;
                    this.FireEvent();
                }
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="MenuKeyBind" /> is interacting.
        /// </summary>
        /// <value>
        ///     <c>true</c> if interacting; otherwise, <c>false</c>.
        /// </value>
        public bool Interacting
        {
            get
            {
                return this.interacting;
            }

            set
            {
                this.interacting = value;
                MenuManager.Instance.ForcedOpen = value;
            }
        }

        /// <summary>
        ///     Gets or sets the KeyBind Key Value.
        /// </summary>
        public Keys Key { get; set; }

        /// <summary>
        ///     Gets or sets the KeyBind Type.
        /// </summary>
        public KeyBindType Type { get; set; }

        /// <summary>
        ///     KeyBind Item Width.
        /// </summary>
        public override int Width
        {
            get
            {
                return ThemeManager.Current.KeyBind.Width(this);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Extracts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Extract(AMenuValue value)
        {
            var keybind = (MenuKeyBind)value;
            this.Key = keybind.Key;
        }

        /// <summary>
        ///     KeyBind Item Draw callback.
        /// </summary>
        public override void OnDraw()
        {
            ThemeManager.Current.KeyBind.Draw(this);
        }

        /// <summary>
        ///     KeyBind Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!MenuGUI.IsChatOpen)
            {
                switch (args.Msg)
                {
                    case WindowsMessages.KEYDOWN:
                        this.HandleDown(args.Key);
                        break;
                    case WindowsMessages.KEYUP:
                        if (this.Interacting && args.SingleKey != Keys.ShiftKey)
                        {
                            this.ChangeKey(args.SingleKey == Keys.Escape ? Keys.None : args.Key);
                        }
                        else
                        {
                            this.HandleUp(args.Key);
                        }

                        break;
                    case WindowsMessages.XBUTTONDOWN:
                        this.HandleDown(args.SideButton);
                        break;
                    case WindowsMessages.XBUTTONUP:
                        if (this.Interacting)
                        {
                            this.ChangeKey(args.SideButton);
                        }
                        else
                        {
                            this.HandleUp(args.SideButton);
                        }

                        break;
                    case WindowsMessages.MBUTTONDOWN:
                        this.HandleDown(Keys.MButton);
                        break;
                    case WindowsMessages.MBUTTONUP:
                        if (this.Interacting)
                        {
                            this.ChangeKey(Keys.MButton);
                        }
                        else
                        {
                            this.HandleUp(Keys.MButton);
                        }

                        break;
                    case WindowsMessages.RBUTTONDOWN:
                        this.HandleDown(Keys.RButton);
                        break;
                    case WindowsMessages.RBUTTONUP:
                        if (this.Interacting)
                        {
                            this.ChangeKey(Keys.RButton);
                        }
                        else
                        {
                            this.HandleUp(Keys.RButton);
                        }

                        break;
                    case WindowsMessages.LBUTTONDOWN:
                        if (this.Interacting)
                        {
                            this.ChangeKey(Keys.LButton);
                        }
                        else if (this.Container.Visible)
                        {
                            var container = ThemeManager.Current.KeyBind.ButtonBoundaries(this);
                            var content = ThemeManager.Current.KeyBind.KeyBindBoundaries(this);

                            if (args.Cursor.IsUnderRectangle(
                                container.X, 
                                container.Y, 
                                container.Width, 
                                container.Height))
                            {
                                this.Active = !this.Active;
                            }
                            else if (args.Cursor.IsUnderRectangle(content.X, content.Y, content.Width, content.Height))
                            {
                                this.Interacting = !this.Interacting;
                            }
                            else
                            {
                                this.HandleDown(Keys.LButton);
                            }
                        }

                        break;
                    case WindowsMessages.LBUTTONUP:
                        this.HandleUp(Keys.LButton);
                        break;
                }
            }
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("key", this.Key, typeof(Keys));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the
        ///     target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
        /// <param name="context">
        ///     The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this
        ///     serialization.
        /// </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("key", this.Key, typeof(Keys));
        }

        /// <summary>
        ///     ChangeKey method.
        /// </summary>
        /// <param name="newKey">
        ///     The new key
        /// </param>
        private void ChangeKey(Keys newKey)
        {
            this.Key = newKey;
            this.Interacting = false;
            MenuManager.Instance.ResetWidth();
        }

        /// <summary>
        ///     HandleDown method.
        /// </summary>
        /// <param name="expectedKey">
        ///     The expected key
        /// </param>
        private void HandleDown(Keys expectedKey)
        {
            if (expectedKey == this.Key && this.Type == KeyBindType.Press)
            {
                this.Active = true;
            }
        }

        /// <summary>
        ///     HandleUp method.
        /// </summary>
        /// <param name="expectedKey">
        ///     The expected key
        /// </param>
        private void HandleUp(Keys expectedKey)
        {
            if (expectedKey == this.Key)
            {
                switch (this.Type)
                {
                    case KeyBindType.Press:
                        this.Active = false;
                        break;
                    case KeyBindType.Toggle:
                        this.Active = !this.Active;
                        break;
                }
            }
        }

        #endregion
    }
}