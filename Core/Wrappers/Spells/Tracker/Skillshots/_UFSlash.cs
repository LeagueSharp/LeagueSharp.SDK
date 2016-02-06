namespace LeagueSharp.SDK.Core.Wrappers.Spells.Tracker.Skillshots
{
    public class _UFSlash : Skillshot
    {
        private CirclePoly Circle;

        #region Constructors and Destructors

        public _UFSlash()
            : base("UFSlash")
        {
            this.SData.MissileSpeed += (int)this.Caster.MoveSpeed;
        }

        #endregion

        #region Public Methods and Operators

        internal override void UpdatePolygon()
        {
            Circle = new CirclePoly(this.EndPosition, this.SData.Radius);
            this.UpdatePath();
        }

        internal override void UpdatePath()
        {
            this.Path = Circle.ToClipperPath();
        }

        #endregion
    }
}
