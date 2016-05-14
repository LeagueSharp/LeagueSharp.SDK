// <copyright file="MenuBool.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.UI
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    using LeagueSharp.SDK.UI.Skins;
    using LeagueSharp.SDK.Utils;

    /// <summary>
    ///     Menu boolean.
    /// </summary>
    [Serializable]
    public class MenuBool : MenuItem, ISerializable
    {
        #region Fields

        /// <summary>
        ///     The original value.
        /// </summary>
        private readonly bool original;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuBool" /> class.
        /// </summary>
        /// <param name="name">
        ///     Internal name of this component
        /// </param>
        /// <param name="displayName">
        ///     Display name of this component
        /// </param>
        /// <param name="value">
        ///     Boolean Value
        /// </param>
        /// <param name="uniqueString">
        ///     String used when saving settings.
        /// </param>
        public MenuBool(string name, string displayName, bool value = false, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            this.Value = value;
            this.original = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuBool" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected MenuBool(SerializationInfo info, StreamingContext context)
        {
            this.Value = (bool)info.GetValue("value", typeof(bool));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the boolean value is true or false.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        ///     Boolean Item Width requirement.
        /// </summary>
        public override int Width => this.Handler.Width();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Boolean Item Draw callback.
        /// </summary>
        public override void Draw()
        {
            this.Handler.Draw();
        }

        /// <summary>
        ///     Extracts the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public override void Extract(MenuItem component)
        {
            this.Value = ((MenuBool)component).Value;
        }

        /// <summary>
        ///     Resets the MenuItem back to his default values.
        /// </summary>
        public override void RestoreDefault()
        {
            this.Value = this.original;
        }

        /// <summary>
        ///     Boolean Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">The <see cref="WindowsKeys" /> instance</param>
        public override void WndProc(WindowsKeys args)
        {
            this.Handler.OnWndProc(args);
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

            info.AddValue("value", this.Value);
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
            return theme.BuildBoolHandler(this);
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
            info.AddValue("value", this.Value);
        }

        #endregion
    }
}