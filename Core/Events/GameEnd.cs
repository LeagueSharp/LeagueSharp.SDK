#region

using System;
using System.Linq;
using LeagueSharp.CommonEx.Core.Utils;

#endregion

namespace LeagueSharp.CommonEx.Core.Events
{
    /// <summary>
    ///     Provides an event for when the game ends.
    /// </summary>
    public class GameEnd
    {
        /// <summary>
        ///     Static constructor.
        /// </summary>
        static GameEnd()
        {
            if (Game.Mode == GameMode.Finished)
            {
                var nexus = ObjectHandler.GetFast<Obj_HQ>().FirstOrDefault(n => n.Health <= 0);
                if (nexus != null && nexus.IsValid)
                {
                    DelayAction.Add(
                        0,
                        () =>
                            Game_OnGameEnd(
                                new GameEndEventArgs(
                                    (nexus.Team == GameObjectTeam.Order) ? GameObjectTeam.Chaos : GameObjectTeam.Order)));
                    return;
                }
                DelayAction.Add(
                    0, () => Game_OnGameEnd(new GameEndEventArgs(GameObjectTeam.Unknown)));
            }
            else
            {
                Game.OnGameUpdate += Game_OnGameEnd;
            }
        }

        /// <summary>
        ///     OnGameEnd is getting called when you the game is finished by either one of the nexuses gets destoryed, also returns
        ///     who won the game.
        /// </summary>
        public static event Action<GameEndEventArgs> OnGameEnd;

        /// <summary>
        ///     Internal event that notifies when a nexus is destroyed, provides also information about which team won.
        /// </summary>
        /// <param name="args">System.EventArgs</param>
        private static void Game_OnGameEnd(EventArgs args)
        {
            foreach (var nexus in ObjectHandler.GetFast<Obj_HQ>().Where(nexus => nexus.Health <= 0))
            {
                if (OnGameEnd != null)
                {
                    OnGameEnd(
                        new GameEndEventArgs(
                            (nexus.Team == GameObjectTeam.Order) ? GameObjectTeam.Chaos : GameObjectTeam.Order));
                }
                Game.OnGameUpdate -= Game_OnGameEnd;
                return;
            }
        }
    }
}