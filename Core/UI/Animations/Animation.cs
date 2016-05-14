// <copyright file="Animation.cs" company="LeagueSharp">
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
    /// <summary>
    /// Animation base
    /// </summary>
    public class Animation
    {
        #region Fields

        /// <summary>
        /// Duration which will be used for the specified mode
        /// </summary>
        protected float duration;

        /// <summary>
        /// Start time of a start method
        /// </summary>
        protected float startTime;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation" /> class.
        /// </summary>
        /// <param name="duration">Selected duration for the defined animation</param>
        public Animation (float duration)
        {
            this.duration = duration;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Is caluclating a new value
        /// </summary>
        public bool IsWorking => this.startTime + this.duration > Game.ClockTime;

        #endregion

        #region Methods

        /// <summary>
        /// Instant changes the value to the end value
        /// </summary>
        public void Stop()
        {
            this.startTime = float.MaxValue;
        }

        #endregion

        #region Linear Methods

        /// <summary>
        /// Calculates in a linear manner
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="endVal">Final Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        protected double Linear(double curTime, double startVal, double endVal, double dur)
        {
            return endVal * curTime / dur + startVal;
        }

        /// <summary>
        /// Calculates in a inverse linear manner
        /// </summary>
        /// <param name="curTime">Current Time (seconds)</param>
        /// <param name="startVal">Start Value</param>
        /// <param name="dur">Duration of the animation</param>
        /// <returns>New calculated value</returns>
        protected double InverseLinear(double curTime, double startVal, double dur)
        {
            return startVal - (curTime / dur) * startVal;
        }

        #endregion

    }
}
