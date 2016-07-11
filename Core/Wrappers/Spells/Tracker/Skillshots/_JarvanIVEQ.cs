namespace LeagueSharp.SDK.Core.Wrappers.Spells.Tracker.Skillshots
{   
    using LeagueSharp.SDK;

    using SharpDX;
  
    public class _JarvanIVEQ : Skillshot
    {
        private RectanglePoly Rectangle;

        #region Constructors and Destructors

        public _JarvanIVEQ()
            : base("JarvanIVEQ")
        {
        }

        #endregion

        #region Public Properties

        public new Vector2 EndPosition
        {
            get
            {
                var extendedE = new RectanglePoly(this.StartPosition, base.EndPosition + this.Direction * 100, this.SData.Width);
                foreach (var skillshot in Tracker.DetectedSkillshots)
                {
                    if (skillshot.Caster.NetworkId == this.Caster.NetworkId && skillshot.SData.Slot == SpellSlot.E)
                    {
                        if (extendedE.IsInside(skillshot.EndPosition))
                            return skillshot.EndPosition;
                    }
                }

                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (minion.CharData.BaseSkinName == "jarvanivstandard" && minion.Team == this.Caster.Team)
                    {
                        if(extendedE.IsInside(minion.Position))
                            return minion.Position.ToVector2();
                    }
                }

                return base.EndPosition;
            }
        }

        #endregion

        #region Public Methods and Operators

        internal override void UpdatePolygon()
        {
            Rectangle = new RectanglePoly(this.StartPosition, this.EndPosition, this.SData.Radius);
            this.UpdatePath();
        }

        internal override void UpdatePath()
        {
            this.Path = Rectangle.ToClipperPath();
        }

        #endregion
    }
}
