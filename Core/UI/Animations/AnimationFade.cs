// <copyright file="AnimationFade.cs" company="LeagueSharp">
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
    public class AnimationFade : Animation
    {
        #region Fields

        /// <summary>
        /// Start Color of the element which will get faded
        /// </summary>
        private ColorBGRA startValue;

        /// <summary>
        /// Final Color of the element which will get faded
        /// </summary>
        private ColorBGRA? endValue;

        /// <summary>
        /// Defines which Fade method will be used to calculate the new element color
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
            /// FadeIn Transparency 100%
            /// </summary>
            FadeIn,
            /// <summary>
            /// FadeIn Transparency 0%
            /// </summary>
            FadeOut
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationFade" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        public AnimationFade(Mode mode, float duration)
            : base(duration)
        {
            this.mode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationFade" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        /// <param name="defaultCol">Default Color of the element</param>
        public AnimationFade(Mode mode, float duration, ColorBGRA defaultCol)
            : base(duration)
        {
            this.mode = mode;
            this.startValue = defaultCol;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the current color of the element
        /// </summary>
        public ColorBGRA GetCurrentValue()
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
        private ColorBGRA Calculate(double curTime, ColorBGRA startVal, double dur)
        {
            switch (this.mode)
            {
                case Mode.FadeIn:
                    this.endValue = this.FadeIn(curTime, startVal, dur);
                    break;

                case Mode.FadeOut:
                    this.endValue = this.FadeOut(curTime, startVal, dur);
                    break;
            }
            return this.endValue ?? this.startValue;
        }

        /// <summary>
        /// Starts the animation
        /// After start you can get the current value in <see cref="AnimationFade.GetCurrentValue" /> method
        /// </summary>
        /// <param name="startVal">Starting Color of the element</param>
        public void Start(ColorBGRA startVal)
        {
            if (this.IsWorking)
            {
                this.Stop();
            }

            this.startValue = startVal;
            this.startTime = Game.ClockTime;
        }

        #endregion

        #region Fade Methods

        /// <summary>
        /// Changes the transparency of a color to 100%
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Color</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated color</returns>
        private ColorBGRA FadeIn(double curTime, ColorBGRA val, double dur)
        {
            return new ColorBGRA(val.B, val.G, val.R, (byte)this.Linear(curTime, val.A, 255 - val.A, dur));
        }

        /// <summary>
        /// Changes the transparency of a color to 0%
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Color</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated color</returns>
        private ColorBGRA FadeOut(double curTime, ColorBGRA val, double dur)
        {
            return new ColorBGRA(val.B, val.G, val.R, (byte)(this.InverseLinear(curTime, val.A, dur)));
        }

        #endregion

    }
}
