// <copyright file="AutoAttack.cs" company="LeagueSharp">
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
                "powerfist", "dariusnoxiantacticsonh", "masochism", "fiorae",
                "gangplankqwrapper", "garenq", "gravesmove", "hecarimramp",
                "illaoiw", "jaxempowertwo", "jaycehypercharge", "netherblade",
                "leonashieldofdaybreak", "luciane", "meditate",
                "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze",
                "takedown", "reksaiq", "renektonpreexecute", "rengarq",
                "riventricleave", "sejuaninorthernwinds",
                "shyvanadoubleattack", "shyvanadoubleattackdragon", "sivirw",
                "talonnoxiandiplomacy", "blindingdart", "trundletrollsmash",
                "vaynetumble", "vie", "volibearq", "monkeykingdoubleattack",
                "xenzhaocombotarget", "yorickspectral",
                "itemtitanichydracleave"
            };

        /// <summary>
        ///     Spells that are attacks even if they don't have the "attack" word in their name.
        /// </summary>
        private static readonly string[] Attacks =
            {
                "caitlynheadshotmissile", "kennenmegaproc", "masteryidoublestrike",
                "quinnwenhanced", "renektonexecute", "renektonsuperexecute",
                /*"rengarnewpassivebuffdash",*/ "trundleq", "viktorqbuff",
                "xenzhaothrust", "xenzhaothrust2", "xenzhaothrust3"
            };

        /// <summary>
        ///     Spells that are not attacks even if they have the "attack" word in their name.
        /// </summary>
        private static readonly string[] NoAttacks =
            {
                "asheqattacknoonhit", "volleyattackwithsound", "volleyattack",
                "annietibbersbasicattack", "annietibbersbasicattack2",
                "azirsoldierbasicattack", "azirsundiscbasicattack",
                "elisespiderlingbasicattack", "heimertyellowbasicattack",
                "heimertyellowbasicattack2", "heimertbluebasicattack",
                "jarvanivcataclysmattack", "kindredwolfbasicattack",
                "malzaharvoidlingbasicattack", "malzaharvoidlingbasicattack2",
                "malzaharvoidlingbasicattack3", "shyvanadoubleattack",
                "shyvanadoubleattackdragon", "sivirwattackbounce",
                "monkeykingdoubleattack", "yorickspectralghoulbasicattack",
                "yorickdecayedghoulbasicattack", "yorickravenousghoulbasicattack",
                "zyragraspingplantattack", "zyragraspingplantattack2",
                "zyragraspingplantattackfire", "zyragraspingplantattack2fire"
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
        /// <param name="hero">
        ///     The hero.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetProjectileSpeed(this Obj_AI_Hero hero)
        {
            return hero.IsMelee || hero.ChampionName.Equals("Azir") || hero.ChampionName.Equals("Velkoz")
                   || hero.ChampionName.Equals("Viktor") && hero.HasBuff("ViktorPowerTransferReturn")
                       ? float.MaxValue
                       : hero.BasicAttack.MissileSpeed;
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
            return GetRealAutoAttackRange(GameObjects.Player, target.Compare(GameObjects.Player) ? null : target);
        }

        /// <summary>
        ///     Returns the auto-attack range.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float GetRealAutoAttackRange(this Obj_AI_Hero sender, AttackableUnit target)
        {
            var result = sender != null && sender.IsValid ? sender.AttackRange + sender.BoundingRadius : 0f;
            if (target != null && target.IsValid)
            {
                var aiBase = target as Obj_AI_Base;
                if (aiBase != null && GameObjects.Player.ChampionName.Equals("Caitlyn"))
                {
                    if (aiBase.HasBuff("caitlynyordletrapinternal"))
                    {
                        result += 650;
                    }
                }
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
            return GameObjects.Player.AttackCastDelay * 1000 - 100 + Game.Ping / 2f
                   + 1000 * GameObjects.Player.Distance(target) / GameObjects.Player.GetProjectileSpeed();
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
                    (target as Obj_AI_Base)?.ServerPosition.ToVector2() ?? target.Position.ToVector2(),
                    GameObjects.Player.ServerPosition.ToVector2()) <= myRange * myRange;
        }

        /// <summary>
        ///     Returns if the name is an auto attack
        /// </summary>
        /// <param name="name">Name of spell</param>
        /// <returns>The <see cref="bool" /></returns>
        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack") && !NoAttacks.Contains(name.ToLower()))
                   || Attacks.Contains(name.ToLower());
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

        #endregion
    }
}