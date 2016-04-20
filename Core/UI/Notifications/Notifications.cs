// <copyright file="Notifications.cs" company="LeagueSharp">
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

    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI;
    using LeagueSharp.SDK.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     The notifications main handler.
    /// </summary>
    public class Notifications
    {
        #region Static Fields

        /// <summary>
        ///     The notifications.
        /// </summary>
        public static readonly List<ANotification> NotificationsList = new List<ANotification>();

        /// <summary>
        ///     The line.
        /// </summary>
        private static Line line;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the position.
        /// </summary>
        public static Vector2 Position { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether edit button down.
        /// </summary>
        private static bool EditButtonDown { get; set; }

        /// <summary>
        ///     Gets the line.
        /// </summary>
        private static Line Line
        {
            get
            {
                if (line != null)
                {
                    return line;
                }

                return line = new Line(Drawing.Direct3DDevice) { Antialias = false, GLLines = true };
            }
        }

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        private static Menu Menu { get; set; }

        /// <summary>
        ///     Gets or sets the mouse location.
        /// </summary>
        private static Vector2? MouseLocation { get; set; }

        /// <summary>
        ///     Gets or sets the mouse offset x.
        /// </summary>
        private static float MouseOffsetX { get; set; }

        /// <summary>
        ///     Gets or sets the mouse offset y.
        /// </summary>
        private static float MouseOffsetY { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Adds a notification towards the list.
        /// </summary>
        /// <param name="notification">
        ///     The notification.
        /// </param>
        public static void Add(ANotification notification)
        {
            if (!NotificationsList.Contains(notification))
            {
                NotificationsList.Add(notification);
            }
        }

        /// <summary>
        ///     Initializes static members of the <see cref="Notifications" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>
        public static void Initialize(Menu menu)
        {
            Events.OnLoad += (sender, args) =>
                {
                    Menu = new Menu("notifications", "Notifications");

                    Menu.Add(new MenuSeparator("editor", "Edit Options"));
                    Menu.Add(new MenuBool("edit", "Edit Position"));
                    Menu.Add(new MenuSeparator("animation", "Animation Options"));
                    Menu.Add(new MenuBool("animations", "Enable Animations") { Value = true });
                    Menu.Add(new MenuBool("flash", "Enable Flash-Animations") { Value = false });
                    Menu.Add(new MenuSeparator("other", "Other Options"));
                    Menu.Add(new MenuBool("autoOpen", "Open Notifications"));
                    Menu.Add(new MenuBool("sticky", "Allow Notification Sticky"));

                    menu.Add(Menu);

                    Position = new Vector2(Drawing.Width - 5f, 90f);
                    Game.OnUpdate += OnUpdate;
                    Drawing.OnDraw += OnDraw;
                    Game.OnWndProc += OnWndProc;
                };
        }

        /// <summary>
        ///     Removes a notification from the list.
        /// </summary>
        /// <param name="notification">
        ///     The notification.
        /// </param>
        public static void Remove(Notification notification)
        {
            if (NotificationsList.Contains(notification))
            {
                NotificationsList.Remove(notification);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     OnDraw event.
        /// </summary>
        /// <param name="args">
        ///     The event data.
        /// </param>
        private static void OnDraw(EventArgs args)
        {
            var height = Position.Y;
            for (var i = 0; i < NotificationsList.Count; ++i)
            {
                NotificationsList[i].OnDraw(new Vector2(Position.X, height));
                if (i < NotificationsList.Count)
                {
                    height += NotificationsList[i].GetReservedHeight();
                }
            }

            if (Menu["edit"].GetValue<MenuBool>().Value)
            {
                var notification = NotificationsList.MaxOrDefault(n => n.GetReservedWidth());
                var width = notification?.GetReservedWidth() ?? 300f;
                if (Math.Abs(height - Position.Y) < float.Epsilon)
                {
                    height += 30f;
                }

                Line.Width = width;
                Line.Begin();
                Line.Draw(
                    new[]
                        {
                            new Vector2(Position.X - (line.Width / 2f), Position.Y),
                            new Vector2(Position.X - (line.Width / 2f), height)
                        },
                    new ColorBGRA(255, 0, 0, 255 / 2));
                Line.End();
            }
        }

        /// <summary>
        ///     OnUpdate event.
        /// </summary>
        /// <param name="args">
        ///     The event data.
        /// </param>
        private static void OnUpdate(EventArgs args)
        {
            foreach (var notification in NotificationsList.ToArray())
            {
                notification.OnUpdate();
            }
        }

        /// <summary>
        ///     <c>OnWndProc</c> event.
        /// </summary>
        /// <param name="args">
        ///     The event data.
        /// </param>
        private static void OnWndProc(WndEventArgs args)
        {
            var windowsKeys = new WindowsKeys(args);
            var height = Position.Y;
            var edit = Menu["edit"].GetValue<MenuBool>().Value;

            foreach (var notification in NotificationsList.ToArray())
            {
                notification.OnWndProc(new Vector2(Position.X, height), windowsKeys, edit);
                height += notification.GetReservedHeight();
            }

            var notificationW = NotificationsList.MaxOrDefault(n => n.GetReservedWidth());
            var widthRectangle = notificationW?.GetReservedWidth() ?? 300f;
            if (windowsKeys.Msg == WindowsMessages.LBUTTONDOWN || windowsKeys.Msg == WindowsMessages.LBUTTONUP)
            {
                var heightRectangle =
                    NotificationsList.Where((t, i) => i < NotificationsList.Count).Sum(t => t.GetReservedHeight());
                if (Math.Abs(heightRectangle) < float.Epsilon)
                {
                    heightRectangle = 30f;
                }

                var value = windowsKeys.Msg == WindowsMessages.LBUTTONDOWN;

                EditButtonDown = value
                                 && windowsKeys.Cursor.IsUnderRectangle(
                                     Position.X - widthRectangle,
                                     Position.Y,
                                     widthRectangle,
                                     heightRectangle);

                if (!value)
                {
                    MouseLocation = null;
                    MouseOffsetX = 0f;
                }
            }

            if (edit && EditButtonDown)
            {
                if (MouseLocation.HasValue)
                {
                    Position += MouseLocation.Value - Position;
                    Position = new Vector2(Position.X + MouseOffsetX, Position.Y + MouseOffsetY);
                }
                else
                {
                    MouseOffsetX = Position.X - windowsKeys.Cursor.X;
                    MouseOffsetY = Position.Y - windowsKeys.Cursor.Y;
                }

                MouseLocation = windowsKeys.Cursor;
            }
        }

        #endregion
    }
}