// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bootstrap.cs" company="LeagueSharp">
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
//   Bootstrap is an initialization pointer for the AppDomainManager to initialize the library correctly once loaded in
//   game.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core
{
    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.INotifications;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Core.Wrappers;

    /// <summary>
    ///     Bootstrap is an initialization pointer for the AppDomainManager to initialize the library correctly once loaded in
    ///     game.
    /// </summary>
    public class Bootstrap
    {
        #region Public Methods and Operators

        /// <summary>
        ///     External attachment handle for the AppDomainManager
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        public static void Init(string[] args)
        {
            // Initial notification.
            Logging.Write()(LogLevel.Info, "[-- SDK Bootstrap Loading --]");

            // Create L# menu
            Variables.LeagueSharpMenu = new Menu("LeagueSharp", "LeagueSharp", true).Attach();
            Logging.Write()(LogLevel.Info, "[SDK Bootstrap] Created L# Menu.");

            // Load the Orbwalker
            Orbwalker.Initialize(Variables.LeagueSharpMenu);
            Logging.Write()(LogLevel.Info, "[SDK Bootstrap] Orbwalker Initialized.");

            // Load the TargetSelector.
            TargetSelector.Initialize(Variables.LeagueSharpMenu);
            Logging.Write()(LogLevel.Info, "[SDK Bootstrap] TargetSelector Initialized.");

            // Load the Notifications
            Notifications.Initialize(Variables.LeagueSharpMenu);
            Logging.Write()(LogLevel.Info, "[SDK Bootstrap] Notifications Initialized.");

            // Load GameObjects.
            GameObjects.Initialize();
            Logging.Write()(LogLevel.Info, "[SDK Bootstrap] GameObjects loaded.");

            // Load Damages.
            // Damage.Initialize(Game.Version);
            Logging.Write()(LogLevel.Info, "[SDK Bootstrap] Damage Library loaded.");

            // Final notification.
            Logging.Write()(LogLevel.Info, "[-- SDK Bootstrap Loading --]");
        }

        #endregion
    }
}