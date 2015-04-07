using System;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Skins.Battlecry;
using LeagueSharp.CommonEx.Core.UI.Skins.Biolight;
using LeagueSharp.CommonEx.Core.UI.Skins.Default;
using SharpDX;
using BoolRender = LeagueSharp.CommonEx.Core.UI.Skins.Default.BoolRender;

namespace LeagueSharp.CommonEx.Core.UI.Skins
{
    /// <summary>
    ///     Skin Index, contains each skin index and linked actions (normally render) of the skin.
    /// </summary>
    public class SkinIndex
    {
        /// <summary>
        ///     Skins Action Container.
        /// </summary>
        public static SkinContainer[] Skin;

        /// <summary>
        ///     Static Constructor.
        /// </summary>
        static SkinIndex()
        {
            Skin = new[]
            {
                new SkinContainer
                {
                    OnDraw = DefaultSkin.OnDraw,
                    OnMenuDraw = MenuRender.OnDraw,
                    OnBoolDraw = BoolRender.OnDraw,
                    GetContainerRectangle = DefaultSkin.GetContainerRectangle,
                    GetBooleanContainerRectangle = BoolRender.GetBooleanContainerRectangle
                },
                new SkinContainer { OnDraw = BiolightSkin.OnDraw, OnBoolDraw = Biolight.BoolRender.OnDraw },
                new SkinContainer { OnDraw = BattlecrySkin.OnDraw, OnBoolDraw = Battlecry.BoolRender.OnDraw }
            };
        }

        /// <summary>
        ///     Skin Container struct.
        /// </summary>
        public struct SkinContainer
        {
            /// <summary>
            ///     Function link towards the boolean container rectangle get value.
            /// </summary>
            public Func<Vector2, Rectangle> GetBooleanContainerRectangle;

            /// <summary>
            ///     Function link towards the container rectangle get value.
            /// </summary>
            public Func<Vector2, Rectangle> GetContainerRectangle;

            /// <summary>
            ///     Action link towards the boolean render function.
            /// </summary>
            public Action<AMenuComponent, Vector2, int> OnBoolDraw;

            /// <summary>
            ///     Action link towards the main render function.
            /// </summary>
            public Action<Vector2> OnDraw;

            /// <summary>
            ///     Action link towards the menu render function.
            /// </summary>
            public Action<AMenuComponent, Vector2, int> OnMenuDraw;
        }
    }
}