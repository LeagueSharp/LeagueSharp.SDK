// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuSeparator.cs" company="LeagueSharp">
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
//   A menu separator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.Values
{
    using LeagueSharp.SDK.Core.UI.Abstracts;
    using LeagueSharp.SDK.Core.UI.Skins;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     A menu separator.
    /// </summary>
    public class MenuSeparator : AMenuValue
    {
        #region Public Properties

        /// <summary>
        ///     Menu Value Position.
        /// </summary>
        public override Vector2 Position { get; set; }

        /// <summary>
        ///     Value Width.
        /// </summary>
        public override int Width
        {
            get
            {
                return 0;
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
        }

        /// <summary>
        ///     Drawing callback.
        /// </summary>
        /// <param name="component">Parent Component</param>
        /// <param name="position">The Position</param>
        /// <param name="index">Item Index</param>
        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            if (!this.Position.Equals(position))
            {
                this.Position = position;
            }

            var animation = ThemeManager.Current.Boolean.Animation;

            if (animation != null && animation.IsAnimating())
            {
                animation.OnDraw(component, position, index);

                return;
            }

            ThemeManager.Current.Separator.OnDraw(component, position, index);
        }

        /// <summary>
        ///     Windows Process Messages callback.
        /// </summary>
        /// <param name="args"><see cref="WindowsKeys" /> data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            // not needed                        
        }

        #endregion
    }
}