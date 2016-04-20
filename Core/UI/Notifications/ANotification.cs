// <copyright file="ANotification.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK
{
    using LeagueSharp.SDK.Utils;

    using SharpDX;

    /// <summary>
    ///     Abstract notification class, used as base template.
    /// </summary>
    public abstract class ANotification
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Get the notification reserved height.
        /// </summary>
        /// <returns>
        ///     The reserved height in float units.
        /// </returns>
        public abstract float GetReservedHeight();

        /// <summary>
        ///     Get the notification reserved width.
        /// </summary>
        /// <returns>The reserved width in float units.</returns>
        public abstract float GetReservedWidth();

        /// <summary>
        ///     OnDraw event, specifies a drawing callback which is after IDirect3DDevice9::BeginScene and before
        ///     IDirect3DDevice9::EndScene.
        /// </summary>
        /// <param name="basePosition">
        ///     The base position
        /// </param>
        public abstract void OnDraw(Vector2 basePosition);

        /// <summary>
        ///     OnUpdate event, occurring after a game tick.
        /// </summary>
        public abstract void OnUpdate();

        /// <summary>
        ///     <c>OnWndProc</c> event, occurs on a windows process message to the thread.
        /// </summary>
        /// <param name="basePosition">
        ///     The base position
        /// </param>
        /// <param name="windowsKeys">
        ///     The windows keys
        /// </param>
        /// <param name="isEdit">
        ///     Indicates whether it's an edit message.
        /// </param>
        public abstract void OnWndProc(Vector2 basePosition, WindowsKeys windowsKeys, bool isEdit);

        #endregion
    }
}