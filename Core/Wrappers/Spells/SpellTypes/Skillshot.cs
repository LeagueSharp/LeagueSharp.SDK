namespace LeagueSharp.SDK
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.SDK.Clipper;
    using LeagueSharp.SDK.Enumerations;
    using LeagueSharp.SDK.Utils;

    using SharpDX;

    using Color = System.Drawing.Color;

    public class Skillshot : BaseSpell
    {
        #region Constructors and Destructors

        public Skillshot(string spellName)
            : base(spellName)
        {
        }

        public Skillshot(SpellDatabaseEntry entry)
            : base(entry)
        {
        }

        #endregion

        #region Public Properties

        public virtual SkillshotDetectionType DetectionType { get; set; }

        public virtual Vector2 Direction { get; set; }

        public virtual List<IntPoint> Path { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Draw(Color color, Color missileColor, int borderWidth = 1)
        {
            if (this.Path == null)
            {
                return;
            }
            //TODO: use playsharp.sdk for this when it's ready :waiting:
            var zValue = ObjectManager.Player.Position.Z;
            for (var i = 0; i < this.Path.Count; i++)
            {
                var startPoint = this.Path[i];
                var endPoint = this.Path[i + 1 == this.Path.Count ? 0 : i + 1];
                var startPointOnScreen = Drawing.WorldToScreen(new Vector3(startPoint.X, startPoint.Y, zValue));
                var endPointOnScreen = Drawing.WorldToScreen(new Vector3(endPoint.X, endPoint.Y, zValue));

                Drawing.DrawLine(startPointOnScreen, endPointOnScreen, borderWidth, Color.White);
            }
        }

        public virtual bool Process()
        {
            if (this.DetectionType == SkillshotDetectionType.ProcessSpell)
            {
                this.StartPosition = Vector2.Zero;

                if (this.SData.FromObject != "")
                {
                    foreach (var o in ObjectManager.Get<GameObject>())
                    {
                        if (o.Name.Contains(this.SData.FromObject))
                        {
                            this.StartPosition = o.Position.ToVector2();
                        }
                    }

                    if (this.StartPosition == Vector2.Zero)
                    {
                        Logging.Write()(
                            LogLevel.Warn,
                            "[Skillshot] Couldn't find the start position for skillshot: {0}, FromObject: {1}",
                            this,
                            this.SData.FromObject);
                        return false;
                    }
                }
                else
                {
                    this.StartPosition = this.Caster.ServerPosition.ToVector2();
                }

                //TODO FromObjects
                //TODO Lucian Q ?
            }

            if (this.DetectionType == SkillshotDetectionType.MissileCreate)
            {
            }

            //Calculate the real end Point:
            this.Direction = (this.EndPosition - this.StartPosition).Normalized();
            if (!this.SData.AvoidMaxRangeReduction
                && this.StartPosition.DistanceSquared(this.EndPosition) > this.SData.Range * this.SData.Range
                || this.SData.FixedRange)
            {
                this.EndPosition = this.StartPosition + this.Direction * this.SData.Range;
            }

            if (this.SData.ExtraRange != 0)
            {
                this.EndPosition = this.EndPosition
                                   + Math.Min(
                                       this.SData.ExtraRange,
                                       this.SData.Range - this.EndPosition.Distance(this.StartPosition))
                                   * this.Direction;
            }

            this.UpdatePolygon();

            return true;
        }

        public override string ToString()
        {
            return "Skillshot: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType
                   + " SpellName=" + this.SData.SpellName;
        }

        #endregion

        #region Methods

        internal virtual void Game_OnUpdate()
        {
        }

        internal virtual void UpdatePath()
        {
        }

        internal virtual void UpdatePolygon()
        {
        }

        #endregion
    }
}