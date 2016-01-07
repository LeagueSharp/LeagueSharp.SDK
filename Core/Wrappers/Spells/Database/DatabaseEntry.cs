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

namespace LeagueSharp.SDK.Core.Wrappers.Spells.Database
{
    using LeagueSharp.SDK.Core.Enumerations;

    /// <summary>
    ///     The spell database entry.
    /// </summary>
    public class DatabaseEntry
    {
        #region Fields

        /// <summary>
        ///     The angle which the skillshot makes.
        /// </summary>
        public int Angle = 30;

        /// <summary>
        ///     The buffs applied by the spell on allies
        /// </summary>
        public BuffType[] AppliedBuffsOnAllies;

        /// <summary>
        ///     The buffs applied by the spell on the target enemy champion/s
        /// </summary>
        public BuffType[] AppliedBuffsOnEnemies;

        /// <summary>
        ///     The buffs applied by the spell on my hero
        /// </summary>
        public BuffType[] AppliedBuffsOnSelf;

        /// <summary>
        ///     Indicates whether the spell can be removed.
        /// </summary>
        public bool CanBeRemoved = false;

        /// <summary>
        ///     Array indicating the possible cast types (on enemy champion, on self, on a position)
        /// </summary>
        public CastType[] CastType;

        /// <summary>
        ///     SpellData Entry's Champion Name
        /// </summary>
        public string ChampionName;

        /// <summary>
        ///     What the spell missile (if any) can collide with.
        /// </summary>
        public CollisionableObjects[] CollisionObjects = { };

        /// <summary>
        ///     Specifies on a scale from 1 to 5 how dangerous our spell is
        /// </summary>
        public int DangerValue = 1;

        /// <summary>
        ///     The Spell Delay
        /// </summary>
        public int Delay = 250;

        /// <summary>
        ///     Extra missile names
        /// </summary>
        public string[] ExtraMissileNames = { };

        /// <summary>
        ///     Extra spell names
        /// </summary>
        public string[] ExtraSpellNames = { };

        /// <summary>
        ///     Indicates whether the spell is forcefully removed.
        /// </summary>
        public bool ForceRemove = false;

        /// <summary>
        ///     Source object name
        /// </summary>
        public string FromObject = string.Empty;

        /// <summary>
        ///     Source objects' names
        /// </summary>
        public string[] FromObjects = { };

        /// <summary>
        ///     Is our spell dangerous?
        /// </summary>
        public bool IsDangerous = false;

        /// <summary>
        ///     The spell's missile acceleration
        /// </summary>
        public int MissileAccel = 0;

        /// <summary>
        ///     Is the missile delayed?
        /// </summary>
        public bool MissileDelayed;

        /// <summary>
        ///     Does the missile follow the target?
        /// </summary>
        public bool MissileFollowsUnit;

        /// <summary>
        ///     The max speed the spell missile can reach
        /// </summary>
        public int MissileMaxSpeed = 0;

        /// <summary>
        ///     The min speed you can find the missile at
        /// </summary>
        public int MissileMinSpeed = 0;

        /// <summary>
        ///     Our spell missile average travel speed
        /// </summary>
        public int MissileSpeed = 1000;

        /// <summary>
        ///     The spell's missile name
        /// </summary>
        public string MissileSpellName = string.Empty;

        /// <summary>
        ///     The raw radius of the spell (skillshots only)
        /// </summary>
        public int Radius;

        /// <summary>
        ///     The Raw Spell Range
        /// </summary>
        public int Range = int.MaxValue;

        /// <summary>
        ///     Does the spell reset the autoattack timer?
        /// </summary>
        public bool ResetsAutoAttackTimer;

        /// <summary>
        ///     The SpellSlot
        /// </summary>
        public SpellSlot Slot;

        /// <summary>
        ///     Source object name
        /// </summary>
        public string SourceObjectName = string.Empty;

        /// <summary>
        ///     The SData Spell Name
        /// </summary>
        public string SpellName = string.Empty;

        /// <summary>
        ///     Tags which define the spell (is it a heal? does it deal damage? etc. see <see cref="SpellTags" />.
        /// </summary>
        public SpellTags[] SpellTags;

        /// <summary>
        ///     The Spell Type (skillshotline, skillshotcircle, targeted and so on)
        /// </summary>
        public SpellType SpellType;

        /// <summary>
        ///     Particle name on toggle
        /// </summary>
        public string ToggleParticleName = string.Empty;

        /// <summary>
        ///     The width of the skillshot.
        /// </summary>
        public int Width = 50;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseEntry" /> class.
        /// </summary>
        public DatabaseEntry()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseEntry" /> class.
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
        public DatabaseEntry(
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