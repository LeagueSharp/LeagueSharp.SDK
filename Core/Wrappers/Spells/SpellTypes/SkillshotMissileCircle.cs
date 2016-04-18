namespace LeagueSharp.SDK.Core.Wrappers.SpellTypes
{
    using LeagueSharp.SDK.Core.Math.Polygons;

    public class SkillshotMissileCircle : SkillshotMissile
    {
        #region Constructors and Destructors

        public SkillshotMissileCircle(string spellName)
            : base(spellName)
        {
        }

        public SkillshotMissileCircle(SpellDatabaseEntry entry)
            : base(entry)
        {
        }

        #endregion

        public override string ToString()
        {
            return "SkillshotMissileCircle: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType + " SpellName=" + this.SData.SpellName;
        }

        #region Public Properties

        internal CirclePoly Circle { get; set; }

        #endregion

        #region Public Methods and Operators

        internal override void UpdatePolygon()
        {
            if (this.Circle == null)
            {
                this.Circle = new CirclePoly(this.EndPosition, this.SData.Radius, 20);
                this.UpdatePath();
            }
        }

        internal override void UpdatePath()
        {
            this.Path = this.Circle.ToClipperPath();
        }

        #endregion
    }
}