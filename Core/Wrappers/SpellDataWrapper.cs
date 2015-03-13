namespace LeagueSharp.CommonEx.Core.Wrappers
{
    /// <summary>
    ///     SpellData Wrapper
    /// </summary>
    public class SpellDataWrapper
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="slot">SpellSlot</param>
        public SpellDataWrapper(SpellSlot slot)
        {
            Slot = slot;
            LoadSpellData(ObjectManager.Player.Spellbook.GetSpell(slot).SData);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="base"><see cref="Obj_AI_Base"/></param>
        /// <param name="slot">SpellSlot</param>
        public SpellDataWrapper(Obj_AI_Base @base, SpellSlot slot)
        {
            Slot = slot;
            LoadSpellData(@base.Spellbook.GetSpell(slot).SData);
        }

        private void LoadSpellData(SpellData spellData)
        {
            Range = spellData.CastRange;
            Width = spellData.LineWidth.Equals(0) ? spellData.CastRadius : spellData.LineWidth;
            Speed = spellData.MissileSpeed;
            Delay = spellData.DelayTotalTimePercent;
            Name = spellData.Name;
        }

        /// <summary>
        ///     Spell Slot
        /// </summary>
        public SpellSlot Slot { get; set; }

        /// <summary>
        ///     Spell Range
        /// </summary>
        public float Range { get; set; }

        /// <summary>
        ///     Spell Width
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        ///     Spell Speed
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        ///     Spell Delay
        /// </summary>
        public float Delay { get; set; }

        /// <summary>
        ///     Spell Name
        /// </summary>
        public string Name { get; set; }
    }
}