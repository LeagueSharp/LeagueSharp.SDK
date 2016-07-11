namespace LeagueSharp.SDK.Core.Wrappers.Spells.Tracker.Skillshots
{
    public class _OriannaQEnd : Skillshot
    {
        private CirclePoly Circle;

        #region Constructors and Destructors

        public _OriannaQEnd()
            : base("OriannasQ")
        {
        }

        #endregion

        #region Public Properties

        public new int Delay => this.SData.Delay + (int)this.StartPosition.Distance(this.EndPosition) / this.SData.MissileSpeed;

        #endregion

        #region Public Methods and Operators

        internal override void UpdatePolygon()
        {
            Circle = new CirclePoly(this.EndPosition, 80);
            this.UpdatePath();
        }

        internal override void UpdatePath()
        {
            this.Path = Circle.ToClipperPath();
        }

        #endregion
    }
}