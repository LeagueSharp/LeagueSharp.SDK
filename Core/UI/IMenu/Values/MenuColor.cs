// <copyright file="MenuColor.cs" company="LeagueSharp">
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

    using SharpDX;

    /// <summary>
    ///     The menu color.
    /// </summary>
    [Serializable]
    public class MenuColor : MenuItem, ISerializable
    {
        #region Fields

        /// <summary>
        ///     The original value.
        /// </summary>
        private readonly ColorBGRA original;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuColor" /> class.
        /// </summary>
        /// <param name="name">
        ///     The internal name of this component
        /// </param>
        /// <param name="displayName">
        ///     The display name of this component
        /// </param>
        /// <param name="color">
        ///     The color
        /// </param>
        /// <param name="uniqueString">
        ///     String used in saving settings
        /// </param>
        public MenuColor(string name, string displayName, ColorBGRA color, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            this.Color = color;
            this.original = color;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuColor" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected MenuColor(SerializationInfo info, StreamingContext context)
        {
            var red = (byte)info.GetValue("red", typeof(byte));
            var green = (byte)info.GetValue("green", typeof(byte));
            var blue = (byte)info.GetValue("blue", typeof(byte));
            var alpha = (byte)info.GetValue("alpha", typeof(byte));
            this.Color = new ColorBGRA(red, green, blue, alpha);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///     Gets or sets the color.
        /// </summary>
        public ColorBGRA Color { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether hovering preview.
        /// </summary>
        public bool HoveringPreview { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting alpha.
        /// </summary>
        public bool InteractingAlpha { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting blue.
        /// </summary>
        public bool InteractingBlue { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting green.
        /// </summary>
        public bool InteractingGreen { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether interacting red.
        /// </summary>
        public bool InteractingRed { get; set; }

        /// <summary>
        ///     Gets the Value Width.
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
        /// <param name="component">The component.</param>
        public override void Extract(MenuItem component)
        {
            this.Color = ((MenuColor)component).Color;
        }

        /// <summary>
        ///     Resets the MenuItem back to his default values.
        /// </summary>
        public override void RestoreDefault()
        {
            this.Color = this.original;
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

            info.AddValue("red", this.Color.R, typeof(byte));
            info.AddValue("green", this.Color.G, typeof(byte));
            info.AddValue("blue", this.Color.B, typeof(byte));
            info.AddValue("alpha", this.Color.A, typeof(byte));
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
            return theme.BuildColorHandler(this);
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
            info.AddValue("red", this.Color.R, typeof(byte));
            info.AddValue("green", this.Color.G, typeof(byte));
            info.AddValue("blue", this.Color.B, typeof(byte));
            info.AddValue("alpha", this.Color.A, typeof(byte));
        }

        #endregion
    }
}