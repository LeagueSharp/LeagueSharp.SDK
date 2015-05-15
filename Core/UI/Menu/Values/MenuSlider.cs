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
namespace LeagueSharp.SDK.Core.UI.Values
{
    using System;
    using System.Runtime.Serialization;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Skins;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Menu Slider.
    /// </summary>
    [Serializable]
    public class MenuSlider : AMenuValue, ISerializable
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuSlider" /> class.
        /// </summary>
        /// <param name="value">
        ///     The Value
        /// </param>
        /// <param name="minValue">
        ///     Minimum Value Boundary
        /// </param>
        /// <param name="maxValue">
        ///     Maximum Value Boundary
        /// </param>
        public MenuSlider(int value = 0, int minValue = 0, int maxValue = 100)
        {
            this.Value = value;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuSlider" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public MenuSlider(SerializationInfo info, StreamingContext context)
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
        public bool Interacting { get; private set; }

        /// <summary>
        ///     Gets or sets the Slider Maximum Value.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        ///     Gets or sets the Slider Minimum Value.
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        ///     Slider Item Position.
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        ///     Gets or sets the Slider Current Value.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        ///     Slider Item Width.
        /// </summary>
        public override int Width
        {
            get
            {
                return 100;
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
            this.Value = ((MenuSlider)value).Value;
        }

        /// <summary>
        ///     Gets the object data.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("value", this.Value, typeof(int));
        }

        /// <summary>
        ///     Slider Item Draw callback.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <param name="index">
        ///     The index.
        /// </param>
        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            if (!this.Position.Equals(position))
            {
                this.Position = position;
            }

            var animation = ThemeManager.Current.Boolean.Animation;
            if (animation != null && animation.IsAnimating())
            {
                animation.OnDraw(component, position, index);

                return;
            }

            ThemeManager.Current.Slider.OnDraw(component, position, index);
        }

        /// <summary>
        ///     Slider Windows Process Messages callback.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!Container.Visible)
            {
                return;
            }

            if (args.Msg == WindowsMessages.MOUSEMOVE && this.Interacting)
            {
                this.CalculateNewValue(args);
            }
            else if (args.Msg == WindowsMessages.LBUTTONDOWN && !this.Interacting)
            {
                var container = ThemeManager.Current.Slider.Bounding(this.Position, this.Container);

                if (args.Cursor.IsUnderRectangle(container.X, container.Y, container.Width, container.Height))
                {
                    this.Interacting = true;
                    this.CalculateNewValue(args);
                }
            }
            else if (args.Msg == WindowsMessages.LBUTTONUP)
            {
                this.Interacting = false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculate the new value based onto the cursor position.
        /// </summary>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
        private void CalculateNewValue(WindowsKeys args)
        {
            var newValue =
                (int)
                Math.Round(
                    this.MinValue
                    + ((args.Cursor.X - this.Position.X) * (this.MaxValue - this.MinValue)) / this.Container.MenuWidth);
            if (newValue < this.MinValue)
            {
                newValue = this.MinValue;
            }
            else if (newValue > this.MaxValue)
            {
                newValue = this.MaxValue;
            }

            if (newValue != this.Value)
            {
                this.Value = newValue;
                this.Container.FireEvent();
            }
        }

        #endregion
    }
}