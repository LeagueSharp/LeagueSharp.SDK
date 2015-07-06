namespace LeagueSharp.SDK.Core.UI.IMenu.Customizer
{
    using LeagueSharp.SDK.Core.UI.IMenu.Skins;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    /// This menu allows the user to modify several properties in <see cref="MenuSettings"/>.
    /// </summary>
    public sealed class MenuCustomizer : Menu
    {

        /// <summary>
        /// An instance of this MenuCustomizer
        /// </summary>
        public static MenuCustomizer Instance;

        /// <summary>
        /// Holds the X position of the menu.
        /// </summary>
        public MenuSlider PositionX { get; private set; }

        /// <summary>
        /// Holds the Y position of the menu.
        /// </summary>
        public MenuSlider PositionY { get; private set; }

        /// <summary>
        /// True if dragging of the menu is enabled.
        /// </summary>
        public MenuBool LockPosition { get; private set; }

        /// <summary>
        /// Holds the container height
        /// </summary>
        public MenuSlider ContainerHeight { get; private set; }

        /// <summary>
        /// Holds the font height
        /// </summary>
        public MenuSlider FontHeight { get; private set; }

        /// <summary>
        /// Holds the background color
        /// </summary>
        public MenuColor BackgroundColor { get; private set; }

        /// <summary>
        /// Creates a new instance of MenuCustomizer
        /// </summary>
        /// <param name="menu">The menu that will hold this MenuCustomizer</param>
        public static void Initialize(Menu menu)
        {
            if (Instance == null)
            {
                Instance = new MenuCustomizer(menu);
            }
        }

        private MenuCustomizer(Menu parentMenu)
            : base("menucustomizer", "Menu", false, "")
        {
            parentMenu.Add(this);
            BuildCustomizer();
            BuildOptions();

            ApplyChanges();
        }

        private void BuildCustomizer()
        {
            var customizeMenu = Add(new Menu("customize", "Customize"));
            AddPosition(customizeMenu);
            AddContainer(customizeMenu);
            customizeMenu.Add(new MenuButton("apply", "Apply Changes", "Apply") { Action = ApplyChanges });
            customizeMenu.Add(
                new MenuButton("reset", "Reset Customization", "Reset")
                    {
                        Action = delegate
                            {
                                customizeMenu.RestoreDefault();
                                ApplyChanges();
                            }
                    });
        }

        private void AddContainer(Menu menu)
        {
            ContainerHeight =
                menu.Add(new MenuSlider("containerheight", "Item Height", MenuSettings.ContainerHeight, 15, 50));
            FontHeight =
                menu.Add(new MenuSlider("fontheight", "Font Size", MenuSettings.Font.Description.Height, 10, 30));
            BackgroundColor =
                menu.Add(new MenuColor("backgroundcolor", "Background Color", MenuSettings.RootContainerColor));
        }

        private void AddPosition(Menu menu)
        {
            PositionX = menu.Add(new MenuSlider("x", "Position (X)", (int)MenuSettings.Position.X, 0, Drawing.Width));
            PositionY = menu.Add(new MenuSlider("y", "Position (Y)", (int)MenuSettings.Position.Y, 0, Drawing.Height));
        }

        private void BuildOptions()
        {
            LockPosition = Add(new MenuBool("lock","Lock Position"));
        }

        private void ApplyChanges()
        {
            MenuSettings.Position = new Vector2(PositionX.Value, PositionY.Value);
            MenuSettings.ContainerHeight = ContainerHeight.Value;
            var oldFont = MenuSettings.Font;
            MenuSettings.Font = new Font(
                Drawing.Direct3DDevice,
                FontHeight.Value,
                0,
                FontWeight.DoNotCare,
                0,
                false,
                FontCharacterSet.Default,
                FontPrecision.Raster,
                FontQuality.Antialiased,
                FontPitchAndFamily.DontCare | FontPitchAndFamily.Decorative,
                "Tahoma");
            oldFont.Dispose();
            MenuSettings.RootContainerColor = BackgroundColor.Color;
            MenuManager.Instance.ResetWidth();
        }


    }
}
