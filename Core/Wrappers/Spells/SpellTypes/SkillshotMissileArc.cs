namespace LeagueSharp.SDK
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using LeagueSharp.SDK.Clipper;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

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

        public override string ToString()
        {
            return "SkillshotMissileArc: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType + " SpellName=" + this.SData.SpellName;
        }

        #region Public Properties

        internal ArcPoly Arc { get; set; }

        #endregion

        #region Public Methods and Operators

        internal override void UpdatePolygon()
        {
            if (this.Arc == null)
            {
                this.Arc = new ArcPoly(this.StartPosition, this.EndPosition, this.SData.ArcAngle, this.SData.Radius, 20);
                this.UpdatePath();
            }
        }

        internal override void UpdatePath()
        {
            this.Path = this.Arc.ToClipperPath();
        }

        #endregion
    }
}