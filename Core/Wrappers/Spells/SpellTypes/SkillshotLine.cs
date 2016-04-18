namespace LeagueSharp.SDK
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using LeagueSharp.SDK.Clipper;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

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

        public override string ToString()
        {
            return "SkillshotLine: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType + " SpellName=" + this.SData.SpellName;
        }

        #region Public Properties

        internal RectanglePoly Rectangle { get; set; }

        #endregion

        #region Public Methods and Operators

        internal override void UpdatePolygon()
        {
            if (this.Rectangle == null)
            {
                this.Rectangle = new RectanglePoly(this.StartPosition, this.EndPosition, this.SData.Radius);
                this.UpdatePath();
            }
        }

        internal override void UpdatePath()
        {
            this.Path = this.Rectangle.ToClipperPath();
        }

        #endregion
    }
}