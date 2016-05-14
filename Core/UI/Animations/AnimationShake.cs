// <copyright file="AnimationShake.cs" company="LeagueSharp">
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
    using System;

    using SharpDX;

    /// <summary>
    /// A implementation of a <see cref="Animation" />
    /// </summary>
    public class AnimationShake : Animation
    {
        #region Fields

        /// <summary>
        /// Start Rectangle of the element which will get shaked
        /// </summary>
        private Rectangle startValue;

        /// <summary>
        /// Final Rectangle of the element which will get shaked
        /// </summary>
        private Rectangle? endValue;

        /// <summary>
        /// Defines which Shake method will be used to calculate the new element rectangle
        /// </summary>
        private readonly Mode mode;

        /// <summary>
        /// Defines the distance from the start initial point to shake
        /// </summary>
        private readonly double distance;

        /// <summary>
        /// How many times the element should get shaked
        /// </summary>
        private readonly int shakeTimes;

        #endregion

        #region Enums

        /// <summary>
        /// Contains 2 Modes
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Horizontal shake
            /// </summary>
            Horizontal,
            /// <summary>
            /// Vertical shake
            /// </summary>
            Vertical
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationShake" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="distance">Shaking distance from main point</param>
        /// <param name="shakeTimes">Shake times until it stops</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        public AnimationShake(Mode mode, double distance, int shakeTimes, float duration)
            : base(duration)
        {
            this.mode = mode;
            this.distance = distance;
            this.shakeTimes = shakeTimes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationShake" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="distance">Shaking distance from main point</param>
        /// <param name="shakeTimes">Shake times until it stops</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        /// <param name="defaultRect">Default Rectangle of the element</param>
        public AnimationShake(Mode mode, double distance, int shakeTimes, float duration, Rectangle defaultRect)
            : base(duration)
        {
            this.mode = mode;
            this.distance = distance;
            this.shakeTimes = shakeTimes;
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
            return this.Calculate(Game.ClockTime - this.startTime, this.startValue, this.distance, this.shakeTimes, this.duration);
        }

        /// <summary>
        /// Calculates the value of the specified mode
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="dist">Distance</param>
        /// <param name="times">Shake Times</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>Returns the calculated value of the specified mode</returns>
        private Rectangle Calculate(double curTime, Rectangle startVal, double dist, int times, double dur)
        {
            switch (this.mode)
            {
                case Mode.Vertical:
                    this.endValue = this.Vertical(curTime, startVal, dist, times, dur);
                    break;

                case Mode.Horizontal:
                    this.endValue = this.Horizontal(curTime, startVal, dist, times, dur);
                    break;
            }
            return this.endValue ?? this.startValue;
        }

        /// <summary>
        /// Starts the animation
        /// After start you can get the current value in <see cref="AnimationShake.GetCurrentValue" /> method
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

        #region Shake Methods

        /// <summary>
        /// Shakes in vertical position
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Color</param>
        /// <param name="dist">Distance</param>
        /// <param name="times">Shake Times</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated color</returns>
        private Rectangle Vertical(double curTime, Rectangle val, double dist, int times, double dur)
        {
            Rectangle retRec = val;
            double b = this.Linear(curTime % (dur / times / 2), 0, dist, (dur / times) / 2);

            if (curTime / ((dur / times) / 2) % 2 <= 1)
            {
                if (curTime / ((dur / times) / 2) < 1)
                {
                    retRec.Y += (int)b;
                }
                else
                {
                    retRec.Y -= (int)dist;
                    retRec.Y += (int)b * 2;
                }
            }
            else
            {
                if (curTime / ((dur / times) / 2) > times * 2 - 1)
                {
                    retRec.Y += (int)dist;
                    retRec.Y -= (int)b;
                }
                else
                {
                    retRec.Y += (int)dist;
                    retRec.Y -= (int)b * 2;
                }
            }

            return retRec;
        }

        /// <summary>
        /// Shakes in horizontal position
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Color</param>
        /// <param name="dist">Distance</param>
        /// <param name="times">Shake Times</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated color</returns>
        private Rectangle Horizontal(double curTime, Rectangle val, double dist, int times, double dur)
        {
            Rectangle retRec = val;
            double b = this.Linear(curTime % (dur / times / 2), 0, dist, (dur / times) / 2);

            if (curTime / ((dur / times) / 2) % 2 <= 1)
            {
                if (curTime / ((dur / times) / 2) < 1)
                {
                    retRec.X += (int)b;
                }
                else
                {
                    retRec.X -= (int)dist;
                    retRec.X += (int)b * 2;
                }
            }
            else
            {
                if (curTime / ((dur / times) / 2) > times * 2 - 1)
                {
                    retRec.X += (int)dist;
                    retRec.X -= (int)b;
                }
                else
                {
                    retRec.X += (int)dist;
                    retRec.X -= (int)b * 2;
                }
            }

            return retRec;
        }

        #endregion

    }
}
