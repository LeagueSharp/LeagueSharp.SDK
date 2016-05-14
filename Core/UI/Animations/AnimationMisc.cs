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
    public class AnimationMisc : Animation
    {
        #region Fields

        /// <summary>
        /// Start Rectangle of the element which will get adjusted
        /// </summary>
        private Rectangle startValue;

        /// <summary>
        /// Final Rectangle of the element which will get adjusted
        /// </summary>
        private Rectangle? endValue;

        /// <summary>
        /// Defines which Misc method will be used to calculate the new element rectangle
        /// </summary>
        private readonly Mode mode;

        #endregion

        #region Enums

        /// <summary>
        /// Contains 12 Modes
        /// </summary>
        public enum Mode
        {
            BlindVerticalDecrease,
            BlindVerticalIncrease,
            BlindHorizontalDecrease,
            BlindHorizontalIncrease,

            ClipVerticalDecrease,
            ClipVerticalIncrease,
            ClipHorizontalDecrease,
            ClipHorizontalIncrease,

            DropVerticalDecrease,
            DropVerticalIncrease,
            DropHorizontalDecrease,
            DropHorizontalIncrease,
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationMisc" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        public AnimationMisc(Mode mode, float duration)
            : base(duration)
        {
            this.mode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationMisc" /> class.
        /// </summary>
        /// <param name="mode">Selected mode for calculation</param>
        /// <param name="duration">Selected duration for the defined animation</param>
        /// <param name="defaultRect">Default Rectangle of the element</param>
        public AnimationMisc(Mode mode, float duration, Rectangle defaultRect)
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
                case Mode.BlindVerticalDecrease:
                    this.endValue = this.BlindVerticalDecrease(curTime, startVal, dur);
                    break;

                case Mode.BlindVerticalIncrease:
                    this.endValue = this.BlindVerticalIncrease(curTime, startVal, dur);
                    break;

                case Mode.BlindHorizontalDecrease:
                    this.endValue = this.BlindHorizontalDecrease(curTime, startVal, dur);
                    break;

                case Mode.BlindHorizontalIncrease:
                    this.endValue = this.BlindHorizontalIncrease(curTime, startVal, dur);
                    break;

                case Mode.ClipVerticalDecrease:
                    this.endValue = this.ClipVerticalDecrease(curTime, startVal, dur);
                    break;

                case Mode.ClipVerticalIncrease:
                    this.endValue = this.ClipVerticalIncrease(curTime, startVal, dur);
                    break;

                case Mode.ClipHorizontalDecrease:
                    this.endValue = this.ClipHorizontalDecrease(curTime, startVal, dur);
                    break;

                case Mode.ClipHorizontalIncrease:
                    this.endValue = this.ClipHorizontalIncrease(curTime, startVal, dur);
                    break;

                case Mode.DropVerticalDecrease:
                    this.endValue = this.DropVerticalDecrease(curTime, startVal, dur);
                    break;

                case Mode.DropVerticalIncrease:
                    this.endValue = this.DropVerticalIncrease(curTime, startVal, dur);
                    break;

                case Mode.DropHorizontalDecrease:
                    this.endValue = this.DropHorizontalDecrease(curTime, startVal, dur);
                    break;

                case Mode.DropHorizontalIncrease:
                    this.endValue = this.DropHorizontalIncrease(curTime, startVal, dur);
                    break;
            }
            return this.endValue ?? this.startValue;
        }

        /// <summary>
        /// Starts the animation
        /// After start you can get the current value in <see cref="AnimationFade.GetCurrentValue" /> method
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

        #region Blind Methods

        /// <summary>
        /// Decreases the Width until it reaches 0
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle BlindHorizontalDecrease(double curTime, Rectangle val, double dur)
        {
            val.Width = val.Width - (int)this.Linear(curTime, 0, val.Width, dur) - 1;
            return val;
        }

        /// <summary>
        /// Increases the Width from 0 to specified width
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle BlindHorizontalIncrease(double curTime, Rectangle val, double dur)
        {
            val.Width = (int)this.Linear(curTime, 0, val.Width, dur) + 1;
            return val;
        }

        /// <summary>
        /// Decreases the Height until it reaches 0
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle BlindVerticalDecrease(double curTime, Rectangle val, double dur)
        {
            val.Height = val.Height - (int)this.Linear(curTime, 0, val.Height, dur) - 1;
            return val;
        }

        /// <summary>
        /// Increases the Height from 0 to specified height
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle BlindVerticalIncrease(double curTime, Rectangle val, double dur)
        {
            val.Height = (int)this.Linear(curTime, 0, val.Height, dur) + 1;
            return val;
        }

        #endregion

        #region Clip Methods

        /// <summary>
        /// Decreases the Width until it reaches 0
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle ClipHorizontalDecrease(double curTime, Rectangle val, double dur)
        {
            val.X = (int)this.Linear(curTime, 0, (double)val.Width / 2, dur) + 1;
            val.Width = val.Width - (int)this.Linear(curTime, 0, val.Width, dur) - 1;
            return val;
        }

        /// <summary>
        /// Increases the Width from 0 to specified width
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle ClipHorizontalIncrease(double curTime, Rectangle val, double dur)
        {
            val.X = val.Width / 2 - (int)this.Linear(curTime, 0, (double)val.Width / 2, dur) - 1;
            val.Width = (int)this.Linear(curTime, 0, val.Width, dur) + 1;
            return val;
        }

        /// <summary>
        /// Decreases the Height until it reaches 0
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle ClipVerticalDecrease(double curTime, Rectangle val, double dur)
        {
            val.Y = (int)this.Linear(curTime, 0, (double)val.Height / 2, dur) + 1;
            val.Height = val.Height - (int)this.Linear(curTime, 0, val.Height, dur) - 1;
            return val;
        }

        /// <summary>
        /// Increases the Height from 0 to specified height
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle ClipVerticalIncrease(double curTime, Rectangle val, double dur)
        {
            val.Y = val.Height / 2 - (int)this.Linear(curTime, 0, (double)val.Height / 2, dur) - 1;
            val.Height = (int)this.Linear(curTime, 0, val.Height, dur) + 1;
            return val;
        }

        #endregion

        #region Drop Methods

        /// <summary>
        /// Decreases the Width until it reaches 0
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle DropHorizontalDecrease(double curTime, Rectangle val, double dur)
        {
            val.X = (int)this.Linear(curTime, 0, (double)val.Width / 2, dur) + 1;
            val.Width = val.Width - (int)this.Linear(curTime, 0, val.Width, dur) - 1;
            return val;
        }

        /// <summary>
        /// Increases the Width from 0 to specified width
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle DropHorizontalIncrease(double curTime, Rectangle val, double dur)
        {
            val.X = val.Width / 2 - (int)this.Linear(curTime, 0, (double)val.Width / 2, dur) - 1;
            val.Width = (int)this.Linear(curTime, 0, val.Width, dur) + 1;
            return val;
        }

        /// <summary>
        /// Decreases the Height until it reaches 0
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle DropVerticalDecrease(double curTime, Rectangle val, double dur)
        {
            val.Y = (int)this.Linear(curTime, 0, (double)val.Height / 2, dur) + 1;
            val.Height = val.Height - (int)this.Linear(curTime, 0, val.Height, dur) - 1;
            return val;
        }

        /// <summary>
        /// Increases the Height from 0 to specified height
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="val">Rectangle</param>
        /// <param name="dur">Duration</param>
        /// <returns>New calculated rectangle</returns>
        private Rectangle DropVerticalIncrease(double curTime, Rectangle val, double dur)
        {
            val.Y = val.Height / 2 - (int)this.Linear(curTime, 0, (double)val.Height / 2, dur) - 1;
            val.Height = (int)this.Linear(curTime, 0, val.Height, dur) + 1;
            return val;
        }

        #endregion

    }
}
