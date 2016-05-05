// <copyright file="OrbwalkerBase.cs" company="LeagueSharp">
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
    using System.Reflection;

    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    using SharpDX;

    /// <summary>
    ///     Base class for Orbwalker
    /// </summary>
    /// <typeparam name="TK">
    ///     The type of the orbwalker modes.
    /// </typeparam>
    /// <typeparam name="T">
    ///     The type of the units.
    /// </typeparam>
    public abstract class OrbwalkerBase<TK, T>
        where TK : struct, IConvertible where T : AttackableUnit
    {
        #region Fields

        private bool enabled;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrbwalkerBase{TK, T}" /> class.
        /// </summary>
        /// <exception cref="ArgumentException">TK must be an enumerated type.</exception>
        internal OrbwalkerBase()
        {
            if (!typeof(TK).IsEnum)
            {
                throw new ArgumentException("TK must be an enumerated type.");
            }

            var enumValues = Enum.GetValues(typeof(TK));

            if (enumValues.Length <= 0)
            {
                throw new ArgumentException("TK must contain at least one value.");
            }

            this.InActiveMode = (TK)enumValues.GetValue(0);
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     The<see cref="OnAction" /> event delegate.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="e">
        ///     The event data
        /// </param>
        public delegate void OnActionDelegate(object sender, OrbwalkingActionArgs e);

        #endregion

        #region Public Events

        /// <summary>
        ///     The OnAction event.
        /// </summary>
        public event OnActionDelegate OnAction;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets value indication in which mode Orbwalk should run.
        /// </summary>
        public TK ActiveMode { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="OrbwalkerBase{TK, T}" /> is enabled.
        /// </summary>
        public virtual bool Enabled
        {
            get
            {
                return this.enabled;
            }

            set
            {
                if (this.enabled != value)
                {
                    if (value)
                    {
                        Obj_AI_Base.OnProcessSpellCast += this.OnObjAiBaseProcessSpellCast;
                        Obj_AI_Base.OnDoCast += this.OnObjAiBaseDoCast;
                        Obj_AI_Base.OnBuffAdd += this.ObjAiBaseOnOnBuffAdd;
                        Spellbook.OnStopCast += this.OnSpellbookStopCast;
                        Game.OnUpdate += this.OnGameUpdate;
                    }
                    else
                    {
                        Obj_AI_Base.OnProcessSpellCast -= this.OnObjAiBaseProcessSpellCast;
                        Obj_AI_Base.OnDoCast -= this.OnObjAiBaseDoCast;
                        Obj_AI_Base.OnBuffAdd -= this.ObjAiBaseOnOnBuffAdd;
                        Spellbook.OnStopCast -= this.OnSpellbookStopCast;
                        Game.OnUpdate -= this.OnGameUpdate;
                    }
                }

                this.enabled = value;
            }
        }

        /// <summary>
        ///     Gets or sets the last auto attack command tick.
        /// </summary>
        public int LastAutoAttackCommandTick { get; protected set; }

        /// <summary>
        ///     Gets or sets the last auto attack tick.
        /// </summary>
        public int LastAutoAttackTick { get; protected set; }

        /// <summary>
        ///     Gets or sets the last movement order tick.
        /// </summary>
        public int LastMovementOrderTick { get; protected set; }

        /// <summary>
        ///     Gets or sets the last target.
        /// </summary>
        public AttackableUnit LastTarget { get; protected set; }

        /// <summary>
        ///     Gets or sets value indicating the amount of executed auto attacks.
        /// </summary>
        public int TotalAutoAttacks { get; protected set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether movement.
        /// </summary>
        protected bool AttackState { get; set; } = true;

        /// <summary>
        ///     Gets or sets value indication in which mode Orbwalk should not run
        /// </summary>
        protected TK InActiveMode { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether missile launched.
        /// </summary>
        protected bool MissileLaunched { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether attack.
        /// </summary>
        protected bool MovementState { get; set; } = true;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Issues the attack order.
        /// </summary>
        /// <param name="target">
        ///     The target to attack.
        /// </param>
        public abstract void Attack(T target);

        /// <summary>
        ///     Indicates whether the orbwalker can issue attacking.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool CanAttack()
        {
            return this.CanAttack(0);
        }

        /// <summary>
        ///     Indicates whether the orbwalker can issue attacking.
        /// </summary>
        /// <param name="extraWindup">
        ///     The extra windup.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanAttack(float extraWindup)
        {
            return Variables.TickCount + (Game.Ping / 2) + 25
                   >= this.LastAutoAttackTick + (GameObjects.Player.AttackDelay * 1000) + extraWindup;
        }

        /// <summary>
        ///     Indicates whether the orbwalker can issue moving.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool CanMove()
        {
            return this.CanMove(0, false);
        }

        /// <summary>
        ///     Indicates whether the orbwalker can issue moving.
        /// </summary>
        /// <param name="extraWindup">
        ///     The extra windup.
        /// </param>
        /// <param name="disableMissileCheck">
        ///     If set to <c>true</c> [disable missile check].
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanMove(float extraWindup, bool disableMissileCheck)
        {
            if (this.MissileLaunched && !disableMissileCheck)
            {
                return true;
            }

            return !GameObjects.Player.CanCancelAutoAttack()
                   || (Variables.TickCount + (Game.Ping / 2)
                       >= this.LastAutoAttackTick + (GameObjects.Player.AttackCastDelay * 1000) + extraWindup);
        }

        /// <summary>
        ///     Indicates whether the target is valid and the orbwalker can attack.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool CanOrbwalk(T target, float range = 0)
        {
            return this.CanOrbwalk(target, range, 0);
        }

        /// <summary>
        ///     Indicates whether the target is valid and the orbwalker can attack.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <param name="extraWindup">
        ///     The extra windup.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public virtual bool CanOrbwalk(T target, float range, float extraWindup)
        {
            return target.IsValidTarget(range > 0 ? range : target.GetRealAutoAttackRange())
                   && this.CanAttack(extraWindup);
        }

        /// <summary>
        ///     Gets the current orbwalker active mode.
        /// </summary>
        /// <returns>
        ///     The active mode.
        /// </returns>
        public TK GetActiveMode()
        {
            return this.ActiveMode;
        }

        /// <summary>
        ///     Gets the target's AA range.
        /// </summary>
        /// <param name="target">
        ///     The target.
        /// </param>
        /// <returns>
        ///     The range.
        /// </returns>
        public float GetAutoAttackRange(T target)
        {
            return target.GetRealAutoAttackRange();
        }

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <returns>
        ///     Returns the target filtered by the orbwalker and/or by the target selector if available.
        /// </returns>
        public abstract T GetTarget();

        /// <summary>
        ///     Issue the move order.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        public abstract void Move(Vector3 position);

        /// <summary>
        ///     <c>Orbwalk</c> command, attempting to attack or move.
        /// </summary>
        /// <param name="target">
        ///     The target of choice
        /// </param>
        /// <param name="position">
        ///     The position of choice
        /// </param>
        public virtual void Orbwalk(T target = null, Vector3? position = null)
        {
            if (this.CanAttack() && this.AttackState && !GameObjects.Player.IsCastingInterruptableSpell())
            {
                var gTarget = target ?? this.GetTarget();
                if (gTarget.InAutoAttackRange())
                {
                    this.Attack(gTarget);
                }
            }

            if (this.CanMove() && this.MovementState && !GameObjects.Player.IsCastingInterruptableSpell(true))
            {
                this.Move(position.HasValue && position.Value.IsValid() ? position.Value : Game.CursorPos);
            }
        }

        /// <summary>
        ///     Resets the swing timer.
        /// </summary>
        public void ResetSwingTimer()
        {
            this.LastAutoAttackTick = 0;
        }

        /// <summary>
        ///     Sets the attack state.
        /// </summary>
        /// <param name="state">
        ///     The state.
        /// </param>
        public void SetAttackState(bool state)
        {
            this.AttackState = state;
        }

        /// <summary>
        ///     Sets the movement state.
        /// </summary>
        /// <param name="state">
        ///     The state.
        /// </param>
        public void SetMovementState(bool state)
        {
            this.MovementState = state;
        }

        /// <summary>
        ///     Indicates whether the depended process should wait before executing.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public abstract bool ShouldWait();

        #endregion

        #region Methods

        /// <summary>
        ///     The <see cref="OnAction" /> invocator.
        /// </summary>
        /// <param name="e">
        ///     The event data.
        /// </param>
        internal void InvokeAction(OrbwalkingActionArgs e)
        {
            this.OnAction?.Invoke(MethodBase.GetCurrentMethod().DeclaringType, e);
        }

        /// <summary>
        ///     OnBuffAdd event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private void ObjAiBaseOnOnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffAddEventArgs args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if (args.Buff.DisplayName == "SonaPassiveReady")
            {
                this.ResetSwingTimer();
            }
        }

        /// <summary>
        ///     OnUpdate event.
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        private void OnGameUpdate(EventArgs args)
        {
            if (GameObjects.Player == null || !GameObjects.Player.IsValid || GameObjects.Player.IsDead || MenuGUI.IsShopOpen)
            {
                return;
            }

            if (!this.InActiveMode.Equals(this.ActiveMode))
            {
                this.Orbwalk();
            }
        }

        /// <summary>
        ///     OnDoCast event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private void OnObjAiBaseDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsValid && sender.IsMe)
            {
                if (Game.Ping <= 30)
                {
                    DelayAction.Add(30, () => this.OnObjAiBaseDoCastDelayed(sender, args));
                }
                else
                {
                    this.OnObjAiBaseDoCastDelayed(sender, args);
                }
            }
        }

        /// <summary>
        ///     Delayed OnDoCast event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private void OnObjAiBaseDoCastDelayed(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (AutoAttack.IsAutoAttackReset(args.SData.Name))
            {
                this.ResetSwingTimer();
            }
            else if (AutoAttack.IsAutoAttack(args.SData.Name))
            {
                this.MissileLaunched = true;
                this.InvokeAction(
                    new OrbwalkingActionArgs
                        { Target = args.Target as AttackableUnit, Sender = sender, Type = OrbwalkingType.AfterAttack });
            }
        }

        /// <summary>
        ///     OnProcessSpellCast event.
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private void OnObjAiBaseProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsValid && sender.IsMe)
            {
                var spellName = args.SData.Name;
                var target = args.Target as AttackableUnit;

                if (target != null && target.IsValid && AutoAttack.IsAutoAttack(spellName))
                {
                    this.LastAutoAttackTick = Variables.TickCount - (Game.Ping / 2);
                    this.MissileLaunched = false;
                    this.LastMovementOrderTick = 0;
                    this.TotalAutoAttacks++;

                    if (!target.Compare(this.LastTarget))
                    {
                        this.InvokeAction(
                            new OrbwalkingActionArgs { Target = target, Type = OrbwalkingType.TargetSwitch });
                        this.LastTarget = target;
                    }

                    this.InvokeAction(
                        new OrbwalkingActionArgs { Target = target, Sender = sender, Type = OrbwalkingType.OnAttack });
                }

                if (AutoAttack.IsAutoAttackReset(spellName))
                {
                    this.ResetSwingTimer();
                }
            }
        }

        /// <summary>
        ///     OnStopCast event.
        /// </summary>
        /// <param name="spellbook">
        ///     The spellbook
        /// </param>
        /// <param name="args">
        ///     The event data
        /// </param>
        private void OnSpellbookStopCast(Spellbook spellbook, SpellbookStopCastEventArgs args)
        {
            if (spellbook.Owner.IsValid && spellbook.Owner.IsMe && args.DestroyMissile && args.StopAnimation)
            {
                this.ResetSwingTimer();
            }
        }

        #endregion
    }

    /// <summary>
    ///     The orbwalking action event data.
    /// </summary>
    public class OrbwalkingActionArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether process.
        /// </summary>
        public bool Process { get; set; }

        /// <summary>
        ///     Gets or sets the sender.
        /// </summary>
        public Obj_AI_Base Sender { get; set; }

        /// <summary>
        ///     Gets the target.
        /// </summary>
        public AttackableUnit Target { get; internal set; }

        /// <summary>
        ///     Gets the type.
        /// </summary>
        public OrbwalkingType Type { get; internal set; }

        #endregion
    }
}