// <copyright file="AnimationResize.cs" company="LeagueSharp">
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
    public class AnimationResize : Animation
    {
        #region Fields

        /// <summary>
        /// Start Rectangle of the element which will get resized
        /// </summary>
        private Rectangle startValue;

        /// <summary>
        /// Final Rectangle of the element which will get resized
        /// </summary>
        private Rectangle endValue;

        /// <summary>
        /// Defines which Resize method will be used to calculate the new element rectangle
        /// </summary>
        private readonly Mode mode;

        #endregion

        #region Enums

        /// <summary>
        /// Contains 1 Modes
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Resizes Width and Height
            /// </summary>
            Resize,
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationResize" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        public AnimationResize(Mode mode, float duration)
            : base(duration)
        {
            this.mode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationResize" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        /// <param name="defaultRect">Default Rectangle of the element</param>
        public AnimationResize(Mode mode, float duration, Rectangle defaultRect)
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
                return this.endValue;
            }
            return this.Calculate(Game.ClockTime - this.startTime, this.startValue, this.endValue, this.duration);
        }

        /// <summary>
        /// Calculates the value of the specified mode
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>Returns the calculated value of the specified mode</returns>
        private Rectangle Calculate(double curTime, Rectangle startVal, Rectangle endVal, double dur)
        {
            switch (this.mode)
            {
                case Mode.Resize:
                    return this.Resize(curTime, startVal, endVal, dur);

            }
            return this.endValue;
        }

        /// <summary>
        /// Starts the animation
        /// After start you can get the current value in <see cref="AnimationResize.GetCurrentValue" /> method
        /// </summary>
        /// <param name="startVal">Starting Rectangle of the element</param>
        /// <param name="endVal">Final Rectangle of the element</param>
        public void Start(Rectangle startVal, Rectangle endVal)
        {
            if (this.IsWorking)
            {
                this.Stop();
            }

            this.startValue = startVal;
            this.endValue = endVal;
            this.startTime = Game.ClockTime;
        }

        #endregion

        #region Resize Methods

        /// <summary>
        /// Decreases the Width / Height until it reaches 0
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Starting Rectangle</param>
        /// <param name="endVal">Final Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle Resize(double curTime, Rectangle startVal, Rectangle endVal, double dur)
        {
            startVal.Width = (int)this.Linear(curTime, startVal.Width, endVal.Width - startVal.Width, dur) + 1;
            startVal.Height = (int)this.Linear(curTime, startVal.Height, endVal.Height - startVal.Height, dur) + 1;
            return startVal;
        }

        #endregion

    }
}
