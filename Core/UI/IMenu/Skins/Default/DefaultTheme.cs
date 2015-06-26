// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTheme.cs" company="LeagueSharp">
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
//   Implements a default ITheme.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using System.Linq;

    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    using Color = SharpDX.Color;
    using Rectangle = SharpDX.Rectangle;

    /// <summary>
    ///     Implements a default ITheme.
    /// </summary>
    public class DefaultTheme : DefaultComponent, ITheme
    {

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultTheme" /> class.
        ///     Creates a new instance of DefaultTheme
        /// </summary>
        public DefaultTheme()
        {
            Bool = new DefaultBool();
            ColorPicker = new DefaultColorPicker();
            Button = new DefaultButton();
            KeyBind = new DefaultKeyBind();
            List = new DefaultList();
            Separator = new DefaultSeparator();
            Slider = new DefaultSlider();
            Menu = new DefaultMenu();
        }

        #endregion

        #region Public Properties
        
        /// <summary>
        /// Gets the <see cref="IDrawable{MenuBool}"/>
        /// </summary>
        public IDrawable<MenuBool> Bool { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDrawable{MenuButton}"/>
        /// </summary>
        public IDrawable<MenuButton> Button { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDrawable{MenuColor}"/>
        /// </summary>
        public IDrawable<MenuColor> ColorPicker { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDrawable{MenuKeyBind}"/>
        /// </summary>
        public IDrawable<MenuKeyBind> KeyBind { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDrawable{MenuList}"/>
        /// </summary>
        public IDrawable<MenuList> List { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDrawable{MenuSeparator}"/>
        /// </summary>
        public IDrawable<MenuSeparator> Separator { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDrawable{MenuSlider}"/>
        /// </summary>
        public IDrawable<MenuSlider> Slider { get; private set; }

        /// <summary>
        /// Gets the <see cref="IDrawable{Menu}"/>
        /// </summary>
        public IDrawable<Menu> Menu { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     OnDraw event.
        /// </summary>
        public void Draw()
        {
            var position = MenuSettings.Position;
            var menuManager = MenuManager.Instance;
            var height = MenuSettings.ContainerHeight * menuManager.Menus.Count;
            var width = MenuSettings.ContainerWidth;
            if (menuManager.Menus.Count > 0)
            {
                width = menuManager.Menus.First().MenuWidth;
            }

            MenuSettings.ContainerLine.Width = width;
            MenuSettings.ContainerLine.Begin();
            MenuSettings.ContainerLine.Draw(
                new[]
                    {
                        new Vector2(position.X + (width / 2f), position.Y), 
                        new Vector2(position.X + (width / 2), position.Y + height)
                    }, 
                MenuSettings.RootContainerColor);
            MenuSettings.ContainerLine.End();

            for (var i = 0; i < menuManager.Menus.Count; ++i)
            {
                var childPos = new Vector2(position.X, position.Y + i * MenuSettings.ContainerHeight);

                if (i < menuManager.Menus.Count - 1)
                {
                    MenuSettings.ContainerSeparatorLine.Begin();
                    MenuSettings.ContainerSeparatorLine.Draw(
                        new[]
                            {
                                new Vector2(childPos.X, childPos.Y + MenuSettings.ContainerHeight), 
                                new Vector2(
                                    childPos.X + menuManager.Menus[i].MenuWidth, 
                                    childPos.Y + MenuSettings.ContainerHeight)
                            }, 
                        MenuSettings.ContainerSeparatorColor);
                    MenuSettings.ContainerSeparatorLine.End();
                }

                menuManager.Menus[i].OnDraw(childPos);
            }

            var contour = new Line(Drawing.Direct3DDevice) { GLLines = true, Width = 1 };
            contour.Begin();
            contour.Draw(
                new[]
                    {
                        new Vector2(position.X, position.Y), new Vector2(position.X + width, position.Y), 
                        new Vector2(position.X + width, position.Y + height), new Vector2(position.X, position.Y + height), 
                        new Vector2(position.X, position.Y)
                    }, 
                Color.Black);
            contour.End();
            contour.Dispose();
            
        }

        #endregion
    }
}