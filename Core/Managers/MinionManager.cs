// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinionManager.cs" company="LeagueSharp">
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
//   Provides utilities to minions, including getting minions, and getting the best farm location.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Managers
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.Math.Prediction;

    using SharpDX;

    /// <summary>
    ///     Provides utilities to minions, including getting minions, and getting the best farm location.
    /// </summary>
    public static class MinionManager
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns the point where, when casted, the circular spell with hit the maximum amount of minions.
        /// </summary>
        /// <param name="minionPositions">List of minion positions</param>
        /// <param name="width">Width of the circle</param>
        /// <param name="range">Minions in the range of the circle.</param>
        /// <param name="useMecMax">ConvexHull maximum. <see cref="ConvexHull" /></param>
        /// <returns>The best <see cref="FarmLocation" /></returns>
        public static FarmLocation GetBestCircularFarmLocation(
            List<Vector2> minionPositions, 
            float width, 
            float range, 
            int useMecMax = 9)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = ObjectManager.Player.ServerPosition.ToVector2();

            range = range * range;

            if (minionPositions.Count == 0)
            {
                return new FarmLocation(result, minionCount);
            }

            /* Use MEC to get the best positions only when there are less than 9 positions because it causes lag with more. */
            if (minionPositions.Count <= useMecMax)
            {
                var subGroups = minionPositions.GetCombinations();
                foreach (var subGroup in subGroups)
                {
                    if (subGroup.Count <= 0)
                    {
                        continue;
                    }

                    var circle = ConvexHull.GetMec(subGroup);

                    if (!(circle.Radius <= width) || !(circle.Center.DistanceSquared(startPos) <= range))
                    {
                        continue;
                    }

                    minionCount = subGroup.Count;
                    return new FarmLocation(circle.Center, minionCount);
                }
            }
            else
            {
                foreach (var pos in minionPositions)
                {
                    if (!(pos.DistanceSquared(startPos) <= range))
                    {
                        continue;
                    }

                    var count = minionPositions.Count(pos2 => pos.DistanceSquared(pos2) <= width * width);

                    if (count < minionCount)
                    {
                        continue;
                    }

                    result = pos;
                    minionCount = count;
                }
            }

            return new FarmLocation(result, minionCount);
        }

        /// <summary>
        ///     Returns the point where, when casted, the lineal spell with hit the maximum amount of minions.
        /// </summary>
        /// <param name="minionPositions">Minion Positions</param>
        /// <param name="width">Width of the line</param>
        /// <param name="range">Range of the line</param>
        /// <returns>Best <see cref="FarmLocation" /></returns>
        public static FarmLocation GetBestLineFarmLocation(List<Vector2> minionPositions, float width, float range)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = ObjectManager.Player.ServerPosition.ToVector2();

            var max = minionPositions.Count;
            for (var i = 0; i < max; i++)
            {
                for (var j = 0; j < max; j++)
                {
                    if (minionPositions[j] != minionPositions[i])
                    {
                        minionPositions.Add((minionPositions[j] + minionPositions[i]) / 2);
                    }
                }
            }

            foreach (var pos in minionPositions)
            {
                if (!(pos.DistanceSquared(startPos) <= range * range))
                {
                    continue;
                }

                var endPos = startPos + range * (pos - startPos).Normalized();
                var count = minionPositions.Count(pos2 => pos2.Distance(startPos + endPos) <= width * width);

                if (count < minionCount)
                {
                    continue;
                }

                result = endPos;
                minionCount = count;
            }

            return new FarmLocation(result, minionCount);
        }

        /// <summary>
        ///     Returns the minions in range from From.
        /// </summary>
        /// <param name="from">
        ///     The from.
        /// </param>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="team">
        ///     The team.
        /// </param>
        /// <param name="order">
        ///     The order.
        /// </param>
        /// <returns>
        ///     A list containing all <c>Obj_AI_Base</c>s of the minions.
        /// </returns>
        public static List<Obj_AI_Base> GetMinions(
            Vector3 from, 
            float range, 
            MinionTypes type = MinionTypes.All, 
            MinionTeam team = MinionTeam.Enemy, 
            MinionOrderTypes order = MinionOrderTypes.Health)
        {
            var result =
                ObjectHandler.GetFast<Obj_AI_Minion>()
                    .Where(minion => minion.IsValidTarget(range, false, @from))
                    .Select(minion => new { minion, minionTeam = minion.Team })
                    .Where(
                        @t =>
                        team == MinionTeam.Neutral && @t.minionTeam == GameObjectTeam.Neutral
                        || team == MinionTeam.Ally
                        && @t.minionTeam
                        == (ObjectManager.Player.Team == GameObjectTeam.Chaos
                                ? GameObjectTeam.Chaos
                                : GameObjectTeam.Order)
                        || team == MinionTeam.Enemy
                        && @t.minionTeam
                        == (ObjectManager.Player.Team == GameObjectTeam.Chaos
                                ? GameObjectTeam.Order
                                : GameObjectTeam.Chaos)
                        || team == MinionTeam.NotAlly && @t.minionTeam != ObjectManager.Player.Team
                        || team == MinionTeam.NotAllyForEnemy
                        && (@t.minionTeam == ObjectManager.Player.Team || @t.minionTeam == GameObjectTeam.Neutral)
                        || team == MinionTeam.All)
                    .Where(
                        @t =>
                        @t.minion.CombatType == GameObjectCombatType.Melee && type == MinionTypes.Melee
                        || @t.minion.CombatType != GameObjectCombatType.Melee && type == MinionTypes.Ranged
                        || type == MinionTypes.All)
                    .Where(@t => IsMinion(@t.minion) || @t.minionTeam == GameObjectTeam.Neutral)
                    .Select(@t => @t.minion)
                    .Cast<Obj_AI_Base>()
                    .ToList();

            switch (order)
            {
                case MinionOrderTypes.Health:
                    result = result.OrderBy(o => o.Health).ToList();
                    break;
                case MinionOrderTypes.MaxHealth:
                    result = result.OrderBy(o => o.MaxHealth).Reverse().ToList();
                    break;
            }

            return result;
        }

        /// <summary>
        ///     Gets the minions in the range, by type, team and orders them.
        /// </summary>
        /// <param name="range">The Range</param>
        /// <param name="type">Type of minion. <see cref="MinionTypes" /></param>
        /// <param name="team">Team of minion. <see cref="MinionTeam" /></param>
        /// <param name="order">Orders the minions. <see cref="MinionOrderTypes" /></param>
        /// <returns>List of minions.</returns>
        public static List<Obj_AI_Base> GetMinions(
            float range, 
            MinionTypes type = MinionTypes.All, 
            MinionTeam team = MinionTeam.Enemy, 
            MinionOrderTypes order = MinionOrderTypes.Health)
        {
            return GetMinions(ObjectManager.Player.ServerPosition, range, type, team, order);
        }

        /// <summary>
        ///     Returns a list of predicted minion positions.
        /// </summary>
        /// <param name="minions">Given Minion List</param>
        /// <param name="delay">Skill-shot Delay</param>
        /// <param name="width">Skill-shot Width</param>
        /// <param name="speed">Skill-shot Speed</param>
        /// <param name="from">The From</param>
        /// <param name="range">Skill-shot Range</param>
        /// <param name="collision">Has Collision Flag</param>
        /// <param name="stype">Skill-shot Type</param>
        /// <param name="rangeCheckFrom">Range check from Vector3 source</param>
        /// <returns>List of Points in <see cref="Vector2" /> type</returns>
        public static List<Vector2> GetMinionsPredictedPositions(
            List<Obj_AI_Base> minions, 
            float delay, 
            float width, 
            float speed, 
            Vector3 from, 
            float range, 
            bool collision, 
            SkillshotType stype, 
            Vector3 rangeCheckFrom = new Vector3())
        {
            from = from.ToVector2().IsValid() ? from : ObjectManager.Player.ServerPosition;

            return (from minion in minions
                    select
                        Movement.GetPrediction(
                            new PredictionInput
                                {
                                    Unit = minion, Delay = delay, Radius = width, Speed = speed, From = @from, 
                                    Range = range, Collision = collision, Type = stype, RangeCheckFrom = rangeCheckFrom
                                })
                    into pos
                    where pos.Hitchance >= HitChance.High
                    select pos.UnitPosition.ToVector2()).ToList();
        }

        /// <summary>
        ///     Tells whether the <see cref="Obj_AI_Minion" /> is an actual minion.
        /// </summary>
        /// <param name="minion">The Minion</param>
        /// <param name="includeWards">Whether to include wards.</param>
        /// <returns>Whether the <see cref="Obj_AI_Minion" /> is an actual minion.</returns>
        public static bool IsMinion(Obj_AI_Minion minion, bool includeWards = false)
        {
            var name = minion.BaseSkinName.ToLower();
            return name.Contains("minion") || (includeWards && (name.Contains("ward") || name.Contains("trinket")));
        }

        #endregion

        /// <summary>
        ///     Struct with the best farm locations
        /// </summary>
        public struct FarmLocation
        {
            #region Fields

            /// <summary>
            ///     The number of minions in the AOE.
            /// </summary>
            public int MinionsHit;

            /// <summary>
            ///     Best farm location position.
            /// </summary>
            public Vector2 Position;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="FarmLocation" /> struct.
            /// </summary>
            /// <param name="position">
            ///     The position
            /// </param>
            /// <param name="minionsHit">
            ///     The minions hit
            /// </param>
            internal FarmLocation(Vector2 position, int minionsHit)
            {
                this.Position = position;
                this.MinionsHit = minionsHit;
            }

            #endregion
        }
    }
}