// <copyright file="MenuKeyBind.cs" company="LeagueSharp">
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
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Windows.Forms;

    using LeagueSharp.SDKEx.Enumerations;
    using LeagueSharp.SDKEx.UI.Skins;
    using LeagueSharp.SDKEx.Utils;

    /// <summary>
    ///     Menu KeyBind.
    /// </summary>
    [Serializable]
    public class MenuKeyBind : MenuItem, ISerializable
    {
        #region Fields

        /// <summary>
        ///     The original value.
        /// </summary>
        private readonly Keys original;

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
        /// </summary>
        /// <param name="name">
        ///     The internal name of the component
        /// </param>
        /// <param name="displayName">
        ///     The display name of the component
        /// </param>
        /// <param name="key">
        ///     The Key to bind
        /// </param>
        /// <param name="type">
        ///     Key bind type
        /// </param>
        /// <param name="uniqueString">
        ///     String used in saving settings
        /// </param>
        public MenuKeyBind(string name, string displayName, Keys key, KeyBindType type, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            this.Key = key;
            this.Type = type;
            this.original = key;
            Game.OnWndProc += this.Game_OnWndProc;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuKeyBind" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected MenuKeyBind(SerializationInfo info, StreamingContext context)
        {
            this.Key = (Keys)info.GetValue("key", typeof(Keys));
            this.original = (Keys)info.GetValue("original", typeof(Keys));
            this.Active = (bool)info.GetValue("active", typeof(bool));
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
        public override int Width => this.Handler.Width();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     KeyBind Item Draw callback.
        /// </summary>
        public override void Draw()
        {
            this.Handler.Draw();
        }

        /// <summary>
        ///     Extracts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Extract(MenuItem value)
        {
            var keybind = (MenuKeyBind)value;
            if (this.original == keybind.original)
            {
                this.Key = keybind.Key;
            }
            this.Active = keybind.active;
        }

        /// <summary>
        ///     Resets the MenuItem back to his default values.
        /// </summary>
        public override void RestoreDefault()
        {
            this.Key = this.original;
            this.Active = false;
        }

        /// <summary>
        ///     KeyBind Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
        public override void WndProc(WindowsKeys args)
        {
            // do nothing, we use the fast OnWndProc for keybinds
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
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue("key", this.Key, typeof(Keys));
            info.AddValue("original", this.original, typeof(Keys));
            info.AddValue("active", this.active, typeof(bool));
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
            return theme.BuildKeyBindHandler(this);
        }

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
            info.AddValue("original", this.original, typeof(Keys));
            info.AddValue("active", this.active, typeof(bool));
        }

        private void Game_OnWndProc(WndEventArgs args)
        {
            this.Handler.OnWndProc(new WindowsKeys(args));
        }

        #endregion
    }
}