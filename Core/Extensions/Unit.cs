#region

using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using Vector2 = SharpDX.Vector2;
using Vector3 = SharpDX.Vector3;

#endregion

namespace LeagueSharp.CommonEx.Core.Extensions
{
    /// <summary>
    ///     Provides helpful extensions to Units
    /// </summary>
    public static class Unit
    {
        #region IsValid

        /// <summary>
        ///     Checks if the Unit is valid.
        /// </summary>
        /// <param name="base">Unit (Obj_AI_Base)</param>
        /// <returns>Boolean</returns>
        public static bool IsValid(this Obj_AI_Base @base)
        {
            return @base != null && @base.IsValid;
        }

        /// <summary>
        ///     Checks if the target unit is valid
        /// </summary>
        /// <param name="unit">Unit</param>
        /// <param name="range">Range</param>
        /// <param name="from">Check From</param>
        /// <param name="checkTeam">Checks if the target is an enemy from the Player's side</param>
        /// <returns>Boolean</returns>
        public static bool IsValidTarget(this AttackableUnit unit,
            float range = float.MaxValue,
            Vector3 from = new Vector3(),
            bool checkTeam = true)
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

            return !(range < float.MaxValue) ||
                   !(Vector2.DistanceSquared(
                       (@from.ToVector2().IsValid() ? @from : ObjectManager.Player.ServerPosition).ToVector2(),
                       unitPosition.ToVector2()) > range * range);
        }

        #endregion
    }
}