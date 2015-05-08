using System;
using LeagueSharp.CommonEx.Core.UI.Abstracts;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI.Skins
{
    public abstract class Theme
    {
        public abstract Drawable Boolean { get; }
        public abstract Drawable Slider { get; }
        public abstract Drawable KeyBind { get; }

        public abstract Drawable Separator { get; }

        public abstract void OnDraw(Vector2 position);
        public abstract void OnMenu(Menu menuComponent, Vector2 position, int index);

        public abstract int CalcWidthMenu(Menu menu);
        public abstract int CalcWidthItem(MenuItem menuItem);
        public abstract int CalcWidthText(string text);

        public class Animation
        {
            public Func<bool> IsAnimating;
            public Action<AMenuComponent, Vector2, int> OnDraw;
        }

        public struct Drawable
        {
            public Func<Vector2, AMenuComponent, Rectangle> AdditionalBoundries;
            public Animation Animation;
            public Func<Vector2, AMenuComponent, Rectangle> Bounding;
            public Action<AMenuComponent, Vector2, int> OnDraw;
        }
    }
}