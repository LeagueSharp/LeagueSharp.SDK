namespace LeagueSharp.SDK.Core.Wrappers.SpellTypes
{
    using System;

    using LeagueSharp.SDK.Core.Math.Polygons;

    public class SkillshotCone : SkillshotMissile
    {
        #region Constructors and Destructors

        public SkillshotCone(string spellName)
            : base(spellName)
        {
        }

        public SkillshotCone(SpellDatabaseEntry entry)
            : base(entry)
        {
        }

        #endregion

        public override string ToString()
        {
            return "SkillshotCone: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType + " SpellName=" + this.SData.SpellName;
        }

        #region Public Properties

        internal SectorPoly Sector { get; set; }

        #endregion

        #region Public Methods and Operators

        internal override void UpdatePolygon()
        {
            if (this.Sector == null)
            {
                this.Sector = new SectorPoly(this.StartPosition, this.EndPosition, (float) (this.SData.Angle * Math.PI / 180), this.SData.Range, 20);
                this.UpdatePath();
            }
        }

        internal override void UpdatePath()
        {
            this.Path = this.Sector.ToClipperPath();
        }

        #endregion
    }
}