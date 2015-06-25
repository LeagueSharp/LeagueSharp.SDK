// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultList.cs" company="LeagueSharp">
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
//   A default implementation of a <see cref="IDrawableList" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LeagueSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using System.Collections.Generic;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of a <see cref="IDrawableList" />
    /// </summary>
    public class DefaultList : DefaultComponent, IDrawableList
    {
        #region Constants

        /// <summary>
        ///     The arrow spacing.
        /// </summary>
        private const int ArrowSpacing = 6;

        /// <summary>
        ///     The text spacing.
        /// </summary>
        private const int TextSpacing = 8;

        #endregion

        #region Fields

        /// <summary>
        ///     The drop down button width.
        /// </summary>
        private readonly int dropDownButtonWidth;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultList" /> class.
        ///     Creates a new instance of a DefaultList
        /// </summary>
        public DefaultList()
        {
            var arrowSize = MenuSettings.Font.MeasureText(null, "V", 0);
            this.dropDownButtonWidth = arrowSize.Width + (2 * ArrowSpacing);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draw a <see cref="MenuList" />
        /// </summary>
        /// <param name="component">The <see cref="MenuList" /></param>
        public virtual void Draw(MenuList component)
        {
            var dropdownMenuWidth = this.dropDownButtonWidth + (2 * TextSpacing) + component.MaxStringWidth;
            var position = component.Position;
            var rectangleName = GetContainerRectangle(component)
                .GetCenteredText(null, MenuSettings.Font, component.DisplayName, CenteredFlags.VerticalCenter);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.DisplayName, 
                (int)(position.X + MenuSettings.ContainerTextOffset), 
                (int)rectangleName.Y, 
                MenuSettings.TextColor);

            var line = new Line(Drawing.Direct3DDevice)
                           {
                              Antialias = false, GLLines = false, Width = this.dropDownButtonWidth 
                           };

            line.Begin();
            line.Draw(
                new[]
                    {
                        new Vector2(
                            position.X + component.MenuWidth - (this.dropDownButtonWidth / 2f), 
                            position.Y + 1), 
                        new Vector2(
                            position.X + component.MenuWidth - (this.dropDownButtonWidth / 2f), 
                            position.Y + MenuSettings.ContainerHeight)
                    }, 
                MenuSettings.HoverColor);
            line.End();
            if (component.Hovering || component.Active)
            {
                line.Width = MenuSettings.ContainerHeight;
                line.Begin();
                line.Draw(
                    new[]
                        {
                            new Vector2(
                                position.X + component.MenuWidth - dropdownMenuWidth, 
                                position.Y + line.Width / 2), 
                            new Vector2(position.X + component.MenuWidth, position.Y + line.Width / 2)
                        }, 
                    MenuSettings.HoverColor);
                line.End();
            }

            line.Dispose();
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                "V", 
                (int)(position.X + component.MenuWidth - this.dropDownButtonWidth + ArrowSpacing), 
                (int)rectangleName.Y, 
                MenuSettings.TextColor);
            MenuSettings.ContainerSeparatorLine.Draw(
                new[]
                    {
                        new Vector2(
                            position.X + component.MenuWidth - this.dropDownButtonWidth - 1, 
                            position.Y + 1), 
                        new Vector2(
                            position.X + component.MenuWidth - this.dropDownButtonWidth - 1, 
                            position.Y + MenuSettings.ContainerHeight)
                    }, 
                MenuSettings.ContainerSeparatorColor);
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                component.SelectedValueAsObject.ToString(), 
                (int)position.X + component.MenuWidth - this.dropDownButtonWidth - TextSpacing
                - component.MaxStringWidth, 
                (int)rectangleName.Y, 
                MenuSettings.TextColor);
            MenuSettings.ContainerSeparatorLine.Draw(
                new[]
                    {
                        new Vector2(
                            position.X + component.MenuWidth - this.dropDownButtonWidth - (2 * TextSpacing)
                            - component.MaxStringWidth, 
                            position.Y + 1), 
                        new Vector2(
                            position.X + component.MenuWidth - this.dropDownButtonWidth - (2 * TextSpacing)
                            - component.MaxStringWidth, 
                            position.Y + MenuSettings.ContainerHeight)
                    }, 
                MenuSettings.ContainerSeparatorColor);

            if (component.Active)
            {
                var valueStrings = component.ValuesAsStrings;
                var dropdownMenuHeight = valueStrings.Length * MenuSettings.ContainerHeight;
                MenuManager.Instance.DrawDelayed(
                    delegate
                        {
                            var backgroundLine = new Line(Drawing.Direct3DDevice)
                                                     {
                                                        Width = dropdownMenuWidth, Antialias = false, GLLines = false 
                                                     };
                            backgroundLine.Begin();
                            backgroundLine.Draw(
                                new[]
                                    {
                                        new Vector2(
                                            position.X + component.MenuWidth - (backgroundLine.Width / 2), 
                                            position.Y + MenuSettings.ContainerHeight), 
                                        new Vector2(
                                            position.X + component.MenuWidth - (backgroundLine.Width / 2), 
                                            position.Y + MenuSettings.ContainerHeight + dropdownMenuHeight)
                                    }, 
                                Color.Black);
                            backgroundLine.End();
                            backgroundLine.Dispose();

                            var x =
                                (int)
                                (position.X + component.MenuWidth - dropDownButtonWidth - TextSpacing
                                 - component.MaxStringWidth);
                            var y = (int)rectangleName.Y;
                            for (var i = 0; i < valueStrings.Length; i++)
                            {
                                if (i == component.HoveringIndex)
                                {
                                    var hoverLine = new Line(Drawing.Direct3DDevice)
                                                        {
                                                            Width = MenuSettings.ContainerHeight, Antialias = false, 
                                                            GLLines = false
                                                        };
                                    hoverLine.Begin();
                                    hoverLine.Draw(
                                        new[]
                                            {
                                                new Vector2(
                                                    position.X + component.MenuWidth - dropdownMenuWidth, 
                                                    position.Y + ((i + 1) * MenuSettings.ContainerHeight)
                                                    + MenuSettings.ContainerHeight / 2f), 
                                                new Vector2(
                                                    position.X + component.MenuWidth, 
                                                    position.Y + ((i + 1) * MenuSettings.ContainerHeight)
                                                    + MenuSettings.ContainerHeight / 2f)
                                            }, 
                                        MenuSettings.HoverColor);
                                    hoverLine.End();
                                    hoverLine.Dispose();
                                }

                                MenuSettings.ContainerSeparatorLine.Draw(
                                    new[]
                                        {
                                            new Vector2(
                                                position.X + component.MenuWidth - dropdownMenuWidth, 
                                                position.Y + (MenuSettings.ContainerHeight * (i + 1))), 
                                            new Vector2(
                                                position.X + component.MenuWidth, 
                                                position.Y + (MenuSettings.ContainerHeight * (i + 1)))
                                        }, 
                                    MenuSettings.ContainerSeparatorColor);
                                y += MenuSettings.ContainerHeight;
                                MenuSettings.Font.DrawText(
                                    MenuManager.Instance.Sprite, 
                                    valueStrings[i], 
                                    x, 
                                    y, 
                                    MenuSettings.TextColor);
                                if (component.Index == i)
                                {
                                    var checkmarkWidth = MenuSettings.Font.MeasureText(null, "\u221A", 0).Width;
                                    MenuSettings.Font.DrawText(
                                        MenuManager.Instance.Sprite, 
                                        "\u221A", 
                                        (int)(position.X + component.MenuWidth - checkmarkWidth - TextSpacing), 
                                        y, 
                                        MenuSettings.TextColor);
                                }
                            }

                            MenuSettings.ContainerSeparatorLine.Draw(
                                new[]
                                    {
                                        new Vector2(
                                            position.X + component.MenuWidth - dropdownMenuWidth, 
                                            position.Y + MenuSettings.ContainerHeight), 
                                        new Vector2(
                                            position.X + component.MenuWidth - dropdownMenuWidth, 
                                            position.Y + MenuSettings.ContainerHeight * (valueStrings.Length + 1)), 
                                        new Vector2(
                                            position.X + component.MenuWidth, 
                                            position.Y + MenuSettings.ContainerHeight * (valueStrings.Length + 1)), 
                                        new Vector2(
                                            position.X + component.MenuWidth, 
                                            position.Y + MenuSettings.ContainerHeight)
                                    }, 
                                MenuSettings.ContainerSeparatorColor);
                        });
            }
        }

        /// <summary>
        ///     Gets the dropdown boundaries (preview)
        /// </summary>
        /// <param name="component">The <see cref="MenuList" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle DropDownBoundaries(MenuList component)
        {
            return
                new Rectangle(
                    (int)
                    (component.Position.X + component.MenuWidth - this.dropDownButtonWidth
                     - (2 * TextSpacing) - component.MaxStringWidth), 
                    (int)component.Position.Y, 
                    this.dropDownButtonWidth + (2 * TextSpacing) + component.MaxStringWidth, 
                    MenuSettings.ContainerHeight);
        }

        /// <summary>
        ///     Gets the complete dropdown boundaries
        /// </summary>
        /// <param name="component">The <see cref="MenuList" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle DropDownExpandedBoundaries(MenuList component)
        {
            return
                new Rectangle(
                    (int)
                    (component.Position.X + component.MenuWidth - this.dropDownButtonWidth
                     - (2 * TextSpacing) - component.MaxStringWidth), 
                    (int)component.Position.Y, 
                    this.dropDownButtonWidth + (2 * TextSpacing) + component.MaxStringWidth, 
                    (component.Count + 1) * MenuSettings.ContainerHeight);
        }

        /// <summary>
        ///     Gets the list of dropdown item boundaries.
        /// </summary>
        /// <param name="component">The <see cref="MenuList" /></param>
        /// <returns>List of <see cref="Rectangle" /></returns>
        public List<Rectangle> DropDownListBoundaries(MenuList component)
        {
            var rectangles = new List<Rectangle>();
            for (var i = 0; i < component.Count; i++)
            {
                rectangles.Add(
                    new Rectangle(
                        (int)
                        (component.Position.X + component.MenuWidth - this.dropDownButtonWidth
                         - (2 * TextSpacing) - component.MaxStringWidth), 
                        (int)(component.Position.Y + ((i + 1) * MenuSettings.ContainerHeight)), 
                        this.dropDownButtonWidth + (2 * TextSpacing) + component.MaxStringWidth, 
                        MenuSettings.ContainerHeight + 1));
            }

            return rectangles;
        }

        /// <summary>
        ///     Gets the width of the MenuList
        /// </summary>
        /// <param name="menuList">The <see cref="MenuList" /></param>
        /// <returns>The <see cref="int" /></returns>
        public virtual int Width(MenuList menuList)
        {
            return menuList.MaxStringWidth + (2 * TextSpacing) + this.dropDownButtonWidth;
        }

        #endregion


        public virtual void OnWndProc(MenuList component, Utils.WindowsKeys args)
        {
            if (!component.Visible)
            {
                return;
            }

            var dropdownRect = DropDownBoundaries(component);
            var entireDropdownRect = DropDownExpandedBoundaries(component);

            if (args.Cursor.IsUnderRectangle(dropdownRect.X, dropdownRect.Y, dropdownRect.Width, dropdownRect.Height))
            {
                component.Hovering = true;

                if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    component.Active = !component.Active;
                }
            }
            else
            {
                component.Hovering = false;
            }

            const int Buffer = 20;
            if (component.Active
                && !args.Cursor.IsUnderRectangle(
                    entireDropdownRect.X - Buffer,
                    entireDropdownRect.Y - Buffer,
                    entireDropdownRect.Width + (2 * Buffer),
                    entireDropdownRect.Height + (2 * Buffer)))
            {
                component.Active = false;
            }

            if (component.Active)
            {
                var found = false;
                var dropdownRectangles = DropDownListBoundaries(component);
                for (var i = 0; i < dropdownRectangles.Count; i++)
                {
                    if (args.Cursor.IsUnderRectangle(
                        dropdownRectangles[i].X,
                        dropdownRectangles[i].Y,
                        dropdownRectangles[i].Width,
                        dropdownRectangles[i].Height))
                    {
                        component.HoveringIndex = i;
                        found = true;
                    }
                }

                if (!found)
                {
                    component.HoveringIndex = -1;
                }
                else if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    component.Index = component.HoveringIndex;
                    args.Process = false;
                }
            }
        }
    }
}