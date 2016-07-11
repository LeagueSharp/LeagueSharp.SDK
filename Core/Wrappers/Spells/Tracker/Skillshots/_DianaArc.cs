namespace LeagueSharp.SDK.Core.Wrappers.Spells.Tracker.Skillshots
{
    public class _DianaArc : Skillshot
    {
        private ArcPoly Arc;

        #region Constructors and Destructors

        public _DianaArc()
            : base("DianaArc")
        {
        }

        #endregion

        #region Public Methods and Operators

        internal override void UpdatePolygon()
        {
            Arc = new ArcPoly(this.StartPosition, this.EndPosition, this.SData.ArcAngle, this.SData.Radius);
            this.UpdatePath();
        }

        internal override void UpdatePath()
        {
            this.Path = Arc.ToClipperPath();
        }

        #endregion
    }
}