namespace LeagueSharp.SDK.Core.UI.Skins.Default
{
    using System.Collections.Generic;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Math;
    using LeagueSharp.SDK.Core.UI.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    /// A default implementation of a IDrawableList
    /// </summary>
    public class DefaultList : DefaultComponent, IDrawableList
    {
        private readonly int dropDownButtonWidth;

        private const int ArrowSpacing = 6;

        private const int TextSpacing = 8;

        /// <summary>
        /// Creates a new instance of a DefaultList
        /// </summary>
        public DefaultList()
        {
            Rectangle arrowSize = DefaultSettings.Font.MeasureText(null, "V", 0);
            dropDownButtonWidth = arrowSize.Width + (2 * ArrowSpacing);
        }

        /// <summary>
        ///     Draw a MenuList
        /// </summary>
        /// <param name="component">MenuList</param>
        public void Draw(MenuList component)
        {
            int dropdownMenuWidth = dropDownButtonWidth + (2 * TextSpacing) + component.MaxStringWidth;
            Vector2 position = component.Container.Position;
            Vector2 rectangleName = GetContainerRectangle(component.Container)
                .GetCenteredText(null, component.Container.DisplayName, CenteredFlags.VerticalCenter);

            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                component.Container.DisplayName,
                (int)(position.X + DefaultSettings.ContainerTextOffset),
                (int)rectangleName.Y,
                DefaultSettings.TextColor);

            var line = new Line(Drawing.Direct3DDevice)
                           { Antialias = false, GLLines = false, Width = dropDownButtonWidth };

