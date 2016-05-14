namespace LeagueSharp.SDKEx
{
    using System;

    using LeagueSharp.Data.DataTypes;

    using SharpDX;

    public class SkillshotMissile : Skillshot
    {
        #region Constructors and Destructors

        public SkillshotMissile(string spellName)
            : base(spellName)
        {
        }

        public SkillshotMissile(SpellDatabaseEntry entry)
            : base(entry)
        {
        }

        #endregion

        #region Public Properties

        public MissileClient Missile { get; set; }

        public bool MissileDestroyed { get; set; }

        #endregion

        #region Public Methods and Operators

        public virtual Vector2 GetMissilePosition(int afterTime)
        {
            var t = Math.Max(0, Variables.TickCount + afterTime - this.StartTime - this.SData.Delay);
            int x;

            //Missile with acceleration = 0.
            if (this.SData.MissileAccel == 0)
            {
                x = t * this.SData.MissileSpeed / 1000;
            }

            //Missile with constant acceleration.
            else
            {
                var t1 = (this.SData.MissileAccel > 0
                              ? this.SData.MissileMaxSpeed
                              : this.SData.MissileMinSpeed - this.SData.MissileSpeed) * 1000f / this.SData.MissileAccel;

                if (t <= t1)
                {
                    x =
                        (int)
                        (t * this.SData.MissileSpeed / 1000d + 0.5d * this.SData.MissileAccel * Math.Pow(t / 1000d, 2));
                }
                else
                {
                    x =
                        (int)
                        (t1 * this.SData.MissileSpeed / 1000d + 0.5d * this.SData.MissileAccel * Math.Pow(t1 / 1000d, 2)
                         + (t - t1) / 1000d
                         * (this.SData.MissileAccel < 0 ? this.SData.MissileMaxSpeed : this.SData.MissileMinSpeed));
                }
            }

            /*TODO: add collision
            t = (int)Math.Max(0, Math.Min(CollisionEnd.Distance(Start), x));
             */
            t = x;

            return this.StartPosition + this.Direction * t;
        }

        public override string ToString()
        {
            return "SkillshotMissile: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType
                   + " SpellName=" + this.SData.SpellName;
        }

        #endregion
    }
}