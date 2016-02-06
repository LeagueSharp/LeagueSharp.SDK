namespace LeagueSharp.SDK.Core.Wrappers.Spells.Tracker.Skillshots
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using LeagueSharp.SDK;

    using Clipper;
    using SharpDX;

    public class _SyndraE : Skillshot
    {
        private Polygon Polygon;

        #region Constructors and Destructors

        public _SyndraE()
            : base("SyndraE")
        {
        }

        #endregion

        #region Public Methods and Operators

        internal override void UpdatePolygon()
        {
            Polygon = new Polygon();

            var angle = 60;
            var edge1 =
                (this.EndPosition - this.Caster.ServerPosition.ToVector2()).Rotated(
                    -angle / 2 * (float)Math.PI / 180);
            var edge2 = edge1.Rotated(angle * (float)Math.PI / 180);

            var positions = new List<Vector2>();

            //detect syndra q which havent exploded yet
            var explodingQ = Tracker.DetectedSkillshots.FirstOrDefault(p => p.SData.SpellName == "SyndraQ");
            if (explodingQ != null)
                positions.Add(explodingQ.EndPosition);

            //detect syndra qs which have already exploded
            var seeds = ObjectManager.Get<Obj_AI_Minion>().Where(p => p.Name == "Seed" && !p.IsDead && p.Team == this.Caster.Team).Select(q => q.ServerPosition.ToVector2());
            foreach (var seed in seeds)
                positions.Add(seed);

            foreach (var position in positions)
            {
                var v = position - this.Caster.ServerPosition.ToVector2();
                if (edge1.CrossProduct(v) > 0 && v.CrossProduct(edge2) > 0 &&
                    position.Distance(this.Caster.ServerPosition) < 800)
                {
                    var start = position;
                    var end = this.Caster.ServerPosition.ToVector2()
                        .Extend(
                            position,
                            this.Caster.Distance(position) > 200 ? 1300 : 1000);

                    Polygon.Add(new RectanglePoly(start, end, this.SData.Width));
                }
            }

            this.UpdatePath();
        }

        internal override void UpdatePath()
        {
            this.Path = Polygon.ToClipperPath();
        }

        #endregion
    }
}
