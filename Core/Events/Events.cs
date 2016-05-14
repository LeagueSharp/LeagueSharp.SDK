// <copyright file="Events.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx
{
    using System;

    /// <summary>
    ///     The provided events by the kit.
    /// </summary>
    public static partial class Events
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Events" /> class.
        /// </summary>
        static Events()
        {
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnNewPath += OnNewPath;
            Spellbook.OnStopCast += OnStopCast;
            GameObject.OnCreate += OnCreate;
            GameObject.OnIntegerPropertyChange += OnIntegerPropertyChange;
            Obj_AI_Base.OnTeleport += OnTeleportEvent;

            EventTurretConstruct();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     On create event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnCreate(GameObject sender, EventArgs args)
        {
            EventTurret(sender);
        }

        /// <summary>
        ///     On integer property change event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnIntegerPropertyChange(GameObject sender, GameObjectIntegerPropertyChangeEventArgs args)
        {
            EventStealth(sender, args);
        }

        /// <summary>
        ///     On new path event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            EventDash(sender, args);
        }

        /// <summary>
        ///     On process spell cast event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            EventGapcloser(sender, args);
            EventInterruptableSpell(sender, args);
            EventTurret(sender);
        }

        /// <summary>
        ///     On stop cast event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnStopCast(Spellbook sender, SpellbookStopCastEventArgs args)
        {
            EventInterruptableSpell(sender);
        }

        /// <summary>
        ///     On teleport event.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnTeleportEvent(Obj_AI_Base sender, GameObjectTeleportEventArgs args)
        {
            EventTeleport(sender, args);
        }

        /// <summary>
        ///     OnUpdate event.
        /// </summary>
        /// <param name="args">
        ///     The event args.
        /// </param>
        private static void OnUpdate(EventArgs args)
        {
            EventLoad();
            EventGapcloser();
            EventInterruptableSpell();
        }

        #endregion
    }
}