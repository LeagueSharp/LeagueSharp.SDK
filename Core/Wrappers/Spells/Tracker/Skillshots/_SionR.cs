namespace LeagueSharp.SDK.Core.Wrappers.Spells.Tracker.Skillshots
{
    using SharpDX;

    public class _SionR : Skillshot
    {
        private RectanglePoly Rectangle;
        #region Constructors and Destructors

        public _SionR()
            : base("SionR")
        {
            this.SData.MissileSpeed = (int)this.Caster.MoveSpeed;
        }

        #endregion

        #region Public Properties

        public new Vector2 Direction => this.Caster.Direction.ToVector2();
        public new Vector2 StartPosition => this.Caster.ServerPosition.ToVector2();
        public new Vector2 EndPosition => this.StartPosition + this.Direction * this.SData.Radius;

        #endregion

        #region Public Methods and Operators

        internal override void UpdatePolygon()
        {
            Rectangle = new RectanglePoly(this.StartPosition, this.EndPosition, this.SData.Width);
            this.UpdatePath();
        }

        internal override void UpdatePath()
        {
            this.Path = Rectangle.ToClipperPath();
        }

        #endregion
    }
}
