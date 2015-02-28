namespace LeagueSharp.CommonEx.Core.Network
{
    internal class Packet
    {
        public enum C2S
        {
            Camera = 0x10,
            ResumeGame = 0x19,
            UndoBuy = 0x1C,
            Ping = 0x22,
            Emote = 0x2C,
            GamePlay = 0x4F,
            Chat = 0x68,
            ScoreScreen = 0x96,
            PauseGame = 0x97,
            LevelSpell = 0x9A,
            SetTarget = 0xA9,
            IssueOrder = 0xB5,
            BuyItem = 0xD7,
            CastSpell = 0xE4,
            LockCamera = 0xFD
        }
    }
}