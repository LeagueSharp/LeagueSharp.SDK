// <copyright file="AnimationPulsate.cs" company="LeagueSharp">
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
    public class AnimationPulsate : Animation
    {
        #region Fields

        /// <summary>
        /// Start Color of the element which will get pulsated
        /// </summary>
        private ColorBGRA startValue;

        /// <summary>
        /// Final Color of the element which will get pulsated
        /// </summary>
        private ColorBGRA? endValue;

        /// <summary>
        /// How many times it should pulsate
        /// </summary>
        private readonly int pulsateTimes;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationPulsate" /> class.
        /// </summary>
        /// <param name="pulsateTimes">Pulsationtimes</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        public AnimationPulsate(int pulsateTimes, float duration)
            : base(duration)
        {
            this.pulsateTimes = pulsateTimes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationPulsate" /> class.
        /// </summary>
        /// <param name="pulsateTimes">Pulsationtimes</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        /// <param name="defaultCol">Default Color of the element</param>
        public AnimationPulsate(int pulsateTimes, float duration, ColorBGRA defaultCol)
            : base(duration)
        {
            this.pulsateTimes = pulsateTimes;
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
            return this.Calculate(Game.ClockTime - this.startTime, this.pulsateTimes, this.startValue, this.duration);
        }

        /// <summary>
        /// Calculates the value
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="times">Pulsate times</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>Returns the calculated value</returns>
        private ColorBGRA Calculate(double curTime, int times, ColorBGRA startVal, double dur)
        {
            this.endValue = this.Pulsate(curTime, times, startVal, dur);
            return this.endValue ?? this.startValue;
        }

        /// <summary>
        /// Starts the animation
        /// After start you can get the current value in <see cref="AnimationPulsate.GetCurrentValue" /> method
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

        #region Pulsate Methods

        /// <summary>
        /// Changes the transparency of a color to 100%
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="times">Pulsate times</param>
        /// <param name="val">Color</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated color</returns>
        private ColorBGRA Pulsate(double curTime, int times, ColorBGRA val, double dur)
        {
            return (curTime % (dur / times) < (dur / times) / 2) ? new ColorBGRA(val.B, val.G, val.R, 0) : val;
        }

        #endregion

    }
}
