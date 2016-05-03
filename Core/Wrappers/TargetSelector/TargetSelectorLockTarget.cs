// <copyright file="TargetSelectorLockTarget.cs" company="LeagueSharp">
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
    using System.Drawing;
    using System.Linq;

    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using static Drawing;

    /// <summary>
    ///     The lock target option.
    /// </summary>
    public class TargetSelectorLockTarget
    {
        #region Constants

        /// <summary>
        ///     Use this to clear any timer in this class
        /// </summary>
        private const double ClearTime = 0F;

        /// <summary>
        ///     The max range allowed to lock targets
        /// </summary>
        private const double MaxRange = 2500F;

        #endregion

        #region Fields

        /// <summary>
        ///     Internal field that represents the targeted champion
        /// </summary>
        private Obj_AI_Hero lockedTarget;

        /// <summary>
        ///     Internal field to control when the must be released
        /// </summary>
        private double lockedTil;

        /// <summary>
        ///     Internal field that represents the menu:
        ///     Target Selector >> Lock target >> Enabled
        /// </summary>
        private MenuBool menuItemEnabled;

        /// <summary>
        ///     Internal field that represents the menu:
        ///     Target Selector >> Lock target >> Show notification
        /// </summary>
        private MenuBool menuItemNotifications;

        /// <summary>
        ///     Internal field that represents the menu:
        ///     Target Selector >> Lock target >> Show countdown
        /// </summary>
        private MenuBool menuItemShowCountdown;

        /// <summary>
        ///     Internal field that represents the menu:
        ///     Target Selector >> Lock target >> Lock target on MIA (ms)
        /// </summary>
        private MenuSlider menuItemTime;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TargetSelectorLockTarget" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The root menu.
        /// </param>
        public TargetSelectorLockTarget(Menu menu)
        {
            this.LoadMenu(menu);

            this.LockedTarget = null;

            Game.OnWndProc += this.Game_OnWndProc_SetTarget;
            OnDraw += this.Drawing_OnDraw_Countdown;
            OnDraw += this.Drawing_OnDraw_Target;
            Game.OnUpdate += this.Game_OnUpdate_DeadTarget;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the value setted on menu:
        ///     Target Selector >> Lock target >> Enabled
        /// </summary>
        public bool Enabled => this.menuItemEnabled.Value;

        /// <summary>
        ///     Gets or sets the targeted champion.
        /// </summary>
        public Obj_AI_Hero LockedTarget
        {
            get
            {
                return this.lockedTarget;
            }

            private set
            {
                this.lockedTil = ClearTime;
                this.lockedTarget = value;

                if (value != null)
                {
                    Game.OnUpdate += this.Game_OnUpdate_Trigger;
                }
                else
                {
                    Game.OnUpdate -= this.Game_OnUpdate_Trigger;
                    Game.OnUpdate -= this.Game_OnUpdate_CheckTargeted;
                }
            }
        }

        /// <summary>
        ///     Gets the value setted on menu:
        ///     Target Selector >> Lock target >> Show countdown
        /// </summary>
        public bool ShowCountdown => this.menuItemShowCountdown.Value;

        /// <summary>
        ///     Gets the value setted on menu:
        ///     Target Selector >> Lock target >> Show notification
        /// </summary>
        public bool ShowNotification => this.menuItemNotifications.Value;

        /// <summary>
        ///     Gets the value setted on menu:
        ///     Target Selector >> Lock target >> Lock target on MIA (ms)
        /// </summary>
        public int Time => this.menuItemTime.Value;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Lock an especific champion.
        /// </summary>
        /// <param name="target">
        ///     The targeted champion.
        /// </param>
        public void LockTarget(Obj_AI_Hero target)
        {
            this.LockTarget(target, true);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Draws the countdown animation
        /// </summary>
        /// <param name="args"></param>
        private void Drawing_OnDraw_Countdown(EventArgs args)
        {
            if (!this.ShowCountdown || this.LockedTarget == null || this.LockedTarget.IsDead
                || this.lockedTil.Equals(ClearTime))
            {
                return;
            }

            var textPosition = ObjectManager.Player.HPBarPosition;
            textPosition.Y += 160F;

            double time = Convert.ToInt32((this.lockedTil - Game.Time) * 1000);
            time = time / this.Time;
            time = time * 100F;
            time = Convert.ToInt32(time);
            time = 100 - time;

            var line1 = "MIA Locked target";

            var i = Convert.ToInt32(time / 10);
            var line2 = "";
            line2 = line2.PadRight(i, 'o');
            line2 = line2.PadRight(10, '_');
            line2 = $"[{line2}] {time}%";

            var layer1 = "[oooooooooo]";
            var layer3 = "[__________]";

            DrawText(textPosition.X + 8F, textPosition.Y, Color.White, line1);
            DrawText(textPosition.X + 5F, textPosition.Y + 20F, Color.Black, layer1);
            DrawText(textPosition.X + 5F, textPosition.Y + 20F, Color.White, line2);
            DrawText(textPosition.X + 5F, textPosition.Y + 20F, Color.Red, layer3);
        }

        /// <summary>
        ///     Draws the red and white target under the locked champion.
        /// </summary>
        /// <param name="args"></param>
        private void Drawing_OnDraw_Target(EventArgs args)
        {
            if (this.LockedTarget == null || !this.LockedTarget.IsVisible)
            {
                return;
            }

            var position = this.LockedTarget.Position;
            position.Y -= 120F;
            position.X += 10F;

            Render.Circle.DrawCircle(position, 30, Color.Red, 5);
            Render.Circle.DrawCircle(position, 20, Color.White, 5);
            Render.Circle.DrawCircle(position, 10, Color.Red, 5);
            Render.Circle.DrawCircle(position, 0, Color.White, 5);
        }

        /// <summary>
        ///     Checks if the targeted champion is MIA or out of range.
        /// </summary>
        /// <param name="args"></param>
        private void Game_OnUpdate_CheckTargeted(EventArgs args)
        {
            if (this.LockedTarget != null && !this.lockedTil.Equals(ClearTime) && Game.Time > this.lockedTil)
            {
                var championName = this.LockedTarget.ChampionName;
                Game.OnUpdate -= this.Game_OnUpdate_CheckTargeted;

                this.LockTarget(null, false);

                this.Notify(
                    $"Target released - {championName}",
                    $"{championName} is MIA or out of range for {this.Time}ms.");
            }
        }

        /// <summary>
        ///     Remove the red and white target under the dead champion.
        /// </summary>
        /// <param name="args"></param>
        private void Game_OnUpdate_DeadTarget(EventArgs args)
        {
            if (this.LockedTarget != null && this.LockedTarget.IsDead)
            {
                this.lockedTil = ClearTime;
                this.LockTarget(null, false);
            }
        }

        /// <summary>
        ///     Triggers the lock event.
        /// </summary>
        /// <param name="args"></param>
        private void Game_OnUpdate_Trigger(EventArgs args)
        {
            if (this.LockedTarget == null)
            {
                return;
            }

            if (this.lockedTil.Equals(ClearTime)
                && (!this.LockedTarget.IsVisible || GameObjects.Player.Distance(this.lockedTarget) > MaxRange))
            {
                this.lockedTil = Game.Time + ((double)this.Time / 1000);
                Game.OnUpdate += this.Game_OnUpdate_CheckTargeted;
            }
            else if (this.LockedTarget.IsVisible && GameObjects.Player.Distance(this.lockedTarget) <= MaxRange)
            {
                Game.OnUpdate -= this.Game_OnUpdate_CheckTargeted;
                this.lockedTil = ClearTime;
            }
        }

        /// <summary>
        ///     Gets the double clicked champion and Locks him.
        /// </summary>
        /// <param name="args"></param>
        private void Game_OnWndProc_SetTarget(WndEventArgs args)
        {
            if (args.Msg != (ulong)WindowsMessages.LBUTTONDBLCLK)
            {
                return;
            }

            var selection =
                GameObjects.EnemyHeroes.Where(
                    h => h.IsValidTarget() && h.Distance(Game.CursorPos) < h.BoundingRadius + 150F)
                    .OrderBy(h => h.Distance(Game.CursorPos))
                    .FirstOrDefault();

            if (this.LockedTarget != selection)
            {
                var notify = true;

                if (selection == null)
                {
                    this.Notify($"Target released - {this.LockedTarget.ChampionName}", "You released the target.");
                    notify = false;
                }

                this.LockTarget(selection, notify);
            }
        }

        /// <summary>
        ///     Load the Lock target menu.
        /// </summary>
        /// <param name="menu">
        ///     The root menu.
        /// </param>
        private void LoadMenu(Menu menu)
        {
            var lockTargetMenu = new Menu("lockTarget", "Lock target");

            this.menuItemEnabled = new MenuBool("lockTargetEnabled", "Enabled", true);
            lockTargetMenu.Add(this.menuItemEnabled);

            this.menuItemTime = new MenuSlider("lockTargetTime", "Lock target on MIA (ms)", 1500, 500, 5000);
            lockTargetMenu.Add(this.menuItemTime);

            this.menuItemShowCountdown = new MenuBool("lockTargetCountdown", "Show countdown", true);
            lockTargetMenu.Add(this.menuItemShowCountdown);

            this.menuItemNotifications = new MenuBool("lockTargetNotification", "Show notification", true);
            lockTargetMenu.Add(this.menuItemNotifications);

            menu.Add(lockTargetMenu);
        }

        /// <summary>
        ///     Private method to lock targets and notify.
        /// </summary>
        /// <param name="target">
        ///     The targeted champion.
        /// </param>
        /// <param name="notify">
        ///     Indicates if the event must be notified.
        /// </param>
        private void LockTarget(Obj_AI_Hero target, bool notify)
        {
            if (!this.Enabled)
            {
                return;
            }

            this.LockedTarget = target;

            if (!notify)
            {
                return;
            }

            if (target != null)
            {
                this.Notify(
                    $"Target locked - {this.LockedTarget.ChampionName}",
                    $"Your damage will be concentrated on {this.LockedTarget.ChampionName}.");
            }
            else
            {
                this.Notify("Target released", "You released the target.");
            }
        }

        /// <summary>
        ///     Notify an event.
        /// </summary>
        /// <param name="header">
        ///     The header text.
        /// </param>
        /// <param name="body">
        ///     The body text.
        /// </param>
        private void Notify(string header, string body)
        {
            if (!this.ShowNotification)
            {
                return;
            }

            var targetReleasedNotification = new Notification(
                header,
                body + "\n\nTo disable this feature goto: Menu Leaguesharp > \nTarget Selector > Lock target > Enabled")
                                                 {
                                                     HeaderTextColor = SharpDX.Color.White,
                                                     BodyTextColor = SharpDX.Color.BlanchedAlmond,
                                                     Icon = NotificationIconType.None, IconFlash = false,
                                                 };
            Notifications.Add(targetReleasedNotification);
            DelayAction.Add(6000, () => { Notifications.Remove(targetReleasedNotification); });
        }

        #endregion
    }
}