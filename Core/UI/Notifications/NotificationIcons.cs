// <copyright file="NotificationIcons.cs" company="LeagueSharp">
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
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    using LeagueSharp.SDKEx.Enumerations;
    using LeagueSharp.SDKEx.Properties;

    using SharpDX.Direct3D9;

    /// <summary>
    ///     The notification icons.
    /// </summary>
    public class NotificationIcons
    {
        #region Static Fields

        /// <summary>
        ///     The icon bitmaps.
        /// </summary>
        private static readonly IDictionary<NotificationIconType, Bitmap> IconBitmaps =
            new Dictionary<NotificationIconType, Bitmap>
                {
                    { NotificationIconType.Error, Resources.notifications_error },
                    { NotificationIconType.Warning, Resources.notifications_warning },
                    { NotificationIconType.Check, Resources.notifications_check },
                    { NotificationIconType.Select, Resources.notifications_select }
                };

        /// <summary>
        ///     The icon textures.
        /// </summary>
        private static readonly IDictionary<NotificationIconType, Texture> IconTextures =
            new Dictionary<NotificationIconType, Texture>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="NotificationIcons" /> class.
        /// </summary>
        static NotificationIcons()
        {
            foreach (var bitmap in IconBitmaps.Where(bitmap => !IconTextures.ContainsKey(bitmap.Key)))
            {
                IconTextures.Add(
                    bitmap.Key,
                    Texture.FromMemory(
                        Drawing.Direct3DDevice,
                        (byte[])new ImageConverter().ConvertTo(bitmap.Value, typeof(byte[])),
                        bitmap.Value.Width,
                        bitmap.Value.Height,
                        0,
                        Usage.None,
                        Format.A1,
                        Pool.Managed,
                        Filter.Default,
                        Filter.Default,
                        0));
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Retrieves the icon texture from the existing notification icons.
        /// </summary>
        /// <param name="iconType">
        ///     The icon type
        /// </param>
        /// <returns>
        ///     The <see cref="Texture" />.
        /// </returns>
        public static Texture GetIcon(NotificationIconType iconType)
        {
            Texture texture;
            return IconTextures.TryGetValue(iconType, out texture) ? texture : null;
        }

        #endregion
    }
}