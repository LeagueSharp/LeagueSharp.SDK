namespace LeagueSharp.SDKEx
{
    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.SDKEx.Polygons;

    public class SkillshotMissileLine : SkillshotMissile
    {
        #region Constructors and Destructors

        public SkillshotMissileLine(string spellName)
            : base(spellName)
        {
        }

        public SkillshotMissileLine(SpellDatabaseEntry entry)
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
            return "SkillshotMissileLine: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType
                   + " SpellName=" + this.SData.SpellName;
        }

        #endregion

        #region Methods

        internal override void Game_OnUpdate()
        {
            if (this.SData.MissileFollowsCaster && this.Caster.IsVisible)
            {
                this.EndPosition = this.Caster.ServerPosition.ToVector2();
                this.Direction = (this.EndPosition - this.StartPosition).Normalized();
            }

            this.UpdatePolygon();
        }

        internal override void UpdatePath()
        {
            this.Path = this.Rectangle.ToClipperPath();
        }

        internal override void UpdatePolygon()
        {
            if (this.Rectangle == null)
            {
                this.Rectangle = new RectanglePoly(this.StartPosition, this.EndPosition, this.SData.Radius);
            }

            this.Rectangle.Start = this.GetMissilePosition(0);
            this.Rectangle.End = this.EndPosition;
            this.Rectangle.UpdatePolygon();

            this.UpdatePath();
        }

        #endregion
    }
}