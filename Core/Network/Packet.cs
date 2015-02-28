namespace LeagueSharp.CommonEx.Core.Network
{
    /// <summary>
    ///     List of packet types, with their headers.
    /// </summary>
    public class Packet
    {
        /// <summary>
        ///     C2S Packets
        /// </summary>
        public enum C2S
        {
            /// <summary>
            ///     Sent on buying an item
            /// </summary>
            BuyItem = 0xD7,

            /// <summary>
            ///     Sent on moving camera
            /// </summary>
            

            /// <summary>
            ///     Sent on a spell cast
            /// </summary>
            CastSpell = 0xE4,

            /// <summary>
            ///     Sent on a chat message(in the game, not client)
            /// </summary>
            Chat = 0x68,

            /// <summary>
            ///     Sent on an emote
            /// </summary>
            Emote = 0x2C,
            
            /// <summary>
            ///     Sent on game play..?
            /// </summary>
            GamePlay = 0x4F,

            /// <summary>
            ///     Sent on interacting with objects(Clicking thresh lantern)
            /// </summary>
            InteractObject = 0x90,

            /// <summary>
            ///     Sent on an issue order(move to, hold position)
            /// </summary>
            IssueOrder = 0xB5,

            /// <summary>
            ///     Sent on leveling spell
            /// </summary>
            LevelSpell = 0x9A,

            /// <summary>
            ///     Sent on locking camera.
            /// </summary>
            LockCamera = 0xFD,

            /// <summary>
            ///     Sent on pausing games(tournament mode, custom games)
            /// </summary>
            PauseGame = 0x97,

            /// <summary>
            ///     Sent on game ping
            /// </summary>
            Ping = 0x22,

            /// <summary>
            ///     Sent on when game resumes from pause
            /// </summary>
            ResumeGame = 0x19,

            /// <summary>
            ///     Sent when opening score board.
            /// </summary>
            ScoreScreen = 0x96,

            /// <summary>
            ///     Sent on left clicking target.
            /// </summary>
            SetTarget = 0xA9,

            /// <summary>
            ///     Sent on pressing the undo button in the shop.
            /// </summary>
            UndoBuy = 0x1C
        }
    }
}