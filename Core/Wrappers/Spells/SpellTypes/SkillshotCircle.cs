namespace LeagueSharp.SDK
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.SDK.Clipper;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

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

        public override string ToString()
        {
            return "SkillshotCircle: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType + " SpellName=" + this.SData.SpellName;
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