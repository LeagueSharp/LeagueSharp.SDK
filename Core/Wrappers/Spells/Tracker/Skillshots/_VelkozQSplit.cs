namespace LeagueSharp.SDK.Core.Wrappers.Spells.Tracker.Skillshots
{
    using System.Linq;
    using LeagueSharp.SDK;

    using SharpDX;

    public class _VelkozQSplit : Skillshot
    {
        private RectanglePoly Rectangle;

        #region Constructors and Destructors

        public _VelkozQSplit()
            : base("VelkozQSplit")
        {
        }

        #endregion

        #region Public Properties

        public new Vector2 Direction => Tracker.DetectedSkillshots.Where(p => p.Caster.NetworkId == this.Caster.NetworkId && p.SData.SpellName == "VelkozQ").FirstOrDefault().Direction.Perpendicular();
        public new Vector2 StartPosition => base.StartPosition - this.Direction * 1100;
        public new Vector2 EndPosition => base.StartPosition + this.Direction * 1100;

        #endregion

        #region Public Methods and Operators

        internal override void UpdatePolygon()
        {
            Rectangle = new RectanglePoly(this.StartPosition, this.EndPosition, 55);
            this.UpdatePath();
        }

        internal override void UpdatePath()
        {
            this.Path = Rectangle.ToClipperPath();
        }

        #endregion
    }
}
