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
    using System.Threading.Tasks;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.UI;
    using LeagueSharp.SDK.Core.UI.IMenu;
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
            // Load the Damage class async.
            Task.Factory.StartNew(Damage.LoadDamage)
                .ContinueWith(task => Logging.Write()(LogLevel.Info, "Damage loaded!"));

            // Log all of the exceptions
            Logging.LogAllExceptions();

            // Create L# menu
            Variables.LeagueSharpMenu = new Menu("LeagueSharp", "LeagueSharp", true).Attach();

            // Load the orbwalker
            Orbwalker.Initialize(Variables.LeagueSharpMenu);
        }

        #endregion
    }
}