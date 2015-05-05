using System;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI.Notifications
{
    /// <summary>
    ///     A basic notification for general purposes.
    /// </summary>
    public class Notification : INotification
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
        ///     Notification Position in the global list.
        /// </summary>
        public int NotifcationPosition { get; set; }

        /// <summary>
        ///     Notification Position.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        ///     Notification Drawing Position.
        /// </summary>
        public Vector2 DrawPosition { get; set; }

        /// <summary>
        ///     Notification auto-disposal after duration has ended.
        /// </summary>
        public bool AutoDispose { get; set; }

        /// <summary>
        ///     On drawing event callback.
        /// </summary>
        public void OnDraw()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     On update event callback.
        /// </summary>
        public void OnUpdate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     On windows process messages callback.
        /// </summary>
        /// <param name="keys">Converted windowskeys</param>
        public void OnWndProc(WindowsKeys keys)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Sets the notification list position.
        /// </summary>
        /// <param name="position">New position</param>
        public void SetPosition(int position)
        {
            NotifcationPosition = position;
        }

        /// <summary>
        ///     Retrieves the notification list position.
        /// </summary>
        /// <returns>Notification list position</returns>
        public int GetPosition()
        {
            return NotifcationPosition;
        }

        /// <summary>
        ///     Retrieves the notification global unique identification.
        /// </summary>
        /// <returns>Notification GUID</returns>
        public string GetGuid()
        {
            return Guid;
        }

        /// <summary>
        ///     Disposal usercall.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     Clone function which clones the notification information and creates a new one, however position and guid are
        ///     re-generated.
        /// </summary>
        /// <returns>Object in the type of <see cref="Notification" /> class</returns>
        public object Clone()
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
                Duration = NotifcationPosition = 0;
                Position = DrawPosition = Vector2.Zero;
                AutoDispose = false;
            }
        }
    }
}