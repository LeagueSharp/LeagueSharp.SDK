using LeagueSharp.CommonEx.Core.UI.Skins.Default;

namespace LeagueSharp.CommonEx.Core.UI.Skins
{
    public class ThemeManager
    {
        private static Theme _Default, _Current;

        public static Theme Default
        {
            get { return (_Default ?? (_Default = new DefaultTheme())); }
        }

        public static Theme Current
        {
            get { return (_Current ?? (_Current = Default)); }
            set { _Current = value; }
        }
    }
}