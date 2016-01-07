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

namespace LeagueSharp.SDK.Core.UI.IMenu.Values
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    using LeagueSharp.SDK.Core.UI.IMenu.Skins;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     Menu Slider.
    /// </summary>
    [Serializable]
    public class MenuSliderBool : MenuItem, ISerializable
    {
        #region Fields

        /// <summary>
        ///     The boriginal.
        /// </summary>
        private readonly bool originalBoolValue;

        /// <summary>
        ///     The original.
        /// </summary>
        private readonly int originalSliderValue;

        private bool boolValue;

        /// <summary>
        ///     The value.
        /// </summary>
        private int sliderValue;

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
        /// <param name="boolean">
        ///     The Bool Value
        /// </param>
        /// <param name="uniqueString">
        ///     String used in saving settings
        /// </param>
        public MenuSliderBool(
            string name,
            string displayName,
            int value = 0,
            int minValue = 0,
            int maxValue = 100,
            bool boolean = false,
            string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            this.SliderMinValue = minValue;
            this.SliderMaxValue = maxValue;
            this.SliderValue = value;
            this.BoolValue = boolean;
            this.originalSliderValue = value;
            this.originalBoolValue = boolean;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuSliderBool" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected MenuSliderBool(SerializationInfo info, StreamingContext context)
        {
            this.sliderValue = (int)info.GetValue("sliderValue", typeof(int));
            this.boolValue = (bool)info.GetValue("boolValue", typeof(bool));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the boolean value is true or false.
        /// </summary>
        public bool BoolValue
        {
            get
            {
                return this.boolValue;
            }
            set
            {
                this.boolValue = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="MenuSliderBool" /> is interacting.
        /// </summary>
        /// <value>
        ///     <c>true</c> if interacting; otherwise, <c>false</c>.
        /// </value>
        public bool Interacting { get; set; }

        /// <summary>
        ///     Gets or sets the Slider Maximum Value.
        /// </summary>
        public int SliderMaxValue { get; set; }

        /// <summary>
        ///     Gets or sets the Slider Minimum Value.
        /// </summary>
        public int SliderMinValue { get; set; }

        /// <summary>
        ///     Gets or sets the Slider Current Value.
        /// </summary>
        public int SliderValue
        {
            get
            {
                return this.sliderValue;
            }

            set
            {
                if (value < this.SliderMinValue)
                {
                    this.sliderValue = this.SliderMinValue;
                }
                else if (value > this.SliderMaxValue)
                {
                    this.sliderValue = this.SliderMaxValue;
                }
                else
                {
                    this.sliderValue = value;
                }
            }
        }

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
            this.SliderValue = ((MenuSliderBool)menuValue).sliderValue;
            this.BoolValue = ((MenuSliderBool)menuValue).boolValue;
        }

        /// <summary>
        ///     Resets the MenuItem back to his default values.
        /// </summary>
        public override void RestoreDefault()
        {
            this.SliderValue = this.originalSliderValue;
            this.BoolValue = this.originalBoolValue;
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

            info.AddValue("sliderValue", this.SliderValue, typeof(int));
            info.AddValue("boolValue", this.BoolValue, typeof(bool));
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
            return theme.BuildSliderBoolHandler(this);
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
            info.AddValue("sliderValue", this.SliderValue, typeof(int));
            info.AddValue("boolValue", this.BoolValue, typeof(bool));
        }

        #endregion
    }
}