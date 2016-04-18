// <copyright file="SpellDatabaseEntry.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace LeagueSharp.SDK.Core.Wrappers
{
    using LeagueSharp.Data.Enumerations;
    using LeagueSharp.SDK.Core.Enumerations;

    /// <summary>
    ///     The spell database entry.
    /// </summary>
    public class SpellDatabaseEntry
    {
        #region Fields

        /// <summary>
        ///     The angle which the skillshot makes.
        /// </summary>
        public int Angle { get; set; } = 45;

        /// <summary>
        ///     The buffs applied by the spell on allies
        /// </summary>
        public BuffType[] AppliedBuffsOnAllies { get; set; }

        /// <summary>
        ///     The buffs applied by the spell on the target enemy champion/s
        /// </summary>
        public BuffType[] AppliedBuffsOnEnemies { get; set; }

        /// <summary>
        ///     The buffs applied by the spell on my hero
        /// </summary>
        public BuffType[] AppliedBuffsOnSelf { get; set; }

        /// <summary>
        ///     The name of the buff that the spell applies on the caster.
        /// </summary>
        public string AppliedBuffOnSelfName { get; set; }

        /// <summary>
        ///     The name of the buff applied on an ally.
        /// </summary>
        public string AppliedBuffOnAllyName {  get; set; }

        /// <summary>
        ///     The name of the buff applied on an enemy.
        /// </summary>
        public string AppliedBuffOnEnemyName { get; set; }

        /// <summary>
        ///     The name of the buff that the spell applies on the target.
        /// </summary>
        public string AppliedBuffName { get; set; }

        /// <summary>
        ///     Indicates if the spell can be cast further than the range. For example that's True for Oriannas Q.
        /// </summary>
        public bool AvoidMaxRangeReduction { get; set; }

        /// <summary>
        ///     Indicates wether the spell has a varying range
        /// </summary>
        public bool FixedRange { get; set; } = false;
        
        /// <summary>
        ///     Indicates whether the spell can be removed.
        /// </summary>
        public bool CanBeRemoved { get; set; } = false;

        /// <summary>
        ///     Array indicating the possible cast types (on enemy champion, on self, on a position)
        /// </summary>
        public CastType[] CastType { get; set; }

        /// <summary>
        ///     SpellData Entry's Champion Name
        /// </summary>
        public string ChampionName { get; set; }

        /// <summary>
        ///     What the spell missile (if any) can collide with.
        /// </summary>
        public CollisionableObjects[] CollisionObjects { get; set; } = { };

        /// <summary>
        ///     Specifies on a scale from 1 to 5 how dangerous our spell is
        /// </summary>
        public int DangerValue { get; set; } = 1;

        /// <summary>
        ///     The Spell Delay
        /// </summary>
        public int Delay { get; set; } = 250;

        /// <summary>
        ///     Extra missile names
        /// </summary>
        public string[] ExtraMissileNames { get; set; } = { };

        /// <summary>
        ///     The extra range of the Spell, used on skillshots.
        /// </summary>
        public int ExtraRange { get; set; } = 0;

        /// <summary>
        ///     Extra spell names
        /// </summary>
        public string[] ExtraSpellNames { get; set; } = { };

        /// <summary>
        ///     Indicates whether the spell is forcefully removed.
        /// </summary>
        public bool ForceRemove { get; set; } = false;

        /// <summary>
        ///     Source object name
        /// </summary>
        public string FromObject { get; set; } = string.Empty;

        /// <summary>
        ///     Source objects' names
        /// </summary>
        public string[] FromObjects { get; set; } = { };

        /// <summary>
        ///     Is our spell dangerous?
        /// </summary>
        public bool IsDangerous { get; set; } = false;

        /// <summary>
        ///     The spell's missile acceleration
        /// </summary>
        public int MissileAccel { get; set; } = 0;

        /// <summary>
        ///     Is the missile delayed?
        /// </summary>
        public bool MissileDelayed { get; set; }

        /// <summary>
        ///     Does the missile follow the caster?
        /// </summary>
        public bool MissileFollowsCaster { get; set; }

        /// <summary>
        ///     The max speed the spell missile can reach
        /// </summary>
        public int MissileMaxSpeed { get; set; } = 0;

        /// <summary>
        ///     The min speed you can find the missile at
        /// </summary>
        public int MissileMinSpeed { get; set; } = 0;

        /// <summary>
        ///     Our spell missile average travel speed
        /// </summary>
        public int MissileSpeed { get; set; } = 1000;

        /// <summary>
        ///     The spell's missile name
        /// </summary>
        public string MissileSpellName { get; set; } = string.Empty;

        /// <summary>
        ///     The Arc Skillshot Angle.
        /// </summary>
        public int ArcAngle { get; set; }

        /// <summary>
        ///     The Ring Skillshot Radius.
        /// </summary>
        public int RingRadius { get; set; }

        /// <summary>
        ///     The raw radius of the spell (skillshots only)
        /// </summary>
        public int Radius { get; set; }

        /// <summary>
        ///     The Raw Spell Range
        /// </summary>
        public int Range { get; set; } = int.MaxValue;

        /// <summary>
        ///     Does the spell reset the autoattack timer?
        /// </summary>
        public bool ResetsAutoAttackTimer { get; set; }

        /// <summary>
        ///     The SpellSlot
        /// </summary>
        public SpellSlot Slot { get; set; }

        /// <summary>
        ///     Source object name
        /// </summary>
        public string SourceObjectName { get; set; } = string.Empty;

        /// <summary>
        ///     The SData Spell Name
        /// </summary>
        public string SpellName { get; set; } = string.Empty;

        /// <summary>
        ///     Tags which define the spell (is it a heal? does it deal damage? etc. see <see cref="SpellTags" />.
        /// </summary>
        public SpellTags[] SpellTags { get; set; }

        /// <summary>
        ///     The Spell Type (skillshotline, skillshotcircle, targeted and so on)
        /// </summary>
        public SpellType SpellType { get; set; }

        /// <summary>
        ///     Particle name on toggle
        /// </summary>
        public string ToggleParticleName { get; set; } = string.Empty;

        /// <summary>
        ///     The width of the skillshot.
        /// </summary>
        public int Width { get; set; } = 50;

        /// <summary>
        ///     The minimum timeframe taken to channel the spell.
        /// </summary>
        public int MinChannelDuration { get; set; } = 0;

        /// <summary>
        ///     The maximum timeframe the spell can be channeled for.
        /// </summary>
        public int MaxChannelDuration { get; set; } = 0;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellDatabaseEntry" /> class.
        /// </summary>
        public SpellDatabaseEntry()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpellDatabaseEntry" /> class.
        /// </summary>
        /// <param name="championName">
        ///     The champion name.
        /// </param>
        /// <param name="spellName">
        ///     The spell name.
        /// </param>
        /// <param name="slot">
        ///     The slot.
        /// </param>
        /// <param name="spellType">
        ///     The spell type.
        /// </param>
        /// <param name="castType">
        ///     The cast type.
        /// </param>
        /// <param name="spellTags">
        ///     The spell tags.
        /// </param>
        /// <param name="resetsAutoAttackTimer">
        ///     Indicates whether the spell resets the auto attack timer.
        /// </param>
        /// <param name="range">
        ///     The range.
        /// </param>
        /// <param name="delay">
        ///     The delay.
        /// </param>
        /// <param name="radius">
        ///     The radius.
        /// </param>
        /// <param name="width">
        ///     The width.
        /// </param>
        /// <param name="missileSpeed">
        ///     The missile speed.
        /// </param>
        /// <param name="angle">
        ///     The angle.
        /// </param>
        /// <param name="defaultDangerValue">
        ///     The default danger value.
        /// </param>
        public SpellDatabaseEntry(
            string championName,
            string spellName,
            SpellSlot slot,
            SpellType spellType,
            CastType[] castType,
            SpellTags[] spellTags,
            bool resetsAutoAttackTimer = false,
            int range = int.MaxValue,
            int delay = 250,
            int radius = 50,
            int width = 300,
            int missileSpeed = 1400,
            int angle = 360,
            int defaultDangerValue = 1)
        {
            this.ChampionName = championName;
            this.SpellName = spellName;
            this.Slot = slot;
            this.SpellType = spellType;
            this.CastType = castType;
            this.SpellTags = spellTags;
            this.ResetsAutoAttackTimer = resetsAutoAttackTimer;
            this.Delay = delay;
            this.Range = range;
            this.Radius = radius;
            this.Width = width;
            this.MissileSpeed = missileSpeed;
            this.Angle = angle;
            this.DangerValue = defaultDangerValue;
        }

        #endregion
    }
}
