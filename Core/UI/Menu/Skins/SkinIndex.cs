using System;
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
                new SkinContainer { OnDraw = DefaultSkin.OnDraw, OnBoolDraw = BoolRender.OnDraw },
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
            ///     Action link towards the boolean render function.
            /// </summary>
            public Action<Vector2> OnBoolDraw;

            /// <summary>
            ///     Action link towards the main render function.
            /// </summary>
            public Action<Vector2> OnDraw;
        }
    }
}