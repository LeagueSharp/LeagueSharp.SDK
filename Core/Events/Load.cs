// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Load.cs" company="LeagueSharp">
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
//   Provides an event for when the game starts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Events
{
    using System;
    using System.Reflection;

    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Core.Enumerations;


    /// <summary>
    ///     Provides an event for when the game starts.
    /// </summary>
    public class Load
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Load" /> class.
        /// </summary>
        static Load()
        {
            if (Game.Mode == GameMode.Running)
            {
                DelayAction.Add(0, () =>
                {
                    try
                    {
                        Game_OnStart(new EventArgs());
                    }
                    catch(Exception e)
                    {
                        Logging.Write()(LogLevel.Error, e.Message);
                    }
                });
            }
            else
            {
                Game.OnStart += Game_OnStart;
            }
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     OnLoad Delegate.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e"><see cref="EventArgs" /> event data</param>
        public delegate void OnLoadDelegate(object sender, EventArgs e);

        #endregion

        #region Public Events

        /// <summary>
        ///     OnLoad is getting called when you get in-game (doesn't matter if started or restarted while game is already
        ///     running) and when reloading an assembly.
        /// </summary>
        public static event OnLoadDelegate OnLoad;

        #endregion

        #region Methods

        /// <summary>
        ///     Internal event that is called when the game starts or is already running (when you get in-game).
        /// </summary>
        /// <param name="args"><see cref="System.EventArgs" /> event data</param>
        private static void Game_OnStart(EventArgs args)
        {
            if (OnLoad != null)
            {
                OnLoad(MethodBase.GetCurrentMethod().DeclaringType, new EventArgs());
            }
        }

        #endregion
    }
}