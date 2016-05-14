namespace LeagueSharp.SDK
{
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.SDK.Polygons;

    public class SkillshotLine : SkillshotMissile
    {
        #region Constructors and Destructors

        public SkillshotLine(string spellName)
            : base(spellName)
        {
        }

        public SkillshotLine(SpellDatabaseEntry entry)
            : base(entry)
        {
        }

        #endregion

        #region Properties

        internal RectanglePoly Rectangle { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return "SkillshotLine: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType
                   + " SpellName=" + this.SData.SpellName;
        }

        #endregion

        #region Methods

        internal override void UpdatePath()
        {
            this.Path = this.Rectangle.ToClipperPath();
        }

        internal override void UpdatePolygon()
        {
            if (this.Rectangle == null)
            {
                this.Rectangle = new RectanglePoly(this.StartPosition, this.EndPosition, this.SData.Radius);
                this.UpdatePath();
            }
        }

        #endregion
    }
}