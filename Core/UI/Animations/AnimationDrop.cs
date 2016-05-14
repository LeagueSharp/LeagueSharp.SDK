// <copyright file="AnimationDrop.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Core.UI.Animations
{
    using SharpDX;

    /// <summary>
    /// A implementation of a <see cref="Animation" />
    /// </summary>
    public class AnimationDrop : Animation
    {
        #region Fields

        /// <summary>
        /// Start AnimationDropData of the element which will get dropped
        /// </summary>
        private AnimationDropData startValue;

        /// <summary>
        /// Final AnimationDropData of the element which will get dropped
        /// </summary>
        private AnimationDropData endValue;

        /// <summary>
        /// Defines which Drop method will be used to calculate the new element AnimationDropData
        /// </summary>
        private readonly Mode mode;

        #endregion

        #region Enums

        /// <summary>
        /// Contains 4 Modes
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Vertically decrease height to 0
            /// </summary>
            VerticalDecrease,
            /// <summary>
            /// Vertically increase height to max height
            /// </summary>
            VerticalIncrease,
            /// <summary>
            /// Horizontally decrease width to 0
            /// </summary>
            HorizontalDecrease,
            /// <summary>
            /// Horizontally increase width to max width
            /// </summary>
            HorizontalIncrease
        }

        #endregion

        #region Classes

        /// <summary>
        /// Data class for <see cref="AnimationDrop" /> class to save <see cref="SharpDX.Rectangle" /> and <see cref="SharpDX.ColorBGRA" />
        /// </summary>
        public class AnimationDropData
        {
            #region Fields

            /// <summary>
            /// Used to save rectangle data
            /// </summary>
            public Rectangle Rectangle;

            /// <summary>
            /// Used to save color data
            /// </summary>
            public ColorBGRA Color { get; set; }

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="AnimationDropData" /> class.
            /// </summary>
            public AnimationDropData(Rectangle rectangle, ColorBGRA color)
            {
                this.Rectangle = rectangle;
                this.Color = color;
            }

            #endregion
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationDrop" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        public AnimationDrop(Mode mode, float duration)
            : base(duration)
        {
            this.mode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationDrop" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        /// <param name="defaultAnimDropData">Default AnimationDropData of the element</param>
        public AnimationDrop(Mode mode, float duration, AnimationDropData defaultAnimDropData)
            : base(duration)
        {
            this.mode = mode;
            this.startValue = defaultAnimDropData;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the current AnimationDropData of the element
        /// </summary>
        public AnimationDropData GetCurrentValue()
        {
            if (!this.IsWorking)
            {
                return this.endValue ?? this.startValue;
            }
            return this.Calculate(Game.ClockTime - this.startTime, this.startValue, this.duration);
        }

        /// <summary>
        /// Calculates the value of the specified mode
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>Returns the calculated value of the specified mode</returns>
        private AnimationDropData Calculate(double curTime, AnimationDropData startVal, double dur)
        {
            switch (this.mode)
            {
                case Mode.VerticalDecrease:
                    this.endValue = this.VerticalDecrease(curTime, startVal, dur);
                    break;

                case Mode.VerticalIncrease:
                    this.endValue = this.VerticalIncrease(curTime, startVal, dur);
                    break;

                case Mode.HorizontalDecrease:
                    this.endValue = this.HorizontalDecrease(curTime, startVal, dur);
                    break;

                case Mode.HorizontalIncrease:
                    this.endValue = this.HorizontalIncrease(curTime, startVal, dur);
                    break;
            }
            return this.endValue ?? this.startValue;
        }

        /// <summary>
        /// Starts the animation
        /// After start you can get the current value in <see cref="AnimationDrop.GetCurrentValue" /> method
        /// </summary>
        /// <param name="startVal">Starting AnimationDropData of the element</param>
        public void Start(AnimationDropData startVal)
        {
            if (this.IsWorking)
            {
                this.Stop();
            }

            this.startValue = startVal;
            this.startTime = Game.ClockTime;
        }

        #endregion

        #region Drop Methods

        /// <summary>
        /// Decreases the Width until it reaches 0
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">AnimationDropData</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated AnimationDropData</returns>
        private AnimationDropData HorizontalDecrease(double curTime, AnimationDropData val, double dur)
        {
            Rectangle rec = val.Rectangle;
            Color col = val.Color;
            rec.Width = val.Rectangle.Width - (int)this.Linear(curTime, 0, val.Rectangle.Width, dur) - 1;
            col.A = (byte)(this.InverseLinear(curTime, val.Color.A, dur));
            AnimationDropData data = new AnimationDropData(rec, col);
            return data;
        }

        /// <summary>
        /// Increases the Width from 0 to specified width
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">AnimationDropData</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated AnimationDropData</returns>
        private AnimationDropData HorizontalIncrease(double curTime, AnimationDropData val, double dur)
        {
            Rectangle rec = val.Rectangle;
            Color col = val.Color;
            rec.Width = (int)this.Linear(curTime, 0, val.Rectangle.Width, dur) + 1;
            col = new ColorBGRA(val.Color.B, val.Color.G, val.Color.R, (byte)this.Linear(curTime, val.Color.A, 255 - val.Color.A, dur));
            AnimationDropData data = new AnimationDropData(rec, col);
            return data;
        }

        /// <summary>
        /// Decreases the Height until it reaches 0
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">AnimationDropData</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated AnimationDropData</returns>
        private AnimationDropData VerticalDecrease(double curTime, AnimationDropData val, double dur)
        {
            Rectangle rec = val.Rectangle;
            Color col = val.Color;
            rec.Height = val.Rectangle.Height - (int)this.Linear(curTime, 0, val.Rectangle.Height, dur) - 1;
            col.A = (byte)(this.InverseLinear(curTime, val.Color.A, dur));
            AnimationDropData data = new AnimationDropData(rec, col);
            return data;
        }

        /// <summary>
        /// Increases the Height from 0 to specified height
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">AnimationDropData</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated AnimationDropData</returns>
        private AnimationDropData VerticalIncrease(double curTime, AnimationDropData val, double dur)
        {
            Rectangle rec = val.Rectangle;
            Color col = val.Color;
            rec.Height = (int)this.Linear(curTime, 0, val.Rectangle.Height, dur) + 1;
            col = new ColorBGRA(val.Color.B, val.Color.G, val.Color.R, (byte)this.Linear(curTime, val.Color.A, 255 - val.Color.A, dur));
            AnimationDropData data = new AnimationDropData(rec, col);
            return data;
        }

        #endregion

    }
}
