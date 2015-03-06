using System;
using System.Drawing;
using System.Linq;

namespace LeagueSharp.CommonEx.Core.Wrappers
{
    /// <summary>
    ///     Utility which displays the damage done to a unit with a Line + Damage.
    /// </summary>
    public static class HpBarDamageIndicator
    {
        /// <summary>
        ///     Delegate for <see cref="DamageToUnit" />
        /// </summary>
        /// <param name="hero">The hero we are drawing on.</param>
        /// <returns>Amount of damage done.</returns>
        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);

        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;

        /// <summary>
        ///     Gets/Sets whether the HpBarDamageIndicator is enabled.
        /// </summary>
        public static bool Enabled = true;

        private static DamageToUnitDelegate _damageToUnit;

        /// <summary>
        ///     Color of the line drawing on the HP Bar.
        /// </summary>
        public static Color Color = Color.Lime;

        /// <summary>
        ///     Gets the damage to the unit.
        /// </summary>
        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += Drawing_OnDraw;
                }
                _damageToUnit = value;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled || _damageToUnit == null)
            {
                return;
            }

            foreach (var unit in
                ObjectHandler.EnemyHeroes.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = System.Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var xPos = barPos.X + XOffset + Width * percentHealthAfterDamage;

                Drawing.DrawText(
                    barPos.X + XOffset, barPos.Y + YOffset - 13, Color.Red, ((int) (unit.Health - damage)).ToString());
                Drawing.DrawLine(xPos, barPos.Y + YOffset, xPos, barPos.Y + YOffset + Height, 2, Color);
            }
        }
    }
}