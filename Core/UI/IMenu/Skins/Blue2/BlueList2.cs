// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlueList2.cs" company="LeagueSharp">
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
//   A custom implementation of a <see cref="ADrawable{MenuList}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LeagueSharp.SDK.UI.Skins.Blue2
{
    using System.Collections.Generic;

    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.UI.Skins.Blue;
    using LeagueSharp.SDK.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of a <see cref="ADrawable{MenuList}" />
    /// </summary>
    public class BlueList2 : BlueList
    {
        #region Constants

        /// <summary>
        ///     The arrow spacing.
        /// </summary>
        private const int ArrowSpacing = 6;

        /// <summary>
        ///     The text spacing.
        /// </summary>
        private const int TextSpacing = 10;

        #endregion

        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice);

        #endregion

        #region Fields

        /// <summary>
        ///     The drop down button width.
        /// </summary>
        private readonly int dropDownButtonWidth;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlueList" /> class.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        public BlueList2(MenuList component)
            : base(component)
        {
            var arrowSize = MenuSettings.Font.MeasureText(null, "V", 0);
            this.dropDownButtonWidth = arrowSize.Width + (2 * ArrowSpacing);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
            // do nothing
        }

        /// <summary>
        ///     Draw a <see cref="MenuList" />
        /// </summary>
        public override void Draw()
        {
            var dropdownMenuWidth = this.dropDownButtonWidth + (2 * TextSpacing) + this.Component.MaxStringWidth;
            var position = this.Component.Position;
            var rectangleName = BlueUtilities.GetContainerRectangle(this.Component)
                .GetCenteredText(
                    null,
                    MenuSettings.Font,
                    MultiLanguage.Translation(this.Component.DisplayName),
                    CenteredFlags.VerticalCenter);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                MultiLanguage.Translation(this.Component.DisplayName),
                (int)(position.X + MenuSettings.ContainerTextOffset),
                (int)rectangleName.Y,
                MenuSettings.TextColor);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                "\u23EC",
                (int)(position.X + this.Component.MenuWidth - this.dropDownButtonWidth + ArrowSpacing),
                (int)rectangleName.Y,
                MenuSettings.TextColor);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                MultiLanguage.Translation(this.Component.SelectedValueAsObject.ToString()),
                (int)position.X + this.Component.MenuWidth - this.dropDownButtonWidth - TextSpacing
                - this.Component.MaxStringWidth,
                (int)rectangleName.Y,
                this.Component.Active ? new ColorBGRA(0, 186, 255, 255) : MenuSettings.TextColor);
            Line.Width = 1f;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(
                            position.X + this.Component.MenuWidth - this.dropDownButtonWidth - (2 * TextSpacing)
                            - this.Component.MaxStringWidth,
                            position.Y + 5),
                        new Vector2(
                            position.X + this.Component.MenuWidth - this.dropDownButtonWidth - (2 * TextSpacing)
                            - this.Component.MaxStringWidth,
                            position.Y + MenuSettings.ContainerHeight - 5)
                    },
                MenuSettings.ContainerSeparatorColor);
            Line.End();

            if (this.Component.Active)
            {
                var valueStrings = this.Component.ValuesAsStrings;
                var dropdownMenuHeight = valueStrings.Length * MenuSettings.ContainerHeight;
                MenuManager.Instance.DrawDelayed(
                    delegate
                        {
                            var color = MenuSettings.RootContainerColor;
                            var dropdownColor = new ColorBGRA(color.R, color.G, color.B, 255);
                            Line.Width = dropdownMenuWidth;
                            Line.Begin();
                            Line.Draw(
                                new[]
                                    {
                                        new Vector2(
                                            position.X + this.Component.MenuWidth - (Line.Width / 2),
                                            position.Y + MenuSettings.ContainerHeight),
                                        new Vector2(
                                            position.X + this.Component.MenuWidth - (Line.Width / 2),
                                            position.Y + MenuSettings.ContainerHeight + dropdownMenuHeight)
                                    },
                                dropdownColor);
                            Line.End();

                            var x =
                                (int)
                                (position.X + this.Component.MenuWidth - this.dropDownButtonWidth - TextSpacing
                                 - this.Component.MaxStringWidth);
                            var y = (int)rectangleName.Y;
                            for (var i = 0; i < valueStrings.Length; i++)
                            {
                                if (i == this.Component.HoveringIndex)
                                {
                                    Line.Width = MenuSettings.ContainerHeight;
                                    Line.Begin();
                                    Line.Draw(
                                        new[]
                                            {
                                                new Vector2(
                                                    position.X + this.Component.MenuWidth - dropdownMenuWidth,
                                                    position.Y + ((i + 1) * MenuSettings.ContainerHeight)
                                                    + MenuSettings.ContainerHeight / 2f),
                                                new Vector2(
                                                    position.X + this.Component.MenuWidth,
                                                    position.Y + ((i + 1) * MenuSettings.ContainerHeight)
                                                    + MenuSettings.ContainerHeight / 2f)
                                            },
                                        MenuSettings.HoverColor);
                                    Line.End();
                                }

                                Line.Width = 1f;
                                Line.Begin();
                                Line.Draw(
                                    new[]
                                        {
                                            new Vector2(
                                                position.X + this.Component.MenuWidth - dropdownMenuWidth + 10,
                                                position.Y + (MenuSettings.ContainerHeight * (i + 1))),
                                            new Vector2(
                                                position.X + this.Component.MenuWidth - 10,
                                                position.Y + (MenuSettings.ContainerHeight * (i + 1)))
                                        },
                                    MenuSettings.ContainerSeparatorColor);
                                Line.End();
                                y += MenuSettings.ContainerHeight;
                                MenuSettings.Font.DrawText(
                                    MenuManager.Instance.Sprite,
                                    valueStrings[i],
                                    x,
                                    y,
                                    MenuSettings.TextColor);
                                if (this.Component.Index == i)
                                {
                                    var checkmarkWidth = MenuSettings.Font.MeasureText(null, "\u2713", 0).Width;
                                    MenuSettings.Font.DrawText(
                                        MenuManager.Instance.Sprite,
                                        "\u2713",
                                        (int)(position.X + this.Component.MenuWidth - checkmarkWidth - TextSpacing),
                                        y,
                                        new ColorBGRA(1, 165, 226, 255));
                                }
                            }

                            Line.Width = 1f;
                            Line.Begin();
                            Line.Draw(
                                new[]
                                    {
                                        new Vector2(
                                            position.X + this.Component.MenuWidth - dropdownMenuWidth,
                                            position.Y + MenuSettings.ContainerHeight),
                                        new Vector2(
                                            position.X + this.Component.MenuWidth - dropdownMenuWidth,
                                            position.Y + MenuSettings.ContainerHeight * (valueStrings.Length + 1)),
                                        new Vector2(
                                            position.X + this.Component.MenuWidth,
                                            position.Y + MenuSettings.ContainerHeight * (valueStrings.Length + 1)),
                                        new Vector2(
                                            position.X + this.Component.MenuWidth,
                                            position.Y + MenuSettings.ContainerHeight)
                                    },
                                MenuSettings.ContainerSeparatorColor);
                            Line.End();
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
                    (component.Position.X + component.MenuWidth - this.dropDownButtonWidth - (2 * TextSpacing)
                     - component.MaxStringWidth),
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
                    (component.Position.X + component.MenuWidth - this.dropDownButtonWidth - (2 * TextSpacing)
                     - component.MaxStringWidth),
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
                        (component.Position.X + component.MenuWidth - this.dropDownButtonWidth - (2 * TextSpacing)
                         - component.MaxStringWidth),
                        (int)(component.Position.Y + ((i + 1) * MenuSettings.ContainerHeight)),
                        this.dropDownButtonWidth + (2 * TextSpacing) + component.MaxStringWidth,
                        MenuSettings.ContainerHeight + 1));
            }

            return rectangles;
        }

        /// <summary>
        ///     Processes windows messages
        /// </summary>
        /// <param name="args">The event data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!this.Component.Visible)
            {
                return;
            }

            var dropdownRect = this.DropDownBoundaries(this.Component);
            var entireDropdownRect = this.DropDownExpandedBoundaries(this.Component);

            if (args.Cursor.IsUnderRectangle(dropdownRect.X, dropdownRect.Y, dropdownRect.Width, dropdownRect.Height))
            {
                this.Component.Hovering = true;

                if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    this.Component.Active = !this.Component.Active;
                }
            }
            else
            {
                this.Component.Hovering = false;
            }

            const int Buffer = 20;
            if (this.Component.Active
                && !args.Cursor.IsUnderRectangle(
                    entireDropdownRect.X - Buffer,
                    entireDropdownRect.Y - Buffer,
                    entireDropdownRect.Width + (2 * Buffer),
                    entireDropdownRect.Height + (2 * Buffer)))
            {
                this.Component.Active = false;
            }

            if (this.Component.Active)
            {
                var found = false;
                var dropdownRectangles = this.DropDownListBoundaries(this.Component);
                for (var i = 0; i < dropdownRectangles.Count; i++)
                {
                    if (args.Cursor.IsUnderRectangle(
                        dropdownRectangles[i].X,
                        dropdownRectangles[i].Y,
                        dropdownRectangles[i].Width,
                        dropdownRectangles[i].Height))
                    {
                        this.Component.HoveringIndex = i;
                        found = true;
                    }
                }

                if (!found)
                {
                    this.Component.HoveringIndex = -1;
                }
                else if (args.Msg == WindowsMessages.LBUTTONDOWN)
                {
                    this.Component.Index = this.Component.HoveringIndex;
                    args.Process = false;
                }
            }
        }

        /// <summary>
        ///     Gets the width of the MenuList
        /// </summary>
        /// <returns>The <see cref="int" /></returns>
        public override int Width()
        {
            return BlueUtilities.CalcWidthItem(this.Component) + this.Component.MaxStringWidth + (2 * TextSpacing)
                   + this.dropDownButtonWidth;
        }

        #endregion
    }
}