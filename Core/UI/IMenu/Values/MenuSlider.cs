// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuSlider.cs" company="LeagueSharp">
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
//   Menu Slider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
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
    public class MenuSlider : MenuItem, ISerializable
    {
        private int _value;

        private readonly int original;

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuSlider" /> class.
        /// </summary>
        /// <param name="displayName">The display name of this component</param>
        /// <param name="value">
        ///     The Value
        /// </param>
        /// <param name="minValue">
        ///     Minimum Value Boundary
        /// </param>
        /// <param name="maxValue">
        ///     Maximum Value Boundary
        /// </param>
        /// <param name="name">The internal name of this component</param>
        /// <param name="uniqueString">String used in saving settings</param>
        public MenuSlider(string name, string displayName, int value = 0, int minValue = 0, int maxValue = 100, string uniqueString = "")
            : base(name, displayName, uniqueString)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.Value = value;
            original = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuSlider" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        protected MenuSlider(SerializationInfo info, StreamingContext context)
        {
            this.Value = (int)info.GetValue("value", typeof(int));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether this <see cref="MenuSlider" /> is interacting.
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
        public int Value {
            get
            {
                return _value;
            }
            set
            {
                if (value < this.MinValue)
                {
                    _value = this.MinValue;
                }
                else if (value > this.MaxValue)
                {
                    _value = this.MaxValue;
                }
                else
                {
                    _value = value;
                }
            } 
        }

        /// <summary>
        ///     Slider Item Width.
        /// </summary>
        public override int Width
        {
            get
            {
                return Handler.Width();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Extracts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Extract(MenuItem value)
        {
            this.Value = ((MenuSlider)value).Value;
        }

        /// <summary>
        ///     Slider Item Draw callback.
        /// </summary>
        public override void Draw()
        {
            Handler.Draw();
        }

        /// <summary>
        ///     Slider Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
        public override void WndProc(WindowsKeys args)
        {
            Handler.OnWndProc(args);
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

            info.AddValue("value", this.Value, typeof(int));
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
            info.AddValue("value", this.Value, typeof(int));
        }
       

        #endregion

        /// <summary>
        /// Resets the MenuItem back to his default values.
        /// </summary>
        public override void RestoreDefault()
        {
            Value = original;
        }

        /// <summary>
        /// Builds an <see cref="ADrawable"/> for this component.
        /// </summary>
        /// <returns></returns>
        protected override ADrawable BuildHandler(ITheme theme)
        {
            return theme.BuildSliderHandler(this);
        }
    }
}