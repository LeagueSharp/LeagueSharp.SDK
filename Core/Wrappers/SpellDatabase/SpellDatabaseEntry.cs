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

using System;
using System.Linq;
using System.Windows.Forms;
using LeagueSharp;
using LeagueSharp.SDK.Core.Enumerations;
using LeagueSharp.SDK.Core.Extensions;
using LeagueSharp.SDK.Core.Extensions.SharpDX;
using LeagueSharp.SDK.Core.UI.IMenu.Values;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.SDK.Core.Wrappers.Damages;

namespace LeagueSharp.SDK.Core.Wrappers.SpellDatabase
{
    public class SpellDatabaseEntry
    {
        /// <summary>
        /// SpellData Entry's Champion Name
        /// </summary>
        public string ChampionName;
        /// <summary>
        /// The SpellSlot
        /// </summary>
        public SpellSlot Slot;
        /// <summary>
        /// The Spell Type (skillshotline, skillshotcircle, targeted and so on)
        /// </summary>
        public SpellType SpellType;
        /// <summary>
        /// Array indicating the possible cast types (on enemy champion, on self, on a position)
        /// </summary>
        public CastType[] CastType;
        /// <summary>
        /// The buffs applied by the spell on the target enemy champion/s
        /// </summary>
        public BuffType[] AppliedBuffsOnEnemies;
        /// <summary>
        /// The buffs applied by the spell on allies
        /// </summary>
        public BuffType[] AppliedBuffsOnAllies;
        /// <summary>
        /// The buffs applied by the spell on my hero
        /// </summary>
        public BuffType[] AppliedBuffsOnSelf;
        /// <summary>
        /// Tags which define the spell (is it a heal? does it deal damage? etc. see <cref="SpellTags">SpellTags</cref>.
        /// </summary>
        public SpellTags[] SpellTags;
        /// <summary>
        /// The SData Spell Name
        /// </summary>
        public string SpellName = "";
        /// <summary>
        /// The Spell Delay
        /// </summary>
        public int Delay = 250;
        /// <summary>
        /// Does the spell reset the autoattack timer?
        /// </summary>
        public bool ResetsAutoAttackTimer = false;
        /// <summary>
        /// The Raw Spell Range
        /// </summary>
        public int Range = int.MaxValue;
        /// <summary>
        /// The raw radius of the spell (skillshots only)
        /// </summary>
        public int Radius;
        /// <summary>
        /// The width of the skillshot.
        /// </summary>
        public int Width = 50;
        /// <summary>
        /// The angle which the skillshot makes.
        /// </summary>
        public int Angle = 30;
        /// <summary>
        /// The spell's missile name
        /// </summary>
        public string MissileSpellName = "";
        /// <summary>
        /// The spell's missile acceleration
        /// </summary>
        public int MissileAccel = 0;
        /// <summary>
        /// Is the missile delayed?
        /// </summary>
        public bool MissileDelayed;
        /// <summary>
        /// Does the missile follow the target?
        /// </summary>
        public bool MissileFollowsUnit;
        /// <summary>
        /// The max speed the spell missile can reach
        /// </summary>
        public int MissileMaxSpeed = 0;
        /// <summary>
        /// The min speed you can find the missile at
        /// </summary>
        public int MissileMinSpeed = 0;
        /// <summary>
        /// Our spell missile average travel speed
        /// </summary>
        public int MissileSpeed = 1000;
        /// <summary>
        /// Specifies on a scale from 1 to 5 how dangerous our spell is
        /// </summary>
        public int DangerValue = 1;
        /// <summary>
        /// Extra missile names
        /// </summary>
        public string[] ExtraMissileNames = { };
        /// <summary>
        /// Extra spell names
        /// </summary>
        public string[] ExtraSpellNames = { };
        /// <summary>
        /// Is our spell dangerous?
        /// </summary>
        public bool IsDangerous = false;
        /// <summary>
        /// Source object name
        /// </summary>
        public string FromObject = "";
        /// <summary>
        /// Source object name
        /// </summary>
        public string SourceObjectName = "";
        /// <summary>
        /// Particle name on toggle
        /// </summary>
        public string ToggleParticleName = "";
        /// <summary>
        /// Source objects' names
        /// </summary>
        public string[] FromObjects = { };
        /// <summary>
        /// What the spell missile (if any) can collide with.
        /// </summary>
        public CollisionableObjects[] CollisionObjects = { };

        //OnProcessSpell Missile Detection stuff
        public bool CanBeRemoved = false;
        public bool ForceRemove = false;

        public SpellDatabaseEntry() { }

        public SpellDatabaseEntry(string championName,
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
            ChampionName = championName;
            SpellName = spellName;
            Slot = slot;
            SpellType = spellType;
            CastType = castType;
            SpellTags = spellTags;
            ResetsAutoAttackTimer = false;
            Delay = delay;
            Range = range;
            Radius = radius;
            Width = width;
            MissileSpeed = missileSpeed;
            Angle = angle;
            DangerValue = defaultDangerValue;
        }
    }
}
