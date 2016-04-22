// <copyright file="Minion.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDK.Enumerations;

    using SharpDX;

    /// <summary>
    ///     The minion utils, contains a set of functions to quickly operate around minions.
    /// </summary>
    public static class Minion
    {
        #region Static Fields

        private static readonly List<string> CloneList = new List<string> { "leblanc", "shaco", "monkeyking" };

        /// <summary>
        ///     The normal minion list.
        /// </summary>
        private static readonly List<string> NormalMinionList = new List<string>
                                                                    {
                                                                        "SRU_ChaosMinionMelee", "SRU_ChaosMinionRanged",
                                                                        "SRU_OrderMinionMelee", "SRU_OrderMinionRanged",
                                                                        "HA_ChaosMinionMelee", "HA_ChaosMinionRanged",
                                                                        "HA_OrderMinionMelee", "HA_OrderMinionRanged"
                                                                    };

        private static readonly List<string> PetList = new List<string>
                                                           {
                                                               "annietibbers", "elisespiderling", "heimertyellow",
                                                               "heimertblue", "malzaharvoidling", "shacobox",
                                                               "yorickspectralghoul", "yorickdecayedghoul",
                                                               "yorickravenousghoul", "zyrathornplant",
                                                               "zyragraspingplant"
                                                           };

        /// <summary>
        ///     The siege minion list.
        /// </summary>
        private static readonly List<string> SiegeMinionList = new List<string>
                                                                   {
                                                                       "SRU_ChaosMinionSiege", "SRU_OrderMinionSiege",
                                                                       "HA_ChaosMinionSiege", "HA_OrderMinionSiege"
                                                                   };

        /// <summary>
        ///     The super minion list.
        /// </summary>
        private static readonly List<string> SuperMinionList = new List<string>
                                                                   {
                                                                       "SRU_ChaosMinionSuper", "SRU_OrderMinionSuper",
                                                                       "HA_ChaosMinionSuper", "HA_OrderMinionSuper"
                                                                   };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the point where, when casted, the circular spell with hit the maximum amount of minions.
        /// </summary>
        /// <param name="minionPositions">
        ///     List of minion positions
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
            List<Vector2> minionPositions,
            float width,
            float range,
            int useMecMax = 9)
        {
            var result = default(Vector2);
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
                    if (subGroup.Count > 0)
                    {
                        var circle = ConvexHull.GetMec(subGroup);

                        if (circle.Radius <= width && circle.Center.DistanceSquared(startPos) <= range)
                        {
                            minionCount = subGroup.Count;
                            return new FarmLocation(circle.Center, minionCount);
                        }
                    }
                }
            }
            else
            {
                foreach (var pos in minionPositions)
                {
                    if (pos.DistanceSquared(startPos) <= range)
                    {
                        var count = minionPositions.Count(pos2 => pos.DistanceSquared(pos2) <= width * width);

                        if (count >= minionCount)
                        {
                            result = pos;
                            minionCount = count;
                        }
                    }
                }
            }

            return new FarmLocation(result, minionCount);
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
        public static FarmLocation GetBestLineFarmLocation(List<Vector2> minions, float width, float range)
        {
            var result = default(Vector2);
            var minionCount = 0;
            var startPos = ObjectManager.Player.ServerPosition.ToVector2();

            var posiblePositions = new List<Vector2>();
            posiblePositions.AddRange(minions);

            var max = minions.Count;
            for (var i = 0; i < max; i++)
            {
                for (var j = 0; j < max; j++)
                {
                    if (minions[j] != minions[i])
                    {
                        posiblePositions.Add((minions[j] + minions[i]) / 2);
                    }
                }
            }

            foreach (var pos in posiblePositions)
            {
                if (pos.DistanceSquared(startPos) <= range * range)
                {
                    var endPos = startPos + (range * (pos - startPos).Normalized());

                    var count = minions.Count(pos2 => pos2.DistanceSquared(startPos, endPos, true) <= width * width);

                    if (count >= minionCount)
                    {
                        result = endPos;
                        minionCount = count;
                    }
                }
            }

            return new FarmLocation(result, minionCount);
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
        public static List<Vector2> GetMinionsPredictedPositions(
            List<Obj_AI_Minion> minions,
            float delay,
            float width,
            float speed,
            Vector3 from,
            float range,
            bool collision,
            SkillshotType stype,
            Vector3 rangeCheckFrom = default(Vector3))
        {
            from = from.IsValid() ? from : ObjectManager.Player.ServerPosition;

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
        ///     Gets the minion type.
        /// </summary>
        /// <param name="minion">
        ///     The minion.
        /// </param>
        /// <returns>
        ///     The <see cref="MinionTypes" />
        /// </returns>
        public static MinionTypes GetMinionType(this Obj_AI_Minion minion)
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

            if (baseSkinName.ToLower().Contains("ward") || baseSkinName.ToLower().Contains("trinket"))
            {
                return MinionTypes.Ward;
            }

            return MinionTypes.Unknown;
        }

        /// <summary>
        ///     Tells whether the <see cref="Obj_AI_Minion" /> is an actual minion.
        /// </summary>
        /// <param name="minion">The Minion</param>
        /// <returns>Whether the <see cref="Obj_AI_Minion" /> is an actual minion.</returns>
        public static bool IsMinion(this Obj_AI_Minion minion)
        {
            return minion.GetMinionType().HasFlag(MinionTypes.Melee)
                   || minion.GetMinionType().HasFlag(MinionTypes.Ranged);
        }

        /// <summary>
        ///     Tells whether the <see cref="Obj_AI_Minion" /> is an actual minion.
        /// </summary>
        /// <param name="minion">The Minion</param>
        /// <param name="includeClones">Whether to include clones.</param>
        /// <returns>Whether the <see cref="Obj_AI_Minion" /> is an actual pet.</returns>
        public static bool IsPet(this Obj_AI_Minion minion, bool includeClones = true)
        {
            var name = minion.CharData.BaseSkinName.ToLower();
            return PetList.Contains(name) || (includeClones && CloneList.Contains(name));
        }

        #endregion
    }

    /// <summary>
    ///     The farm location.
    /// </summary>
    public struct FarmLocation
    {
        #region Fields

        /// <summary>
        ///     The minions hit.
        /// </summary>
        public int MinionsHit;

        /// <summary>
        ///     The position.
        /// </summary>
        public Vector2 Position;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FarmLocation" /> struct.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <param name="minionsHit">
        ///     The minions hit.
        /// </param>
        public FarmLocation(Vector2 position, int minionsHit)
        {
            this.Position = position;
            this.MinionsHit = minionsHit;
        }

        #endregion
    }
}
