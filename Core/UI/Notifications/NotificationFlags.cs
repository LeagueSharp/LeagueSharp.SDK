// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationFlags.cs" company="LeagueSharp">
//   Copyright (C) 2015 LeagueSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Notification runtime flags.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.Notifications
{
    using System;

    /// <summary>
    ///     Notification runtime flags.
    /// </summary>
    [Flags]
    public enum NotificationFlags
    {
        /// <summary>
        ///     Drawing runtime flag.
        /// </summary>
        Draw = 1 << 0, 

        /// <summary>
        ///     Windows Process Messages flag.
        /// </summary>
        Wpm = Draw << 1, 

        /// <summary>
        ///     Update flag.
        /// </summary>
        Update = Wpm << 2, 

        /// <summary>
        ///     Initialization flag.
        /// </summary>
        Initalized = Update << 3, 

        /// <summary>
        ///     Idle flag.
        /// </summary>
        Idle = Initalized << 4, 

        /// <summary>
        ///     Animation flag.
        /// </summary>
        Animation = Idle << 5
    }
}