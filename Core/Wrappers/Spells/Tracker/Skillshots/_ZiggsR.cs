namespace LeagueSharp.SDK.Core.Wrappers.Tracker.Skillshots
{
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Wrappers.SpellTypes;

    public class _ZiggsR : Skillshot
    {
        #region Constructors and Destructors

        public _ZiggsR()
            : base("ZiggsR")
        {
        }

        #endregion

        #region Public Properties

        public new int Delay => (int)(1500 + 1500 * this.EndPosition.Distance(this.StartPosition) / this.SData.Range);

        #endregion
    }
}