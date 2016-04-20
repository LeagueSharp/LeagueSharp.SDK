// <copyright file="MenuSliderButton.cs" company="LeagueSharp">
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
    ///     Menu Slider.
    /// </summary>
    [Serializable]
    public class MenuSliderButton : MenuItem, ISerializable
    {
        #region Fields

        /// <summary>
        ///     The boriginal.
        /// </summary>
        private readonly bool bOriginal;

        /// <summary>
        ///     The original.
        /// </summary>
        private readonly int original;

        /// <summary>
        ///     The Button value.
        /// </summary>
        private bool bValue;

        /// <summary>
        ///     The value.
        /// </summary>
        private int value;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuSlider" /> class.
        /// </summary>
        /// <param name="name">
        ///     The internal name of this component
        /// </param>
        /// <param name="displayName">
        ///     The display name of this component
        /// </param>
        /// <param name="value">
        ///     The Value
        /// </param>
        /// <param name="minValue">
        ///     Minimum Value Boundary
        /// </param>
        /// <param name="maxValue">
        ///     Maximum Value Boundary
        /// </param>
        /// <param name="bValue">
        ///     The Button Value
        /// </param>
        /// <param name="uniqueString">
        ///     String used in saving settings
        /// </param>
        public MenuSliderButton(
            string name,
            string displayName,
            int value = 0,
            int minValue = 0,
            int maxValue = 100,
            bool bValue = false,
            string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.SValue = value;
            this.BValue = bValue;
            this.original = value;
            this.bOriginal = bValue;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuSlider" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected MenuSliderButton(SerializationInfo info, StreamingContext context)
        {
            this.value = (int)info.GetValue("value", typeof(int));
            this.bValue = (bool)info.GetValue("bValue", typeof(bool));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the boolean value is true or false.
        /// </summary>
        public bool BValue { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="MenuSlider" /> is interacting.
        /// </summary>
        /// <value>
        ///     <c>true</c> if interacting; otherwise, <c>false</c>.
        /// </value>
        public bool Interacting { get; set; }

        /// <summary>
        ///     Gets or sets the Slider Maximum Value.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        ///     Gets or sets the Slider Minimum Value.
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        ///     Gets or sets the Slider Current Value.
        /// </summary>
        public int SValue
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value < this.MinValue)
                {
                    this.value = this.MinValue;
                }
                else if (value > this.MaxValue)
                {
                    this.value = this.MaxValue;
                }
                else
                {
                    this.value = value;
                }
            }
        }

        /// <summary>
        ///     Gets the Slider Value if Button is active.
        /// </summary>
        public int Value => this.SValue != this.MinValue && this.BValue ? this.value : -1;

        /// <summary>
        ///     Slider Item Width.
        /// </summary>
        public override int Width => this.Handler.Width();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Slider Item Draw callback.
        /// </summary>
        public override void Draw()
        {
            this.Handler.Draw();
        }

        /// <summary>
        ///     Extracts the specified value.
        /// </summary>
        /// <param name="menuValue">The value.</param>
        public override void Extract(MenuItem menuValue)
        {
            this.SValue = ((MenuSliderButton)menuValue).value;
            this.BValue = ((MenuSliderButton)menuValue).bValue;
        }

        /// <summary>
        ///     Resets the MenuItem back to his default values.
        /// </summary>
        public override void RestoreDefault()
        {
            this.SValue = this.original;
            this.BValue = this.bOriginal;
        }

        /// <summary>
        ///     Slider Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
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

            info.AddValue("value", this.SValue, typeof(int));
            info.AddValue("bValue", this.BValue, typeof(bool));
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
        ///     The <see cref="ADrawable" /> handle.
        /// </returns>
        protected override ADrawable BuildHandler(ITheme theme)
        {
            return theme.BuildSliderButtonHandler(this);
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
            info.AddValue("value", this.SValue, typeof(int));
            info.AddValue("bValue", this.BValue, typeof(bool));
        }

        #endregion
    }
}