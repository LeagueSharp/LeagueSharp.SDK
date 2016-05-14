namespace LeagueSharp.SDKEx
{
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.SDKEx.Polygons;

    public class SkillshotMissileArc : SkillshotMissile
    {
        #region Constructors and Destructors

        public SkillshotMissileArc(string spellName)
            : base(spellName)
        {
        }

        public SkillshotMissileArc(SpellDatabaseEntry entry)
            : base(entry)
        {
        }

        #endregion

        #region Properties

        internal ArcPoly Arc { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return "SkillshotMissileArc: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType
                   + " SpellName=" + this.SData.SpellName;
        }

        #endregion

        #region Methods

        internal override void UpdatePath()
        {
            this.Path = this.Arc.ToClipperPath();
        }

        internal override void UpdatePolygon()
        {
            if (this.Arc == null)
            {
                this.Arc = new ArcPoly(this.StartPosition, this.EndPosition, this.SData.ArcAngle, this.SData.Radius, 20);
                this.UpdatePath();
            }
        }

        #endregion
    }
}