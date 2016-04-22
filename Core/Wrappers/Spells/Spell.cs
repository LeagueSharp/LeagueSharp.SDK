// <copyright file="Spell.cs" company="LeagueSharp">
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

namespace LeagueSharp.SDK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    using SharpDX;

    /// <summary>
    ///     Spell Container
    /// </summary>
    public class Spell
    {
        #region Fields

        /// <summary>
        ///     Charged Cast Tick
        /// </summary>
        private int chargedCastedT;

        /// <summary>
        ///     Charged Request Sent Tick
        /// </summary>
        private int chargedReqSentT;

        /// <summary>
        ///     From Vector3 Source
        /// </summary>
        private Vector3 @from;

        /// <summary>
        ///     The Minimum Mana Percentage
        /// </summary>
        private float minManaPercent;

        /// <summary>
        ///     The Range
        /// </summary>
        private float range;

        /// <summary>
        ///     Range Check From Vector3 Source
        /// </summary>
        private Vector3 rangeCheckFrom;

        /// <summary>
        ///     The Width
        /// </summary>
        private float width;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Spell" /> class.
        /// </summary>
        /// <param name="slot">
        ///     The SpellSlot
        /// </param>
        /// <param name="loadFromGame">
        ///     Load SpellData From Game
        /// </param>
        /// <param name="hitChance">
        ///     Minimum Hit Chance
        /// </param>
        public Spell(SpellSlot slot, bool loadFromGame, HitChance hitChance = HitChance.Medium)
        {
            this.Slot = slot;

            if (!loadFromGame)
            {
                return;
            }

            var spellData = GameObjects.Player.Spellbook.GetSpell(slot).SData;

            this.Range = spellData.CastRange;
            this.Width = spellData.LineWidth.Equals(0) ? spellData.CastRadius : spellData.LineWidth;
            this.Speed = spellData.MissileSpeed;
            this.Delay = spellData.DelayTotalTimePercent;

            this.MinHitChance = hitChance;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Spell" /> class.
        /// </summary>
        /// <param name="slot">
        ///     The Slot
        /// </param>
        /// <param name="range">
        ///     Spell Range
        /// </param>
        public Spell(SpellSlot slot, float range = float.MaxValue)
        {
            this.Slot = slot;
            this.Range = range;
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Cast Condition Delegate
        /// </summary>
        public delegate bool CastConditionDelegate();

        #endregion

        #region Public Properties

        /// <summary>
        ///     Condition to Cast Spell
        /// </summary>
        public CastConditionDelegate CastCondition { get; set; }

        /// <summary>
        ///     Gets or sets the charged buff name.
        /// </summary>
        public string ChargedBuffName { get; set; }

        /// <summary>
        ///     Gets or sets the charged max range.
        /// </summary>
        public int ChargedMaxRange { get; set; }

        /// <summary>
        ///     Gets or sets the charged min range.
        /// </summary>
        public int ChargedMinRange { get; set; }

        /// <summary>
        ///     Gets or sets the charged spell name.
        /// </summary>
        public string ChargedSpellName { get; set; }

        /// <summary>
        ///     Gets or sets the charge duration.
        /// </summary>
        public int ChargeDuration { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether collision.
        /// </summary>
        public bool Collision { get; set; }

        /// <summary>
        ///     Gets or sets the damage type.
        /// </summary>
        public DamageType DamageType { get; set; }

        /// <summary>
        ///     Gets or sets the delay.
        /// </summary>
        public float Delay { get; set; }

        /// <summary>
        ///     Gets or sets the from.
        /// </summary>
        public Vector3 From
        {
            get
            {
                return !this.@from.IsValid() ? GameObjects.Player.ServerPosition : this.@from;
            }

            set
            {
                this.@from = value;
            }
        }

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        public SpellDataInst Instance => GameObjects.Player.Spellbook.GetSpell(this.Slot);

        /// <summary>
        ///     Gets or sets a value indicating whether is charged spell.
        /// </summary>
        public bool IsChargedSpell { get; set; }

        /// <summary>
        ///     Gets a value indicating whether is charging.
        /// </summary>
        public bool IsCharging
        {
            get
            {
                if (!this.IsReady())
                {
                    return false;
                }

                return GameObjects.Player.HasBuff(this.ChargedBuffName)
                       || Variables.TickCount - this.chargedCastedT < 300 + Game.Ping;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether is skill-shot.
        /// </summary>
        public bool IsSkillshot { get; set; }

        /// <summary>
        ///     Gets or sets the last cast attempt t.
        /// </summary>
        public int LastCastAttemptT { get; set; }

        /// <summary>
        ///     Gets the level.
        /// </summary>
        public int Level => GameObjects.Player.Spellbook.GetSpell(this.Slot).Level;

        /// <summary>
        ///     Gets or sets the min hit chance.
        /// </summary>
        public HitChance MinHitChance { get; set; }

        /// <summary>
        ///     Gets or sets the range.
        /// </summary>
        public float Range
        {
            get
            {
                if (!this.IsChargedSpell)
                {
                    return this.range;
                }

                if (this.IsCharging)
                {
                    return this.ChargedMinRange
                           + Math.Min(
                               this.ChargedMaxRange - this.ChargedMinRange,
                               ((Variables.TickCount - this.chargedCastedT)
                                * (this.ChargedMaxRange - this.ChargedMinRange) / this.ChargeDuration) - 150);
                }

                return this.ChargedMaxRange;
            }

            set
            {
                this.range = value;
            }
        }

        /// <summary>
        ///     Gets or sets the range check from.
        /// </summary>
        public Vector3 RangeCheckFrom
        {
            get
            {
                return !this.rangeCheckFrom.IsValid() ? GameObjects.Player.ServerPosition : this.rangeCheckFrom;
            }

            set
            {
                this.rangeCheckFrom = value;
            }
        }

        /// <summary>
        ///     Gets the range squared.
        /// </summary>
        public float RangeSqr => this.Range * this.Range;

        /// <summary>
        ///     Gets or sets the slot.
        /// </summary>
        public SpellSlot Slot { get; set; }

        /// <summary>
        ///     Gets or sets the speed.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        public SkillshotType Type { get; set; }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        public float Width
        {
            get
            {
                return this.width;
            }

            set
            {
                this.width = value;
                this.WidthSqr = value * value;
            }
        }

        /// <summary>
        ///     Gets the width squared.
        /// </summary>
        public float WidthSqr { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns if a spell can be cast and the target is in range.
        /// </summary>
        /// <param name="unit">
        ///     The Target
        /// </param>
        /// <returns>
        ///     Can spell be casted and target is in range
        /// </returns>
        public bool CanCast(Obj_AI_Base unit)
        {
            return this.IsReady() && unit.IsValidTarget(this.Range);
        }

        /// <summary>
        ///     Returns if a spell can kill a target.
        /// </summary>
        /// <param name="unit">The Target</param>
        /// <param name="stage">
        ///     The <see cref="DamageStage" /> of the spell.
        /// </param>
        /// <returns>Can spell kill target</returns>
        public bool CanKill(Obj_AI_Base unit, DamageStage stage = DamageStage.Default)
        {
            return unit.IsValidTarget() && this.GetDamage(unit, stage) > unit.Health;
        }

        /// <summary>
        ///     Casts the spell.
        /// </summary>
        /// <param name="unit">Unit to cast on</param>
        /// <param name="exactHitChance">
        ///     Is exact hit chance
        /// </param>
        /// <param name="areaOfEffect">
        ///     Is Area of Effect
        /// </param>
        /// <param name="minTargets">
        ///     Minimum of Targets
        /// </param>
        /// <param name="tempHitChance">
        ///     Temporary HitChance Override
        /// </param>
        /// <returns>
        ///     The <see cref="CastStates" />
        /// </returns>
        public CastStates Cast(
            Obj_AI_Base unit,
            bool exactHitChance = false,
            bool areaOfEffect = false,
            int minTargets = -1,
            HitChance tempHitChance = HitChance.None)
        {
            if (!unit.IsValid())
            {
                return CastStates.InvalidTarget;
            }

            if (!this.IsReady())
            {
                return CastStates.NotReady;
            }

            if (!this.minManaPercent.Equals(0) && ObjectManager.Player.ManaPercent < this.minManaPercent)
            {
                return CastStates.LowMana;
            }

            if (this.CastCondition != null && !this.CastCondition())
            {
                return CastStates.FailedCondition;
            }

            if (!areaOfEffect && minTargets != -1)
            {
                areaOfEffect = true;
            }

            if (!this.IsSkillshot)
            {
                if (this.RangeCheckFrom.DistanceSquared(unit.ServerPosition) > this.RangeSqr)
                {
                    return CastStates.OutOfRange;
                }

                this.LastCastAttemptT = Variables.TickCount;

                return !GameObjects.Player.Spellbook.CastSpell(this.Slot, unit)
                           ? CastStates.NotCasted
                           : CastStates.SuccessfullyCasted;
            }

            var prediction = this.GetPrediction(unit, areaOfEffect);

            if (minTargets != -1 && prediction.AoeTargetsHitCount <= minTargets)
            {
                return CastStates.NotEnoughTargets;
            }

            if (prediction.CollisionObjects.Count > 0)
            {
                return CastStates.Collision;
            }

            if (this.RangeCheckFrom.DistanceSquared(prediction.CastPosition) > this.RangeSqr)
            {
                return CastStates.OutOfRange;
            }

            if (prediction.Hitchance < ((tempHitChance == HitChance.None) ? this.MinHitChance : tempHitChance)
                || (exactHitChance
                    && prediction.Hitchance != ((tempHitChance == HitChance.None) ? this.MinHitChance : tempHitChance)))
            {
                return CastStates.LowHitChance;
            }

            this.LastCastAttemptT = Variables.TickCount;

            if (this.IsChargedSpell)
            {
                if (this.IsCharging)
                {
                    ShootChargedSpell(this.Slot, prediction.CastPosition);
                }
                else
                {
                    this.StartCharging();
                }
            }
            else
            {
                if (!GameObjects.Player.Spellbook.CastSpell(this.Slot, prediction.CastPosition))
                {
                    return CastStates.NotCasted;
                }
            }

            return CastStates.SuccessfullyCasted;
        }

        /// <summary>
        ///     Cast Spell onto self
        /// </summary>
        /// <returns>
        ///     Was Spell Casted
        /// </returns>
        public bool Cast()
        {
            return this.CastOnUnit(GameObjects.Player);
        }

        /// <summary>
        ///     Cast Spell from a Vector2 to another Vector2 boundaries
        /// </summary>
        /// <param name="fromPosition">
        ///     From Position
        /// </param>
        /// <param name="toPosition">
        ///     To Position
        /// </param>
        /// <returns>
        ///     Was Spell Casted
        /// </returns>
        public bool Cast(Vector2 fromPosition, Vector2 toPosition)
        {
            return this.Cast(fromPosition.ToVector3(), toPosition.ToVector3());
        }

        /// <summary>
        ///     Cast Spell from a Vector3 to another Vector3 boundaries
        /// </summary>
        /// <param name="fromPosition">
        ///     From Position
        /// </param>
        /// <param name="toPosition">
        ///     To Position
        /// </param>
        /// <returns>
        ///     Was Spell Casted
        /// </returns>
        public bool Cast(Vector3 fromPosition, Vector3 toPosition)
        {
            return this.IsReady() && GameObjects.Player.Spellbook.CastSpell(this.Slot, fromPosition, toPosition);
        }

        /// <summary>
        ///     Cast Spell to a Vector2
        /// </summary>
        /// <param name="position">
        ///     The Position
        /// </param>
        /// <returns>
        ///     Was Spell Casted
        /// </returns>
        public bool Cast(Vector2 position)
        {
            return this.Cast(position.ToVector3());
        }

        /// <summary>
        ///     Cast Spell to a Vector3
        /// </summary>
        /// <param name="position">
        ///     The Position
        /// </param>
        /// <returns>
        ///     Was Spell Casted
        /// </returns>
        public bool Cast(Vector3 position)
        {
            if (!this.IsReady())
            {
                return false;
            }

            this.LastCastAttemptT = Variables.TickCount;

            if (this.IsChargedSpell)
            {
                if (this.IsCharging)
                {
                    ShootChargedSpell(this.Slot, position);
                }
                else
                {
                    this.StartCharging();
                }
            }
            else
            {
                return GameObjects.Player.Spellbook.CastSpell(this.Slot, position);
            }

            return false;
        }

        /// <summary>
        ///     Cast Spell if HitChance equals to input HitChance
        /// </summary>
        /// <param name="unit">
        ///     The Target
        /// </param>
        /// <param name="hitChance">
        ///     The HitChance
        /// </param>
        /// <returns>
        ///     Was Spell Casted
        /// </returns>
        public CastStates CastIfHitchanceEquals(Obj_AI_Base unit, HitChance hitChance)
        {
            return this.Cast(unit, true, false, -1, hitChance);
        }

        /// <summary>
        ///     Cast Spell if HitChance is more than the minimum to input HitChance
        /// </summary>
        /// <param name="unit">
        ///     The Target
        /// </param>
        /// <param name="hitChance">
        ///     The HitChance
        /// </param>
        /// <returns>
        ///     Was Spell Casted
        /// </returns>
        public CastStates CastIfHitchanceMinimum(Obj_AI_Base unit, HitChance hitChance)
        {
            return this.Cast(unit, false, false, -1, hitChance);
        }

        /// <summary>
        ///     Cast Spell if will hit Minimum input targets counts.
        /// </summary>
        /// <param name="unit">
        ///     Main Target
        /// </param>
        /// <param name="minTargets">
        ///     Minimum Targets
        /// </param>
        /// <returns>
        ///     Was Spell Casted
        /// </returns>
        public CastStates CastIfWillHit(Obj_AI_Base unit, int minTargets = 5)
        {
            return this.Cast(unit, false, true, minTargets);
        }

        /// <summary>
        ///     Cast Spell on best Target.
        /// </summary>
        /// <param name="extraRange">
        ///     Extra Range
        /// </param>
        /// <param name="areaOfEffect">
        ///     Area of Effect
        /// </param>
        /// <param name="minTargets">
        ///     Minimum Area-of-Effect targets
        /// </param>
        /// <returns>
        ///     CastState. <seealso cref="CastStates" />
        /// </returns>
        public CastStates CastOnBestTarget(float extraRange = 0, bool areaOfEffect = false, int minTargets = -1)
        {
            return this.Cast(this.GetTarget(extraRange), false, areaOfEffect, minTargets);
        }

        /// <summary>
        ///     Cast Spell directly onto a unit
        /// </summary>
        /// <param name="unit">
        ///     The Target
        /// </param>
        /// <returns>
        ///     Was Spell Casted
        /// </returns>
        public bool CastOnUnit(Obj_AI_Base unit)
        {
            if (!this.IsReady() || this.From.DistanceSquared(unit.ServerPosition) > this.RangeSqr)
            {
                return false;
            }

            this.LastCastAttemptT = Variables.TickCount;

            return GameObjects.Player.Spellbook.CastSpell(this.Slot, unit);
        }

        /// <summary>
        ///     Returns the spell counted hits.
        /// </summary>
        /// <param name="units">
        ///     The Minions
        /// </param>
        /// <param name="castPosition">
        ///     Cast Position Vector3 Source
        /// </param>
        /// <returns>
        ///     The hits
        /// </returns>
        public int CountHits(List<Obj_AI_Base> units, Vector3 castPosition)
        {
            var points = units.Select(unit => this.GetPrediction(unit).UnitPosition).ToList();
            return this.CountHits(points, castPosition);
        }

        /// <summary>
        ///     Returns the spell counted hits.
        /// </summary>
        /// <param name="points">
        ///     Minion Positions
        /// </param>
        /// <param name="castPosition">
        ///     Cast Position Vector3 Source
        /// </param>
        /// <returns>
        ///     The hits
        /// </returns>
        public int CountHits(List<Vector3> points, Vector3 castPosition)
        {
            return points.Count(point => this.WillHit(point, castPosition));
        }

        /// <summary>
        ///     Get Circular Farm Location
        /// </summary>
        /// <param name="minions">
        ///     The Minions
        /// </param>
        /// <param name="overrideWidth">
        ///     Override Width
        /// </param>
        /// <returns>
        ///     Farm Location. <seealso cref="FarmLocation" />
        /// </returns>
        public FarmLocation GetCircularFarmLocation(List<Obj_AI_Minion> minions, float overrideWidth = -1)
        {
            var positions = Minion.GetMinionsPredictedPositions(
                minions,
                this.Delay,
                this.Width,
                this.Speed,
                this.From,
                this.Range,
                false,
                SkillshotType.SkillshotCircle);

            return this.GetCircularFarmLocation(positions, overrideWidth);
        }

        /// <summary>
        ///     Get Circular Farm Location
        /// </summary>
        /// <param name="minionPositions">
        ///     Minion Positions
        /// </param>
        /// <param name="overrideWidth">
        ///     Override Width
        /// </param>
        /// <returns>
        ///     Farm Location. <seealso cref="FarmLocation" />
        /// </returns>
        public FarmLocation GetCircularFarmLocation(List<Vector2> minionPositions, float overrideWidth = -1)
        {
            return Minion.GetBestCircularFarmLocation(
                minionPositions,
                overrideWidth >= 0 ? overrideWidth : this.Width,
                this.Range);
        }

        /// <summary>
        ///     Returns Collision List
        /// </summary>
        /// <param name="fromVector2">
        ///     From Vector3 Source
        /// </param>
        /// <param name="to">
        ///     To Vector3 Source
        /// </param>
        /// <param name="delayOverride">
        ///     Delay Override
        /// </param>
        /// <returns>
        ///     Collision List
        /// </returns>
        public List<Obj_AI_Base> GetCollision(Vector2 fromVector2, List<Vector2> to, float delayOverride = -1)
        {
            return SDK.Collision.GetCollision(
                to.Select(h => h.ToVector3()).ToList(),
                new PredictionInput
                    {
                        From = fromVector2.ToVector3(), Type = this.Type, Radius = this.Width,
                        Delay = delayOverride > 0 ? delayOverride : this.Delay, Speed = this.Speed
                    });
        }

        /// <summary>
        ///     Returns the damage a spell will deal to target.
        /// </summary>
        /// <param name="target">
        ///     The <see cref="Obj_AI_Hero" /> target.
        /// </param>
        /// <param name="stage">
        ///     The <see cref="DamageStage" /> of the spell.
        /// </param>
        /// <returns>
        ///     The damage value to target unit.
        /// </returns>
        public float GetDamage(Obj_AI_Base target, DamageStage stage = DamageStage.Default)
        {
            return (float)GameObjects.Player.GetSpellDamage(target, this.Slot, stage);
        }

        /// <summary>
        ///     Returns health prediction on a unit.
        /// </summary>
        /// <param name="unit">
        ///     The Unit
        /// </param>
        /// <returns>
        ///     Unit's predicted health
        /// </returns>
        public float GetHealthPrediction(Obj_AI_Base unit)
        {
            var time = (this.Delay * 1000) - 100 + (Game.Ping / 2f);
            if (Math.Abs(this.Speed - float.MaxValue) > float.Epsilon)
            {
                time += 1000 * unit.Distance(this.From) / this.Speed;
            }
            return Health.GetPrediction(unit, (int)time);
        }

        /// <summary>
        ///     Returns Hit Count
        /// </summary>
        /// <param name="hitChance">
        ///     The HitChance
        /// </param>
        /// <returns>
        ///     Hit Count
        /// </returns>
        public float GetHitCount(HitChance hitChance = HitChance.High)
        {
            return GameObjects.EnemyHeroes.Select(e => this.GetPrediction(e)).Count(p => p.Hitchance >= hitChance);
        }

        /// <summary>
        ///     Get Line Farm Location
        /// </summary>
        /// <param name="minionPositions">
        ///     The Minions
        /// </param>
        /// <param name="overrideWidth">
        ///     Override Width
        /// </param>
        /// <returns>
        ///     Farm Location. <seealso cref="FarmLocation" />
        /// </returns>
        public FarmLocation GetLineFarmLocation(List<Obj_AI_Minion> minionPositions, float overrideWidth = -1)
        {
            var positions = Minion.GetMinionsPredictedPositions(
                minionPositions,
                this.Delay,
                this.Width,
                this.Speed,
                this.From,
                this.Range,
                false,
                SkillshotType.SkillshotLine);

            return this.GetLineFarmLocation(positions, overrideWidth >= 0 ? overrideWidth : this.Width);
        }

        /// <summary>
        ///     Get Line Farm Location
        /// </summary>
        /// <param name="minionPositions">
        ///     Minion Positions
        /// </param>
        /// <param name="overrideWidth">
        ///     Override Width
        /// </param>
        /// <returns>
        ///     Farm Location. <seealso cref="FarmLocation" />
        /// </returns>
        public FarmLocation GetLineFarmLocation(List<Vector2> minionPositions, float overrideWidth = -1)
        {
            return Minion.GetBestLineFarmLocation(
                minionPositions,
                overrideWidth >= 0 ? overrideWidth : this.Width,
                this.Range);
        }

        /// <summary>
        ///     Returns Spell Prediction
        /// </summary>
        /// <param name="unit">
        ///     Predicted Unit
        /// </param>
        /// <param name="aoe">
        ///     Is Area of effect
        /// </param>
        /// <param name="overrideRange">
        ///     Override Range
        /// </param>
        /// <param name="collisionable">
        ///     Collision-able Flags
        /// </param>
        /// <returns>
        ///     <see cref="PredictionOutput" /> output
        /// </returns>
        public PredictionOutput GetPrediction(
            Obj_AI_Base unit,
            bool aoe = false,
            float overrideRange = -1,
            CollisionableObjects collisionable = CollisionableObjects.Minions | CollisionableObjects.YasuoWall)
        {
            return
                Movement.GetPrediction(
                    new PredictionInput
                        {
                            Unit = unit, Delay = this.Delay, Radius = this.Width, Speed = this.Speed, From = this.From,
                            Range = (overrideRange > 0) ? overrideRange : this.Range, Collision = this.Collision,
                            Type = this.Type, RangeCheckFrom = this.RangeCheckFrom, AoE = aoe,
                            CollisionObjects = collisionable
                        });
        }

        /// <summary>
        ///     Returns the best target found using the current TargetSelector Mode.
        ///     Please make sure to set the Spell.DamageType Property to the type of damage this spell does (if not done on
        ///     initialization).
        /// </summary>
        /// <param name="extraRange">
        ///     Extra Range
        /// </param>
        /// <param name="accountForCollision">
        ///     If true, will get a target that can be hit by the spell.
        /// </param>
        /// <param name="champsToIgnore">
        ///     Champions to Ignore
        /// </param>
        /// <returns>
        ///     The <see cref="Obj_AI_Hero" /> target.
        /// </returns>
        public Obj_AI_Hero GetTarget(
            float extraRange = 0,
            bool accountForCollision = false,
            IEnumerable<Obj_AI_Hero> champsToIgnore = null)
        {
            return accountForCollision
                       ? Variables.TargetSelector.GetTargetNoCollision(this, true, champsToIgnore)
                       : Variables.TargetSelector.GetTarget(
                           this.Range + extraRange,
                           this.DamageType,
                           true,
                           this.From,
                           champsToIgnore);
        }

        /// <summary>
        ///     Gets all of the units that this spell can hit that is greater then or equal to the <see cref="HitChance" />
        ///     provided.
        /// </summary>
        /// <param name="minimumHitChance">Minimum HitChance</param>
        /// <returns>
        ///     All of the units that this spell can hit that is greater then or equal to the <see cref="HitChance" />
        ///     provided.
        /// </returns>
        public IEnumerable<Obj_AI_Base> GetUnitsByHitChance(HitChance minimumHitChance = HitChance.High)
        {
            return
                GameObjects.Enemy.Where(
                    unit => this.WillHit(unit, GameObjects.Player.ServerPosition, 0, minimumHitChance));
        }

        /// <summary>
        ///     Returns if the GameObject is in range of the spell.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="GameObject" />
        /// </param>
        /// <param name="otherRange">The Range</param>
        /// <returns>
        ///     Is GameObject in range of spell
        /// </returns>
        public bool IsInRange(GameObject obj, float otherRange = -1)
        {
            return this.IsInRange(
                (obj as Obj_AI_Base)?.ServerPosition.ToVector2() ?? obj.Position.ToVector2(),
                otherRange);
        }

        /// <summary>
        ///     Returns if the Vector3 is in range of the spell.
        /// </summary>
        /// <param name="point">
        ///     Vector3 point
        /// </param>
        /// <param name="otherRange">
        ///     The Range
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool IsInRange(Vector3 point, float otherRange = -1)
        {
            return this.IsInRange(point.ToVector2(), otherRange);
        }

        /// <summary>
        ///     Returns if the Vector2 is in range of the spell.
        /// </summary>
        /// <param name="point">
        ///     Vector2 point
        /// </param>
        /// <param name="otherRange">
        ///     The Range
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool IsInRange(Vector2 point, float otherRange = -1)
        {
            return this.RangeCheckFrom.DistanceSquared(point)
                   < (otherRange < 0 ? this.RangeSqr : otherRange * otherRange);
        }

        /// <summary>
        ///     Returns if the Spell is ready to use.
        /// </summary>
        /// <param name="t">Time Left</param>
        /// <returns>Is Spell Ready to use</returns>
        public bool IsReady(int t = 0)
        {
            return this.Slot.IsReady(t);
        }

        /// <summary>
        ///     Sets the Spell Data to Charged data.
        /// </summary>
        /// <param name="spellName">
        ///     Spell Name
        /// </param>
        /// <param name="buffName">
        ///     Spell Buff Name
        /// </param>
        /// <param name="minRange">
        ///     Spell Minimum Range
        /// </param>
        /// <param name="maxRange">
        ///     Spell Maximum Range
        /// </param>
        /// <param name="deltaT">
        ///     Charge Duration
        /// </param>
        /// <returns>
        ///     The <see cref="Spell" />.
        /// </returns>
        public Spell SetCharged(string spellName, string buffName, int minRange, int maxRange, float deltaT)
        {
            this.IsChargedSpell = true;
            this.ChargedSpellName = spellName;
            this.ChargedBuffName = buffName;
            this.ChargedMinRange = minRange;
            this.ChargedMaxRange = maxRange;
            this.ChargeDuration = (int)(deltaT * 1000);
            this.chargedCastedT = 0;

            Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
            Spellbook.OnUpdateChargedSpell += this.Spellbook_OnUpdateChargedSpell;
            Spellbook.OnCastSpell += this.SpellbookOnCastSpell;

            return this;
        }

        /// <summary>
        ///     Sets the minimum mana percentage to cast the spell.
        /// </summary>
        /// <param name="percentage">
        ///     Mana Percentage
        /// </param>
        public void SetMinimumManaPercentage(float percentage)
        {
            this.minManaPercent = percentage;
        }

        /// <summary>
        ///     Sets the Spell Data to Skill-shot data.
        /// </summary>
        /// <param name="delay">
        ///     Spell Delay
        /// </param>
        /// <param name="skillWidth">
        ///     Spell Width
        /// </param>
        /// <param name="speed">
        ///     Spell Speed
        /// </param>
        /// <param name="collision">
        ///     Spell Collision Flag
        /// </param>
        /// <param name="type">
        ///     Skill-shot Type
        /// </param>
        /// <param name="fromVector3">
        ///     From Vector3 Source
        /// </param>
        /// <param name="rangeCheckFromVector3">
        ///     Range Check From Vector3 Source
        /// </param>
        /// <returns>
        ///     The <see cref="Spell" />.
        /// </returns>
        public Spell SetSkillshot(
            float delay,
            float skillWidth,
            float speed,
            bool collision,
            SkillshotType type,
            Vector3 fromVector3 = default(Vector3),
            Vector3 rangeCheckFromVector3 = default(Vector3))
        {
            this.Delay = delay;
            this.Width = skillWidth;
            this.Speed = speed;
            this.From = fromVector3;
            this.Collision = collision;
            this.Type = type;
            this.RangeCheckFrom = rangeCheckFromVector3;
            this.IsSkillshot = true;

            return this;
        }

        /// <summary>
        ///     Sets the Spell Data to Skill-shot data.
        /// </summary>
        /// <param name="collision">
        ///     Spell Collision Flag
        /// </param>
        /// <param name="type">
        ///     Skill-shot Type
        /// </param>
        /// <param name="fromVector3">
        ///     From Vector3 Source
        /// </param>
        /// <param name="rangeCheckFromVector3">
        ///     Range Check From Vector3 Source
        /// </param>
        /// <returns>
        ///     The <see cref="Spell" />.
        /// </returns>
        public Spell SetSkillshot(
            bool collision,
            SkillshotType type,
            Vector3 fromVector3 = default(Vector3),
            Vector3 rangeCheckFromVector3 = default(Vector3))
        {
            this.From = fromVector3;
            this.Collision = collision;
            this.Type = type;
            this.RangeCheckFrom = rangeCheckFromVector3;
            this.IsSkillshot = true;

            return this;
        }

        /// <summary>
        ///     Sets the Spell Data to targeted data.
        /// </summary>
        /// <param name="delay">
        ///     Spell Delay
        /// </param>
        /// <param name="speed">
        ///     Spell Speed
        /// </param>
        /// <param name="fromVector3">
        ///     From Vector3 Source
        /// </param>
        /// <param name="rangeCheckFromVector3">
        ///     Range Check From Vector3 Source
        /// </param>
        /// <returns>
        ///     The <see cref="Spell" />.
        /// </returns>
        public Spell SetTargetted(
            float delay,
            float speed,
            Vector3 fromVector3 = default(Vector3),
            Vector3 rangeCheckFromVector3 = default(Vector3))
        {
            this.Delay = delay;
            this.Speed = speed;
            this.From = fromVector3;
            this.RangeCheckFrom = rangeCheckFromVector3;
            this.IsSkillshot = false;

            return this;
        }

        /// <summary>
        ///     Sets the Spell Data to targeted data.
        /// </summary>
        /// <param name="fromVector3">
        ///     From Vector3 Source
        /// </param>
        /// <param name="rangeCheckFromVector3">
        ///     Range Check From Vector3 Source
        /// </param>
        /// <returns>
        ///     The <see cref="Spell" />.
        /// </returns>
        public Spell SetTargetted(
            Vector3 fromVector3 = default(Vector3),
            Vector3 rangeCheckFromVector3 = default(Vector3))
        {
            this.From = fromVector3;
            this.RangeCheckFrom = rangeCheckFromVector3;
            this.IsSkillshot = false;

            return this;
        }

        /// <summary>
        ///     Start charging the spell if its not charging.
        /// </summary>
        public void StartCharging()
        {
            if (this.IsCharging || Variables.TickCount - this.chargedReqSentT <= 400 + Game.Ping)
            {
                return;
            }

            GameObjects.Player.Spellbook.CastSpell(this.Slot);
            this.chargedReqSentT = Variables.TickCount;
        }

        /// <summary>
        ///     Start charging the spell if its not charging.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        public void StartCharging(Vector3 position)
        {
            if (this.IsCharging || Variables.TickCount - this.chargedReqSentT <= 400 + Game.Ping)
            {
                return;
            }

            GameObjects.Player.Spellbook.CastSpell(this.Slot, position);
            this.chargedReqSentT = Variables.TickCount;
        }

        /// <summary>
        ///     Update Source Position
        /// </summary>
        /// <param name="fromVector3">
        ///     From Vector3 Source
        /// </param>
        /// <param name="rangeCheckFromVector3">
        ///     Range Check From Vector3 Source
        /// </param>
        public void UpdateSourcePosition(
            Vector3 fromVector3 = default(Vector3),
            Vector3 rangeCheckFromVector3 = default(Vector3))
        {
            this.From = fromVector3;
            this.RangeCheckFrom = rangeCheckFromVector3;
        }

        /// <summary>
        ///     Returns if the spell will hit the unit when casted.
        /// </summary>
        /// <param name="unit">
        ///     The Target
        /// </param>
        /// <param name="castPosition">
        ///     Cast Position
        /// </param>
        /// <param name="extraWidth">
        ///     Extra Width
        /// </param>
        /// <param name="minHitChance">
        ///     Minimum Hit Chance
        /// </param>
        /// <returns>
        ///     Will Spell Hit
        /// </returns>
        public bool WillHit(
            Obj_AI_Base unit,
            Vector3 castPosition,
            int extraWidth = 0,
            HitChance minHitChance = HitChance.High)
        {
            var unitPosition = this.GetPrediction(unit);
            return unitPosition.Hitchance >= minHitChance
                   && this.WillHit(unitPosition.UnitPosition, castPosition, extraWidth);
        }

        /// <summary>
        ///     Returns if the spell will hit the point when casted
        /// </summary>
        /// <param name="point">
        ///     Vector3 Target
        /// </param>
        /// <param name="castPosition">
        ///     Cast Position
        /// </param>
        /// <param name="extraWidth">
        ///     Extra Width
        /// </param>
        /// <returns>
        ///     Will Spell Hit
        /// </returns>
        public bool WillHit(Vector3 point, Vector3 castPosition, int extraWidth = 0)
        {
            switch (this.Type)
            {
                case SkillshotType.SkillshotCircle:
                    if (point.DistanceSquared(castPosition) < this.WidthSqr)
                    {
                        return true;
                    }

                    break;

                case SkillshotType.SkillshotLine:
                    if (point.ToVector2().DistanceSquared(castPosition.ToVector2(), this.From.ToVector2(), true)
                        < Math.Pow(this.Width + extraWidth, 2))
                    {
                        return true;
                    }

                    break;
                case SkillshotType.SkillshotCone:
                    var edge1 = (castPosition.ToVector2() - this.From.ToVector2()).Rotated(-this.Width / 2);
                    var edge2 = edge1.Rotated(this.Width);
                    var v = point.ToVector2() - this.From.ToVector2();
                    if (point.DistanceSquared(this.From) < this.RangeSqr && edge1.CrossProduct(v) > 0
                        && v.CrossProduct(edge2) > 0)
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Shoot Charged Spell
        /// </summary>
        /// <param name="slot">
        ///     The SpellSlot
        /// </param>
        /// <param name="position">
        ///     Vector3 Position
        /// </param>
        /// <param name="releaseCast">
        ///     Release Cast
        /// </param>
        private static void ShootChargedSpell(SpellSlot slot, Vector3 position, bool releaseCast = true)
        {
            position.Z = NavMesh.GetHeightForPosition(position.X, position.Y);
            GameObjects.Player.Spellbook.UpdateChargedSpell(slot, position, releaseCast, false);
            GameObjects.Player.Spellbook.CastSpell(slot, position, false);
        }

        /// <summary>
        ///     On Process Spell Cast event catch.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="Obj_AI_Base" /> sender
        /// </param>
        /// <param name="args">
        ///     Process Spell Cast Data
        /// </param>
        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == this.ChargedSpellName)
            {
                this.chargedCastedT = Variables.TickCount;
            }
        }

        /// <summary>
        ///     On Charged Spell Update subscribed event function.
        /// </summary>
        /// <param name="sender">
        ///     <see cref="Spellbook" /> sender
        /// </param>
        /// <param name="args">
        ///     Spell-book Update Charged Spell Data
        /// </param>
        private void Spellbook_OnUpdateChargedSpell(Spellbook sender, SpellbookUpdateChargedSpellEventArgs args)
        {
            if (sender.Owner.IsMe && Variables.TickCount - this.chargedReqSentT < 3000 && args.ReleaseCast)
            {
                args.Process = false;
            }
        }

        /// <summary>
        ///     Spell-book On Cast Spell subscribed event function.
        /// </summary>
        /// <param name="spellbook">
        ///     <see cref="Spellbook" /> sender
        /// </param>
        /// <param name="args">
        ///     Spell-book Cast Spell Data
        /// </param>
        private void SpellbookOnCastSpell(Spellbook spellbook, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot != this.Slot)
            {
                return;
            }

            if (Variables.TickCount - this.chargedReqSentT > 500)
            {
                if (this.IsCharging)
                {
                    this.Cast(new Vector2(args.EndPosition.X, args.EndPosition.Y));
                }
            }
        }

        #endregion
    }
}
