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

namespace LeagueSharp.SDKEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDKEx.Polygons;
    using LeagueSharp.SDKEx.Utils;

    using SharpDX;

    /// <summary>
    ///     Collision class, calculates collision for moving objects.
    /// </summary>
    public static class Collision
    {
        #region Static Fields

        private static MissileClient yasuoWallLeft, yasuoWallRight;

        private static RectanglePoly yasuoWallPoly;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="Collision" /> class.
        ///     Static Constructor
        /// </summary>
        static Collision()
        {
            GameObject.OnCreate += (sender, args) =>
                {
                    var missile = sender as MissileClient;
                    var spellCaster = missile?.SpellCaster as Obj_AI_Hero;

                    if (spellCaster == null || spellCaster.ChampionName != "Yasuo"
                        || spellCaster.Team == GameObjects.Player.Team)
                    {
                        return;
                    }

                    switch (missile.SData.Name)
                    {
                        case "YasuoWMovingWallMisL":
                            yasuoWallLeft = missile;
                            break;
                        case "YasuoWMovingWallMisR":
                            yasuoWallRight = missile;
                            break;
                    }
                };
            GameObject.OnDelete += (sender, args) =>
                {
                    var missile = sender as MissileClient;

                    if (missile == null)
                    {
                        return;
                    }

                    if (missile.Compare(yasuoWallLeft))
                    {
                        yasuoWallLeft = null;
                    }
                    else if (missile.Compare(yasuoWallRight))
                    {
                        yasuoWallRight = null;
                    }
                };
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
                        GameObjects.EnemyMinions.Where(i => i.IsMinion() || i.IsPet())
                            .Concat(GameObjects.Jungle)
                            .Where(
                                minion =>
                                minion.IsValidTarget(
                                    Math.Min(input.Range + input.Radius + 100, 2000),
                                    true,
                                    input.RangeCheckFrom) && IsHitCollision(minion, input, position, 15)));
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
                    if (yasuoWallLeft == null || yasuoWallRight == null)
                    {
                        continue;
                    }

                    yasuoWallPoly = new RectanglePoly(yasuoWallLeft.Position, yasuoWallRight.Position, 75);

                    var intersections = new List<Vector2>();
                    for (var i = 0; i < yasuoWallPoly.Points.Count; i++)
                    {
                        var inter =
                            yasuoWallPoly.Points[i].Intersection(
                                yasuoWallPoly.Points[i != yasuoWallPoly.Points.Count - 1 ? i + 1 : 0],
                                input.From.ToVector2(),
                                position.ToVector2());

                        if (inter.Intersects)
                        {
                            intersections.Add(inter.Point);
                        }
                    }

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

        private static bool IsHitCollision(Obj_AI_Base collision, ICloneable input, Vector3 pos, float extraRadius)
        {
            var inputSub = input.Clone() as PredictionInput;

            if (inputSub == null)
            {
                return false;
            }

            inputSub.Unit = collision;
            var unitRadius = inputSub.Unit.BoundingRadius;
            var predPos = Movement.GetPrediction(inputSub, false, false).UnitPosition.ToVector2();
            return predPos.Distance(inputSub.From) < inputSub.Radius + unitRadius / 2
                   || predPos.Distance(pos) < inputSub.Radius + unitRadius / 2
                   || predPos.DistanceSquared(inputSub.From.ToVector2(), pos.ToVector2(), true)
                   <= Math.Pow(inputSub.Radius + unitRadius + extraRadius, 2);
        }

        #endregion
    }
}