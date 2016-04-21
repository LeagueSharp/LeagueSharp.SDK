// <copyright file="Collision.cs" company="LeagueSharp">
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
    using System.Linq;
    using System.Text.RegularExpressions;

    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK.Polygons;

    using SharpDX;

    using Color = System.Drawing.Color;

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
                    result.AddRange(
                        GameObjects.EnemyMinions.Where(
                            minion =>
                            minion.IsValidTarget(
                                Math.Min(input.Range + input.Radius + 100, 2000),
                                true,
                                input.RangeCheckFrom) && IsHitCollision(minion, input, position, 20)));
                }

                if (input.CollisionObjects.HasFlag(CollisionableObjects.Heroes))
                {
                    result.AddRange(
                        GameObjects.EnemyHeroes.Where(
                            hero =>
                            hero.IsValidTarget(
                                Math.Min(input.Range + input.Radius + 100, 2000),
                                true,
                                input.RangeCheckFrom) && IsHitCollision(hero, input, position, 50)));
                }

                if (input.CollisionObjects.HasFlag(CollisionableObjects.Walls))
                {
                    var step = position.Distance(input.From) / 20;
                    for (var i = 0; i < 20; i++)
                    {
                        if (input.From.ToVector2().Extend(position, step * i).IsWall())
                        {
                            result.Add(GameObjects.Player);
                        }
                    }
                }

                if (input.CollisionObjects.HasFlag(CollisionableObjects.YasuoWall))
                {
                    /*if (Variables.TickCount - wallCastT > 4000)
                    {
                        continue;
                    }*/

                    var wall =
                        GameObjects.AllGameObjects.FirstOrDefault(
                            gameObject =>
                            gameObject.IsValid
                            && Regex.IsMatch(gameObject.Name, "_w_windwall_enemy_0.\\.troy", RegexOptions.IgnoreCase));

                    if (wall == null)
                    {
                        continue;
                    }
                    Game.PrintChat(wall.Name + " => " + wall.Type + " | " + wall.Team);

                    var level = wall.Name.Substring(wall.Name.Length - 6, 1);
                    var wallWidth = 300 + (50 * Convert.ToInt32(level));

                    var wallDirection =
                        (wall.Position.ToVector2() - wall.Orientation.ToVector2()).Normalized().Perpendicular();
                    var wallStart = wall.Position.ToVector2() + (wallWidth / 2f * wallDirection);
                    var wallEnd = wallStart - (wallWidth * wallDirection);

                    /*var wallIntersect = wallStart.Intersection(wallEnd, position.ToVector2(), input.From.ToVector2());

                    if (wallIntersect.Intersects)
                    {
                        result.Add(GameObjects.Player);
                    }*/

                    var wallPolygon = new RectanglePoly(wallStart, wallEnd, 75);
                    var intersections = new List<Vector2>();
                    for (var i = 0; i < wallPolygon.Points.Count; i++)
                    {
                        var inter =
                            wallPolygon.Points[i].Intersection(
                                wallPolygon.Points[i != wallPolygon.Points.Count - 1 ? i + 1 : 0],
                                input.From.ToVector2(),
                                position.ToVector2());

                        if (inter.Intersects)
                        {
                            intersections.Add(inter.Point);
                        }
                    }
                    wallPolygon.Draw(Color.Red);

                    if (intersections.Count > 0)
                    {
                        result.Add(GameObjects.Player);
                    }
                }
            }

            return result.Distinct().ToList();
        }

        #endregion

        #region Methods

        private static bool IsHitCollision(Obj_AI_Base collision, PredictionInput input, Vector3 pos, float extraRadius)
        {
            var inputSub = input.Clone() as PredictionInput;

            if (inputSub == null)
            {
                return false;
            }

            inputSub.Unit = collision;
            var predPos = Movement.GetPrediction(inputSub, false, false).UnitPosition.ToVector2();
            return predPos.Distance(input.From) < input.Radius + input.Unit.BoundingRadius / 2
                   || predPos.Distance(pos) < input.Radius + input.Unit.BoundingRadius / 2
                   || predPos.DistanceSquared(input.From.ToVector2(), pos.ToVector2(), true)
                   <= Math.Pow(input.Radius + input.Unit.BoundingRadius + extraRadius, 2);
        }

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