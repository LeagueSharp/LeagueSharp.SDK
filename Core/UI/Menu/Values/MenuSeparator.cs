using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.UI.Skins;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    public class MenuSeparator : AMenuValue
    {
        public override int Width
        {
            get { return 0; }
        }

        public override Vector2 Position { get; set; }

        public override void OnDraw(AMenuComponent component, Vector2 position, int index)
        {
            if (!Position.Equals(position))
            {
                Position = position;
            }

            Theme.Animation animation = ThemeManager.Current.Boolean.Animation;

            if (animation != null && animation.IsAnimating())
            {
                animation.OnDraw(component, position, index);

                return;
            }
            ThemeManager.Current.Separator.OnDraw(component, position, index);
        }

        public override void OnWndProc(WindowsKeys args)
        {
            // not needed                        
        }

        public override void Extract(AMenuValue component) {}
    }
}