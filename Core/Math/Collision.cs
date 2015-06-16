// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Collision.cs" company="LeagueSharp">
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
//   Collision class, calculates collision for moving objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Math
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Math.Prediction;

    using SharpDX;

    /// <summary>
    ///     Collision class, calculates collision for moving objects.
    /// </summary>
    public static class Collision
    {
        #region Static Fields

        /// <summary>
        ///     Wall Cast Tick (for <c>Yasuo</c>)
        /// </summary>
        private static int wallCastT;

        /// <summary>
        ///     <c>Yasuo</c>'s wall position in Vector2 format.
        /// </summary>
        private static Vector2 yasuoWallCastedPos;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Collision" /> class.
        ///     Static Constructor
        /// </summary>
        static Collision()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the list of the units that the skill-shot will hit before reaching the set positions.
        /// </summary>
        /// <param name="positions">
        ///     The positions.
        /// </param>
        /// <param name="input">
        ///     The input.
        /// </param>
        /// <returns>
        ///     A list of <c>Obj_AI_Base</c>s which the input collides with.
        /// </returns>
        public static List<Obj_AI_Base> GetCollision(List<Vector3> positions, PredictionInput input)
        {
            var result = new List<Obj_AI_Base>();

            foreach (var position in positions)
            {
                if (input.CollisionObjects.HasFlag(CollisionableObjects.Minions))
                {
                    foreach (var minion in
                        GameObjects.EnemyMinions.Where(
                            minion =>
                            minion.IsValidTarget(
                                Math.Min(input.Range + input.Radius + 100, 2000), 
                                true, 
                                input.RangeCheckFrom)))
                    {
                        input.Unit = minion;
                        var minionPrediction = Movement.GetPrediction(input, false, false);
                        if (minionPrediction.UnitPosition.ToVector2()
                                .DistanceSquared(input.From.ToVector2(), position.ToVector2(), true)
                            <= Math.Pow(input.Radius + 15 + minion.BoundingRadius, 2))
                        {
                            result.Add(minion);
                        }
                    }
                }

                if (input.CollisionObjects.HasFlag(CollisionableObjects.Heroes))
                {
                    foreach (var hero in
                        GameObjects.EnemyHeroes.Where(
                            hero =>
                            hero.IsValidTarget(
                                Math.Min(input.Range + input.Radius + 100, 2000), 
                                true, 
                                input.RangeCheckFrom)))
                    {
                        input.Unit = hero;
                        var prediction = Movement.GetPrediction(input, false, false);
                        if (prediction.UnitPosition.ToVector2()
                                .DistanceSquared(input.From.ToVector2(), position.ToVector2(), true)
                            <= Math.Pow(input.Radius + 50 + hero.BoundingRadius, 2))
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
                            result.Add(GameObjects.Player);
                        }
                    }
                }

                if (input.CollisionObjects.HasFlag(CollisionableObjects.YasuoWall))
                {
                    if (Variables.TickCount - wallCastT > 4000)
                    {
                        continue;
                    }

                    GameObject wall = null;
                    foreach (var gameObject in
                        GameObjects.AllGameObjects.Where(
                            gameObject =>
                            gameObject.IsValid
                            && Regex.IsMatch(gameObject.Name, "_w_windwall_enemy_0.\\.troy", RegexOptions.IgnoreCase)))
                    {
                        wall = gameObject;
                    }

                    if (wall == null)
                    {
                        break;
                    }

                    var level = wall.Name.Substring(wall.Name.Length - 6, 1);
                    var wallWidth = 300 + 50 * Convert.ToInt32(level);

                    var wallDirection = (wall.Position.ToVector2() - yasuoWallCastedPos).Normalized().Perpendicular();
                    var wallStart = wall.Position.ToVector2() + wallWidth / 2f * wallDirection;
                    var wallEnd = wallStart - wallWidth * wallDirection;

                    if (wallStart.Intersection(wallEnd, position.ToVector2(), input.From.ToVector2()).Intersects)
                    {
                        var t = Variables.TickCount
                                + (wallStart.Intersection(wallEnd, position.ToVector2(), input.From.ToVector2())
                                       .Point.Distance(input.From) / input.Speed + input.Delay) * 1000;
                        if (t < wallCastT + 4000)
                        {
                            result.Add(GameObjects.Player);
                        }
                    }
                }
            }

            return result.Distinct().ToList();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Processed Casted Spell subscribed event function
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base" /> sender.</param>
        /// <param name="args">Processed Spell Cast Data</param>
        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsValid && sender.Team != GameObjects.Player.Team && args.SData.Name == "YasuoWMovingWall")
            {
                wallCastT = Variables.TickCount;
                yasuoWallCastedPos = sender.ServerPosition.ToVector2();
            }
        }

        #endregion
    }
}