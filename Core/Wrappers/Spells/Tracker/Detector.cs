namespace LeagueSharp.SDKEx
{
    using System;
    using System.Security.Permissions;

    using LeagueSharp.Data.DataTypes;
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDKEx.Enumerations;
    using LeagueSharp.SDKEx.Utils;

    using SharpDX;

    public class Detector
    {
        #region Constructors and Destructors

        static Detector()
        {
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            GameObject.OnCreate += MissileClient_OnCreate;
            GameObject.OnCreate += GameObject_OnCreate;
        }

        #endregion

        #region Delegates

        public delegate void OnDetectSkillshotH(Skillshot skillshot);

        #endregion

        #region Events

        internal static event OnDetectSkillshotH OnDetectSkillshot;

        #endregion

        #region Methods

        static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            var spellDatabaseEntry = SpellDatabase.GetBySourceObjectName(sender.Name);

            if (spellDatabaseEntry == null)
            {
                return;
            }

            TriggerOnDetectSkillshot(
                spellDatabaseEntry,
                GameObjects.Heroes.MinOrDefault(
                    h => h.IsAlly || h.ChampionName != spellDatabaseEntry.ChampionName ? 1 : 0),
                //Since we can't really know the owner of the object we just assume is enemy :kappa:
                SkillshotDetectionType.CreateObject,
                sender.Position.ToVector2(),
                sender.Position.ToVector2(),
                Variables.TickCount - Game.Ping / 2);
        }

        static void MissileClient_OnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile == null)
            {
                return;
            }

            var spellDatabaseEntry = SpellDatabase.GetByMissileName(missile.SData.Name);

            if (spellDatabaseEntry == null)
            {
                return;
            }

            //Looks useless, but it's not :nerd:
            DelayAction.Add(0, delegate { MissileClient_OnCreate_Delayed(missile, spellDatabaseEntry); });
        }

        static void MissileClient_OnCreate_Delayed(MissileClient missile, SpellDatabaseEntry spellDatabaseEntry)
        {
            if (!missile.IsValid || !missile.SpellCaster.IsValid)
            {
                return;
            }

            var castTime = Variables.TickCount - Game.Ping / 2
                           - (spellDatabaseEntry.MissileDelayed ? 0 : spellDatabaseEntry.Delay)
                           - (int)
                             (1000f * missile.Position.Distance(missile.StartPosition) / spellDatabaseEntry.MissileSpeed);

            TriggerOnDetectSkillshot(
                spellDatabaseEntry,
                missile.SpellCaster,
                SkillshotDetectionType.MissileCreate,
                missile.StartPosition.ToVector2(),
                missile.EndPosition.ToVector2(),
                castTime,
                missile);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var spellDatabaseEntry = SpellDatabase.GetByName(args.SData.Name);

            if (spellDatabaseEntry == null)
            {
                return;
            }

            TriggerOnDetectSkillshot(
                spellDatabaseEntry,
                sender,
                SkillshotDetectionType.ProcessSpell,
                args.Start.ToVector2(),
                args.End.ToVector2(),
                Variables.TickCount - Game.Ping / 2);
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        private static void TriggerOnDetectSkillshot(
            SpellDatabaseEntry spellDatabaseEntry,
            Obj_AI_Base caster,
            SkillshotDetectionType detectionType,
            Vector2 start,
            Vector2 end,
            int time,
            MissileClient missile = null)
        {
            Skillshot skillshot = null;

            switch (spellDatabaseEntry.SpellType)
            {
                case SpellType.SkillshotMissileArc:
                    skillshot = new SkillshotMissileArc(spellDatabaseEntry);
                    break;
                case SpellType.SkillshotMissileCircle:
                    skillshot = new SkillshotMissileCircle(spellDatabaseEntry);
                    break;
                case SpellType.SkillshotMissileLine:
                    skillshot = new SkillshotMissileLine(spellDatabaseEntry);
                    break;
                case SpellType.SkillshotCircle:
                    skillshot = new SkillshotCircle(spellDatabaseEntry);
                    break;
                case SpellType.SkillshotCone:
                    skillshot = new SkillshotCone(spellDatabaseEntry);
                    break;
                case SpellType.SkillshotLine:
                    skillshot = new SkillshotLine(spellDatabaseEntry);
                    break;
                case SpellType.SkillshotRing:
                    skillshot = new SkillshotRing(spellDatabaseEntry);
                    break;
            }

            if (skillshot == null)
            {
                return;
            }

            var type =
                Type.GetType(
                    $"LeagueSharp.SDKEx.Core.Wrappers.Spells.Detector.Skillshots_{skillshot.SData.ChampionName}{skillshot.SData.Slot}");
            if (type != null)
            {
                skillshot = (Skillshot)Activator.CreateInstance(type);
            }

            skillshot.DetectionType = detectionType;
            skillshot.Caster = caster;
            skillshot.StartPosition = start;
            skillshot.EndPosition = end;
            skillshot.StartTime = time;

            if (missile != null)
            {
                try
                {
                    ((SkillshotMissile)skillshot).Missile = missile;
                }
                catch (Exception)
                {
                    Logging.Write()(
                        LogLevel.Warn,
                        "Wrong SpellType for Skillshot {0}, a Missile Type was expected",
                        skillshot.SData.SpellName);
                }
            }

            if (!skillshot.Process())
            {
                return;
            }

            TriggerOnDetectSkillshot(skillshot);
        }

        /// <summary>
        ///     Gets called when a skillshot is detected, take into account that it can trigger twice for the same skillshot, one
        ///     when OnProcessSpellCast is called and another when OnMissileCreate is called.
        /// </summary>
        /// <param name="skillshot">The detected skillshot</param>
        private static void TriggerOnDetectSkillshot(Skillshot skillshot)
        {
            OnDetectSkillshot?.Invoke(skillshot);
        }

        #endregion
    }
}