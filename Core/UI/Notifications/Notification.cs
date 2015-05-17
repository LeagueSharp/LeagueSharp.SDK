// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Notification.cs" company="LeagueSharp">
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
//   A basic notification for general purposes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.Notifications
{
    using System;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     A basic notification for general purposes.
    /// </summary>
    public class Notification : NotificationBase
    {
        #region Fields

        /// <summary>
        ///     Notification Click Tick.
        /// </summary>
        private int lastClickTick = Variables.TickCount;

        /// <summary>
        ///     Notification Color Tick.
        /// </summary>
        private int lastColorTick = Variables.TickCount;

        /// <summary>
        ///     Notification Position Tick.
        /// </summary>
        private int lastPositionTick = Variables.TickCount;

        /// <summary>
        ///     Notification Update Tick.
        /// </summary>
        private int lastUpdateTick = Variables.TickCount;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Notification" /> class.
        ///     Constructor
        /// </summary>
        /// <param name="text">
        ///     Display Text
        /// </param>
        /// <param name="duration">
        ///     Duration of notification, -1 for infinite
        /// </param>
        /// <param name="autoDispose">
        ///     Auto dispose at the end of duration
        /// </param>
        public Notification(string text, int duration, bool autoDispose)
        {
            this.Guid = System.Guid.NewGuid().ToString("N");
            this.Text = text;
            this.Duration = duration;
            this.AutoDispose = autoDispose;
            this.Flags =
                this.Flags.SetFlags(
                    NotificationFlags.Draw | NotificationFlags.Update | NotificationFlags.Wpm
                    | NotificationFlags.Initalized);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="Notification" /> class.
        /// </summary>
        ~Notification()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the notification auto-disposes after duration has ended.
        /// </summary>
        public bool AutoDispose { get; set; }

        /// <summary>
        ///     Gets or sets the Notification color decay interval.
        /// </summary>
        public int ColorInterval { get; set; }

        /// <summary>
        ///     Gets or sets the Notification Drawing Position.
        /// </summary>
        public Vector2 DrawPosition { get; set; }

        /// <summary>
        ///     Gets or sets the Notification duration.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        ///     Gets or sets the Notification global unique identification.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        ///     Gets or sets the Notification Speed.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        ///     Gets or sets the Notification display text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Gets or sets the Notification update interval.
        /// </summary>
        public int UpdateInterval { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the Notification Animation ID.
        /// </summary>
        private int AnimationId { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Clone function which clones the notification information and creates a new one, however position and GUID are
        ///     re-generated.
        /// </summary>
        /// <returns>Object in the type of <see cref="Notification" /> class</returns>
        public override object Clone()
        {
            return new Notification(this.Text, this.Duration, this.AutoDispose);
        }

        /// <summary>
        ///     Disposal user-call.
        /// </summary>
        public override sealed void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     Retrieves the global unique identification.
        /// </summary>
        /// <returns>Global Unique Identification</returns>
        public override string GetGuid()
        {
            return this.Guid;
        }

        /// <summary>
        ///     On drawing event callback.
        /// </summary>
        /// <param name="parentPosition">Base Position</param>
        public override void OnDraw(Vector2 parentPosition)
        {
            if (!this.Flags.HasFlag(NotificationFlags.Draw))
            {
                return;
            }
        }

        /// <summary>
        ///     On update event callback.
        /// </summary>
        public override void OnUpdate()
        {
            if (!this.Flags.HasFlag(NotificationFlags.Update)
                || Variables.TickCount - this.lastUpdateTick < this.UpdateInterval)
            {
                return;
            }

            this.lastUpdateTick = Variables.TickCount;

            if (this.Flags.HasFlag(NotificationFlags.Initalized))
            {
                if (!this.DrawPosition.IsValid())
                {
                    this.DrawPosition = this.Position;
                }

                this.Flags = this.Flags.ClearFlags(NotificationFlags.Initalized).SetFlags(NotificationFlags.Animation);
            }
            else if (this.Flags.HasFlag(NotificationFlags.Animation))
            {
                switch (this.AnimationId)
                {
                    case 0:
                        break;
                }

                this.Flags = this.Flags.ClearFlags(NotificationFlags.Animation).SetFlags(NotificationFlags.Idle);
            }
            else if (this.Flags.HasFlag(NotificationFlags.Idle))
            {
                if (this.DrawPosition != this.Position
                    && Variables.TickCount - this.lastPositionTick >= this.Speed * 0.5f)
                {
                    this.DrawPosition = this.DrawPosition.Extend(this.Position, 1f * this.Speed);
                    if (this.DrawPosition.X > this.Position.X)
                    {
                        this.DrawPosition = new Vector2(this.Position.X, this.DrawPosition.Y);
                    }

                    if (this.DrawPosition.Y > this.Position.Y)
                    {
                        this.DrawPosition = new Vector2(this.DrawPosition.X, this.Position.Y);
                    }

                    this.lastPositionTick = Variables.TickCount;
                }
            }
        }

        /// <summary>
        ///     On windows process messages callback.
        /// </summary>
        /// <param name="keys">Converted WindowsKeys</param>
        public override void OnWndProc(WindowsKeys keys)
        {
            if (!this.Flags.HasFlag(NotificationFlags.Wpm))
            {
                return;
            }

            if (Geometry.IsUnderRectangle(
                keys.Cursor, 
                this.DrawPosition.X, 
                this.DrawPosition.Y, 
                this.Width, 
                this.Height) && keys.Msg == WindowsMessages.LBUTTONUP)
            {
                if (Variables.TickCount - this.lastClickTick <= 500)
                {
                    this.Dispose();
                }

                this.lastClickTick = Variables.TickCount;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Safe and managed disposal of the object.
        /// </summary>
        /// <param name="value">Indicates whether the GC finalization called or user-call.</param>
        private void Dispose(bool value)
        {
            if (value)
            {
                if (this.IsValid())
                {
                    this.RemoveNotification();
                }

                this.Guid = this.Text = string.Empty;
                this.Position = this.DrawPosition = Vector2.Zero;
                this.AutoDispose = false;
                this.Flags = 0;
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}