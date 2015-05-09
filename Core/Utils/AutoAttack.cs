#region

using System.Linq;
using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using SharpDX;

#endregion

namespace LeagueSharp.CommonEx.Core.Utils
{
    /// <summary>
    ///     AutoAttack utility class.
    /// </summary>
    public static class AutoAttack
    {
        /// <summary>
        ///     Spells which reset the attack timer.
        /// </summary>
        private static readonly string[] AttackResets =
        {
            "dariusnoxiantacticsonh", "fioraflurry", "garenq",
            "hecarimrapidslash", "jaxempowertwo", "jaycehypercharge", "leonashieldofdaybreak", "luciane", "lucianq",
            "monkeykingdoubleattack", "mordekaisermaceofspades", "nasusq", "nautiluspiercinggaze", "netherblade",
            "parley", "poppydevastatingblow", "powerfist", "renektonpreexecute", "rengarq", "shyvanadoubleattack",
            "sivirw", "takedown", "talonnoxiandiplomacy", "trundletrollsmash", "vaynetumble", "vie", "volibearq",
            "xenzhaocombotarget", "yorickspectral", "reksaiq"
        };

        /// <summary>
        ///     Champions which can't cancel AA.
        /// </summary>
        private static readonly string[] NoCancelChamps = { "Kalista" };

        /// <summary>
        ///     Returns if the name is an auto attack
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsAutoAttack(string name)
        {
            return (name.ToLower().Contains("attack"));
        }

        /// <summary>
        ///     Returns true if the spellname resets the attack timer.
        /// </summary>
        public static bool IsAutoAttackReset(string name)
        {
            return AttackResets.Contains(name.ToLower());
        }

        /// <summary>
        ///     Returns the auto-attack range.
        /// </summary>
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
        ///     Returns the time it takes to thit a target with an auto attack
        /// </summary>
        /// <param name="target"><see cref="AttackableUnit" /> target</param>
        /// <returns></returns>
        public static float GetTimeToHit(this AttackableUnit target)
        {
            return ObjectHandler.Player.AttackCastDelay * 1000 - 100 + Game.Ping / 2f +
                   1000 * ObjectHandler.Player.Distance(target) / GetProjectileSpeed();
        }

        /// <summary>
        ///     Returns true if the target is in auto-attack range.
        /// </summary>
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
                        ? ((Obj_AI_Base) target).ServerPosition.ToVector2()
                        : target.Position.ToVector2(), ObjectManager.Player.ServerPosition.ToVector2()) <=
                myRange * myRange;
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

        /// <summary>
        ///     Returns player auto-attack missile speed.
        /// </summary>
        public static float GetProjectileSpeed()
        {
            return IsMelee(ObjectManager.Player) || ObjectManager.Player.ChampionName == "Azir"
                ? float.MaxValue
                : ObjectManager.Player.BasicAttack.MissileSpeed;
        }

        /// <summary>
        ///     Returns if the hero can't cancel an AA
        /// </summary>
        /// <param name="hero">The Hero (<see cref="Obj_AI_Hero" />)</param>
        /// <returns>Returns if the hero can't cancel his AA</returns>
        public static bool CanCancelAutoAttack(this Obj_AI_Hero hero)
        {
            return !NoCancelChamps.Contains(hero.ChampionName);
        }
    }
}