// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoAttack.cs" company="LeagueSharp">
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
//   AutoAttack utility class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.Utils
{
    using System.Linq;

    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;

    using SharpDX;

    /// <summary>
    ///     AutoAttack utility class.
    /// </summary>
    public static class AutoAttack
    {
        #region Static Fields

        /// <summary>
        ///     Spells which reset the attack timer.
        /// </summary>
        private static readonly string[] AttackResets =
            {
                "dariusnoxiantacticsonh", "fioraflurry", "garenq", 
                "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge", 
                "leonashieldofdaybreak", "luciane", "lucianq", 
                "monkeykingdoubleattack", "mordekaisermaceofspades", "nasusq", 
                "nautiluspiercinggaze", "netherblade", "parley", 
                "poppydevastatingblow", "powerfist", "renektonpreexecute", 
                "rengarq", "shyvanadoubleattack", "sivirw", "takedown", 
                "talonnoxiandiplomacy", "trundletrollsmash", "vaynetumble", 
                "vie", "volibearq", "xenzhaocombotarget", "yorickspectral", 
                "reksaiq"
            };

        /// <summary>
        ///     Champions which can't cancel AA.
        /// </summary>
        private static readonly string[] NoCancelChamps = { "Kalista" };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns if the hero can't cancel an AA
        /// </summary>
        /// <param name="hero">The Hero (<see cref="Obj_AI_Hero" />)</param>
        /// <returns>Returns if the hero can't cancel his AA</returns>
        public static bool CanCancelAutoAttack(this Obj_AI_Hero hero)
        {
            return !NoCancelChamps.Contains(hero.ChampionName);
        }

        /// <summary>
        ///     Returns player auto-attack missile speed.
        /// </summary>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetProjectileSpeed()
        {
            return IsMelee(ObjectManager.Player) || ObjectManager.Player.ChampionName == "Azir"
                       ? float.MaxValue
                       : ObjectManager.Player.BasicAttack.MissileSpeed;
        }

        /// <summary>
        ///     Returns the auto-attack range.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetRealAutoAttackRange(this AttackableUnit target)
        {
            var result = ObjectManager.Player.AttackRange + ObjectManager.Player.BoundingRadius;
            if (target.IsValidTarget())
            {
                return result + target.BoundingRadius;
            }

            return result;
        }

        /// <summary>
        ///     Returns the time it takes to hit a target with an auto attack
        /// </summary>
        /// <param name="target"><see cref="AttackableUnit" /> target</param>
        /// <returns>The <see cref="float" /></returns>
        public static float GetTimeToHit(this AttackableUnit target)
        {
            return ObjectHandler.Player.AttackCastDelay * 1000 - 100 + Game.Ping / 2f
                   + 1000 * ObjectHandler.Player.Distance(target) / GetProjectileSpeed();
        }

        /// <summary>
        ///     Returns true if the target is in auto-attack range.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool InAutoAttackRange(this AttackableUnit target)
        {
            if (!target.IsValidTarget())
            {
                return false;
            }

            var myRange = GetRealAutoAttackRange(target);

            return
                Vector2.DistanceSquared(
                    (target is Obj_AI_Base)
                        ? ((Obj_AI_Base)target).ServerPosition.ToVector2()
                        : target.Position.ToVector2(), 
                    ObjectManager.Player.ServerPosition.ToVector2()) <= myRange * myRange;
        }

        /// <summary>
        ///     Returns if the name is an auto attack
        /// </summary>
        /// <param name="name">Name of spell</param>
        /// <returns>The <see cref="bool" /></returns>
        public static bool IsAutoAttack(string name)
        {
            return name.ToLower().Contains("attack");
        }

        /// <summary>
        ///     Returns true if the SpellName resets the attack timer.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        /// <summary>
        ///     Returns whether the object is a melee combat type or ranged.
        /// </summary>
        /// <param name="sender"><see cref="Obj_AI_Base" /> sender</param>
        /// <returns>Is object melee.</returns>
        public static bool IsMelee(this Obj_AI_Base sender)
        {
            return sender.CombatType == GameObjectCombatType.Melee;
        }

        #endregion
    }
}