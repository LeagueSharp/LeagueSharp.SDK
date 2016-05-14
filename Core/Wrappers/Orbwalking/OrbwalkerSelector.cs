// <copyright file="OrbwalkerSelector.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDKEx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.SDKEx.Enumerations;
    using LeagueSharp.SDKEx.UI;
    using LeagueSharp.SDKEx.Utils;

    /// <summary>
    ///     The target selecting system for <c>Orbwalker</c>.
    /// </summary>
    internal class OrbwalkerSelector
    {
        #region Constants

        /// <summary>
        ///     The lane clear wait time.
        /// </summary>
        private const float LaneClearWaitTime = 2f;

        #endregion

        #region Fields

        /// <summary>
        ///     The clones
        /// </summary>
        private readonly string[] clones = { "shaco", "monkeyking", "leblanc" };

        /// <summary>
        ///     The ignored minions
        /// </summary>
        private readonly string[] ignoreMinions = { "jarvanivstandard" };

        /// <summary>
        ///     The <see cref="Orbwalker" /> class.
        /// </summary>
        private readonly Orbwalker orbwalker;

        /// <summary>
        ///     The special minions
        /// </summary>
        private readonly string[] specialMinions =
            {
                "zyrathornplant", "zyragraspingplant", "heimertyellow",
                "heimertblue", "malzaharvoidling", "yorickdecayedghoul",
                "yorickravenousghoul", "yorickspectralghoul", "shacobox",
                "annietibbers", "teemomushroom", "elisespiderling"
            };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrbwalkerSelector" /> class.
        /// </summary>
        /// <param name="orbwalker">
        ///     The orbwalker.
        /// </param>
        public OrbwalkerSelector(Orbwalker orbwalker)
        {
            this.orbwalker = orbwalker;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the forced target.
        /// </summary>
        public AttackableUnit ForceTarget { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the farm delay
        /// </summary>
        private int FarmDelay => this.orbwalker.Menu["advanced"]["delayFarm"].GetValue<MenuSlider>().Value;

        /// <summary>
        ///     Gets or sets the last minion used for lane clear.
        /// </summary>
        private Obj_AI_Base LaneClearMinion { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the enemy minions.
        /// </summary>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <returns>
        ///     The <see cref="List{T}" /> of <see cref="Obj_AI_Minion" />.
        /// </returns>
        public List<Obj_AI_Minion> GetEnemyMinions(float range = 0)
        {
            return
                GameObjects.EnemyMinions.Where(
                    m => IsValidUnit(m, range) && !this.ignoreMinions.Any(b => b.Equals(m.CharData.BaseSkinName)))
                    .ToList();
        }

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <param name="mode">
        ///     The mode.
        /// </param>
        /// <returns>
        ///     Returns the filtered target.
        /// </returns>
        public AttackableUnit GetTarget(OrbwalkingMode mode)
        {
            if ((mode == OrbwalkingMode.Hybrid || mode == OrbwalkingMode.LaneClear)
                && !this.orbwalker.Menu["advanced"]["prioritizeFarm"].GetValue<MenuBool>().Value)
            {
                var target = Variables.TargetSelector.GetTarget(-1f, DamageType.Physical);
                if (target != null && target.InAutoAttackRange())
                {
                    return target;
                }
            }

            var minions = new List<Obj_AI_Minion>();
            if (mode != OrbwalkingMode.None)
            {
                minions = this.GetMinions(mode);
            }

            // Killable Minion
            if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Hybrid || mode == OrbwalkingMode.LastHit)
            {
                foreach (var minion in minions.OrderBy(m => m.Health))
                {
                    if (minion.IsHPBarRendered && minion.Health < GameObjects.Player.GetAutoAttackDamage(minion))
                    {
                        return minion;
                    }
                    if (minion.MaxHealth <= 10)
                    {
                        if (minion.Health <= 1)
                        {
                            return minion;
                        }
                    }
                    else
                    {
                        var predHealth = Health.GetPrediction(minion, (int)minion.GetTimeToHit(), this.FarmDelay);
                        if (predHealth <= 0)
                        {
                            this.orbwalker.InvokeAction(
                                new OrbwalkingActionArgs
                                    {
                                        Position = minion.Position, Target = minion, Process = true,
                                        Type = OrbwalkingType.NonKillableMinion
                                    });
                        }

                        if (predHealth > 0 && predHealth < GameObjects.Player.GetAutoAttackDamage(minion))
                        {
                            return minion;
                        }
                    }
                }
            }

            // Forced Target
            if (this.ForceTarget.IsValidTarget() && this.ForceTarget.InAutoAttackRange())
            {
                return this.ForceTarget;
            }

            // Turrets | Inhibitors | Nexus
            if (mode == OrbwalkingMode.LaneClear
                && (!this.orbwalker.Menu["advanced"]["prioritizeMinions"].GetValue<MenuBool>().Value || !minions.Any()))
            {
                foreach (var turret in GameObjects.EnemyTurrets.Where(t => t.IsValidTarget() && t.InAutoAttackRange()))
                {
                    return turret;
                }

                foreach (var inhib in
                    GameObjects.EnemyInhibitors.Where(i => i.IsValidTarget() && i.InAutoAttackRange()))
                {
                    return inhib;
                }

                if (GameObjects.EnemyNexus != null && GameObjects.EnemyNexus.IsValidTarget()
                    && GameObjects.EnemyNexus.InAutoAttackRange())
                {
                    return GameObjects.EnemyNexus;
                }
            }

            // Champions
            if (mode != OrbwalkingMode.LastHit)
            {
                var target = Variables.TargetSelector.GetTarget(-1f, DamageType.Physical);
                if (target.IsValidTarget() && target.InAutoAttackRange())
                {
                    return target;
                }
            }

            // Jungle Minions
            if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Hybrid)
            {
                var minion = minions.FirstOrDefault(m => m.Team == GameObjectTeam.Neutral);
                if (minion != null)
                {
                    return minion;
                }
            }

            // Under-Turret Farming
            if (mode == OrbwalkingMode.LaneClear || mode == OrbwalkingMode.Hybrid || mode == OrbwalkingMode.LastHit)
            {
                Obj_AI_Minion farmUnderTurretMinion = null;
                Obj_AI_Minion noneKillableMinion = null;

                // return all the minions under turret
                var turretMinions = minions.Where(m => m.IsMinion() && m.Position.IsUnderAllyTurret()).ToList();
                if (turretMinions.Any())
                {
                    // get the turret aggro minion
                    var turretMinion = turretMinions.FirstOrDefault(Health.HasTurretAggro);
                    if (turretMinion != null)
                    {
                        var hpLeftBeforeDie = 0;
                        var hpLeft = 0;
                        var turretAttackCount = 0;
                        var turret = Health.GetAggroTurret(turretMinion);
                        if (turret != null)
                        {
                            var turretStarTick = Health.TurretAggroStartTick(turretMinion);

                            // from healthprediction (blame Lizzaran)
                            var turretLandTick = turretStarTick + (int)(turret.AttackCastDelay * 1000)
                                                 + (1000
                                                    * Math.Max(
                                                        0,
                                                        (int)(turretMinion.Distance(turret) - turret.BoundingRadius))
                                                    / (int)(turret.BasicAttack.MissileSpeed + 70));

                            // calculate the HP before try to balance it
                            for (float i = turretLandTick + 50;
                                 i < turretLandTick + (3 * turret.AttackDelay * 1000) + 50;
                                 i = i + (turret.AttackDelay * 1000))
                            {
                                var time = (int)i - Variables.TickCount + (Game.Ping / 2);
                                var predHp =
                                    (int)
                                    Health.GetPrediction(
                                        turretMinion,
                                        time > 0 ? time : 0,
                                        70,
                                        HealthPredictionType.Simulated);
                                if (predHp > 0)
                                {
                                    hpLeft = predHp;
                                    turretAttackCount += 1;
                                    continue;
                                }

                                hpLeftBeforeDie = hpLeft;
                                hpLeft = 0;
                                break;
                            }

                            // calculate the hits is needed and possibilty to balance
                            if (hpLeft == 0 && turretAttackCount != 0 && hpLeftBeforeDie != 0)
                            {
                                var damage = (int)GameObjects.Player.GetAutoAttackDamage(turretMinion);
                                var hits = hpLeftBeforeDie / damage;
                                var timeBeforeDie = turretLandTick
                                                    + ((turretAttackCount + 1) * (int)(turret.AttackDelay * 1000))
                                                    - Variables.TickCount;
                                var timeUntilAttackReady = this.orbwalker.LastAutoAttackTick
                                                           + (int)(GameObjects.Player.AttackDelay * 1000)
                                                           > (Variables.TickCount + (Game.Ping / 2) + 25)
                                                               ? this.orbwalker.LastAutoAttackTick
                                                                 + (int)(GameObjects.Player.AttackDelay * 1000)
                                                                 - (Variables.TickCount + (Game.Ping / 2) + 25)
                                                               : 0;
                                var timeToLandAttack = turretMinion.GetTimeToHit();
                                if (hits >= 1
                                    && (hits * GameObjects.Player.AttackDelay * 1000) + timeUntilAttackReady
                                    + timeToLandAttack < timeBeforeDie)
                                {
                                    farmUnderTurretMinion = turretMinion;
                                }
                                else if (hits >= 1
                                         && (hits * GameObjects.Player.AttackDelay * 1000) + timeUntilAttackReady
                                         + timeToLandAttack > timeBeforeDie)
                                {
                                    noneKillableMinion = turretMinion;
                                }
                            }
                            else if (hpLeft == 0 && turretAttackCount == 0 && hpLeftBeforeDie == 0)
                            {
                                noneKillableMinion = turretMinion;
                            }

                            // should wait before attacking a minion.
                            if (this.ShouldWaitUnderTurret(noneKillableMinion))
                            {
                                return null;
                            }

                            if (farmUnderTurretMinion != null)
                            {
                                return farmUnderTurretMinion;
                            }

                            // balance other minions
                            return
                                (from minion in
                                     turretMinions.Where(
                                         x => x.NetworkId != turretMinion.NetworkId && !Health.HasMinionAggro(x))
                                 where
                                     (int)minion.Health % (int)turret.GetAutoAttackDamage(minion)
                                     > (int)GameObjects.Player.GetAutoAttackDamage(minion)
                                 select minion).FirstOrDefault();
                        }
                    }
                    else
                    {
                        if (this.ShouldWaitUnderTurret())
                        {
                            return null;
                        }

                        // balance other minions
                        return (from minion in turretMinions.Where(x => !Health.HasMinionAggro(x))
                                let turret =
                                    GameObjects.AllyTurrets.FirstOrDefault(
                                        x => x.IsValidTarget(950f, false, minion.Position))
                                where
                                    turret != null
                                    && (int)minion.Health % (int)turret.GetAutoAttackDamage(minion)
                                    > (int)GameObjects.Player.GetAutoAttackDamage(minion)
                                select minion).FirstOrDefault();
                    }

                    return null;
                }
            }

            // Lane Clear Minions
            if (mode == OrbwalkingMode.LaneClear)
            {
                if (!this.ShouldWait())
                {
                    if (this.LaneClearMinion.IsValidTarget() && this.LaneClearMinion.InAutoAttackRange())
                    {
                        if (this.LaneClearMinion.MaxHealth <= 10)
                        {
                            return this.LaneClearMinion;
                        }

                        var predHealth = Health.GetPrediction(
                            this.LaneClearMinion,
                            (int)(GameObjects.Player.AttackDelay * 1000 * LaneClearWaitTime),
                            this.FarmDelay,
                            HealthPredictionType.Simulated);
                        if (predHealth >= 2 * GameObjects.Player.GetAutoAttackDamage(this.LaneClearMinion)
                            || Math.Abs(predHealth - this.LaneClearMinion.Health) < float.Epsilon)
                        {
                            return this.LaneClearMinion;
                        }
                    }

                    foreach (var minion in minions.Where(m => m.Team != GameObjectTeam.Neutral))
                    {
                        if (minion.MaxHealth <= 10)
                        {
                            this.LaneClearMinion = minion;
                            return minion;
                        }

                        var predHealth = Health.GetPrediction(
                            minion,
                            (int)(GameObjects.Player.AttackDelay * 1000 * LaneClearWaitTime),
                            this.FarmDelay,
                            HealthPredictionType.Simulated);
                        if (predHealth >= 2 * GameObjects.Player.GetAutoAttackDamage(minion)
                            || Math.Abs(predHealth - minion.Health) < float.Epsilon)
                        {
                            this.LaneClearMinion = minion;
                            return minion;
                        }
                    }
                }
            }

            // Special Minions if no enemy is near
            if (mode == OrbwalkingMode.Combo)
            {
                if (minions.Any() && !GameObjects.EnemyHeroes.Any(e => e.IsValidTarget(e.GetRealAutoAttackRange() * 2f)))
                {
                    return minions.FirstOrDefault();
                }
            }

            return null;
        }

        /// <summary>
        ///     Indicates whether the depended process should wait before executing.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool ShouldWait()
        {
            return
                this.GetEnemyMinions()
                    .Any(
                        m =>
                        Health.GetPrediction(
                            m,
                            (int)(GameObjects.Player.AttackDelay * 1000 * LaneClearWaitTime),
                            this.FarmDelay,
                            HealthPredictionType.Simulated) < GameObjects.Player.GetAutoAttackDamage(m));
        }

        /// <summary>
        ///     Determines if the orbwalker should wait before attacking a minion under turret.
        /// </summary>
        /// <param name="noneKillableMinion">
        ///     The non killable minion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool ShouldWaitUnderTurret(Obj_AI_Minion noneKillableMinion = null)
        {
            return
                this.GetEnemyMinions()
                    .Any(
                        m =>
                        (noneKillableMinion == null || noneKillableMinion.NetworkId != m.NetworkId) && m.IsValidTarget()
                        && m.InAutoAttackRange()
                        && Health.GetPrediction(
                            m,
                            (int)((GameObjects.Player.AttackDelay * 1000) + m.GetTimeToHit()),
                            this.FarmDelay,
                            HealthPredictionType.Simulated) < GameObjects.Player.GetAutoAttackDamage(m));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Determines whether the unit is valid.
        /// </summary>
        /// <param name="unit">
        ///     The unit.
        /// </param>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private static bool IsValidUnit(AttackableUnit unit, float range = 0f)
        {
            return unit.IsValidTarget(range > 0 ? range : unit.GetRealAutoAttackRange());
        }

        /// <summary>
        ///     Orders the enemy minions.
        /// </summary>
        /// <param name="minions">
        ///     The minions.
        /// </param>
        /// <returns>
        ///     The <see cref="List{T}" /> of <see cref="Obj_AI_Minion" />.
        /// </returns>
        private static List<Obj_AI_Minion> OrderEnemyMinions(IEnumerable<Obj_AI_Minion> minions)
        {
            return
                minions?.OrderByDescending(minion => minion.GetMinionType().HasFlag(MinionTypes.Siege))
                    .ThenBy(minion => minion.GetMinionType().HasFlag(MinionTypes.Super))
                    .ThenBy(minion => minion.Health)
                    .ThenByDescending(minion => minion.MaxHealth)
                    .ToList();
        }

        /// <summary>
        ///     Returns possible minions based on settings.
        /// </summary>
        /// <param name="mode">
        ///     The requested mode
        /// </param>
        /// <returns>
        ///     The <see cref="List{Obj_AI_Minion}" />.
        /// </returns>
        private List<Obj_AI_Minion> GetMinions(OrbwalkingMode mode)
        {
            var minions = mode != OrbwalkingMode.Combo;
            var attackWards = this.orbwalker.Menu["advanced"]["attackWards"].GetValue<MenuBool>().Value;
            var attackClones = this.orbwalker.Menu["advanced"]["attackClones"].GetValue<MenuBool>().Value;
            var attackSpecialMinions =
                this.orbwalker.Menu["advanced"]["attackSpecialMinions"].GetValue<MenuBool>().Value;
            var prioritizeWards = this.orbwalker.Menu["advanced"]["prioritizeWards"].GetValue<MenuBool>().Value;
            var prioritizeSpecialMinions =
                this.orbwalker.Menu["advanced"]["prioritizeSpecialMinions"].GetValue<MenuBool>().Value;
            var minionList = new List<Obj_AI_Minion>();
            var specialList = new List<Obj_AI_Minion>();
            var cloneList = new List<Obj_AI_Minion>();
            var wardList = new List<Obj_AI_Minion>();
            foreach (var minion in
                GameObjects.EnemyMinions.Where(m => IsValidUnit(m)))
            {
                var baseName = minion.CharData.BaseSkinName.ToLower();
                if (minions && minion.IsMinion())
                {
                    minionList.Add(minion);
                }
                else if (attackSpecialMinions && this.specialMinions.Any(s => s.Equals(baseName)))
                {
                    specialList.Add(minion);
                }
                else if (attackClones && this.clones.Any(c => c.Equals(baseName)))
                {
                    cloneList.Add(minion);
                }
            }

            if (minions)
            {
                minionList = OrderEnemyMinions(minionList);
                minionList.AddRange(
                    this.OrderJungleMinions(
                        GameObjects.Jungle.Where(
                            j => IsValidUnit(j) && !j.CharData.BaseSkinName.Equals("gangplankbarrel")).ToList()));
            }

            if (attackWards)
            {
                wardList.AddRange(GameObjects.EnemyWards.Where(w => IsValidUnit(w)));
            }

            var finalMinionList = new List<Obj_AI_Minion>();
            if (attackWards && prioritizeWards && attackSpecialMinions && prioritizeSpecialMinions)
            {
                finalMinionList.AddRange(wardList);
                finalMinionList.AddRange(specialList);
                finalMinionList.AddRange(minionList);
            }
            else if ((!attackWards || !prioritizeWards) && attackSpecialMinions && prioritizeSpecialMinions)
            {
                finalMinionList.AddRange(specialList);
                finalMinionList.AddRange(minionList);
                finalMinionList.AddRange(wardList);
            }
            else if (attackWards && prioritizeWards)
            {
                finalMinionList.AddRange(wardList);
                finalMinionList.AddRange(minionList);
                finalMinionList.AddRange(specialList);
            }
            else
            {
                finalMinionList.AddRange(minionList);
                finalMinionList.AddRange(specialList);
                finalMinionList.AddRange(wardList);
            }

            if (this.orbwalker.Menu["advanced"]["attackBarrels"].GetValue<MenuBool>().Value)
            {
                finalMinionList.AddRange(
                    GameObjects.Jungle.Where(
                        j => IsValidUnit(j) && j.Health <= 1 && j.CharData.BaseSkinName.Equals("gangplankbarrel"))
                        .ToList());
            }

            if (attackClones)
            {
                finalMinionList.AddRange(cloneList);
            }

            return finalMinionList.Where(m => !this.ignoreMinions.Any(b => b.Equals(m.CharData.BaseSkinName))).ToList();
        }

        /// <summary>
        ///     Orders the jungle minions.
        /// </summary>
        /// <param name="minions">
        ///     The minions.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable{T}" /> of <see cref="Obj_AI_Minion" />.
        /// </returns>
        private IEnumerable<Obj_AI_Minion> OrderJungleMinions(List<Obj_AI_Minion> minions)
        {
            return minions != null
                       ? (this.orbwalker.Menu["advanced"]["prioritizeSmallJungle"].GetValue<MenuBool>().Value
                              ? minions.OrderBy(m => m.MaxHealth)
                              : minions.OrderByDescending(m => m.MaxHealth)).ToList()
                       : null;
        }

        #endregion
    }
}