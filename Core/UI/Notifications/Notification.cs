using System;

using LeagueSharp.CommonEx.Core.Extensions;
using LeagueSharp.CommonEx.Core.Extensions.SharpDX;
using LeagueSharp.CommonEx.Core.Math;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI.Notifications
{
    using LeagueSharp.SDK.Core.Enumerations;

    /// <summary>
    ///     A basic notification for general purposes.
    /// </summary>
    public class Notification : NotificationBase
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="text">Display Text</param>
        /// <param name="duration">Duration of notification, -1 for infinite</param>
        /// <param name="autoDispose">Auto dispose at the end of duration</param>
        public Notification(string text, int duration, bool autoDispose)
        {
            Guid = System.Guid.NewGuid().ToString("N");
            Text = text;
            Duration = duration;
            AutoDispose = autoDispose;
            Flags =
                Flags.SetFlags(
                    NotificationFlags.Draw | NotificationFlags.Update | NotificationFlags.WPM |
                    NotificationFlags.Initalized);
        }

        /// <summary>
        ///     Notification global unique identification.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        ///     Notification display text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///     Notification duration.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        ///     Notification Drawing Position.
        /// </summary>
        public Vector2 DrawPosition { get; set; }

        /// <summary>
        ///     Notification auto-disposal after duration has ended.
        /// </summary>
        public bool AutoDispose { get; set; }

        /// <summary>
        ///     Notification color decay interval.
        /// </summary>
        public int ColorInterval { get; set; }

        /// <summary>
        ///     Notification update interval.
        /// </summary>
        public int UpdateInterval { get; set; }

        /// <summary>
        ///     Notification Position Tick.
        /// </summary>
        private int _lastPositionTick = Variables.TickCount;

        /// <summary>
        ///     Notification Color Tick.
        /// </summary>
        private int _lastColorTick = Variables.TickCount;

        /// <summary>
        ///     Notification Update Tick.
        /// </summary>
        private int _lastUpdateTick = Variables.TickCount;

        /// <summary>
        ///     Notification Click Tick.
        /// </summary>
        private int _lastClickTick = Variables.TickCount;

        /// <summary>
        ///     Notification Animation ID.
        /// </summary>
        private int AnimationId { get; set; }

        /// <summary>
        ///     Notification Speed.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        ///     On drawing event callback.
        /// </summary>
        /// <param name="parentPosition">Base Position</param>
        public override void OnDraw(Vector2 parentPosition)
        {
            if (!Flags.HasFlag(NotificationFlags.Draw))
            {
                return;
            }
        }

        /// <summary>
        ///     On update event callback.
        /// </summary>
        public override void OnUpdate()
        {
            if (!Flags.HasFlag(NotificationFlags.Update) || Variables.TickCount - _lastUpdateTick < UpdateInterval)
            {
                return;
            }
            _lastUpdateTick = Variables.TickCount;

            if (Flags.HasFlag(NotificationFlags.Initalized))
            {
                if (!DrawPosition.IsValid())
                {
                    DrawPosition = Position;
                }

                Flags = Flags.ClearFlags(NotificationFlags.Initalized).SetFlags(NotificationFlags.Animation);
            }
            else if (Flags.HasFlag(NotificationFlags.Animation))
            {
                switch (AnimationId)
                {
                    
                }

                Flags = Flags.ClearFlags(NotificationFlags.Animation).SetFlags(NotificationFlags.Idle);
            }
            else if (Flags.HasFlag(NotificationFlags.Idle))
            {
                if (DrawPosition != Position && Variables.TickCount - _lastPositionTick >= Speed * 0.5f)
                {
                    DrawPosition = DrawPosition.Extend(Position, 1f * Speed);
                    if (DrawPosition.X > Position.X)
                    {
                        DrawPosition = new Vector2(Position.X, DrawPosition.Y);
                    }
                    if (DrawPosition.Y > Position.Y)
                    {
                        DrawPosition = new Vector2(DrawPosition.X, Position.Y);
                    }
                    _lastPositionTick = Variables.TickCount;
                }
            }
        }

        /// <summary>
        ///     On windows process messages callback.
        /// </summary>
        /// <param name="keys">Converted windowskeys</param>
        public override void OnWndProc(WindowsKeys keys)
        {
            if (!Flags.HasFlag(NotificationFlags.WPM))
            {
                return;
            }

            if (Geometry.IsUnderRectangle(keys.Cursor, DrawPosition.X, DrawPosition.Y, Width, Height) && keys.Msg == WindowsMessages.LBUTTONUP)
            {
                if (Variables.TickCount - _lastClickTick <= 500)
                {
                    Dispose();
                }
                _lastClickTick = Variables.TickCount;
            }
        }

        /// <summary>
        ///     Retrieves the global unique identification.
        /// </summary>
        /// <returns>Global Unique Identification</returns>
        public override string GetGuid()
        {
            return Guid;
        }

        /// <summary>
        ///     Disposal usercall.
        /// </summary>
        public override sealed void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Clone function which clones the notification information and creates a new one, however position and guid are
        ///     re-generated.
        /// </summary>
        /// <returns>Object in the type of <see cref="Notification" /> class</returns>
        public override object Clone()
        {
            return new Notification(Text, Duration, AutoDispose);
        }

        /// <summary>
        ///     Finailization of the class.
        /// </summary>
        ~Notification()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Safe and managed disposal of the object.
        /// </summary>
        /// <param name="value">Indicates whether the GC finailization called or usercall.</param>
        private void Dispose(bool value)
        {
            if (value)
            {
                if (this.IsValid())
                {
                    this.RemoveNotification();
                }
                Guid = Text = "";
                Position = DrawPosition = Vector2.Zero;
                AutoDispose = false;
                Flags = 0;
                GC.SuppressFinalize(this);
            }
        }
    }
}