// <copyright file="Teleport.cs" company="LeagueSharp">
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
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using LeagueSharp.SDK.Enumerations;

    /// <summary>
    ///     Teleport class, contains Teleport even which is triggered on recalls, teleports and shen or twisted fate
    ///     ultimates.
    /// </summary>
    public static partial class Events
    {
        #region Constants

        /// <summary>
        ///     The error buffer.
        /// </summary>
        private const int ErrorBuffer = 100; // in ticks

        #endregion

        #region Static Fields

        /// <summary>
        ///     The teleport data by network id dictionary.
        /// </summary>
        private static readonly IDictionary<int, TeleportEventArgs> TeleportDataByNetworkId =
            new Dictionary<int, TeleportEventArgs>();

        /// <summary>
        ///     The type by string dictionary.
        /// </summary>
        private static readonly IDictionary<string, ITeleport> TypeByString = new Dictionary<string, ITeleport>
                                                                                  {
                                                                                      { "Recall", new RecallTeleport() },
                                                                                      {
                                                                                          "Teleport", new TeleportTeleport()
                                                                                      },
                                                                                      {
                                                                                          "Gate", new TwistedFateTeleport()
                                                                                      },
                                                                                      { "Shen", new ShenTeleport() },
                                                                                  };

        #endregion

        #region Public Events

        /// <summary>
        ///     This event is triggered on recalls, teleports and <c>shen</c> or twisted fate <c>ultimates</c>.
        /// </summary>
        public static event EventHandler<TeleportEventArgs> OnTeleport;

        #endregion

        #region Methods

        /// <summary>
        ///     OnTeleport event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void EventTeleport(Obj_AI_Base sender, GameObjectTeleportEventArgs args)
        {
            var eventArgs = new TeleportEventArgs { Status = TeleportStatus.Unknown, Type = TeleportType.Unknown };

            if (sender == null || !sender.IsValid)
            {
                FireEvent(eventArgs);
                return;
            }

            if (!TeleportDataByNetworkId.ContainsKey(sender.NetworkId))
            {
                TeleportDataByNetworkId[sender.NetworkId] = eventArgs;
            }

            if (!string.IsNullOrEmpty(args.RecallType))
            {
                ITeleport value;
                if (TypeByString.TryGetValue(args.RecallType, out value))
                {
                    var teleportMethod = value;

                    eventArgs.Status = TeleportStatus.Start;
                    eventArgs.Duration = teleportMethod.GetDuration(args);
                    eventArgs.Type = teleportMethod.Type;
                    eventArgs.Start = Variables.TickCount;
                    eventArgs.IsTarget = teleportMethod.IsTarget(args);
                    eventArgs.Object = sender;

                    TeleportDataByNetworkId[sender.NetworkId] = eventArgs;
                }
                else
                {
                    Console.WriteLine(
                        @"Teleport type {0} with name {1} is not supported yet. Please report it!",
                        args.RecallType,
                        args.RecallName);
                }
            }
            else
            {
                eventArgs = TeleportDataByNetworkId[sender.NetworkId];
                var shorter = Variables.TickCount - eventArgs.Start < eventArgs.Duration - ErrorBuffer;
                eventArgs.Status = shorter ? TeleportStatus.Abort : TeleportStatus.Finish;
            }

            FireEvent(eventArgs);
        }

        /// <summary>
        ///     Fires the event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        private static void FireEvent(TeleportEventArgs args)
        {
            OnTeleport?.Invoke(MethodBase.GetCurrentMethod().DeclaringType, args);
        }

        #endregion
    }

    /// <summary>
    ///     Contains data about a teleport.
    /// </summary>
    public class TeleportEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        ///     Gets the duration of the teleport
        /// </summary>
        public int Duration { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether the object/sender is the target of a teleport: <c>eg</c>. turret is target of the
        ///     <c>summoner</c> Teleport, or <see cref="Obj_AI_Hero" /> is target of a <c>shen</c> ultimate.
        /// </summary>
        public bool IsTarget { get; internal set; }

        /// <summary>
        ///     Gets the <see cref="Obj_AI_Hero" /> who is teleporting or is the target of a teleport.
        /// </summary>
        public Obj_AI_Base Object { get; internal set; }

        /// <summary>
        ///     Gets the start time of the teleport
        /// </summary>
        public int Start { get; internal set; }

        /// <summary>
        ///     Gets the status of the teleport
        /// </summary>
        public TeleportStatus Status { get; internal set; }

        /// <summary>
        ///     Gets the type of teleport
        /// </summary>
        public TeleportType Type { get; internal set; }

        #endregion
    }

    /// <summary>
    ///     The Teleport interface.
    /// </summary>
    internal interface ITeleport
    {
        #region Public Properties

        /// <summary>
        ///     Gets the type.
        /// </summary>
        TeleportType Type { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get duration
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        /// <returns>
        ///     Duration of the teleport.
        /// </returns>
        int GetDuration(GameObjectTeleportEventArgs args);

        /// <summary>
        ///     Is Target
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        /// <returns>
        ///     returns where the teleport is target.
        /// </returns>
        bool IsTarget(GameObjectTeleportEventArgs args);

        #endregion
    }

    /// <summary>
    ///     The recall teleport.
    /// </summary>
    internal class RecallTeleport : ITeleport
    {
        #region Public Properties

        /// <summary>
        ///     Gets the type.
        /// </summary>
        public TeleportType Type => TeleportType.Recall;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get duration
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        /// <returns>
        ///     Duration of the teleport.
        /// </returns>
        public int GetDuration(GameObjectTeleportEventArgs args)
        {
            var duration = 0;

            switch (args.RecallName.ToLower())
            {
                case "recall":
                    duration = 8000;
                    break;
                case "recallimproved":
                    duration = 7000;
                    break;
                case "odinrecall":
                    duration = 4500;
                    break;
                case "odinrecallimproved":
                    duration = 4000;
                    break;
                case "superrecall":
                    duration = 4000;
                    break;
                case "superrecallimproved":
                    duration = 4000;
                    break;
                default:
                    Console.WriteLine($"Recall {args.RecallName} is not supported yet. Please report it!");
                    break;
            }

            return duration;
        }

        /// <summary>
        ///     Is Target
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        /// <returns>
        ///     returns where the teleport is target.
        /// </returns>
        public bool IsTarget(GameObjectTeleportEventArgs args)
        {
            return false;
        }

        #endregion
    }

    /// <summary>
    ///     The teleport teleport.
    /// </summary>
    internal class TeleportTeleport : ITeleport
    {
        #region Public Properties

        /// <summary>
        ///     Gets the type.
        /// </summary>
        public TeleportType Type => TeleportType.Teleport;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get duration
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        /// <returns>
        ///     Duration of the teleport.
        /// </returns>
        public int GetDuration(GameObjectTeleportEventArgs args)
        {
            return 3500;
        }

        /// <summary>
        ///     Is Target
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        /// <returns>
        ///     returns where the teleport is target.
        /// </returns>
        public bool IsTarget(GameObjectTeleportEventArgs args)
        {
            return !string.Equals(args.RecallName, "summonerteleport", StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }

    /// <summary>
    ///     The twisted fate teleport.
    /// </summary>
    internal class TwistedFateTeleport : ITeleport
    {
        #region Public Properties

        /// <summary>
        ///     Gets the type.
        /// </summary>
        public TeleportType Type => TeleportType.TwistedFate;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get duration
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        /// <returns>
        ///     Duration of the teleport.
        /// </returns>
        public int GetDuration(GameObjectTeleportEventArgs args)
        {
            return 1500;
        }

        /// <summary>
        ///     Is Target
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        /// <returns>
        ///     returns where the teleport is target.
        /// </returns>
        public bool IsTarget(GameObjectTeleportEventArgs args)
        {
            return !string.Equals(args.RecallName, "gate", StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }

    /// <summary>
    ///     The <c>shen</c> teleport.
    /// </summary>
    internal class ShenTeleport : ITeleport
    {
        #region Public Properties

        /// <summary>
        ///     Gets the type.
        /// </summary>
        public TeleportType Type => TeleportType.Shen;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Get duration
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        /// <returns>
        ///     Duration of the teleport.
        /// </returns>
        public int GetDuration(GameObjectTeleportEventArgs args)
        {
            return 3000;
        }

        /// <summary>
        ///     Is Target
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        /// <returns>
        ///     returns where the teleport is target.
        /// </returns>
        public bool IsTarget(GameObjectTeleportEventArgs args)
        {
            return !string.Equals(args.RecallName, "ShenStandUnited", StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}