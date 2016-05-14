// <copyright file="AnimationBlind.cs" company="LeagueSharp">
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
    public class AnimationSlide : Animation
    {
        #region Fields

        /// <summary>
        /// Start Rectangle of the element which will get slided
        /// </summary>
        private Vector2 startValue;

        /// <summary>
        /// Final Rectangle of the element which will get slided
        /// </summary>
        private Vector2? endValue;

        /// <summary>
        /// Defines which Slide method will be used to calculate the new element position
        /// </summary>
        private readonly Mode mode;

        /// <summary>
        /// Distance for the element which get moved
        /// </summary>
        private readonly double distance;

        #endregion

        #region Enums

        /// <summary>
        /// Contains 4 Modes
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Adjust to left
            /// </summary>
            Left,
            /// <summary>
            /// Adjust to top
            /// </summary>
            Top,
            /// <summary>
            /// Adjust to right
            /// </summary>
            Right,
            /// <summary>
            /// Adjust to bottom
            /// </summary>
            Bottom
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationSlide" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="distance">Distance for the defined animation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        public AnimationSlide(Mode mode, double distance, float duration)
            : base(duration)
        {
            this.mode = mode;
            this.distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationSlide" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="distance">Distance for the defined animation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        /// <param name="defaultPos">Default Position of the element</param>
        public AnimationSlide(Mode mode, double distance, float duration, Vector2 defaultPos)
            : base(duration)
        {
            this.mode = mode;
            this.distance = distance;
            this.startValue = defaultPos;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the current rectangle of the element
        /// </summary>
        public Vector2 GetCurrentValue()
        {
            if (!this.IsWorking)
            {
                return this.endValue ?? this.startValue;
            }
            return this.Calculate(Game.ClockTime - this.startTime, this.startValue, this.distance, this.duration);
        }

        /// <summary>
        /// Calculates the value of the specified mode
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="dist">Distance</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>Returns the calculated value of the specified mode</returns>
        private Vector2 Calculate(double curTime, Vector2 startVal, double dist, double dur)
        {
            switch (this.mode)
            {
                case Mode.Left:
                    this.endValue = this.Left(curTime, startVal, dist, dur);
                    break;

                case Mode.Top:
                    this.endValue = this.Top(curTime, startVal, dist, dur);
                    break;

                case Mode.Right:
                    this.endValue = this.Right(curTime, startVal, dist, dur);
                    break;

                case Mode.Bottom:
                    this.endValue = this.Bottom(curTime, startVal, dist, dur);
                    break;
            }
            return this.endValue ?? this.startValue;
        }

        /// <summary>
        /// Starts the animation
        /// After start you can get the current value in <see cref="AnimationSlide.GetCurrentValue" /> method
        /// </summary>
        /// <param name="startVal">Starting Position of the element</param>
        public void Start(Vector2 startVal)
        {
            if (this.IsWorking)
            {
                this.Stop();
            }

            this.startValue = startVal;
            this.startTime = Game.ClockTime;
        }

        #endregion

        #region Slide Methods

        /// <summary>
        /// Moves element to left
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Position</param>
        /// <param name="dist">Distance</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Vector2 Left(double curTime, Vector2 val, double dist, double dur)
        {
            val.X -= (int)this.Linear(curTime, 0, dist, dur);
            return val;
        }

        /// <summary>
        /// Moves element to top
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Position</param>
        /// <param name="dist">Distance</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Vector2 Top(double curTime, Vector2 val, double dist, double dur)
        {
            val.Y -= (int)this.Linear(curTime, 0, dist, dur);
            return val;
        }

        /// <summary>
        /// Moves element to right
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Position</param>
        /// <param name="dist">Distance</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Vector2 Right(double curTime, Vector2 val, double dist, double dur)
        {
            val.X += (int)this.Linear(curTime, 0, dist, dur);
            return val;
        }

        /// <summary>
        /// Moves element to bottom
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Position</param>
        /// <param name="dist">Distance</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Vector2 Bottom(double curTime, Vector2 val, double dist, double dur)
        {
            val.Y += (int)this.Linear(curTime, 0, dist, dur);
            return val;
        }

        #endregion

    }
}
