#region

#endregion

namespace LeagueSharp.CommonEx.Core.Utils
{
    using LeagueSharp.SDK.Core.Enumerations;

    /// <summary>
    ///     General Utils.
    /// </summary>
    public static class Spell
    {
        /// <summary>
        ///     Returns if the spell is ready to use.
        /// </summary>
        /// <param name="spell">Spell Data Instance</param>
        /// <param name="t">Time Left</param>
        /// <returns>Is Spell Ready to use</returns>
        public static bool IsReady(this SpellDataInst spell, int t = 0)
        {
            return (spell != null) &&
                   (spell.Slot != SpellSlot.Unknown && t == 0
                       ? spell.State == SpellState.Ready
                       : (spell.State == SpellState.Ready ||
                          (spell.State == SpellState.Cooldown && (spell.CooldownExpires - Game.Time) <= t / 1000f)));
        }

        /// <summary>
        ///     Returns if the spell is ready to use.
        /// </summary>
        /// <param name="spell">Spell</param>
        /// <param name="t">Time Left</param>
        /// <returns>Is Spell Ready to use</returns>
        public static bool IsReady(this Wrappers.Spell spell, int t = 0)
        {
            return IsReady(spell.Instance, t);
        }

        /// <summary>
        ///     Returns if the spell is ready to use.
        /// </summary>
        /// <param name="slot">SpellSlot</param>
        /// <param name="t">Time Left</param>
        /// <returns>Is Spell Ready to use</returns>
        public static bool IsReady(this SpellSlot slot, int t = 0)
        {
            var s = ObjectManager.Player.Spellbook.GetSpell(slot);
            return s != null && IsReady(s, t);
        }

        /// <summary>
        ///     Converts a spell cast state into a boolean.
        /// </summary>
        /// <param name="castState">Spell Cast State</param>
        /// <returns>Boolean</returns>
        public static bool ToBoolean(this CastStates castState)
        {
            return castState == CastStates.SuccessfullyCasted;
        }
    }
}