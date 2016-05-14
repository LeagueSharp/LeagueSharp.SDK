namespace LeagueSharp.SDKEx
{
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.SDKEx.Polygons;

    public class SkillshotCircle : Skillshot
    {
        #region Constructors and Destructors

        public SkillshotCircle(string spellName)
            : base(spellName)
        {
        }

        public SkillshotCircle(SpellDatabaseEntry entry)
            : base(entry)
        {
        }

        #endregion

        #region Properties

        internal CirclePoly Circle { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return "SkillshotCircle: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType
                   + " SpellName=" + this.SData.SpellName;
        }

        #endregion

        #region Methods

        internal override void UpdatePath()
        {
            this.Path = this.Circle.ToClipperPath();
        }

        internal override void UpdatePolygon()
        {
            if (this.Circle == null)
            {
                this.Circle = new CirclePoly(this.EndPosition, this.SData.Radius, 20);
                this.UpdatePath();
            }
        }

        #endregion
    }
}