// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Turret.cs" company="LeagueSharp">
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
//   Contains events involving turrets.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Events
{
    using System;
    using System.Reflection;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Utils;

    /// <summary>
    ///     Contains events involving turrets.
    /// </summary>
    public class Turret
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Turret" /> class.
        ///     Static Constructor
        /// </summary>
        static Turret()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     OnTurretShot Delegate.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">OnTurretShot Arguments Container</param>
        public delegate void OnTurretShotDelegate(object sender, TurretShotArgs e);

        #endregion

        #region Public Events

        /// <summary>
        ///     This event gets called when any unit gets shot by a tower.
        /// </summary>
        public static event OnTurretShotDelegate OnTurretShot;

        #endregion

        #region Methods

        /// <summary>
        ///     Attempts to fire the <see cref="OnTurretShot" /> event.
        /// </summary>
        /// <param name="gameObject">
        ///     The target
        /// </param>
        /// <param name="turret">
        ///     The turret
        /// </param>
        /// <param name="type">
        ///     Turret shot type
        /// </param>
        private static void FireOnTurretShot(GameObject gameObject, Obj_AI_Turret turret, TurretShotType type)
        {
            if (OnTurretShot != null)
            {
                OnTurretShot(MethodBase.GetCurrentMethod().DeclaringType, new TurretShotArgs(gameObject, turret, type));
            }
        }

        /// <summary>
        ///     On Process Spell Cast event catch.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     Process Spell Cast event data
        /// </param>
        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var turret = sender as Obj_AI_Turret;

            if (turret == null)
            {
                return;
            }

            FireOnTurretShot(args.Target, turret, TurretShotType.TurretShot);

            DelayAction.Add(
                (int)(1000 * turret.Distance(args.Target) / args.SData.MissileSpeed), 
                () => FireOnTurretShot(args.Target, turret, TurretShotType.TurretShotHit));
        }

        #endregion
    }

    /// <summary>
    ///     Contains the event arguments for the OnTurretShot event.
    /// </summary>
    public class TurretShotArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TurretShotArgs" /> class.
        /// </summary>
        /// <param name="gameObject">
        ///     The unit which is targeted by the turret
        /// </param>
        /// <param name="turret">
        ///     The turret
        /// </param>
        /// <param name="type">
        ///     Turret shot type
        /// </param>
        internal TurretShotArgs(GameObject gameObject, Obj_AI_Turret turret, TurretShotType type)
        {
            this.Target = gameObject;
            this.Turret = turret;
            this.Type = type;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the target.
        /// </summary>
        public GameObject Target { get; set; }

        /// <summary>
        ///     Gets or sets the turret.
        /// </summary>
        public Obj_AI_Turret Turret { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        public TurretShotType Type { get; set; }

        #endregion
    }
}