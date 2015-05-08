using LeagueSharp.CommonEx.Core.UI.Abstracts;
using LeagueSharp.CommonEx.Core.Utils;
using SharpDX;

namespace LeagueSharp.CommonEx.Core.UI.Values
{
    public class MenuList : AMenuValue
    {
        public override int Width
        {
            get { return 0; }
        }

        public override Vector2 Position { get; set; }

        public override void OnDraw(AMenuComponent component, Vector2 position, int index) {}

        public override void OnWndProc(WindowsKeys args) {}


        public override void Extract(AMenuValue component) {}
    }
}