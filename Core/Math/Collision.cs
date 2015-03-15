#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LeagueSharp.CommonEx.Core.Enumerations;
using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.Math.Prediction;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.Math
{
    /// <summary>
    ///     Collision class, calculates collision for moving objects.
    /// </summary>
    public static class Collision
    {
        /// <summary>
        ///     Wall Cast Tick (for Yasuo)
        /// </summary>
        private static int _wallCastT;

        /// <summary>
        ///     Yasuo's wall position in Vector2 format.
        /// </summary>
        private static Vector2 _yasuoWallCastedPos;

        /// <summary>
        ///     Static Constructor
        /// </summary>
        static Collision()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
        }

        /// <summary>
        ///     Processed Casted Spell subscribed event function
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base"/> sender.</param>
        /// <param name="args">Processed Spell Cast Data</param>
        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsValid && sender.Team != ObjectManager.Player.Team && args.SData.Name == "YasuoWMovingWall")
            {
                _wallCastT = Variables.TickCount;
                _yasuoWallCastedPos = sender.ServerPosition.ToVector2();
            }
        }

        /// <summary>
        ///     Returns the list of the units that the skillshot will hit before reaching the set positions.
        /// </summary>
        public static List<Obj_AI_Base> GetCollision(List<Vector3> positions, PredictionInput input)
        {
            var result = new List<Obj_AI_Base>();

            foreach (var position in positions)
            {
                if (input.CollisionObjects.HasFlag(CollisionableObjects.Minions))
                {
                    foreach (var minion in
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .Where(
                                        minion =>
                                            minion.IsValidTarget(
                                                System.Math.Min(input.Range + input.Radius + 100, 2000), true,
                                                input.RangeCheckFrom)))
                    {
                        input.Unit = minion;
                        var minionPrediction = Movement.GetPrediction(input, false, false);
                        if (
                            minionPrediction.UnitPosition.ToVector2()
                                .DistanceSquared(input.From.ToVector2(), position.ToVector2(), true) <=
                            System.Math.Pow((input.Radius + 15 + minion.BoundingRadius), 2))
                        {
                            result.Add(minion);
                        }
                    }
                }
                if (input.CollisionObjects.HasFlag(CollisionableObjects.Heroes))
                {
                    foreach (var hero in
                                ObjectHandler.Enemies.Where(
                                    hero =>
                                        hero.IsValidTarget(
                                            System.Math.Min(input.Range + input.Radius + 100, 2000), true,
                                            input.RangeCheckFrom)))
                    {
                        input.Unit = hero;
                        var prediction = Movement.GetPrediction(input, false, false);
                        if (
                            prediction.UnitPosition.ToVector2()
                                .DistanceSquared(input.From.ToVector2(), position.ToVector2(), true) <=
                            System.Math.Pow((input.Radius + 50 + hero.BoundingRadius), 2))
                        {
                            result.Add(hero);
                        }
                    }
                }
                if (input.CollisionObjects.HasFlag(CollisionableObjects.Walls))
                {
                    var step = position.Distance(input.From) / 20;
                    for (var i = 0; i < 20; i++)
                    {
                        var p = input.From.ToVector2().Extend(position.ToVector2(), step * i);
                        if (NavMesh.GetCollisionFlags(p.X, p.Y).HasFlag(CollisionFlags.Wall))
                        {
                            result.Add(ObjectManager.Player);
                        }
                    }
                }
                if (input.CollisionObjects.HasFlag(CollisionableObjects.YasuoWall))
                {
                    if (Variables.TickCount - _wallCastT > 4000)
                    {
                        continue;
                    }

                    GameObject wall = null;
                    foreach (var gameObject in
                        ObjectManager.Get<GameObject>()
                            .Where(
                                gameObject =>
                                    gameObject.IsValid &&
                                    Regex.IsMatch(
                                        gameObject.Name, "_w_windwall_enemy_0.\\.troy", RegexOptions.IgnoreCase))
                        )
                    {
                        wall = gameObject;
                    }
                    if (wall == null)
                    {
                        break;
                    }
                    var level = wall.Name.Substring(wall.Name.Length - 6, 1);
                    var wallWidth = (300 + 50 * Convert.ToInt32(level));

                    var wallDirection =
                        (wall.Position.ToVector2() - _yasuoWallCastedPos).Normalized().Perpendicular();
                    var wallStart = wall.Position.ToVector2() + wallWidth / 2f * wallDirection;
                    var wallEnd = wallStart - wallWidth * wallDirection;

                    if (wallStart.Intersection(wallEnd, position.ToVector2(), input.From.ToVector2()).Intersects)
                    {
                        var t = Variables.TickCount +
                                (wallStart.Intersection(wallEnd, position.ToVector2(), input.From.ToVector2())
                                    .Point.Distance(input.From) / input.Speed + input.Delay) * 1000;
                        if (t < _wallCastT + 4000)
                        {
                            result.Add(ObjectManager.Player);
                        }
                    }
                }
            }

            return result.Distinct().ToList();
        }
    }
}