// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Minion.cs" company="LeagueSharp">
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
//   The minion utils, contains a set of functions to quickly operate around minions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Utils
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
    ///     The minion utils, contains a set of functions to quickly operate around minions.
    /// </summary>
    public static class Minion
    {
        #region Static Fields

        /// <summary>
        ///     The normal minion list.
        /// </summary>
        private static readonly List<string> NormalMinionList = new List<string>
                                                                    {
                                                                        "SRU_ChaosMinionMelee", "SRU_ChaosMinionRanged", 
                                                                        "SRU_OrderMinionMelee", "SRU_OrderMinionRanged"
                                                                    };

        /// <summary>
        ///     The siege minion list.
        /// </summary>
        private static readonly List<string> SiegeMinionList = new List<string>
                                                                   {
                                                                      "SRU_ChaosMinionSiege", "SRU_OrderMinionSiege" 
                                                                   };

        /// <summary>
        ///     The super minion list.
        /// </summary>
        private static readonly List<string> SuperMinionList = new List<string>
                                                                   {
                                                                      "SRU_ChaosMinionSuper", "SRU_OrderMinionSuper" 
                                                                   };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the point where, when casted, the circular spell with hit the maximum amount of minions.
        /// </summary>
        /// <param name="minions">
        ///     List of minions
        /// </param>
        /// <param name="width">
        ///     Width of the circle
        /// </param>
        /// <param name="range">
        ///     Minions in the range of the circle.
        /// </param>
        /// <param name="useMecMax">
        ///     ConvexHull maximum. <see cref="ConvexHull" />
        /// </param>
        /// <returns>
        ///     The best <see cref="FarmLocation" />
        /// </returns>
        public static FarmLocation GetBestCircularFarmLocation(
            IDictionary<Obj_AI_Base, Vector2> minions, 
            float width, 
            float range, 
            int useMecMax = 9)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = GameObjects.Player.ServerPosition.ToVector2();
            var minionPositions = minions.Select(m => m.Value).ToList();
            var hitMinions = new List<Obj_AI_Base>();

            range = range * range;

            if (minionPositions.Count == 0)
            {
                return new FarmLocation(result, minionCount, new List<Obj_AI_Base>());
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
                    return new FarmLocation(
                        circle.Center, 
                        minionCount, 
                        minions.Where(
                            m => m.Value.Distance(circle.Center) <= width || m.Value.Distance(circle.Center) <= range)
                            .Select(m => m.Key)
                            .ToList());
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

                    var pos1 = pos;
                    hitMinions =
                        minions.Where(m => pos1.DistanceSquared(m.Key.Position) <= width * width)
                            .Select(m => m.Key)
                            .ToList();

                    result = pos;
                    minionCount = count;
                }
            }

            return new FarmLocation(result, minionCount, hitMinions);
        }

        /// <summary>
        ///     Returns the point where, when casted, the lineal spell with hit the maximum amount of minions.
        /// </summary>
        /// <param name="minions">
        ///     The Minions
        /// </param>
        /// <param name="width">
        ///     Width of the line
        /// </param>
        /// <param name="range">
        ///     Range of the line
        /// </param>
        /// <returns>
        ///     Best <see cref="FarmLocation" />.
        /// </returns>
        public static FarmLocation GetBestLineFarmLocation(List<Obj_AI_Base> minions, float width, float range)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = GameObjects.Player.ServerPosition.ToVector2();
            var minionPositions = minions.ToDictionary(minion => minion, minion => minion.Position.ToVector2());
            var minionsHit = new List<Obj_AI_Base>();

            var max = minions.Count;
            for (var i = 0; i < max; i++)
            {
                for (var j = 0; j < max; j++)
                {
                    if (minions[j].Position != minions[i].Position)
                    {
                        minionPositions.Add(minions[j], ((minions[j].Position + minions[i].Position) / 2).ToVector2());
                    }
                }
            }

            foreach (var pos in minionPositions)
            {
                if (!(pos.Value.DistanceSquared(startPos) <= range * range))
                {
                    continue;
                }

                var endPos = startPos + range * (pos.Value - startPos).Normalized();
                var count = minionPositions.Count(pos2 => pos2.Value.Distance(startPos + endPos) <= width * width);

                if (count < minionCount)
                {
                    continue;
                }

                result = endPos;
                minionCount = count;
                minionsHit =
                    minionPositions.Where(p => p.Value.Distance(startPos + endPos) <= width * width)
                        .Select(k => k.Key)
                        .ToList();
            }

            return new FarmLocation(result, minionCount, minionsHit);
        }

        /// <summary>
        ///     Returns the point where, when casted, the lineal spell with hit the maximum amount of minions.
        /// </summary>
        /// <param name="minions">
        ///     The Minions
        /// </param>
        /// <param name="width">
        ///     Width of the line
        /// </param>
        /// <param name="range">
        ///     Range of the line
        /// </param>
        /// <returns>
        ///     Best <see cref="FarmLocation" />.
        /// </returns>
        public static FarmLocation GetBestLineFarmLocation(
            IDictionary<Obj_AI_Base, Vector2> minions, 
            float width, 
            float range)
        {
            var result = new Vector2();
            var minionCount = 0;
            var startPos = GameObjects.Player.ServerPosition.ToVector2();
            var minionPositions = minions.ToDictionary(minion => minion.Key, minion => minion.Value);
            var minionsHit = new List<Obj_AI_Base>();

            foreach (var pair in minions)
            {
                var pair1 = pair;
                foreach (var secondPair in minions.Where(secondPair => pair1.Value != secondPair.Value))
                {
                    minionPositions.Add(pair.Key, (pair.Value + secondPair.Value) / 2);
                }
            }

            foreach (var pos in minionPositions)
            {
                if (!(pos.Value.DistanceSquared(startPos) <= range * range))
                {
                    continue;
                }

                var endPos = startPos + range * (pos.Value - startPos).Normalized();
                var count = minionPositions.Count(pos2 => pos2.Value.Distance(startPos + endPos) <= width * width);

                if (count < minionCount)
                {
                    continue;
                }

                result = endPos;
                minionCount = count;
                minionsHit =
                    minionPositions.Where(p => p.Value.Distance(startPos + endPos) <= width * width)
                        .Select(k => k.Key)
                        .ToList();
            }

            return new FarmLocation(result, minionCount, minionsHit);
        }

        /// <summary>
        ///     Returns a list of predicted minion positions.
        /// </summary>
        /// <param name="minions">
        ///     Given Minion List
        /// </param>
        /// <param name="delay">
        ///     Skill-shot Delay
        /// </param>
        /// <param name="width">
        ///     Skill-shot Width
        /// </param>
        /// <param name="speed">
        ///     Skill-shot Speed
        /// </param>
        /// <param name="from">
        ///     The From
        /// </param>
        /// <param name="range">
        ///     Skill-shot Range
        /// </param>
        /// <param name="collision">
        ///     Has Collision Flag
        /// </param>
        /// <param name="stype">
        ///     Skill-shot Type
        /// </param>
        /// <param name="rangeCheckFrom">
        ///     Range check from Vector3 source
        /// </param>
        /// <returns>
        ///     List of Points in <see cref="Vector2" /> type
        /// </returns>
        public static IDictionary<Obj_AI_Base, Vector2> GetMinionsPredictedPositions(
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
            from = from.ToVector2().IsValid() ? from : GameObjects.Player.ServerPosition;

            var value = new Dictionary<Obj_AI_Base, Vector2>();
            foreach (var minion in minions)
            {
                var position =
                    Movement.GetPrediction(
                        new PredictionInput
                            {
                                Unit = minion, Delay = delay, Radius = width, Speed = speed, From = @from, Range = range, 
                                Collision = collision, Type = stype, RangeCheckFrom = rangeCheckFrom
                            });
                if (position.Hitchance >= HitChance.High)
                {
                    value.Add(minion, position.UnitPosition.ToVector2());
                }
            }

            return value;
        }

        /// <summary>
        ///     Gets the minion type.
        /// </summary>
        /// <param name="minion">
        ///     The minion.
        /// </param>
        /// <returns>
        ///     The <see cref="MinionTypes" />
        /// </returns>
        public static MinionTypes GetMinionType(this Obj_AI_Base minion)
        {
            var baseSkinName = minion.CharData.BaseSkinName;

            if (NormalMinionList.Any(n => baseSkinName.Equals(n)))
            {
                return MinionTypes.Normal
                       | (minion.CharData.BaseSkinName.Contains("Melee") ? MinionTypes.Melee : MinionTypes.Ranged);
            }

            if (SiegeMinionList.Any(n => baseSkinName.Equals(n)))
            {
                return MinionTypes.Siege | MinionTypes.Ranged;
            }

            if (SuperMinionList.Any(n => baseSkinName.Equals(n)))
            {
                return MinionTypes.Super | MinionTypes.Melee;
            }

            if (baseSkinName.ToLower().Contains("ward") || baseSkinName.Contains("trinket"))
            {
                return MinionTypes.Ward;
            }

            return MinionTypes.Unknown;
        }

        /// <summary>
        ///     Tells whether the <see cref="Obj_AI_Minion" /> is an actual minion.
        /// </summary>
        /// <param name="minion">The Minion</param>
        /// <param name="includeWards">Whether to include wards.</param>
        /// <returns>Whether the <see cref="Obj_AI_Minion" /> is an actual minion.</returns>
        public static bool IsMinion(Obj_AI_Minion minion, bool includeWards = false)
        {
            var name = minion.CharData.BaseSkinName.ToLower();
            return name.Contains("minion") || (includeWards && (name.Contains("ward") || name.Contains("trinket")));
        }

        #endregion
    }

    /// <summary>
    ///     Struct with the best farm data
    /// </summary>
    public struct FarmLocation
    {
        #region Fields

        /// <summary>
        ///     The hit collection list.
        /// </summary>
        public List<Obj_AI_Base> Hits;

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
        /// <param name="hits">
        ///     The hits.
        /// </param>
        internal FarmLocation(Vector2 position, int minionsHit, List<Obj_AI_Base> hits)
        {
            this.Position = position;
            this.MinionsHit = minionsHit;
            this.Hits = hits;
        }

        #endregion
    }
}