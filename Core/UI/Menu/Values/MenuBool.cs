// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuBool.cs" company="LeagueSharp">
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
//   Menu boolean.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.Values
{
    using System;
    using System.Runtime.Serialization;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Skins;
    using LeagueSharp.SDK.Core.UI.Skins.Default;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Menu boolean.
    /// </summary>
    [Serializable]
    public class MenuBool : AMenuValue, ISerializable
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuBool" /> class.
        /// </summary>
        /// <param name="value">
        ///     Boolean Value
        /// </param>
        public MenuBool(bool value = false)
        {
            this.Value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuBool" /> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public MenuBool(SerializationInfo info, StreamingContext context)
        {
            this.Value = (bool)info.GetValue("value", typeof(bool));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Boolean Item Position.
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the boolean value is true or false.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        ///     Boolean Item Width requirement.
        /// </summary>
        public override int Width
        {
            get
            {
                return DefaultSettings.ContainerHeight;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Extracts the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        public override void Extract(AMenuValue component)
        {
            this.Value = ((MenuBool)component).Value;
        }

        /// <summary>
        ///     Gets the object data.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("value", this.Value, typeof(bool));
        }

        /// <summary>
        ///     Boolean Item Draw callback.
        /// </summary>
        /// <param name="component">Parent Component</param>
        /// <param name="position">The Position</param>
        /// <param name="index">Item Index</param>
        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            var animation = ThemeManager.Current.Boolean.Animation;

            if (animation != null && animation.IsAnimating())
            {
                animation.OnDraw(component, position, index);

                return;
            }

            ThemeManager.Current.Boolean.OnDraw(component, position, index);
        }

        /// <summary>
        ///     Boolean Item Windows Process Messages callback.
        /// </summary>
        /// <param name="args">The <see cref="WindowsKeys" /> instance</param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!this.Container.Visible)
            {
                return;
            }

            if (args.Msg == WindowsMessages.LBUTTONDOWN && this.Position.IsValid())
            {
                var rect = ThemeManager.Current.Boolean.AdditionalBoundries(this.Position, this.Container);

                if (args.Cursor.IsUnderRectangle(rect.X, rect.Y, rect.Width, rect.Height))
                {
                    this.Value = !this.Value;
                    FireEvent();
                }
            }
        }

        #endregion
    }
}