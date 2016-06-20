namespace LeagueSharp.SDK
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.SDK.Clipper;
    using LeagueSharp.SDK.Core.Utils;

    using SharpDX;

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

        public override string ToString()
        {
            return "SkillshotMissileLine: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType + " SpellName=" + this.SData.SpellName;
        }

        #region Public Properties

        internal RectanglePoly Rectangle { get; set; }

        #endregion

        #region Public Methods and Operators

        internal override void Game_OnUpdate()
        {
            if (this.SData.MissileFollowsCaster && this.Caster.IsVisible)
            {
                this.EndPosition = this.Caster.ServerPosition.ToVector2();
                this.Direction = (this.EndPosition - this.StartPosition).Normalized();
            }

            this.UpdatePolygon();
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

        internal override void UpdatePath()
        {
            this.Path = this.Rectangle.ToClipperPath();
        }

        #endregion
    }
}