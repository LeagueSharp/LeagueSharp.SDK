namespace LeagueSharp.SDK.Core.Wrappers.Spells.Tracker.Skillshots
{
    using SharpDX;

    public class _AlZaharCalloftheVoid : Skillshot
    {
        private RectanglePoly Rectangle;

        #region Constructors and Destructors

        public _AlZaharCalloftheVoid()
            : base("AlZaharCalloftheVoid")
        {
        }

        #endregion

        #region Public Properties

        public new Vector2 Direction => base.Direction.Perpendicular();
        public new Vector2 StartPosition => base.EndPosition - this.Direction * 400;
        public new Vector2 EndPosition => base.EndPosition + this.Direction * 400;

        #endregion

        #region Public Methods and Operators

        internal override void UpdatePolygon()
        {
            Rectangle = new RectanglePoly(this.StartPosition, this.EndPosition, 85);
            this.UpdatePath();
        }

        internal override void UpdatePath()
        {
            this.Path = Rectangle.ToClipperPath();
        }

        #endregion
    }
}
