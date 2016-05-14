// <copyright file="AnimationScale.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx.Core.UI.Animations
{
    using SharpDX;

    /// <summary>
    /// A implementation of a <see cref="Animation" />
    /// </summary>
    public class AnimationScale : Animation
    {
        #region Fields

        /// <summary>
        /// Start Rectangle of the element which will get scaled
        /// </summary>
        private Rectangle startValue;

        /// <summary>
        /// Final Rectangle of the element which will get scaled
        /// </summary>
        private Rectangle? endValue;

        /// <summary>
        /// Defines which Clip method will be used to calculate the new element rectangle
        /// </summary>
        private readonly Mode mode;

        #endregion

        #region Enums

        /// <summary>
        /// Contains 2 Modes
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Decrease height / width to 0
            /// </summary>
            ScaleDecrease,
            /// <summary>
            /// Increase height / width to max height / width
            /// </summary>
            ScaleIncrease,
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationScale" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        public AnimationScale(Mode mode, float duration)
            : base(duration)
        {
            this.mode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationScale" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        /// <param name="defaultRect">Default Rectangle of the element</param>
        public AnimationScale(Mode mode, float duration, Rectangle defaultRect)
            : base(duration)
        {
            this.mode = mode;
            this.startValue = defaultRect;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the current rectangle of the element
        /// </summary>
        public Rectangle GetCurrentValue()
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
        private Rectangle Calculate(double curTime, Rectangle startVal, double dur)
        {
            switch (this.mode)
            {
                case Mode.ScaleDecrease:
                    this.endValue = this.ScaleDecrease(curTime, startVal, dur);
                    break;

                case Mode.ScaleIncrease:
                    this.endValue = this.ScaleIncrease(curTime, startVal, dur);
                    break;
            }
            return this.endValue ?? this.startValue;
        }

        /// <summary>
        /// Starts the animation
        /// After start you can get the current value in <see cref="AnimationScale.GetCurrentValue" /> method
        /// </summary>
        /// <param name="startVal">Starting Rectangle of the element</param>
        public void Start(Rectangle startVal)
        {
            if (this.IsWorking)
            {
                this.Stop();
            }

            this.startValue = startVal;
            this.startTime = Game.ClockTime;
        }

        #endregion

        #region Scale Methods

        /// <summary>
        /// Decreases the Width / Height until it reaches 0
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle ScaleDecrease(double curTime, Rectangle val, double dur)
        {
            val.X = (int)this.Linear(curTime, 0, (double)val.Width / 2, dur) + 1;
            val.Width = val.Width - (int)this.Linear(curTime, 0, val.Width, dur) - 1;
            val.Y = (int)this.Linear(curTime, 0, (double)val.Height / 2, dur) + 1;
            val.Height = val.Height - (int)this.Linear(curTime, 0, val.Height, dur) - 1;
            return val;
        }

        /// <summary>
        /// Increases the Width / Height from 0 to specified width / height
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle ScaleIncrease(double curTime, Rectangle val, double dur)
        {
            val.X = val.Width / 2 - (int)this.Linear(curTime, 0, (double)val.Width / 2, dur) - 1;
            val.Width = (int)this.Linear(curTime, 0, val.Width, dur) + 1;
            val.Y = val.Height / 2 - (int)this.Linear(curTime, 0, (double)val.Height / 2, dur) - 1;
            val.Height = (int)this.Linear(curTime, 0, val.Height, dur) + 1;
            return val;
        }

        #endregion

    }
}