            line.Begin();
            line.Draw(
                new[]
                    {
                        new Vector2(
                            position.X + component.Container.MenuWidth - (dropDownButtonWidth / 2f),
                            position.Y + 1),
                        new Vector2(
                            position.X + component.Container.MenuWidth - (dropDownButtonWidth / 2f),
                            position.Y + DefaultSettings.ContainerHeight)
                    },
                DefaultSettings.HoverColor);
            line.End();
            if (component.Hovering || component.Active)
            {
                line.Width = DefaultSettings.ContainerHeight;
                line.Begin();
                line.Draw(
                    new[]
                        {
                            new Vector2(
                                position.X + component.Container.MenuWidth - dropdownMenuWidth,
                                position.Y + line.Width / 2),
                            new Vector2(position.X + component.Container.MenuWidth, position.Y + line.Width / 2)
                        },
                    DefaultSettings.HoverColor);
                line.End();
            }
            line.Dispose();
            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                "V",
                (int)(position.X + component.Container.MenuWidth - dropDownButtonWidth + ArrowSpacing),
                (int)(rectangleName.Y),
                DefaultSettings.TextColor);
            DefaultSettings.ContainerSeparatorLine.Draw(
                new[]
                    {
                        new Vector2(position.X + component.Container.MenuWidth - dropDownButtonWidth - 1, position.Y + 1),
                        new Vector2(
                            position.X + component.Container.MenuWidth - dropDownButtonWidth - 1,
                            position.Y + DefaultSettings.ContainerHeight)
                    },
                DefaultSettings.ContainerSeparatorColor);
            DefaultSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                component.SelectedValueAsObject.ToString(),
                (int)position.X + component.Container.MenuWidth - dropDownButtonWidth - TextSpacing
                - component.MaxStringWidth,
                (int)rectangleName.Y,
                DefaultSettings.TextColor);
            DefaultSettings.ContainerSeparatorLine.Draw(
                new[]
                    {
                        new Vector2(
                            position.X + component.Container.MenuWidth - dropDownButtonWidth - (2 * TextSpacing)
                            - component.MaxStringWidth,
                            position.Y + 1),
                        new Vector2(
                            position.X + component.Container.MenuWidth - dropDownButtonWidth - (2 * TextSpacing)
                            - component.MaxStringWidth,
                            position.Y + DefaultSettings.ContainerHeight)
                    },
                DefaultSettings.ContainerSeparatorColor);

            if (component.Active)
            {
                string[] valueStrings = component.ValuesAsStrings;
                int dropdownMenuHeight = valueStrings.Length * DefaultSettings.ContainerHeight;
                MenuManager.Instance.DrawDelayed(
                    delegate
                        {
                            var backgroundLine = new Line(Drawing.Direct3DDevice)
                                                     { Width = dropdownMenuWidth, Antialias = false, GLLines = false };
                            backgroundLine.Begin();
                            backgroundLine.Draw(
                                new[]
                                    {
                                        new Vector2(
                                            position.X + component.Container.MenuWidth - (backgroundLine.Width / 2),
                                            position.Y + DefaultSettings.ContainerHeight),
                                        new Vector2(
                                            position.X + component.Container.MenuWidth - (backgroundLine.Width / 2),
                                            position.Y + DefaultSettings.ContainerHeight + dropdownMenuHeight)
                                    },
                                Color.Black);
                            backgroundLine.End();
                            backgroundLine.Dispose();

                            var x =
                                (int)
                                (position.X + component.Container.MenuWidth - dropDownButtonWidth - TextSpacing
                                 - component.MaxStringWidth);
                            var y = (int)rectangleName.Y;
                            for (int i = 0; i < valueStrings.Length; i++)
                            {
                                if (i == component.HoveringIndex)
                                {
                                    var hoverLine = new Line(Drawing.Direct3DDevice)
                                                        {
                                                            Width = DefaultSettings.ContainerHeight, Antialias = false,
                                                            GLLines = false
                                                        };
                                    hoverLine.Begin();
                                    hoverLine.Draw(
                                        new[]
                                            {
                                                new Vector2(
                                                    position.X + component.Container.MenuWidth - dropdownMenuWidth,
                                                    position.Y + ((i + 1) * DefaultSettings.ContainerHeight)
                                                    + DefaultSettings.ContainerHeight / 2f),
                                                new Vector2(
                                                    position.X + component.Container.MenuWidth,
                                                    position.Y + ((i + 1) * DefaultSettings.ContainerHeight)
                                                    + DefaultSettings.ContainerHeight / 2f)
                                            },
                                        DefaultSettings.HoverColor);
                                    hoverLine.End();
                                    hoverLine.Dispose();
                                }
                                DefaultSettings.ContainerSeparatorLine.Draw(
                                    new[]
                                        {
                                            new Vector2(
                                                position.X + component.Container.MenuWidth - dropdownMenuWidth,
                                                position.Y + (DefaultSettings.ContainerHeight * (i + 1))),
                                            new Vector2(
                                                position.X + component.Container.MenuWidth,
                                                position.Y + (DefaultSettings.ContainerHeight * (i + 1)))
                                        },
                                    DefaultSettings.ContainerSeparatorColor);
                                y += DefaultSettings.ContainerHeight;
                                DefaultSettings.Font.DrawText(
                                    MenuManager.Instance.Sprite,
                                    valueStrings[i],
                                    x,
                                    y,
                                    DefaultSettings.TextColor);
                                if (component.Index == i)
                                {
                                    int checkmarkWidth = DefaultSettings.Font.MeasureText(null, "\u221A", 0).Width;
                                    DefaultSettings.Font.DrawText(
                                        MenuManager.Instance.Sprite,
                                        "\u221A",
                                        (int)(position.X + component.Container.MenuWidth - checkmarkWidth - TextSpacing),
                                        y,
                                        DefaultSettings.TextColor);
                                }
                            }
                            DefaultSettings.ContainerSeparatorLine.Draw(
                                new[]
                                    {
                                        new Vector2(
                                            position.X + component.Container.MenuWidth - dropdownMenuWidth,
                                            position.Y + DefaultSettings.ContainerHeight),
                                        new Vector2(
                                            position.X + component.Container.MenuWidth - dropdownMenuWidth,
                                            position.Y + DefaultSettings.ContainerHeight * (valueStrings.Length + 1)),
                                        new Vector2(
                                            position.X + component.Container.MenuWidth,
                                            position.Y + DefaultSettings.ContainerHeight * (valueStrings.Length + 1)),
                                        new Vector2(
                                            position.X + component.Container.MenuWidth,
                                            position.Y + DefaultSettings.ContainerHeight)
                                    },
                                DefaultSettings.ContainerSeparatorColor);
                        });
            }
        }

        /// <summary>
        ///     Gets the dropdown boundaries (preview)
        /// </summary>
        /// <param name="component">MenuList</param>
        /// <returns>Rectangle</returns>
        public Rectangle DropDownBoundaries(MenuList component)
        {
            return
                new Rectangle(
                    (int)
                    (component.Container.Position.X + component.Container.MenuWidth - dropDownButtonWidth
                     - (2 * TextSpacing) - component.MaxStringWidth),
                    (int)component.Container.Position.Y,
                    dropDownButtonWidth + (2 * TextSpacing) + component.MaxStringWidth,
                    DefaultSettings.ContainerHeight);
        }

        /// <summary>
        ///     Gets the list of dropdown item boundaries.
        /// </summary>
        /// <param name="component">MenuList</param>
        /// <returns>List of Rectangles</returns>
        public List<Rectangle> DropDownListBoundaries(MenuList component)
        {
            var rectangles = new List<Rectangle>();
            for (int i = 0; i < component.Count; i++)
            {
                rectangles.Add(
                    new Rectangle(
                        (int)
                        (component.Container.Position.X + component.Container.MenuWidth - dropDownButtonWidth
                         - (2 * TextSpacing) - component.MaxStringWidth),
                        (int)(component.Container.Position.Y + ((i + 1) * DefaultSettings.ContainerHeight)),
                        dropDownButtonWidth + (2 * TextSpacing) + component.MaxStringWidth,
                        DefaultSettings.ContainerHeight + 1));
            }
            return rectangles;
        }

        /// <summary>
        ///     Gets the complete dropdown boundaries
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public Rectangle DropDownExpandedBoundaries(MenuList component)
        {
            return
                new Rectangle(
                    (int)
                    (component.Container.Position.X + component.Container.MenuWidth - dropDownButtonWidth
                     - (2 * TextSpacing) - component.MaxStringWidth),
                    (int)component.Container.Position.Y,
                    dropDownButtonWidth + (2 * TextSpacing) + component.MaxStringWidth,
                    (component.Count + 1) * DefaultSettings.ContainerHeight);
        }

        /// <summary>
        ///     Gets the width of the MenuList
        /// </summary>
        /// <param name="menuList">MenuList</param>
        /// <returns>int</returns>
        public int Width(MenuList menuList)
        {
            return menuList.MaxStringWidth + (2 * TextSpacing) + dropDownButtonWidth;
        }
    }
}