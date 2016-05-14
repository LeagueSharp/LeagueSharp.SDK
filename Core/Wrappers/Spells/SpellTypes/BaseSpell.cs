namespace LeagueSharp.SDKEx
{
    using System;

    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.SDKEx.Enumerations;

    using SharpDX;

    using Color = System.Drawing.Color;

    public abstract class BaseSpell
    {
        #region Fields

        public SpellDatabaseEntry SData;

        #endregion

        #region Constructors and Destructors

        public BaseSpell(string spellName)
        {
            this.SData = SpellDatabase.GetByName(spellName);
        }

        public BaseSpell(SpellDatabaseEntry entry)
        {
            this.SData = entry;
        }

        #endregion

        #region Public Properties

        public virtual Obj_AI_Base Caster { get; set; }

        public virtual SkillshotDetectionType DetectionType { get; set; }

        public virtual Vector2 EndPosition { get; set; }

        public bool HasMissile => this is SkillshotMissile;

        public virtual Vector2 StartPosition { get; set; }

        public virtual int StartTime { get; set; }

        #endregion

        #region Public Methods and Operators

        public virtual void Draw(Color color, Color missileColor, int borderWidth = 1)
        {
        }

        public virtual bool HasExpired()
        {
            if (this.SData.MissileAccel != 0)
            {
                return Variables.TickCount >= this.StartTime + 5000;
            }

            return Variables.TickCount
                   > this.StartTime + this.SData.Delay
                   + /* this.ExtraDuration + */
                   1000 * (this.StartPosition.Distance(this.EndPosition) / this.SData.MissileSpeed);
        }

        //TODO: ADD DAMAGE STUFF

        public virtual bool IsAboutToHit(Obj_AI_Base unit, int afterTime)
        {
            //TODO
            return false;
        }

        public virtual bool IsAboutToHit(Vector3 position, int afterTime)
        {
            //TODO
            return false;
        }

        public void PrintSpellData()
        {
            Console.WriteLine(@"=================");
            var properties = new[]
                                 {
                                     "ChampionName", "SpellType", "SpellName", "Range", "Radius", "Delay", "MissileSpeed",
                                     "CanBeRemoved", "Angle", "FixedRange"
                                 };
            properties.ForEach(
                property =>
                    {
                        Console.WriteLine(
                            "{0} => {1}",
                            property,
                            this.SData.GetType().GetProperty(property).GetValue(this.SData, null));
                    });
        }

        public override string ToString()
        {
            return "BaseSpell: Champion=" + this.SData.ChampionName + " SpellType=" + this.SData.SpellType
                   + " SpellName=" + this.SData.SpellName;
        }

        #endregion

        #region Methods

        internal virtual void Game_OnUpdate()
        {
        }

        #endregion
    }
}