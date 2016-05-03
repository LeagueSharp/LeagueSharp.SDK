// <copyright file="AnimationEase.cs" company="LeagueSharp">
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
    using System;
    using SharpDX;

    /// <summary>
    /// A implementation of a <see cref="Animation" />
    /// </summary>
    public class AnimationEase : Animation
    {
        #region Fields

        /// <summary>
        /// Start Position of the element which will get moved
        /// </summary>
        private Vector2 startPosition;

        /// <summary>
        /// End Position of the element which will get moved
        /// </summary>
        private Vector2 endPosition;

        /// <summary>
        /// Defines which Ease method will be used to calculate the new element position
        /// </summary>
        private readonly Mode mode;

        #endregion

        #region Enums

        /// <summary>
        /// Contains 41 Modes
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Linear calculation
            /// </summary>
            Linear,

            /// <summary>
            /// BackEaseIn calculation
            /// </summary>
            BackEaseIn,
            /// <summary>
            /// BackEaseOut calculation
            /// </summary>
            BackEaseOut,
            /// <summary>
            /// BackEaseInOut calculation
            /// </summary>
            BackEaseInOut,
            /// <summary>
            /// BackEaseOutIn calculation
            /// </summary>
            BackEaseOutIn,

            /// <summary>
            /// BounceEaseIn calculation
            /// </summary>
            BounceEaseIn,
            /// <summary>
            /// BounceEaseOut calculation
            /// </summary>
            BounceEaseOut,
            /// <summary>
            /// BounceEaseInOut calculation
            /// </summary>
            BounceEaseInOut,
            /// <summary>
            /// BounceEaseOutIn calculation
            /// </summary>
            BounceEaseOutIn,

            /// <summary>
            /// CircEaseIn calculation
            /// </summary>
            CircEaseIn,
            /// <summary>
            /// CircEaseOut calculation
            /// </summary>
            CircEaseOut,
            /// <summary>
            /// CircEaseInOut calculation
            /// </summary>
            CircEaseInOut,
            /// <summary>
            /// CircEaseOutIn calculation
            /// </summary>
            CircEaseOutIn,

            /// <summary>
            /// CubicEaseIn calculation
            /// </summary>
            CubicEaseIn,
            /// <summary>
            /// CubicEaseOut calculation
            /// </summary>
            CubicEaseOut,
            /// <summary>
            /// CubicEaseInOut calculation
            /// </summary>
            CubicEaseInOut,
            /// <summary>
            /// CubicEaseOutIn calculation
            /// </summary>
            CubicEaseOutIn,

            /// <summary>
            /// ElasticEaseIn calculation
            /// </summary>
            ElasticEaseIn,
            /// <summary>
            /// ElasticEaseOut calculation
            /// </summary>
            ElasticEaseOut,
            /// <summary>
            /// ElasticEaseInOut calculation
            /// </summary>
            ElasticEaseInOut,
            /// <summary>
            /// ElasticEaseOutIn calculation
            /// </summary>
            ElasticEaseOutIn,

            /// <summary>
            /// ExpoEaseIn calculation
            /// </summary>
            ExpoEaseIn,
            /// <summary>
            /// ExpoEaseOut calculation
            /// </summary>
            ExpoEaseOut,
            /// <summary>
            /// ExpoEaseInOut calculation
            /// </summary>
            ExpoEaseInOut,
            /// <summary>
            /// ExpoEaseOutIn calculation
            /// </summary>
            ExpoEaseOutIn,

            /// <summary>
            /// QuadEaseIn calculation
            /// </summary>
            QuadEaseIn,
            /// <summary>
            /// QuadEaseOut calculation
            /// </summary>
            QuadEaseOut,
            /// <summary>
            /// QuadEaseInOut calculation
            /// </summary>
            QuadEaseInOut,
            /// <summary>
            /// QuadEaseOutIn calculation
            /// </summary>
            QuadEaseOutIn,

            /// <summary>
            /// QuartEaseIn calculation
            /// </summary>
            QuartEaseIn,
            /// <summary>
            /// QuartEaseOut calculation
            /// </summary>
            QuartEaseOut,
            /// <summary>
            /// QuartEaseInOut calculation
            /// </summary>
            QuartEaseInOut,
            /// <summary>
            /// QuartEaseOutIn calculation
            /// </summary>
            QuartEaseOutIn,

            /// <summary>
            /// QuintEaseIn calculation
            /// </summary>
            QuintEaseIn,
            /// <summary>
            /// QuintEaseOut calculation
            /// </summary>
            QuintEaseOut,
            /// <summary>
            /// QuintEaseInOut calculation
            /// </summary>
            QuintEaseInOut,
            /// <summary>
            /// QuintEaseOutIn calculation
            /// </summary>
            QuintEaseOutIn,

            /// <summary>
            /// SineEaseIn calculation
            /// </summary>
            SineEaseIn,
            /// <summary>
            /// SineEaseOut calculation
            /// </summary>
            SineEaseOut,
            /// <summary>
            /// SineEaseInOut calculation
            /// </summary>
            SineEaseInOut,
            /// <summary>
            /// SineEaseOutIn calculation
            /// </summary>
            SineEaseOutIn,
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationEase" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        public AnimationEase(Mode mode, float duration)
            : base(duration)
        {
            this.mode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationEase" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        /// <param name="defaultPos">Default Position of the element</param>
        public AnimationEase(Mode mode, float duration, Vector2 defaultPos)
            : base(duration)
        {
            this.mode = mode;
            this.endPosition = defaultPos;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the current position of the element
        /// </summary>
        public Vector2 GetCurrentPosition
        {
            get
            {
                if (!this.IsWorking)
                {
                    return this.endPosition;
                }
                return this.startPosition.Extend(
                    this.endPosition,
                    (float)this.Calculate(Game.ClockTime - this.startTime,
                        0, this.endPosition.Distance(this.startPosition), this.duration));
            }
        }

        /// <summary>
        /// Calculates the value of the specified mode
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>Returns the calculated value of the specified mode</returns>
        private double Calculate(double curTime, double startVal, double endVal, double dur)
        {
            switch (this.mode)
            {
                case Mode.Linear:
                    return this.Linear(curTime, startVal, endVal, dur);

                case Mode.BackEaseIn:
                    return this.BackEaseIn(curTime, startVal, endVal, dur);

                case Mode.BackEaseOut:
                    return this.BackEaseOut(curTime, startVal, endVal, dur);

                case Mode.BackEaseInOut:
                    return this.BackEaseInOut(curTime, startVal, endVal, dur);

                case Mode.BackEaseOutIn:
                    return this.BackEaseOutIn(curTime, startVal, endVal, dur);

                case Mode.BounceEaseIn:
                    return this.BounceEaseIn(curTime, startVal, endVal, dur);

                case Mode.BounceEaseOut:
                    return this.BounceEaseOut(curTime, startVal, endVal, dur);

                case Mode.BounceEaseInOut:
                    return this.BounceEaseInOut(curTime, startVal, endVal, dur);

                case Mode.BounceEaseOutIn:
                    return this.BounceEaseOutIn(curTime, startVal, endVal, dur);

                case Mode.CircEaseIn:
                    return this.CircEaseIn(curTime, startVal, endVal, dur);

                case Mode.CircEaseOut:
                    return this.CircEaseOut(curTime, startVal, endVal, dur);

                case Mode.CircEaseInOut:
                    return this.CircEaseInOut(curTime, startVal, endVal, dur);

                case Mode.CircEaseOutIn:
                    return this.CircEaseOutIn(curTime, startVal, endVal, dur);

                case Mode.CubicEaseIn:
                    return this.CubicEaseIn(curTime, startVal, endVal, dur);

                case Mode.CubicEaseOut:
                    return this.CubicEaseOut(curTime, startVal, endVal, dur);

                case Mode.CubicEaseInOut:
                    return this.CubicEaseInOut(curTime, startVal, endVal, dur);

                case Mode.CubicEaseOutIn:
                    return this.CubicEaseOutIn(curTime, startVal, endVal, dur);

                case Mode.ElasticEaseIn:
                    return this.ElasticEaseIn(curTime, startVal, endVal, dur);

                case Mode.ElasticEaseOut:
                    return this.ElasticEaseOut(curTime, startVal, endVal, dur);

                case Mode.ElasticEaseInOut:
                    return this.ElasticEaseInOut(curTime, startVal, endVal, dur);

                case Mode.ElasticEaseOutIn:
                    return this.ElasticEaseOutIn(curTime, startVal, endVal, dur);

                case Mode.ExpoEaseIn:
                    return this.ExpoEaseIn(curTime, startVal, endVal, dur);

                case Mode.ExpoEaseOut:
                    return this.ExpoEaseOut(curTime, startVal, endVal, dur);

                case Mode.ExpoEaseInOut:
                    return this.ExpoEaseInOut(curTime, startVal, endVal, dur);

                case Mode.ExpoEaseOutIn:
                    return this.ExpoEaseOutIn(curTime, startVal, endVal, dur);

                case Mode.QuadEaseIn:
                    return this.QuadEaseIn(curTime, startVal, endVal, dur);

                case Mode.QuadEaseOut:
                    return this.QuadEaseOut(curTime, startVal, endVal, dur);

                case Mode.QuadEaseInOut:
                    return this.QuadEaseInOut(curTime, startVal, endVal, dur);

                case Mode.QuadEaseOutIn:
                    return this.QuadEaseOutIn(curTime, startVal, endVal, dur);

                case Mode.QuartEaseIn:
                    return this.QuartEaseIn(curTime, startVal, endVal, dur);

                case Mode.QuartEaseOut:
                    return this.QuartEaseOut(curTime, startVal, endVal, dur);

                case Mode.QuartEaseInOut:
                    return this.QuartEaseInOut(curTime, startVal, endVal, dur);

                case Mode.QuartEaseOutIn:
                    return this.QuartEaseOutIn(curTime, startVal, endVal, dur);

                case Mode.QuintEaseIn:
                    return this.QuintEaseIn(curTime, startVal, endVal, dur);

                case Mode.QuintEaseOut:
                    return this.QuintEaseOut(curTime, startVal, endVal, dur);

                case Mode.QuintEaseInOut:
                    return this.QuintEaseInOut(curTime, startVal, endVal, dur);

                case Mode.QuintEaseOutIn:
                    return this.QuintEaseOutIn(curTime, startVal, endVal, dur);

                case Mode.SineEaseIn:
                    return this.SineEaseIn(curTime, startVal, endVal, dur);

                case Mode.SineEaseOut:
                    return this.SineEaseOut(curTime, startVal, endVal, dur);

                case Mode.SineEaseInOut:
                    return this.SineEaseInOut(curTime, startVal, endVal, dur);

                case Mode.SineEaseOutIn:
                    return this.SineEaseOutIn(curTime, startVal, endVal, dur);


            }
            return endVal;
        }

        /// <summary>
        /// Starts the animation
        /// After start you can get the current value by calling <see cref="AnimationEase.GetCurrentPosition" /> method
        /// </summary>
        /// <param name="startPos">Starting Position of the element</param>
        /// <param name="endPos">Final Position of the element</param>
        public void Start(Vector2 startPos, Vector2 endPos)
        {
            if (this.IsWorking)
            {
                this.Stop();
            }

            this.startPosition = startPos;
            this.endPosition = endPos;
            this.startTime = Game.ClockTime;
        }

        #endregion

        #region Ease Methods
        //For a preview check http://easings.net/en

        #region Back

        /// <summary>
        /// Easing equation: Back Ease In
        /// Overshooting cubic easing (s+1)*t^3 - s*t^2
        /// Accelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double BackEaseIn(double curTime, double startVal, double endVal, double dur)
        {
            double indicator = 1.70158;
            return endVal * (curTime /= dur) * curTime * ((indicator + 1) * curTime - indicator) + startVal;
        }

        /// <summary>
        /// Easing equation: Back Ease Out
        /// Overshooting cubic easing (s+1)*t^3 - s*t^2
        /// Decelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double BackEaseOut(double curTime, double startVal, double endVal, double dur)
        {
            double indicator = 1.70158;
            return endVal * ((curTime = curTime / dur - 1) * curTime * ((indicator + 1) * curTime + indicator) + 1) + startVal;
        }

        /// <summary>
        /// Easing equation: Back Ease In Out
        /// Overshooting cubic easing (s+1)*t^3 - s*t^2
        /// Accelerating from zero velocity until half then decelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double BackEaseInOut(double curTime, double startVal, double endVal, double dur)
        {
            double indicator1 = 1.70158;
            double indicator2 = 1.525;
            if ((curTime /= dur / 2) < 1)
                return endVal / 2 * (curTime * curTime * (((indicator1 *= (indicator2)) + 1) * curTime - indicator1)) + startVal;
            return endVal / 2 * ((curTime -= 2) * curTime * (((indicator1 *= (indicator2)) + 1) * curTime + indicator1) + 2) + startVal;
        }

        /// <summary>
        /// Easing equation: Back Ease Out In
        /// Overshooting cubic easing (s+1)*t^3 - s*t^2
        /// Decelerating from zero velocity until half then accelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double BackEaseOutIn(double curTime, double startVal, double endVal, double dur)
        {
            if (curTime < dur / 2)
                return this.BackEaseOut(curTime * 2, startVal, endVal / 2, dur);
            return this.BackEaseIn((curTime * 2) - dur, startVal + endVal / 2, endVal / 2, dur);
        }

        #endregion

        #region Bounce

        /// <summary>
        /// Easing equation: Bounce Ease In
        /// Exponentially decaying parabolic bounce
        /// Accelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double BounceEaseIn(double curTime, double startVal, double endVal, double dur)
        {
            return endVal - this.BounceEaseOut(dur - curTime, 0, endVal, dur) + startVal;
        }

        /// <summary>
        /// Easing equation: Bounce Ease Out
        /// Exponentially decaying parabolic bounce
        /// Decelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double BounceEaseOut(double curTime, double startVal, double endVal, double dur)
        {
            if ((curTime /= dur) < (1 / 2.75))
                return endVal * (7.5625 * curTime * curTime) + startVal;
            else if (curTime < (2 / 2.75))
                return endVal * (7.5625 * (curTime -= (1.5 / 2.75)) * curTime + 0.75) + startVal;
            else if (curTime < (2.5 / 2.75))
                return endVal * (7.5625 * (curTime -= (2.25 / 2.75)) * curTime + 0.9375) + startVal;
            else
                return endVal * (7.5625 * (curTime -= (2.625 / 2.75)) * curTime + 0.984375) + startVal;
        }

        /// <summary>
        /// Easing equation: Bounce Ease In Out
        /// Exponentially decaying parabolic bounce
        /// Accelerating from zero velocity until half then decelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double BounceEaseInOut(double curTime, double startVal, double endVal, double dur)
        {
            if (curTime < dur / 2)
                return this.BounceEaseIn(curTime * 2, 0, endVal, dur) * .5 + startVal;
            else
                return this.BounceEaseOut(curTime * 2 - dur, 0, endVal, dur) * .5 + endVal * .5 + startVal;
        }

        /// <summary>
        /// Easing equation: Bounce Ease Out In
        /// Exponentially decaying parabolic bounce
        /// Decelerating from zero velocity until half then accelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double BounceEaseOutIn(double curTime, double startVal, double endVal, double dur)
        {
            if (curTime < dur / 2)
                return this.BounceEaseOut(curTime * 2, startVal, endVal / 2, dur);
            return this.BounceEaseIn((curTime * 2) - dur, startVal + endVal / 2, endVal / 2, dur);
        }

        #endregion

        #region Circular

        /// <summary>
        /// Easing equation: Circular Ease In
        /// Circular (sqrt(1-t^2))
        /// Accelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double CircEaseIn(double curTime, double startVal, double endVal, double dur)
        {
            return -endVal * (Math.Sqrt(1 - (curTime /= dur) * curTime) - 1) + startVal;
        }

        /// <summary>
        /// Easing equation: Circular Ease Out
        /// Circular (sqrt(1-t^2))
        /// Decelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double CircEaseOut(double curTime, double startVal, double endVal, double dur)
        {
            return endVal * Math.Sqrt(1 - (curTime = curTime / dur - 1) * curTime) + startVal;
        }

        /// <summary>
        /// Easing equation: Circular Ease In Out
        /// Circular (sqrt(1-t^2))
        /// Accelerating from zero velocity until half then decelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double CircEaseInOut(double curTime, double startVal, double endVal, double dur)
        {
            if ((curTime /= dur / 2) < 1)
                return -endVal / 2 * (Math.Sqrt(1 - curTime * curTime) - 1) + startVal;

            return endVal / 2 * (Math.Sqrt(1 - (curTime -= 2) * curTime) + 1) + startVal;
        }

        /// <summary>
        /// Easing equation: Circular Ease Out In
        /// Circular (sqrt(1-t^2))
        /// Decelerating from zero velocity until half then accelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double CircEaseOutIn(double curTime, double startVal, double endVal, double dur)
        {
            if (curTime < dur / 2)
                return this.CircEaseOut(curTime * 2, startVal, endVal / 2, dur);

            return this.CircEaseIn((curTime * 2) - dur, startVal + endVal / 2, endVal / 2, dur);
        }

        #endregion

        #region Cubic

        /// <summary>
        /// Easing equation: Cubic Ease In
        /// Cubic (t^3)
        /// Accelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double CubicEaseIn(double curTime, double startVal, double endVal, double dur)
        {
            return endVal * (curTime /= dur) * curTime * curTime + startVal;
        }

        /// <summary>
        /// Easing equation: Cubic Ease Out
        /// Cubic (t^3)
        /// Decelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double CubicEaseOut(double curTime, double startVal, double endVal, double dur)
        {
            return endVal * ((curTime = curTime / dur - 1) * curTime * curTime + 1) + startVal;
        }

        /// <summary>
        /// Easing equation: Cubic Ease In Out
        /// Cubic (t^3)
        /// Accelerating from zero velocity until half then decelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double CubicEaseInOut(double curTime, double startVal, double endVal, double dur)
        {
            if ((curTime /= dur / 2) < 1)
                return endVal / 2 * curTime * curTime * curTime + startVal;

            return endVal / 2 * ((curTime -= 2) * curTime * curTime + 2) + startVal;
        }

        /// <summary>
        /// Easing equation: Cubic Ease Out In
        /// Cubic (t^3)
        /// Decelerating from zero velocity until half then accelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double CubicEaseOutIn(double curTime, double startVal, double endVal, double dur)
        {
            if (curTime < dur / 2)
                return this.CubicEaseOut(curTime * 2, startVal, endVal / 2, dur);

            return this.CubicEaseIn((curTime * 2) - dur, startVal + endVal / 2, endVal / 2, dur);
        }

        #endregion

        #region Elastic

        /// <summary>
        /// Easing equation: Elastic Ease In
        /// Exponentially decaying sine wave
        /// Accelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double ElasticEaseIn(double curTime, double startVal, double endVal, double dur)
        {
            if ((curTime /= dur).Equals(1))
                return startVal + endVal;

            double p = dur * .3;
            double s = p / 4;

            return -(endVal * Math.Pow(2, 10 * (curTime -= 1)) * Math.Sin((curTime * dur - s) * (2 * Math.PI) / p)) + startVal;
        }

        /// <summary>
        /// Easing equation: Elastic Ease Out
        /// Exponentially decaying sine wave
        /// Decelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double ElasticEaseOut(double curTime, double startVal, double endVal, double dur)
        {
            if ((curTime /= dur).Equals(1))
                return startVal + endVal;

            double p = dur * .3;
            double s = p / 4;

            return (endVal * Math.Pow(2, -10 * curTime) * Math.Sin((curTime * dur - s) * (2 * Math.PI) / p) + endVal + startVal);
        }

        /// <summary>
        /// Easing equation: Elastic Ease In Out
        /// Exponentially decaying sine wave
        /// Accelerating from zero velocity until half then decelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double ElasticEaseInOut(double curTime, double startVal, double endVal, double dur)
        {
            if ((curTime /= dur / 2).Equals(2))
                return startVal + endVal;

            double p = dur * (.3 * 1.5);
            double s = p / 4;

            if (curTime < 1)
                return -.5 * (endVal * Math.Pow(2, 10 * (curTime -= 1)) * Math.Sin((curTime * dur - s) * (2 * Math.PI) / p)) + startVal;
            return endVal * Math.Pow(2, -10 * (curTime -= 1)) * Math.Sin((curTime * dur - s) * (2 * Math.PI) / p) * .5 + endVal + startVal;
        }

        /// <summary>
        /// Easing equation: Elastic Ease Out In
        /// Exponentially decaying sine wave
        /// Decelerating from zero velocity until half then accelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double ElasticEaseOutIn(double curTime, double startVal, double endVal, double dur)
        {
            if (curTime < dur / 2)
                return this.ElasticEaseOut(curTime * 2, startVal, endVal / 2, dur);
            return this.ElasticEaseIn((curTime * 2) - dur, startVal + endVal / 2, endVal / 2, dur);
        }

        #endregion

        #region Exponential

        /// <summary>
        /// Easing equation: Exponential Ease In
        /// Exponential (2^t)
        /// Accelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double ExpoEaseIn(double curTime, double startVal, double endVal, double dur)
        {
            return (curTime.Equals(0)) ? startVal : endVal * Math.Pow(2, 10 * (curTime / dur - 1)) + startVal;
        }

        /// <summary>
        /// Easing equation: Exponential Ease Out
        /// Exponential (2^t)
        /// Decelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double ExpoEaseOut(double curTime, double startVal, double endVal, double dur)
        {
            return (curTime.Equals(dur)) ? startVal + endVal : endVal * (-Math.Pow(2, -10 * curTime / dur) + 1) + startVal;
        }

        /// <summary>
        /// Easing equation: Exponential Ease In Out
        /// Exponential (2^t)
        /// Accelerating from zero velocity until half then decelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double ExpoEaseInOut(double curTime, double startVal, double endVal, double dur)
        {
            if (curTime.Equals(0))
                return startVal;

            if (curTime.Equals(dur))
                return startVal + endVal;

            if ((curTime /= dur / 2) < 1)
                return endVal / 2 * Math.Pow(2, 10 * (curTime - 1)) + startVal;

            return endVal / 2 * (-Math.Pow(2, -10 * --curTime) + 2) + startVal;
        }

        /// <summary>
        /// Easing equation: Exponential Ease Out In
        /// Exponential (2^t)
        /// Decelerating from zero velocity until half then accelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double ExpoEaseOutIn(double curTime, double startVal, double endVal, double dur)
        {
            if (curTime < dur / 2)
                return this.ExpoEaseOut(curTime * 2, startVal, endVal / 2, dur);

            return this.ExpoEaseIn((curTime * 2) - dur, startVal + endVal / 2, endVal / 2, dur);
        }

        #endregion

        #region Quadratic

        /// <summary>
        /// Easing equation: Quadratic Ease In
        /// Quadratic (t^2)
        /// Accelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double QuadEaseIn(double curTime, double startVal, double endVal, double dur)
        {
            return endVal * (curTime /= dur) * curTime + startVal;
        }

        /// <summary>
        /// Easing equation: Quadratic Ease Out
        /// Quadratic (t^2)
        /// Decelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double QuadEaseOut(double curTime, double startVal, double endVal, double dur)
        {
            return -endVal * (curTime /= dur) * (curTime - 2) + startVal;
        }

        /// <summary>
        /// Easing equation: Quadratic Ease In Out
        /// Quadratic (t^2)
        /// Accelerating from zero velocity until half then decelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double QuadEaseInOut(double curTime, double startVal, double endVal, double dur)
        {
            if ((curTime /= dur / 2) < 1)
                return endVal / 2 * curTime * curTime + startVal;

            return -endVal / 2 * ((--curTime) * (curTime - 2) - 1) + startVal;
        }

        /// <summary>
        /// Easing equation: Quadratic Ease Out In
        /// Quadratic (t^2)
        /// Decelerating from zero velocity until half then accelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double QuadEaseOutIn(double curTime, double startVal, double endVal, double dur)
        {
            if (curTime < dur / 2)
                return this.QuadEaseOut(curTime * 2, startVal, endVal / 2, dur);

            return this.QuadEaseIn((curTime * 2) - dur, startVal + endVal / 2, endVal / 2, dur);
        }

        #endregion

        #region Quartic

        /// <summary>
        /// Easing equation: Quartic Ease In
        /// Quartic (t^4)
        /// Accelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double QuartEaseIn(double curTime, double startVal, double endVal, double dur)
        {
            return endVal * (curTime /= dur) * curTime * curTime * curTime + startVal;
        }

        /// <summary>
        /// Easing equation: Quartic Ease Out
        /// Quartic (t^4)
        /// Decelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double QuartEaseOut(double curTime, double startVal, double endVal, double dur)
        {
            return -endVal * ((curTime = curTime / dur - 1) * curTime * curTime * curTime - 1) + startVal;
        }

        /// <summary>
        /// Easing equation: Quartic Ease In Out
        /// Quartic (t^4)
        /// Accelerating from zero velocity until half then decelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double QuartEaseInOut(double curTime, double startVal, double endVal, double dur)
        {
            if ((curTime /= dur / 2) < 1)
                return endVal / 2 * curTime * curTime * curTime * curTime + startVal;

            return -endVal / 2 * ((curTime -= 2) * curTime * curTime * curTime - 2) + startVal;
        }

        /// <summary>
        /// Easing equation: Quartic Ease Out In
        /// Quartic (t^4)
        /// Decelerating from zero velocity until half then accelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double QuartEaseOutIn(double curTime, double startVal, double endVal, double dur)
        {
            if (curTime < dur / 2)
                return this.QuartEaseOut(curTime * 2, startVal, endVal / 2, dur);

            return this.QuartEaseIn((curTime * 2) - dur, startVal + endVal / 2, endVal / 2, dur);
        }

        #endregion

        #region Quintic

        /// <summary>
        /// Easing equation: Quintic Ease In
        /// Quintic (t^5)
        /// Accelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double QuintEaseIn(double curTime, double startVal, double endVal, double dur)
        {
            return endVal * (curTime /= dur) * curTime * curTime * curTime * curTime + startVal;
        }

        /// <summary>
        /// Easing equation: Quintic Ease Out
        /// Quintic (t^5)
        /// Decelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double QuintEaseOut(double curTime, double startVal, double endVal, double dur)
        {
            return endVal * ((curTime = curTime / dur - 1) * curTime * curTime * curTime * curTime + 1) + startVal;
        }

        /// <summary>
        /// Easing equation: Quintic Ease In Out
        /// Quintic (t^5)
        /// Accelerating from zero velocity until half then decelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double QuintEaseInOut(double curTime, double startVal, double endVal, double dur)
        {
            if ((curTime /= dur / 2) < 1)
                return endVal / 2 * curTime * curTime * curTime * curTime * curTime + startVal;
            return endVal / 2 * ((curTime -= 2) * curTime * curTime * curTime * curTime + 2) + startVal;
        }

        /// <summary>
        /// Easing equation: Quintic Ease Out In
        /// Quintic (t^5)
        /// Decelerating from zero velocity until half then accelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double QuintEaseOutIn(double curTime, double startVal, double endVal, double dur)
        {
            if (curTime < dur / 2)
                return this.QuintEaseOut(curTime * 2, startVal, endVal / 2, dur);
            return this.QuintEaseIn((curTime * 2) - dur, startVal + endVal / 2, endVal / 2, dur);
        }

        #endregion

        #region Sinusoidal

        /// <summary>
        /// Easing equation: Sinusoidal Ease In
        /// Sinusoidal (sin(t))
        /// Accelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double SineEaseIn(double curTime, double startVal, double endVal, double dur)
        {
            return -endVal * Math.Cos(curTime / dur * (Math.PI / 2)) + endVal + startVal;
        }

        /// <summary>
        /// Easing equation: Sinusoidal Ease Out
        /// Sinusoidal (sin(t))
        /// Decelerating from zero velocity
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double SineEaseOut(double curTime, double startVal, double endVal, double dur)
        {
            return endVal * Math.Sin(curTime / dur * (Math.PI / 2)) + startVal;
        }

        /// <summary>
        /// Easing equation: Sinusoidal Ease In Out
        /// Sinusoidal (sin(t))
        /// Accelerating from zero velocity until half then decelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double SineEaseInOut(double curTime, double startVal, double endVal, double dur)
        {
            if ((curTime /= dur / 2) < 1)
                return endVal / 2 * (Math.Sin(Math.PI * curTime / 2)) + startVal;

            return -endVal / 2 * (Math.Cos(Math.PI * --curTime / 2) - 2) + startVal;
        }

        /// <summary>
        /// Easing equation: Sinusoidal Ease Out In
        /// Sinusoidal (sin(t))
        /// Decelerating from zero velocity until half then accelerating
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        private double SineEaseOutIn(double curTime, double startVal, double endVal, double dur)
        {
            if (curTime < dur / 2)
                return this.SineEaseOut(curTime * 2, startVal, endVal / 2, dur);

            return this.SineEaseIn((curTime * 2) - dur, startVal + endVal / 2, endVal / 2, dur);
        }

        #endregion

        #endregion
    }
}
