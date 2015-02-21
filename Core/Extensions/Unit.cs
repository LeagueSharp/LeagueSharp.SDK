#region

using System.Linq;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using Vector3 = SharpDX.Vector3;

#endregion

namespace LeagueSharp.CommonEx.Core.Extensions
{
    /// <summary>
    ///     Provides helpful extensions to Units.
    /// </summary>
    public static class Unit
    {
        #region IsValid

        /// <summary>
        ///     Checks if the Unit is valid.
        /// </summary>
        /// <param name="unit">Unit (Obj_AI_Base)</param>
        /// <returns>Boolean</returns>
        public static bool IsValid(this Obj_AI_Base unit)
        {
            return unit != null && unit.IsValid;
        }

        /// <summary>
        ///     Checks if the target unit is valid.
        /// </summary>
        /// <param name="unit">Unit</param>
        /// <param name="range">Range</param>
        /// <param name="checkTeam">Checks if the target is an enemy from the Player's side</param>
        /// <param name="from">Check From</param>
        /// <returns>Boolean</returns>
        public static bool IsValidTarget(this AttackableUnit unit,
            float range = float.MaxValue,
            bool checkTeam = true,
            Vector3 from = new Vector3())
        {
            if (unit == null || !unit.IsValid || unit.IsDead || !unit.IsVisible || !unit.IsTargetable ||
                unit.IsInvulnerable)
            {
                return false;
            }

            if (checkTeam && ObjectManager.Player.Team == unit.Team)
            {
                return false;
            }

            var @base = unit as Obj_AI_Base;
            var unitPosition = @base != null ? @base.ServerPosition : unit.Position;

            return (@from.IsValid())
                ? @from.DistanceSquared(unitPosition) < range * range
                : ObjectManager.Player.ServerPosition.DistanceSquared(unitPosition) < range * range;
        }

        #endregion

        #region Information

        /// <summary>
        ///     Returns the unit's health percentage (From 0 to 100).
        /// </summary>
        /// <param name="unit">Extended unit</param>
        /// <returns>Returns the unit's health percentage in float-units</returns>
        public static float HealthPercentage(this Obj_AI_Base unit)
        {
            return unit.Health / unit.MaxHealth * 100;
        }

        /// <summary>
        ///     Returns the unit's mana percentage (From 0 to 100).
        /// </summary>
        /// <param name="unit">Extended unit</param>
        /// <returns>Returns the unit's mana percentage in float-units</returns>
        public static float ManaPercentage(this Obj_AI_Base unit)
        {
            return unit.Mana / unit.MaxMana * 100;
        }

        /// <summary>
        ///     Returns the unit's total magic damage.
        /// </summary>
        /// <param name="unit">Extended unit</param>
        /// <returns>Returns the unit's total magic damage in float units</returns>
        public static float TotalMagicalDamage(this Obj_AI_Hero unit)
        {
            return unit.BaseAbilityDamage + unit.FlatMagicDamageMod;
        }

        /// <summary>
        ///     Returns the unit's total attack damage.
        /// </summary>
        /// <param name="unit">Extended unit</param>
        /// <returns>Returns the unit's total attack damage in float units</returns>
        public static float TotalAttackDamage(this Obj_AI_Hero unit)
        {
            return unit.BaseAttackDamage + unit.FlatPhysicalDamageMod;
        }

        /// <summary>
        ///     Returns the unit's total attack range.
        /// </summary>
        /// <param name="unit">Extended unit</param>
        /// <returns>Returns the unit's total attack range in float units</returns>
        public static float TotalAttackRange(this Obj_AI_Hero unit)
        {
            return unit.AttackRange + unit.BoundingRadius;
        }

        /// <summary>
        ///     Returns if the unit is recalling.
        /// </summary>
        /// <param name="unit">Extended unit</param>
        /// <returns>Returns if the unit is recalling (boolean)</returns>
        public static bool IsRecalling(this Obj_AI_Hero unit)
        {
            return unit.Buffs.Any(buff => buff.Name.ToLower().Contains("recall"));
        }

        #endregion

        #region IsFacing

        /// <summary>
        ///     Calculates if the source is facing the target.
        /// </summary>
        /// <param name="source">Extended source</param>
        /// <param name="target">Target</param>
        /// <returns>Returns if the source is facing the target (boolean)</returns>
        public static bool IsFacing(this Obj_AI_Base source, Obj_AI_Base target)
        {
            return (source.IsValid() && target.IsValid()) &&
                   source.Direction.AngleBetween(target.Position - source.Position) < 90;
        }

        /// <summary>
        ///     Calculates if the source and the target are facing each-other.
        /// </summary>
        /// <param name="source">Extended source</param>
        /// <param name="target">Target</param>
        /// <returns>Returns if the source and target are facing each-other (boolean)</returns>
        public static bool IsBothFacing(this Obj_AI_Base source, Obj_AI_Base target)
        {
            return source.IsFacing(target) && target.IsFacing(source);
        }

        #endregion
    }
}