using System;
using System.Collections.Generic;

namespace LeagueSharp.SDK.Core.Events
{
    using LeagueSharp.SDK.Core.Enumerations;

    /// <summary>
    ///  Teleport class, contains Teleport even which is triggered on recalls, teleports and shen or twisted fate ultimates.
    /// </summary>
    public class Teleport
    {

        /// <summary>
        /// Teleport eventhandler
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">Teleport arguments</param>
        public delegate void TeleportHandler(Obj_AI_Base sender, TeleportEventArgs args);

        private const int ErrorBuffer = 100; //in ticks

        private static readonly IDictionary<string, ITeleport> TypeByString = new Dictionary<string, ITeleport>
                {
                    {"Recall", new RecallTeleport()},
                    {"Teleport", new TeleportTeleport()},
                    {"Gate", new TwistedFateTeleport()},
                    {"Shen", new ShenTeleport()},
                };
        private static readonly IDictionary<int, TeleportEventArgs> TeleportDataByNetworkId =
                    new Dictionary<int, TeleportEventArgs>();

        /// <summary>
        /// This event is triggered on recalls, teleports and shen or twisted fate ultimates.
        /// </summary>
        public static event TeleportHandler OnTeleport;

        static Teleport()
        {
            Obj_AI_Base.OnTeleport += Obj_AI_Base_OnTeleport;
        }

        private static void FireEvent(Obj_AI_Base sender, TeleportEventArgs args)
        {
            if (OnTeleport != null)
            {
                OnTeleport(sender, args);
            }
        }

        private static void Obj_AI_Base_OnTeleport(Obj_AI_Base sender, GameObjectTeleportEventArgs args)
        {
            var eventArgs = new TeleportEventArgs
            {
                Status = TeleportStatus.Unknown,
                Type = TeleportType.Unknown
            };

            if (sender == null || !sender.IsValid)
            {
                FireEvent(sender, eventArgs);
                return;
            }

            if (!TeleportDataByNetworkId.ContainsKey(sender.NetworkId))
            {
                TeleportDataByNetworkId[sender.NetworkId] = eventArgs;
            }

            if (!string.IsNullOrEmpty(args.RecallType))
            {
                if (TypeByString.ContainsKey(args.RecallType))
                {
                    ITeleport teleportMethod = TypeByString[args.RecallType];

                    eventArgs.Status = TeleportStatus.Start;
                    eventArgs.Duration = teleportMethod.GetDuration(args);
                    eventArgs.Type = teleportMethod.Type;
                    eventArgs.Start = Variables.TickCount;
                    eventArgs.IsTarget = teleportMethod.IsTarget(args);

                    TeleportDataByNetworkId[sender.NetworkId] = eventArgs;
                }
                else
                {
                    Console.WriteLine(@"Teleport type {0} with name {1} is not supported yet. Please report it!", args.RecallType, args.RecallName);
                }
            }
            else
            {
                eventArgs = TeleportDataByNetworkId[sender.NetworkId];
                bool shorter = Variables.TickCount - eventArgs.Start < eventArgs.Duration - ErrorBuffer;
                eventArgs.Status = shorter ? TeleportStatus.Abort : TeleportStatus.Finish;
            }
            FireEvent(sender, eventArgs);
        }
    }

    /// <summary>
    /// Contains data about a teleport.
    /// </summary>
    public class TeleportEventArgs : EventArgs
    {
        /// <summary>
        /// The duration of the teleport
        /// </summary>
        public int Duration { get; internal set; }
        /// <summary>
        /// The status of the teleport
        /// </summary>
        public TeleportStatus Status { get; internal set; }
        /// <summary>
        /// The type of teleport
        /// </summary>
        public TeleportType Type { get; internal set; }
        /// <summary>
        /// The start time of the teleport
        /// </summary>
        public int Start { get; internal set; }
        /// <summary>
        /// If the object is the target of a teleport: eg. turret is target of the summoner Teleport
        /// </summary>
        public bool IsTarget { get; internal set; }
    }

    internal interface ITeleport
    {
        TeleportType Type { get; }
        int GetDuration(GameObjectTeleportEventArgs args);
        bool IsTarget(GameObjectTeleportEventArgs args);
    }

    internal class RecallTeleport : ITeleport
    {
        public TeleportType Type
        {
            get { return TeleportType.Recall; }
        }

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
                    Console.WriteLine(@"Recall {0} is not supported yet. Please report it!", args.RecallName);
                    break;
            }
            return duration;
        }


        public bool IsTarget(GameObjectTeleportEventArgs args)
        {
            return false;
        }
    }

    internal class TeleportTeleport : ITeleport
    {
        public TeleportType Type
        {
            get { return TeleportType.Teleport; }
        }

        public int GetDuration(GameObjectTeleportEventArgs args)
        {
            return 3500;
        }


        public bool IsTarget(GameObjectTeleportEventArgs args)
        {
            return !string.Equals(args.RecallName, "summonerteleport", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    internal class TwistedFateTeleport : ITeleport
    {
        public TeleportType Type
        {
            get { return TeleportType.TwistedFate; }
        }

        public int GetDuration(GameObjectTeleportEventArgs args)
        {
            return 1500;
        }


        public bool IsTarget(GameObjectTeleportEventArgs args)
        {
            return !string.Equals(args.RecallName, "gate", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    internal class ShenTeleport : ITeleport
    {
        public TeleportType Type
        {
            get { return TeleportType.Shen; }
        }

        public int GetDuration(GameObjectTeleportEventArgs args)
        {
            return 3000;
        }


        public bool IsTarget(GameObjectTeleportEventArgs args)
        {
            return !string.Equals(args.RecallName, "ShenStandUnited", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
