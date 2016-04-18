namespace LeagueSharp.SDK.Core.Wrappers.Tracker
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp.SDK.Core.Enumerations;
    using LeagueSharp.SDK.Core.Extensions.SharpDX;
    using LeagueSharp.SDK.Core.Utils;
    using LeagueSharp.SDK.Core.Wrappers.SpellTypes;

    public class Tracker
    {
        #region Constructors and Destructors

        static Tracker()
        {
            Game.OnUpdate += Game_OnUpdate;
            Detector.OnDetectSkillshot += Detector_OnDetectSkillshot;
            MissileClient.OnDelete += MissileClient_OnDelete;
        }

        #endregion

        #region Public Events

        internal static event Detector.OnDetectSkillshotH OnDetectSkillshot;

        #endregion

        #region Static Fields

        public static readonly List<Skillshot> DetectedSkillshots = new List<Skillshot>();

        #endregion

        #region Methods
        
        private static void MissileClient_OnDelete(GameObject sender, EventArgs args)
        {
            if (!(sender is MissileClient))
            {
                return;
            }

            //Remove the detected skillshots that have collided.
            for (int i = DetectedSkillshots.Count - 1; i >= 0; i--)
            {
                var skillshot = DetectedSkillshots[i] as SkillshotMissile;

                if (skillshot?.Missile != null && skillshot.SData.CanBeRemoved && skillshot.Missile.NetworkId == sender.NetworkId)
                {
                    skillshot.MissileDestroyed = true;
                    DetectedSkillshots.RemoveAt(i);
                }
            }
        }

        static void Game_OnUpdate(EventArgs args)
        {
            //Remove the detected skillshots that have expired.
            DetectedSkillshots.RemoveAll(skillshot => skillshot.HasExpired());
            
            //Trigger Game_OnUpdate on each skillshot to update the polygon and other calculations :nerd:
            foreach (var skillshot in DetectedSkillshots)
            {
                skillshot.Game_OnUpdate();
            }
        }

        /// <summary>
        /// Gets called when a new skillshot gets detected. It only gets called once per skillshot.
        /// </summary>
        /// <param name="skillshot"></param>
        static void Detector_OnDetectSkillshot(Skillshot skillshot)
        {
            var isAlreadyDetected = false;
            foreach (var detectedSkillshot in DetectedSkillshots)
            {
                if (detectedSkillshot.SData.SpellName != skillshot.SData.SpellName || detectedSkillshot.Caster.NetworkId != skillshot.Caster.NetworkId)
                {
                    continue;
                }
                
                //TODO: additional distance check(s) might be required.
                if (skillshot.Direction.AngleBetween(detectedSkillshot.Direction) < 5)
                {
                    isAlreadyDetected = true;
                    
                    //Add the missile information to the detected skillshot.
                    if (skillshot.DetectionType == SkillshotDetectionType.MissileCreate)
                    {
                        try
                        {
                            ((SkillshotMissile)detectedSkillshot).Missile = ((SkillshotMissile)skillshot).Missile;
                        }
                        catch (Exception ex)
                        {
                            Logging.Write()(LogLevel.Warn, "Wrong SpellType for Skillshot {0}, a Missile Type was expected", skillshot.SData.SpellName);
                        }
                    }
                }
            }

            if (isAlreadyDetected)
            {
                return;
            }

            skillshot.PrintSpellData();
            DetectedSkillshots.Add(skillshot);
            OnDetectSkillshot?.Invoke(skillshot);
        }

        #endregion
    }
}