namespace LeagueSharp.SDKEx
{
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.SDKEx.Polygons;

    public class SkillshotRing : SkillshotMissile
    {
        #region Constructors and Destructors

        public SkillshotRing(string spellName)
            : base(spellName)
        {
        }

        public SkillshotRing(SpellDatabaseEntry entry)
            : base(entry)
        {
        }

        #endregion

        #region Properties

        internal RingPoly Ring { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return "SkillshotRing: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType
                   + " SpellName=" + this.SData.SpellName;
        }

        #endregion

        #region Methods

        internal override void UpdatePath()
        {
            this.Path = this.Ring.ToClipperPath();
        }

        internal override void UpdatePolygon()
        {
            if (this.Ring == null)
            {
                this.Ring = new RingPoly(this.EndPosition, this.SData.Radius, this.SData.RingRadius, 20);
                this.UpdatePath();
            }
        }

        #endregion
    }
}