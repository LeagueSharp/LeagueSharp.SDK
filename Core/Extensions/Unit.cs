// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Unit.cs" company="LeagueSharp">
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
//   Provides helpful extensions to Units.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Extensions
{
    using System;
    using System.Linq;

    using global::SharpDX;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Wrappers;

    /// <summary>
    ///     Provides helpful extensions to Units.
    /// </summary>
    public static class Unit
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the distance between two GameObjects
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="target">The Target</param>
        /// <returns>The distance between the two objects</returns>
        public static float Distance(this GameObject source, GameObject target)
        {
            return source.Position.Distance(target.Position);
        }

        /// <summary>
        ///     Gets the distance between a <see cref="GameObject" /> and a <see cref="Vector3" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance between a <see cref="GameObject" /> and a <see cref="Vector3" /></returns>
        public static float Distance(this GameObject source, Vector3 position)
        {
            return source.Position.Distance(position);
        }

        /// <summary>
        ///     Gets the distance between a <see cref="GameObject" /> and a <see cref="Vector2" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance between a <see cref="GameObject" /> and a <see cref="Vector2" /></returns>
        public static float Distance(this GameObject source, Vector2 position)
        {
            return source.Position.Distance(position);
        }

        /// <summary>
        ///     Gets the distance between two <c>Obj_AI_Base</c>s using ServerPosition
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="target">The Target</param>
        /// <returns>The Distance</returns>
        public static float Distance(this Obj_AI_Base source, Obj_AI_Base target)
        {
            return source.ServerPosition.Distance(target.ServerPosition);
        }

        /// <summary>
        ///     Gets the distance between a <see cref="Obj_AI_Base" /> and a <see cref="Vector3" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance between a <see cref="Obj_AI_Base" /> and a <see cref="Vector3" /></returns>
        public static float Distance(this Obj_AI_Base source, Vector3 position)
        {
            return source.ServerPosition.Distance(position);
        }

        /// <summary>
        ///     Gets the distance between a <see cref="Obj_AI_Base" /> and a <see cref="Vector2" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance between a <see cref="Obj_AI_Base" /> and a <see cref="Vector2" /></returns>
        public static float Distance(this Obj_AI_Base source, Vector2 position)
        {
            return source.ServerPosition.Distance(position);
        }

        /// <summary>
        ///     Gets the distance squared between two GameObjects
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="target">The Target</param>
        /// <returns>The squared distance between the two objects</returns>
        public static float DistanceSquared(this GameObject source, GameObject target)
        {
            return source.Position.DistanceSquared(target.Position);
        }

        /// <summary>
        ///     Gets the distance squared between a <see cref="GameObject" /> and a <see cref="Vector3" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance squared between a <see cref="GameObject" /> and a <see cref="Vector3" /></returns>
        public static float DistanceSquared(this GameObject source, Vector3 position)
        {
            return source.Position.DistanceSquared(position);
        }

        /// <summary>
        ///     Gets the distance squared between a <see cref="GameObject" /> and a <see cref="Vector2" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance squared between a <see cref="GameObject" /> and a <see cref="Vector2" /></returns>
        public static float DistanceSquared(this GameObject source, Vector2 position)
        {
            return source.Position.DistanceSquared(position);
        }

        /// <summary>
        ///     Gets the distance squared between a <see cref="Obj_AI_Base" /> and a <see cref="Vector3" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance squared between a <see cref="Obj_AI_Base" /> and a <see cref="Vector3" /></returns>
        public static float DistanceSquared(this Obj_AI_Base source, Vector3 position)
        {
            return source.ServerPosition.DistanceSquared(position);
        }

        /// <summary>
        ///     Gets the distance squared between a <see cref="Obj_AI_Base" /> and a <see cref="Vector2" />
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="position">The Position</param>
        /// <returns>The distance squared between a <see cref="Obj_AI_Base" /> and a <see cref="Vector2" /></returns>
        public static float DistanceSquared(this Obj_AI_Base source, Vector2 position)
        {
            return source.ServerPosition.DistanceSquared(position);
        }

        /// <summary>
        ///     Gets the distance squared between two <c>Obj_AI_Base</c>s using ServerPosition
        /// </summary>
        /// <param name="source">The Source</param>
        /// <param name="target">The Target</param>
        /// <returns>Distance Squared</returns>
        public static float DistanceSquared(this Obj_AI_Base source, Obj_AI_Base target)
        {
            return source.ServerPosition.DistanceSquared(target.ServerPosition);
        }

        /// <summary>
        ///     Returns the recall time duration for a specified <see cref="Obj_AI_Hero" />
        /// </summary>
        /// <param name="hero">The Hero</param>
        /// <returns>Recall Time Duration</returns>
        public static int GetRecallTime(this Obj_AI_Hero hero)
        {
            return GetRecallTime(hero.Spellbook.GetSpell(SpellSlot.Recall).Name);
        }

        /// <summary>
        ///     Returns the recall time duration for a specific recall type by name.
        /// </summary>
        /// <param name="recallName">Recall type name</param>
        /// <returns>Recall Time Duration</returns>
        public static int GetRecallTime(string recallName)
        {
            switch (recallName.ToLower())
            {
                case "recall":
                    return 8000;
                case "recallimproved":
                    return 7000;
                case "odinrecall":
                    return 4500;
                case "odinrecallimproved":
                case "superrecall":
                case "superrecallimproved":
                    return 4000;
                default:
                    return 0;
            }
        }

        /// <summary>
        ///     Returns the spell slot with the name.
        /// </summary>
        /// <param name="unit">
        ///     The Unit
        /// </param>
        /// <param name="name">
        ///     Spell Name
        /// </param>
        /// <returns>
        ///     The <see cref="SpellSlot" />.
        /// </returns>
        public static SpellSlot GetSpellSlot(this Obj_AI_Hero unit, string name)
        {
            foreach (var spell in
                unit.Spellbook.Spells.Where(
                    spell => string.Equals(spell.Name, name, StringComparison.CurrentCultureIgnoreCase)))
            {
                return spell.Slot;
            }

            return SpellSlot.Unknown;
        }

        /// <summary>
        ///     Gets the turret tier type.
        /// </summary>
        /// <param name="turret">
        ///     The turret.
        /// </param>
        /// <returns>
        ///     The <see cref="TurretType" />.
        /// </returns>
        public static TurretType GetTurretType(this Obj_AI_Base turret)
        {
            switch (turret.CharData.BaseSkinName)
            {
                case "SRUAP_Turret_Order1":
                case "SRUAP_Turret_Chaos1":
                    return TurretType.TierOne;

                case "SRUAP_Turret_Order2":
                case "SRUAP_Turret_Chaos2":
                    return TurretType.TierTwo;

                case "SRUAP_Turret_Order3_Test":
                case "SRUAP_Turret_Chaos3_Test":
                    return TurretType.TierThree;

                case "SRUAP_Turret_Order4":
                case "SRUAP_Turret_Chaos4":
                    return TurretType.TierFour;

                default:
                    return TurretType.Unknown;
            }
        }

        /// <summary>
        ///     Returns whether the hero is in fountain range.
        /// </summary>
        /// <param name="hero">The Hero</param>
        /// <returns>Is Hero in fountain range</returns>
        public static bool InFountain(this Obj_AI_Hero hero)
        {
            float fountainRange = 562500; // 750 * 750
            var map = Map.GetMap();
            if (map != null && map.Type == MapType.SummonersRift)
            {
                fountainRange = 1102500; // 1050 * 1050
            }

            return hero.IsVisible
                   && GameObjects.AllySpawnPoints.Any(sp => hero.DistanceSquared(sp.Position) < fountainRange);
        }

        /// <summary>
        ///     Returns whether the hero is in shop range.
        /// </summary>
        /// <param name="hero">The Hero</param>
        /// <returns>Is Hero in shop range</returns>
        public static bool InShop(this Obj_AI_Hero hero)
        {
            return hero.IsVisible && GameObjects.AllyShops.Any(s => hero.DistanceSquared(s.Position) < 1562500);
        }

        /// <summary>
        ///     Calculates if the source and the target are facing each-other.
        /// </summary>
        /// <param name="source">Extended source</param>
        /// <param name="target">The Target</param>
        /// <returns>Returns if the source and target are facing each-other (boolean)</returns>
        public static bool IsBothFacing(this Obj_AI_Base source, Obj_AI_Base target)
        {
            return source.IsFacing(target) && target.IsFacing(source);
        }

        /// <summary>
        ///     Calculates if the source is facing the target.
        /// </summary>
        /// <param name="source">Extended source</param>
        /// <param name="target">The Target</param>
        /// <returns>Returns if the source is facing the target (boolean)</returns>
        public static bool IsFacing(this Obj_AI_Base source, Obj_AI_Base target)
        {
            return (source.IsValid() && target.IsValid())
                   && source.Direction.AngleBetween(target.Position - source.Position) < 90;
        }

        /// <summary>
        ///     Returns if the unit is recalling.
        /// </summary>
        /// <param name="unit">Extended unit</param>
        /// <returns>Returns if the unit is recalling (boolean)</returns>
        public static bool IsRecalling(this Obj_AI_Hero unit)
        {
            return unit.Buffs.Any(buff => buff.Name.ToLower().Contains("recall") && buff.Type == BuffType.Aura);
        }

        /// <summary>
        ///     Returns whether the specific unit is under an enemy turret.
        /// </summary>
        /// <param name="unit"><see cref="Obj_AI_Base" /> unit</param>
        /// <returns>Is Unit under an Enemy Turret</returns>
        public static bool IsUnderTurret(this Obj_AI_Base unit)
        {
            return unit.Position.IsUnderTurret(true);
        }

        /// <summary>
        ///     Returns whether the specific unit is under a turret.
        /// </summary>
        /// <param name="unit"><see cref="Obj_AI_Base" /> unit</param>
        /// <param name="enemyTurretsOnly">Include Enemy Turrets Only</param>
        /// <returns>Is Unit under a Turret</returns>
        public static bool IsUnderTurret(this Obj_AI_Base unit, bool enemyTurretsOnly)
        {
            return unit.Position.IsUnderTurret(enemyTurretsOnly);
        }

        /// <summary>
        ///     Checks if the Unit is valid.
        /// </summary>
        /// <param name="unit">
        ///     Unit from <c>Obj_AI_Base</c> type
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsValid(this Obj_AI_Base unit)
        {
            return unit != null && unit.IsValid;
        }

        /// <summary>
        ///     Checks if the target unit is valid.
        /// </summary>
        /// <param name="unit">
        ///     The Unit
        /// </param>
        /// <param name="range">
        ///     The Range
        /// </param>
        /// <param name="checkTeam">
        ///     Checks if the target is an enemy from the Player's side
        /// </param>
        /// <param name="from">
        ///     Check From
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsValidTarget(
            this AttackableUnit unit, 
            float range = float.MaxValue, 
            bool checkTeam = true, 
            Vector3 from = new Vector3())
        {
            if (unit == null || !unit.IsValid || unit.IsDead || !unit.IsVisible || !unit.IsTargetable
                || unit.IsInvulnerable)
            {
                return false;
            }

            if (checkTeam && GameObjects.Player.Team == unit.Team)
            {
                return false;
            }

            var @base = unit as Obj_AI_Base;
            var unitPosition = @base != null ? @base.ServerPosition : unit.Position;

            return @from.IsValid()
                       ? @from.DistanceSquared(unitPosition) < range * range
                       : GameObjects.Player.ServerPosition.DistanceSquared(unitPosition) < range * range;
        }

        #endregion
    }
}