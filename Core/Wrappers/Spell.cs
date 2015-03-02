using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.Wrappers
{
    /// <summary>
    ///     Manages Spells.
    /// </summary>
    public class Spell
    {
        /// <summary>
        ///     Gets/Sets the Range of the spell.
        /// </summary>
        public float Range { get; set; }

        /// <summary>
        ///     Gets/Sets the Width(Radius) of the spell.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        ///     Gets/Sets the Missle Speed of the spell.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        ///     Gets/Sets the delay in seconds.
        /// </summary>
        public float Delay { get; set; }

        /// <summary>
        ///     Gets/Sets whether the spell has collision.
        /// </summary>
        public bool Collision { get; set; }

        /// <summary>
        ///     Gets/Sets the <see cref="SpellSlot"/>.
        /// </summary>
        public SpellSlot Slot { get; set; }

        /// <summary>
        ///     Gets the <see cref="SpellDataInst"/>.
        /// </summary>
        public SpellDataInst Instance
        {
            get { return ObjectManager.Player.Spellbook.GetSpell(Slot); }
        }

        /// <summary>
        ///     Creates a new <see cref="Spell"/> instance, taking the 
        /// </summary>
        /// <param name="slot">The <see cref="SpellSlot"/></param>
        /// <param name="takeFromSpellData">Get the data(speed, range, etc) from SpellData.</param>
        public Spell(SpellSlot slot, bool takeFromSpellData)
        {
            Slot = slot;

            if (!takeFromSpellData)
            {
                return;
            }

            var spellData = ObjectManager.Player.Spellbook.GetSpell(slot).SData;

            Range = spellData.CastRange;
            Width = spellData.LineWidth.Equals(0) ? spellData.CastRadius : spellData.LineWidth;
            Speed = spellData.MissileSpeed;
            Delay = spellData.DelayTotalTimePercent;
            // Unsure if this is correct.
            Collision = spellData.HaveHitBone;
        }

        /// <summary>
        ///     Creats a new spell, with the range.
        /// </summary>
        /// <param name="slot">Slot</param>
        /// <param name="range">Range of spell</param>
        public Spell(SpellSlot slot, float range = float.MaxValue)
        {
            Slot = slot;
            Range = range;
        }

        // TODO: Add SetSkillShot, etc when we get prediciton

        /// <summary>
        ///     Self-casts the spell.
        /// </summary>
        /// <returns>If the spell was casted.</returns>
        public bool Cast()
        {
            if (!IsReady())
            {
                return false;
            }

            return ObjectManager.Player.Spellbook.CastSpell(Slot, true);
        }

        /// <summary>
        ///     Casts the spell at a poisiton.
        /// </summary>
        /// <param name="position">Position</param>
        /// <returns>If the spell was casted.</returns>
        public bool Cast(Vector3 position)
        {
            if (!IsReady() || !IsInRange(position))
            {
                return false;
            }

            return ObjectManager.Player.Spellbook.CastSpell(Slot, position, true);
        }

        /// <summary>
        ///     Casts the spell at a position.
        /// </summary>
        /// <param name="position">Position</param>
        /// <returns>If the spell was casted</returns>
        public bool Cast(Vector2 position)
        {
            return Cast(position.ToVector3());
        }

        /// <summary>
        ///     Casts the spell on the <see cref="GameObject"/> with/without prediction
        /// </summary>
        /// <param name="gameObject">The object to cast to</param>
        /// <param name="castOnUnit">Will do prediction on the object if this is false.</param>
        /// <returns>If the spell was casted.</returns>
        public bool Cast(GameObject gameObject, bool castOnUnit = false)
        {
            if (!IsReady() || !IsInRange(gameObject))
            {
                return false;
            }

            if (castOnUnit)
            {
                return ObjectManager.Player.Spellbook.CastSpell(Slot, gameObject, true);
            }

            //TODO: Add Prediction
            return ObjectManager.Player.Spellbook.CastSpell(Slot, gameObject.Position, true);
        }

        // TODO: Move this out of Spell.cs
        /// <summary>
        ///     Gets whether the spell is ready to be casted.
        /// </summary>
        /// <param name="t">What cooldown the spell should be for it to be ready.</param>
        /// <returns>Whether the spell is ready to be casted.</returns>
        public bool IsReady(int t = 0)
        {
            var spell = Instance;
            return spell != null &&
                   (spell.Slot != SpellSlot.Unknown && t == 0
                       ? spell.State == SpellState.Ready
                       : (spell.State == SpellState.Ready ||
                          (spell.State == SpellState.Cooldown && (spell.CooldownExpires - Game.Time) <= t / 1000f)));
        }

        /// <summary>
        ///    Checks if the <see cref="GameObject"/> is in range of the spell.
        /// </summary>
        /// <param name="gameObject"><see cref="GameObject"/></param>
        /// <returns>If the <see cref="GameObject"/> is in range of the spell.</returns>
        public bool IsInRange(GameObject gameObject)
        {
            return ObjectManager.Player.Distance(gameObject) < Range;
        }

        /// <summary>
        ///    Checks if the <see cref="Vector3"/> is in range of the spell.
        /// </summary>
        /// <param name="position">Position></param>
        /// <returns>If the <see cref="Vector3"/> is in range of the spell.</returns>
        public bool IsInRange(Vector3 position)
        {
            return ObjectManager.Player.Distance(position) < Range;
        }

        /// <summary>
        ///    Checks if the <see cref="Vector2"/> is in range of the spell.
        /// </summary>
        /// <param name="position">Position></param>
        /// <returns>If the <see cref="Vector2"/> is in range of the spell.</returns>
        public bool IsInRange(Vector2 position)
        {
            return ObjectManager.Player.Distance(position) < Range;
        }
    }
}