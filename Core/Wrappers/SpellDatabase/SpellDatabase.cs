// <copyright file="SpellDatabase.cs" company="LeagueSharp">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.SDK.Core.Enumerations;
using LeagueSharp.SDK.Core.Extensions;
using LeagueSharp.SDK.Core.Extensions.SharpDX;
using LeagueSharp.SDK.Core.UI.IMenu.Values;
using LeagueSharp.SDK.Core.Utils;
using LeagueSharp.SDK.Core.Wrappers.Damages;

namespace LeagueSharp.SDK.Core.Wrappers.SpellDatabase
{

    public static class SpellDatabase
    {
        /// <summary>
        /// A list of all the entries in the SpellDatabase.
        /// </summary>
        public static List<SpellDatabaseEntry> Spells = new List<SpellDatabaseEntry>();

        static SpellDatabase()
        {
            //Add spells to the database

            #region Aatrox

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Aatrox",
                    SpellName = "AatroxQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockup },
                    SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Damage, SpellTags.Dash },
                    Delay = 600,
                    Range = 650,
                    Radius = 250,
                    MissileSpeed = 2000,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                }); 
            
            Spells.Add(
                  new SpellDatabaseEntry
                  {
                      ChampionName = "Aatrox",
                      SpellName = "AatroxW",
                      Slot = SpellSlot.W,
                      SpellType = SpellType.Toggled,
                      CastType = new[] { CastType.Toggle },
                      SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.Heal },
                      Delay = 0,
                      Range = int.MaxValue,
                      Radius = int.MaxValue,
                      MissileSpeed = int.MaxValue,
                      DangerValue = 2,
                      IsDangerous = false,
                      MissileSpellName = "",
                  });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Aatrox",
                     SpellName = "AatroxWPower",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.Toggled,
                     CastType = new[] { CastType.Toggle },
                     SpellTags = new[] { SpellTags.DamageAmplifier },
                     Delay = 0,
                     Range = int.MaxValue,
                     Radius = int.MaxValue,
                     MissileSpeed = int.MaxValue,
                     DangerValue = 2,
                     IsDangerous = false,
                     MissileSpellName = "",
                 });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Aatrox",
                     SpellName = "AatroxWHeal",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.Toggled,
                     CastType = new[] { CastType.Toggle },
                     SpellTags = new[] { SpellTags.Heal },
                     Delay = 0,
                     Range = int.MaxValue,
                     Radius = int.MaxValue,
                     MissileSpeed = int.MaxValue,
                     DangerValue = 2,
                     IsDangerous = false,
                     MissileSpellName = "",
                 });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Aatrox",
                    SpellName = "AatroxE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 1075,
                    Radius = 35,
                    MissileSpeed = 1250,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "AatroxEConeMissile",
                });

            Spells.Add(
                  new SpellDatabaseEntry
                  {
                      ChampionName = "Aatrox",
                      SpellName = "AatroxR",
                      Slot = SpellSlot.R,
                      SpellType = SpellType.Activated,
                      CastType = new[] { CastType.Activate },
                      SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.AttackSpeedAmplifier, SpellTags.AttackRangeModifier },
                      Delay = 0,
                      Range = 550,
                      Radius = int.MaxValue,
                      MissileSpeed = int.MaxValue,
                      DangerValue = 2,
                      IsDangerous = false,
                      MissileSpellName = "",
                  });

            #endregion Aatrox

            #region Akali

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Akali",
                    SpellName = "AkaliMota",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.LeavesMark },
                    Delay = 250,
                    Range = 600,
                    Radius = 600,
                    MissileSpeed = 1000,
                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "AkaliMota",
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Akali",
                     SpellName = "AkaliSmokeBomb",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.SkillshotCircle,
                     CastType = new[] { CastType.Position },
                     AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                     AppliedBuffsOnSelf = new[] { BuffType.Invisibility },
                     SpellTags = new[] { SpellTags.MovementSpeedAmplifier },
                     Delay = 500,
                     Range = 700,
                     Radius = 400,
                     MissileSpeed = int.MaxValue,
                     DangerValue = 2,
                     IsDangerous = false,
                     MissileSpellName = "AkaliSmokeBomb",
                 });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Akali",
                    SpellName = "AkaliShadowSwipe",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CanDetonateMark },
                    Delay = 250,
                    Range = 325,
                    Radius = int.MaxValue,
                    MissileSpeed = 1250,
                    DangerValue = 3,
                    IsDangerous = false,
                });

            Spells.Add(
                  new SpellDatabaseEntry
                  {
                      ChampionName = "Akali",
                      SpellName = "AkaliShadowDance",
                      Slot = SpellSlot.R,
                      SpellType = SpellType.Targeted,
                      CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                      SpellTags = new[] { SpellTags.Dash, SpellTags.Damage, SpellTags.CanDetonateMark },
                      Delay = 0,
                      Range = 700,
                      Radius = int.MaxValue,
                      MissileSpeed = 2200,
                      DangerValue = 4,
                      IsDangerous = true,
                      MissileSpellName = "AkaliShadowDanceKick",
                  });

            #endregion Akali

            #region Alistar

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Alistar",
                    SpellName = "Pulverize",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockup, BuffType.Stun },
                    SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Damage, SpellTags.AoE },
                    Delay = 250,
                    Range = 365,
                    Radius = 365,
                    MissileSpeed = 20,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Alistar",
                     SpellName = "Headbutt",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.Targeted,
                     CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                     AppliedBuffsOnEnemies = new[] { BuffType.Knockback, BuffType.Stun },
                     SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Damage },
                     Delay = 0,
                     Range = 650,
                     Radius = 650,
                     MissileSpeed = 0,
                     DangerValue = 3,
                     IsDangerous = false,
                     MissileSpellName = "",
                 });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Alistar",
                     SpellName = "TriumphantRoar",
                     Slot = SpellSlot.E,
                     SpellType = SpellType.Targeted,
                     CastType = new[] { CastType.Activate },
                     SpellTags = new[] { SpellTags.Heal },
                     Delay = 0,
                     Range = 575,
                     Radius = 575,
                     MissileSpeed = 0,
                     DangerValue = 2,
                     IsDangerous = false,
                     MissileSpellName = "",
                 });

            Spells.Add(
                  new SpellDatabaseEntry
                  {
                      ChampionName = "Alistar",
                      SpellName = "FerociousHowl",
                      Slot = SpellSlot.R,
                      SpellType = SpellType.Activated,
                      CastType = new[] { CastType.Activate },
                      SpellTags = new[] { SpellTags.DefensiveBuff, SpellTags.DamageAmplifier },
                      Delay = 0,
                      Range = int.MaxValue,
                      Radius = int.MaxValue,
                      MissileSpeed = 828,
                      DangerValue = 2,
                      IsDangerous = false,
                      MissileSpellName = "",
                  });

            #endregion Alistar

            #region Ahri

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ahri",
                    SpellName = "AhriOrbofDeception",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.MovementSpeedAmplifier },
                    Delay = 250,
                    Range = 1000,
                    Radius = 100,
                    MissileSpeed = 2500,
                    MissileAccel = -3200,
                    MissileMaxSpeed = 2500,
                    MissileMinSpeed = 400,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "AhriOrbMissile",
                    CanBeRemoved = true,
                    ForceRemove = true,
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ahri",
                    SpellName = "AhriOrbReturn",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.ImpossibleToCast },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 1000,
                    Radius = 100,
                    MissileSpeed = 60,
                    MissileAccel = 1900,
                    MissileMinSpeed = 60,
                    MissileMaxSpeed = 2600,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileFollowsUnit = true,
                    CanBeRemoved = true,
                    ForceRemove = true,
                    MissileSpellName = "AhriOrbReturn",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Ahri",
                     SpellName = "AhriFoxFire",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.Targeted,
                     CastType = new[] { CastType.Activate },
                     SpellTags = new[] { SpellTags.Damage, SpellTags.Heal },
                     Delay = 0,
                     Range = 550,
                     Radius = int.MaxValue,
                     MissileSpeed = 900,
                     MissileAccel = 0,
                     MissileMinSpeed = 1400,
                     MissileMaxSpeed = 1400,
                     DangerValue = 2,
                     IsDangerous = false,
                     MissileFollowsUnit = true,
                     CanBeRemoved = true,
                     ForceRemove = true,
                     MissileSpellName = "AhriFoxFireMissile",
                     CollisionObjects = new[] { CollisionableObjects.YasuoWall, CollisionableObjects.Minions, CollisionableObjects.Heroes }
                 });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ahri",
                    SpellName = "AhriSeduce",
                    Slot = SpellSlot.E,
                    CastType = new[] { CastType.Position },
                    SpellType = SpellType.SkillshotMissileLine,
                    AppliedBuffsOnEnemies = new[] { BuffType.Charm },
                    Delay = 250,
                    Range = 1000,
                    Radius = 60,
                    MissileSpeed = 1550,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "AhriSeduceMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Ahri",
                     SpellName = "AhriTumble",
                     Slot = SpellSlot.R,
                     SpellType = SpellType.SkillshotLine,
                     CastType = new[] { CastType.Position },
                     SpellTags = new[] { SpellTags.Damage, SpellTags.Dash },
                     Delay = 0,
                     Range = 450,
                     Radius = 600,
                     MissileSpeed = 2200,
                     MissileAccel = 0,
                     MissileMinSpeed = 2200,
                     MissileMaxSpeed = 2200,
                     DangerValue = 2,
                     IsDangerous = false,
                     MissileSpellName = "AhriTumbleMissile",
                 });

            #endregion Ahri

            #region Amumu

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Amumu",
                    SpellName = "BandageToss",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                    Delay = 250,
                    Range = 1100,
                    Radius = 90,
                    MissileSpeed = 2000,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "SadMummyBandageToss",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                   new SpellDatabaseEntry
                   {
                       ChampionName = "Amumu",
                       SpellName = "AuraofDespair",
                       Slot = SpellSlot.W,
                       SpellType = SpellType.Toggled,
                       CastType = new[] { CastType.Toggle },
                       SpellTags = new[] { SpellTags.Damage },
                       Delay = 250,
                       Range = 300,
                       Radius = 300,
                       MissileSpeed = int.MaxValue,
                       DangerValue = 2,
                       IsDangerous = false,
                       MissileSpellName = "",
                   });

            Spells.Add(
                   new SpellDatabaseEntry
                   {
                       ChampionName = "Amumu",
                       SpellName = "Tantrum",
                       Slot = SpellSlot.E,
                       SpellType = SpellType.SkillshotCone,
                       CastType = new[] { CastType.Activate },
                       SpellTags = new[] { SpellTags.Damage },
                       Delay = 250,
                       Range = 350,
                       Radius = 350,
                       MissileSpeed = int.MaxValue,
                       DangerValue = 2,
                       IsDangerous = false,
                       MissileSpellName = "",
                   });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Amumu",
                    SpellName = "CurseoftheSadMummy",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Snare },
                    Delay = 250,
                    Range = 0,
                    Radius = 550,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            #endregion Amumu

            #region Anivia

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Anivia",
                    SpellName = "FlashFrost",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl, SpellTags.AoE },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                    Delay = 250,
                    Range = 1100,
                    Radius = 110,
                    MissileSpeed = 850,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "FlashFrostSpell",
                    CanBeRemoved = true,
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Anivia",
                    SpellName = "Crystalize",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockback },
                    Delay = 250,
                    Range = 1000,
                    Radius = 100,
                    MissileSpeed = 1600,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "",
                    CanBeRemoved = false,
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Anivia",
                    SpellName = "Frostbite",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 650,
                    Radius = 650,
                    MissileSpeed = 1200,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "",
                    CanBeRemoved = false,
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Anivia",
                    SpellName = "GlacialStorm",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl, SpellTags.AoE },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 625,
                    Radius = 400,
                    MissileSpeed = 1600,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "GlacialStormSpell",
                    CanBeRemoved = false,
                });

            #endregion Anivia

            #region Annie

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Annie",
                    SpellName = "Disintegrate",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 625,
                    Radius = 710,
                    MissileSpeed = 1400,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "Disintegrate",
                    CanBeRemoved = false,
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Annie",
                    SpellName = "Incinerate",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCone,
                    CastType = new[] { CastType.Direction },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Delay = 250,
                    Range = 825,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Annie",
                    SpellName = "MoltenShield",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DefensiveBuff },
                    Delay = 250,
                    Range = int.MaxValue,
                    Radius = int.MaxValue,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Annie",
                    SpellName = "InfernalGuardian",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Delay = 250,
                    Range = 600,
                    Radius = 251,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            #endregion Annie

            #region Ashe

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ashe",
                    SpellName = "AsheQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AttackSpeedAmplifier },
                    Delay = 250,
                    Range = int.MaxValue,
                    Radius = int.MaxValue,
                    MissileSpeed = 2500,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                    CanBeRemoved = false,
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ashe",
                    SpellName = "Volley",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Direction },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 1250,
                    Radius = 60,
                    MissileSpeed = 1500,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VolleyAttack",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall, CollisionableObjects.Minions }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ashe",
                    SpellName = "AsheSpiritOfTheHawk",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.GrantsVision },
                    Delay = 250,
                    Range = 25000,
                    Radius = 299,
                    MissileSpeed = 1400,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                    CanBeRemoved = false,
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ashe",
                    SpellName = "EnchantedCrystalArrow",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                    Delay = 250,
                    Range = 20000,
                    Radius = 130,
                    MissileSpeed = 1600,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "EnchantedCrystalArrow",
                    CanBeRemoved = true,
                    CollisionObjects = new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall }
                });

            #endregion Ashe

            #region Azir

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Azir",
                    SpellName = "AzirQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl, SpellTags.AoE },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 875,
                    Radius = 60,
                    MissileSpeed = 2550,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                    CanBeRemoved = false,
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Azir",
                    SpellName = "AzirW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 450,
                    Radius = 350,
                    MissileSpeed = 2500,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "AzirSoldierMissile",
                    CanBeRemoved = true,
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Azir",
                    SpellName = "AzirE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE, SpellTags.Dash },
                    Delay = 250,
                    Range = 1100,
                    Radius = 0,
                    MissileSpeed = 2000,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                    CanBeRemoved = false,
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Azir",
                    SpellName = "AzirR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCone,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockback },
                    Delay = 250,
                    Range = 250,
                    Radius = 60,
                    MissileSpeed = 1000,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "AzirSoldierRMissile",
                    CanBeRemoved = true,
                });

            #endregion Ashe

            #region Bard

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Bard",
                    SpellName = "BardQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1600,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "BardQMissile",
                    CanBeRemoved = true,
                    CollisionObjects = new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Bard",
                     SpellName = "BardW",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.SkillshotCircle,
                     CastType = new[] { CastType.Position },
                     SpellTags = new[] { SpellTags.Heal, SpellTags.MovementSpeedAmplifier },
                     Delay = 250,
                     Range = 800,
                     Radius = 100,
                     MissileSpeed = 0,
                     DangerValue = 3,
                     IsDangerous = true,
                     MissileSpellName = "BardWHealthPack",
                     CanBeRemoved = true,
                 });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Bard",
                     SpellName = "BardE",
                     Slot = SpellSlot.E,
                     SpellType = SpellType.SkillshotLine,
                     CastType = new[] { CastType.Position },
                     SpellTags = new[] { SpellTags.MovementSpeedAmplifier },
                     Delay = 250,
                     Range = 900,
                     Radius = 2600,
                     MissileSpeed = 0,
                     DangerValue = 0,
                     IsDangerous = true,
                     MissileSpellName = "",
                     CanBeRemoved = true,
                 });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Bard",
                    SpellName = "BardR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    AppliedBuffsOnAllies = new[] { BuffType.Invulnerability },
                    AppliedBuffsOnEnemies = new[] { BuffType.Invulnerability },
                    AppliedBuffsOnSelf = new[] { BuffType.Invulnerability },
                    Delay = 500,
                    Range = 3400,
                    Radius = 350,
                    MissileSpeed = 2100,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "BardR",
                });

            #endregion

            #region Blitzcrank

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Blitzcrank",
                    SpellName = "RocketGrab",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                    Delay = 250,
                    Range = 1050,
                    Radius = 70,
                    MissileSpeed = 1800,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "RocketGrabMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Blitzcrank",
                    SpellName = "Overdrive",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.MovementSpeedAmplifier },
                    Delay = 250,
                    Range = int.MaxValue,
                    Radius = int.MaxValue,
                    MissileSpeed = 0,
                    DangerValue = 0,
                    MissileSpellName = "",
                    CanBeRemoved = true,
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Blitzcrank",
                    SpellName = "PowerFist",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Damage },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockup },
                    Delay = 250,
                    Range = int.MaxValue,
                    Radius = int.MaxValue,
                    MissileSpeed = 0,
                    DangerValue = 0,
                    MissileSpellName = "",
                    CanBeRemoved = true,
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Blitzcrank",
                    SpellName = "StaticField",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Damage, SpellTags.AoE },
                    AppliedBuffsOnEnemies = new[] { BuffType.Silence },
                    Delay = 250,
                    Range = 0,
                    Radius = 600,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            #endregion Blitzcrink

            #region Brand

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Brand",
                    SpellName = "BrandBlaze",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 1100,
                    Radius = 60,
                    MissileSpeed = 1600,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "BrandBlazeMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Brand",
                    SpellName = "BrandFissure",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Delay = 850,
                    Range = 900,
                    Radius = 240,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Brand",
                    SpellName = "BrandConflagration",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 625,
                    Radius = int.MaxValue,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Brand",
                    SpellName = "BrandWildfire",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Delay = 250,
                    Range = 750,
                    Radius = 600,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "BrandWildfireMissile",
                });

            #endregion Brand

            #region Braum

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Braum",
                    SpellName = "BraumQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 1050,
                    Radius = 60,
                    MissileSpeed = 1700,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "BraumQMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall, CollisionableObjects.BraumShield }
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Braum",
                     SpellName = "BraumW",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.Targeted,
                     CastType = new[] { CastType.AllyChampions, CastType.AllyMinions },
                     SpellTags = new[] { SpellTags.DefensiveBuff },
                     Delay = 250,
                     Range = 650,
                     DangerValue = 1,
                     IsDangerous = false,
                 });

            Spells.Add(
      new SpellDatabaseEntry
      {
          ChampionName = "Braum",
          SpellName = "BraumE",
          Slot = SpellSlot.E,
          SpellType = SpellType.SkillshotLine,
          CastType = new[] { CastType.Direction },
          SpellTags = new[] { SpellTags.DefensiveBuff, SpellTags.Shield },
          Delay = 250,
          Range = 325,
          DangerValue = 1,
          IsDangerous = false,
      });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Braum",
                    SpellName = "BraumRWrapper",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl, SpellTags.AoE },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockup },
                    Delay = 500,
                    Range = 1200,
                    Radius = 115,
                    MissileSpeed = 1400,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "braumrmissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            #endregion Braum

            #region Caitlyn

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Caitlyn",
                    SpellName = "CaitlynPiltoverPeacemaker",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Delay = 625,
                    Range = 1300,
                    Radius = 90,
                    MissileSpeed = 2200,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "CaitlynPiltoverPeacemaker",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Caitlyn",
                     SpellName = "CaitlynYordleTrap",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.SkillshotCircle,
                     CastType = new[] { CastType.Position },
                     SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                     AppliedBuffsOnEnemies = new[] { BuffType.Snare },
                     Delay = 1100,
                     Range = 800,
                     Radius = 67,
                     DangerValue = 1,
                     IsDangerous = false,
                     MissileSpellName = "CaitlynTrap",
                 });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Caitlyn",
                    SpellName = "CaitlynEntrapment",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 125,
                    Range = 1000,
                    Radius = 70,
                    MissileSpeed = 1600,
                    DangerValue = 1,
                    IsDangerous = false,
                    MissileSpellName = "CaitlynEntrapmentMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall }
                });

            Spells.Add(
     new SpellDatabaseEntry
     {
         ChampionName = "Caitlyn",
         SpellName = "CaitlynAceintheHole",
         Slot = SpellSlot.R,
         SpellType = SpellType.Targeted,
         CastType = new[] { CastType.EnemyChampions },
         SpellTags = new[] { SpellTags.Damage },
         Delay = 3000,
         Range = 2000,
         DangerValue = 1,
         IsDangerous = false,
         CollisionObjects = new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall, CollisionableObjects.BraumShield }
     });

            #endregion Caitlyn

            #region Cassiopeia

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Cassiopeia",
                    SpellName = "CassiopeiaNoxiousBlast",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE, SpellTags.LeavesMark },
                    AppliedBuffsOnEnemies = new[] { BuffType.Poison },
                    Delay = 750,
                    Range = 850,
                    Radius = 100,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "CassiopeiaNoxiousBlast",
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Cassiopeia",
                     SpellName = "CassiopeiaMiasma",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.SkillshotCircle,
                     CastType = new[] { CastType.Position },
                     SpellTags = new[] { SpellTags.Damage, SpellTags.AoE, SpellTags.LeavesMark, SpellTags.CrowdControl },
                     AppliedBuffsOnEnemies = new[] { BuffType.Poison, BuffType.Slow },
                     Delay = 750,
                     Range = 850,
                     Radius = 150,
                     MissileSpeed = int.MaxValue,
                     DangerValue = 2,
                     IsDangerous = false,
                     MissileSpellName = "CassiopeiaMiasma",
                 });

            Spells.Add(
      new SpellDatabaseEntry
      {
          ChampionName = "Cassiopeia",
          SpellName = "CassiopeiaTwinFang",
          Slot = SpellSlot.E,
          SpellType = SpellType.Targeted,
          CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
          SpellTags = new[] { SpellTags.Damage, SpellTags.CanDetonateMark },
          Delay = 125,
          Range = 700,
          DangerValue = 1,
          IsDangerous = false,
      });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Cassiopeia",
                    SpellName = "CassiopeiaPetrifyingGaze",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCone,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl, SpellTags.AoE },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                    Delay = 600,
                    Range = 825,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "CassiopeiaPetrifyingGaze",
                });

            #endregion Cassiopeia

            #region Chogath

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Chogath",
                    SpellName = "Rupture",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockup },
                    Delay = 1200,
                    Range = 950,
                    Radius = 250,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "Rupture",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Chogath",
                    SpellName = "FeralScream",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCone,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Silence },
                    Delay = 500,
                    Range = 800,
                    Radius = 210,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            Spells.Add(
                  new SpellDatabaseEntry
                  {
                      ChampionName = "Chogath",
                      SpellName = "VorpalSpikes",
                      Slot = SpellSlot.E,
                      SpellType = SpellType.Toggled,
                      CastType = new[] { CastType.Toggle },
                      SpellTags = new[] { SpellTags.Damage, SpellTags.AppliesOnHitEffects, SpellTags.AoE },
                      Delay = 250,
                      Range = 500,
                      DangerValue = 1,
                      IsDangerous = false,
                  });

            Spells.Add(
                  new SpellDatabaseEntry
                  {
                      ChampionName = "Chogath",
                      SpellName = "Feast",
                      Slot = SpellSlot.R,
                      SpellType = SpellType.Targeted,
                      CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                      SpellTags = new[] { SpellTags.Damage, SpellTags.DefensiveBuff },
                      Delay = 250,
                      Range = 175,
                      DangerValue = 1,
                      IsDangerous = false,
                  });

            #endregion Chogath

            #region Corki

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Corki",
                    SpellName = "PhosphorusBomb",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 300,
                    Range = 825,
                    Radius = 250,
                    MissileSpeed = 1000,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "PhosphorusBombMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Corki",
                     SpellName = "CarpetBomb",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.SkillshotLine,
                     CastType = new[] { CastType.Position },
                     SpellTags = new[] { SpellTags.Damage, SpellTags.Dash },
                     Delay = 300,
                     Range = 600,
                     Radius = 200,
                     MissileSpeed = 1000,
                     DangerValue = 2,
                     IsDangerous = false,
                     MissileSpellName = "",
                 });

            Spells.Add(
                  new SpellDatabaseEntry
                  {
                      ChampionName = "Corki",
                      SpellName = "CarpetBombMega",
                      Slot = SpellSlot.W,
                      SpellType = SpellType.SkillshotLine,
                      CastType = new[] { CastType.Position },
                      SpellTags = new[] { SpellTags.Damage, SpellTags.Dash, SpellTags.CrowdControl },
                      AppliedBuffsOnEnemies = new[] { BuffType.Knockback },
                      Delay = 300,
                      Range = 1800,
                      Radius = 200,
                      MissileSpeed = 1000,
                      DangerValue = 2,
                      IsDangerous = false,
                      MissileSpellName = "",
                  });

            Spells.Add(
                   new SpellDatabaseEntry
                   {
                       ChampionName = "Corki",
                       SpellName = "GGun",
                       Slot = SpellSlot.E,
                       SpellType = SpellType.SkillshotCone,
                       CastType = new[] { CastType.Activate },
                       SpellTags = new[] { SpellTags.Damage },
                       Delay = 300,
                       Range = 600,
                       MissileSpeed = 1500,
                       DangerValue = 2,
                       IsDangerous = false,
                       MissileSpellName = "",
                   });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Corki",
                    SpellName = "MissileBarrage",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 200,
                    Range = 1300,
                    Radius = 40,
                    MissileSpeed = 2000,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "MissileBarrageMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Corki",
                    SpellName = "MissileBarrage2",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 200,
                    Range = 1500,
                    Radius = 40,
                    MissileSpeed = 2000,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "MissileBarrageMissile2",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall }
                });

            #endregion Corki

            #region Darius

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Darius",
                    SpellName = "DariusCleave",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Heal, SpellTags.AppliesOnHitEffects },
                    Delay = 750,
                    Range = 425,
                    Radius = 425,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "DariusCleave",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Darius",
                    SpellName = "DariusNoxianTacticsONH",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.AttackRangeModifier, SpellTags.DamageAmplifier, SpellTags.AppliesOnHitEffects },
                    ResetsAutoAttackTimer = true,
                    Delay = 750,
                    Range = int.MaxValue,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 1,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Darius",
                    SpellName = "DariusAxeGrabCone",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCone,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl, SpellTags.AppliesOnHitEffects },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 550,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DariusAxeGrabCone",
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Darius",
                     SpellName = "DariusExecute",
                     Slot = SpellSlot.R,
                     SpellType = SpellType.Targeted,
                     CastType = new[] { CastType.EnemyChampions },
                     SpellTags = new[] { SpellTags.Damage },
                     Delay = 500,
                     Range = 460,
                     MissileSpeed = int.MaxValue,
                     DangerValue = 3,
                     IsDangerous = true,
                     MissileSpellName = "",
                 });

            #endregion Darius

            #region Diana

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Diana",
                    SpellName = "DianaArc",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.AoE, SpellTags.Damage, SpellTags.LeavesMark },
                    Delay = 250,
                    Range = 895,
                    Radius = 195,
                    MissileSpeed = 1400,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DianaArcArc",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
            new SpellDatabaseEntry
            {
                ChampionName = "Diana",
                SpellName = "DianaOrbs",
                Slot = SpellSlot.W,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.AoE, SpellTags.Damage, SpellTags.Shield },
                Delay = 0,
                Range = 200,
                Radius = 315,
                MissileSpeed = 1400,
                DangerValue = 2,
                IsDangerous = false,
            });

            Spells.Add(
            new SpellDatabaseEntry
            {
                ChampionName = "Diana",
                SpellName = "DianaVortex",
                Slot = SpellSlot.E,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.AoE, SpellTags.Damage, SpellTags.CrowdControl },
                AppliedBuffsOnEnemies = new[] { BuffType.Slow, BuffType.Knockback },
                Delay = 0,
                Range = 350,
                Radius = 450,
                MissileSpeed = 0,
                DangerValue = 3,
                IsDangerous = false,
            });

            Spells.Add(
            new SpellDatabaseEntry
            {
                ChampionName = "Diana",
                SpellName = "DianaTeleport",
                Slot = SpellSlot.R,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                SpellTags = new[] { SpellTags.Damage, SpellTags.Dash },
                Delay = 0,
                Range = 825,
                Radius = 250,
                MissileSpeed = 0,
                DangerValue = 3,
                IsDangerous = false,
            });

            #endregion Diana

            #region DrMundo

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "DrMundo",
                    SpellName = "InfectedCleaverMissileCast",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 1050,
                    Radius = 60,
                    MissileSpeed = 2000,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "InfectedCleaverMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "DrMundo",
                    SpellName = "BurningAgony",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Toggled,
                    CastType = new[] { CastType.Toggle },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.DefensiveBuff },
                    Delay = 300,
                    Range = 162,
                    MissileSpeed = 20,
                    DangerValue = 1,
                    IsDangerous = false,
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "DrMundo",
                     SpellName = "Masochism",
                     Slot = SpellSlot.E,
                     SpellType = SpellType.Activated,
                     CastType = new[] { CastType.Activate },
                     SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.AttackRangeModifier },
                     ResetsAutoAttackTimer = true,
                     Delay = 300,
                     Range = 25,
                     DangerValue = 1,
                     IsDangerous = false,
                 });

            Spells.Add(
                  new SpellDatabaseEntry
                  {
                      ChampionName = "DrMundo",
                      SpellName = "Sadism",
                      Slot = SpellSlot.R,
                      SpellType = SpellType.Activated,
                      CastType = new[] { CastType.Activate },
                      SpellTags = new[] { SpellTags.Heal, SpellTags.MovementSpeedAmplifier },
                      Delay = 250,
                      Range = int.MaxValue,
                      DangerValue = 1,
                      IsDangerous = false,
                  });

            #endregion DrMundo

            #region Draven

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Draven",
                    SpellName = "DravenSpinning",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier },
                    Range = int.MaxValue
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Draven",
                    SpellName = "DravenFury",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.MovementSpeedAmplifier, SpellTags.AttackSpeedAmplifier },
                    Range = int.MaxValue
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Draven",
                    SpellName = "DravenDoubleShot",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockback },
                    Delay = 250,
                    Range = 1100,
                    Radius = 130,
                    MissileSpeed = 1400,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DravenDoubleShotMissile",
                    CanBeRemoved = true,
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Draven",
                    SpellName = "DravenRCast",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 400,
                    Range = 20000,
                    Radius = 160,
                    MissileSpeed = 2000,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "DravenR",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            #endregion Draven

            #region Ekko

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1650,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "ekkoqmis",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Shield },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                    Delay = 3750,
                    Range = 1600,
                    Radius = 375,
                    MissileSpeed = 1650,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "EkkoW",
                    CanBeRemoved = true
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Direction },
                    SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.AppliesOnHitEffects, SpellTags.Dash },
                    ResetsAutoAttackTimer = true,
                    Delay = 250,
                    Range = 325,
                    Radius = 100,
                    MissileSpeed = 2500
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ekko",
                    SpellName = "EkkoR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Heal, SpellTags.Dash },
                    Delay = 250,
                    Range = 1600,
                    Radius = 375,
                    MissileSpeed = 1650,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "EkkoR",
                    CanBeRemoved = true,
                    FromObjects = new[] { "Ekko_Base_R_TrailEnd.troy" }
                });

            #endregion Ekko

            #region Elise
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Elise",
                    SpellName = "EliseHumanQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 625
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Elise",
                    SpellName = "EliseSpiderQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Dash },
                    Delay = 250,
                    Range = 475
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Elise",
                    SpellName = "EliseHumanW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Delay = 3000,
                    Range = 975,
                    Radius = 235
                });

            Spells.Add(
            new SpellDatabaseEntry
            {
                ChampionName = "Elise",
                SpellName = "EliseSpiderW",
                Slot = SpellSlot.W,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.AttackSpeedAmplifier },
                ResetsAutoAttackTimer = true,
                Delay = 250,
                Range = int.MaxValue
            });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Elise",
                    SpellName = "EliseHumanE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                    Delay = 250,
                    Range = 1100,
                    Radius = 55,
                    MissileSpeed = 1600,
                    DangerValue = 4,
                    IsDangerous = true,
                    MissileSpellName = "EliseHumanE",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall }
                });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Elise",
                SpellName = "EliseSpiderE",
                Slot = SpellSlot.E,
                SpellType = SpellType.SkillshotCircle,
                CastType = new[] { CastType.Position, CastType.EnemyChampions, CastType.EnemyMinions },
                SpellTags = new[] { SpellTags.Dash, SpellTags.DamageAmplifier, SpellTags.AppliesOnHitEffects, SpellTags.DefensiveBuff },
                Delay = 1000,
                Range = 750,
                Radius = 400, //#TODO: Get the actual elise spider form's hitbox, although it's can also be casted to a target so it doesn't really matter.
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Elise",
                SpellName = "EliseR",
                Slot = SpellSlot.R,
                SpellType = SpellType.Toggled,
                CastType = new[] { CastType.Toggle },
                SpellTags = new[] { SpellTags.Transformation },
                Delay = 1000,
                Range = 125
            });

            #endregion Elise

            #region Evelynn
            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Evelynn",
                SpellName = "EvelynnQ",
                Slot = SpellSlot.Q,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                Delay = 250,
                Range = 500,
                Radius = 450
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Evelynn",
                SpellName = "EvelynnW",
                Slot = SpellSlot.W,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.MovementSpeedAmplifier },
                Delay = 250,
                Range = int.MaxValue
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Evelynn",
                SpellName = "EvelynnE",
                Slot = SpellSlot.E,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions },
                SpellTags = new[] { SpellTags.AttackSpeedAmplifier, SpellTags.AppliesOnHitEffects },
                Delay = 250,
                Range = 225
            });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Evelynn",
                    SpellName = "EvelynnR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Damage },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 650,
                    Radius = 350,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "EvelynnR",
                });

            #endregion Evelynn

            #region Ezreal

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ezreal",
                    SpellName = "EzrealMysticShot",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AppliesOnHitEffects },
                    Delay = 250,
                    Range = 1200,
                    Radius = 60,
                    MissileSpeed = 2000,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "EzrealMysticShotMissile",
                    ExtraMissileNames = new[] { "EzrealMysticShotPulseMissile" },
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ezreal",
                    SpellName = "EzrealEssenceFlux",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Delay = 250,
                    Range = 1050,
                    Radius = 80,
                    MissileSpeed = 1600,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "EzrealEssenceFluxMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ezreal",
                    SpellName = "EzrealArcaneShift",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Dash },
                    ResetsAutoAttackTimer = true,
                    Delay = 250,
                    Range = 475,
                    Radius = 275
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ezreal",
                    SpellName = "EzrealTrueshotBarrage",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Delay = 1000,
                    Range = 20000,
                    Radius = 160,
                    MissileSpeed = 2000,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "EzrealTrueshotBarrage",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            #endregion Ezreal

            #region FiddleSticks
            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "FiddleSticks",
                SpellName = "Terrify",
                Slot = SpellSlot.Q,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Damage },
                AppliedBuffsOnEnemies = new[] { BuffType.Flee },
                Delay = 250,
                Range = 575

            });


            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "FiddleSticks",
                SpellName = "Drain",
                Slot = SpellSlot.W,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                SpellTags = new[] { SpellTags.Damage, SpellTags.Heal },
                Delay = 250,
                Range = 575

            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "FiddleSticks",
                SpellName = "FiddlesticksDarkWind",
                Slot = SpellSlot.E,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                AppliedBuffsOnEnemies = new[] { BuffType.Silence },
                Delay = 250,
                Range = 750

            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "FiddleSticks",
                SpellName = "Crowstorm",
                Slot = SpellSlot.R,
                SpellType = SpellType.SkillshotCircle,
                CastType = new[] { CastType.Position },
                SpellTags = new[] { SpellTags.Damage },
                Delay = 250,
                Range = 800,
                Radius = 600

            });
            #endregion FiddleSticks

            #region Fiora
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Fiora",
                    SpellName = "FioraQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Dash, SpellTags.DamageAmplifier, SpellTags.AppliesOnHitEffects },
                    Delay = 250,
                    Range = 400,
                    Radius = 300
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Fiora",
                    SpellName = "FioraW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow, BuffType.Stun },
                    Delay = 500,
                    Range = 800,
                    Radius = 70,
                    MissileSpeed = 3200,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "FioraWMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Fiora",
                SpellName = "FioraE",
                Slot = SpellSlot.E,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.AttackRangeModifier, SpellTags.AttackSpeedAmplifier, SpellTags.CrowdControl },
                AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                Delay = 250,
                Range = 200
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Fiora",
                SpellName = "FioraR",
                Slot = SpellSlot.R,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions },
                SpellTags = new[] { SpellTags.MovementSpeedAmplifier, SpellTags.DamageAmplifier, SpellTags.Heal },
                Delay = 250,
                Range = 500
            });

            #endregion Fiora

            #region Fizz

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Fizz",
                    SpellName = "FizzPiercingStrike",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.AppliesOnHitEffects, SpellTags.Damage, },
                    Delay = 250,
                    Range = 550
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Fizz",
                    SpellName = "FizzSeastonePassive",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier },
                    Delay = 250,
                    Range = int.MaxValue
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Fizz",
                    SpellName = "FizzJump",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Range = 400,
                    Radius = 330
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Fizz",
                     SpellName = "FizzJumpTwo",
                     Slot = SpellSlot.E,
                     SpellType = SpellType.SkillshotCircle,
                     CastType = new[] { CastType.Position },
                     SpellTags = new[] { SpellTags.Damage },
                     Range = 400,
                     Radius = 270
                 });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Fizz",
                    SpellName = "FizzMarinerDoom",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 1300,
                    Radius = 120,
                    MissileSpeed = 1350,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "FizzMarinerDoomMissile",
                    CollisionObjects = new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall },
                    CanBeRemoved = true,
                });

            #endregion Fizz

            #region Galio

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Galio",
                    SpellName = "GalioResoluteSmite",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 900,
                    Radius = 200,
                    MissileSpeed = 1300,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GalioResoluteSmite",
                });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Galio",
                SpellName = "GalioBulwark",
                Slot = SpellSlot.W,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.AllyChampions },
                SpellTags = new[] { SpellTags.Shield, SpellTags.DefensiveBuff },
                Delay = 250,
                Range = 800
            });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Galio",
                    SpellName = "GalioRighteousGust",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.MovementSpeedAmplifier },
                    Delay = 250,
                    Range = 1200,
                    Radius = 120,
                    MissileSpeed = 1200,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GalioRighteousGust",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Galio",
                    SpellName = "GalioIdolOfDurand",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Damage },
                    AppliedBuffsOnEnemies = new[] { BuffType.Taunt },
                    Delay = 250,
                    Range = 0,
                    Radius = 550,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            #endregion Galio

            #region Gangplank
            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Gangplank",
                SpellName = "GangplankQWrapper",
                Slot = SpellSlot.Q,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                SpellTags = new[] { SpellTags.Damage },
                Delay = 250,
                Range = 625
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Gangplank",
                SpellName = "GangplankW",
                Slot = SpellSlot.W,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.Heal, SpellTags.RemoveCrowdControl },
                Delay = 250,
                Range = int.MaxValue
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Gangplank",
                SpellName = "GangplankE",
                Slot = SpellSlot.E,
                SpellType = SpellType.SkillshotCircle,
                CastType = new[] { CastType.Position },
                SpellTags = new[] { SpellTags.Damage, SpellTags.MovementSpeedAmplifier, SpellTags.CrowdControl },
                AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                Delay = 1000,
                Range = 1000,
                Radius = 400
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Gangplank",
                SpellName = "GangplankR",
                Slot = SpellSlot.R,
                SpellType = SpellType.SkillshotCircle,
                CastType = new[] { CastType.Position },
                SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                Delay = 500,
                Range = 20000,
                Radius = 600
            });
            #endregion

            #region Gnar
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 1125,
                    Radius = 60,
                    MissileSpeed = 2500,
                    MissileAccel = -3000,
                    MissileMaxSpeed = 2500,
                    MissileMinSpeed = 1400,
                    DangerValue = 2,
                    IsDangerous = false,
                    CanBeRemoved = true,
                    ForceRemove = true,
                    MissileSpellName = "gnarqmissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarBigQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 500,
                    Range = 1150,
                    Radius = 90,
                    MissileSpeed = 2100,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GnarBigQMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Gnar",
                SpellName = "GnarW",
                Slot = SpellSlot.W,
                SpellType = SpellType.Passive,
                CastType = new[] { CastType.ImpossibleToCast },
                SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.MovementSpeedAmplifier }
            });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarBigW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                    Delay = 600,
                    Range = 600,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GnarBigW",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Dash },
                    Delay = 0,
                    Range = 473,
                    Radius = 150,
                    MissileSpeed = 903,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GnarE",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarBigE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Dash },
                    Delay = 250,
                    Range = 475,
                    Radius = 200,
                    MissileSpeed = 1000,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GnarBigE",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Gnar",
                    SpellName = "GnarR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                    Delay = 250,
                    Range = 0,
                    Radius = 500,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            #endregion

            #region Gragas

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl, SpellTags.AoE },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 1100,
                    Radius = 275,
                    MissileSpeed = 1300,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GragasQMissile",
                    ToggleParticleName = "Gragas_.+_Q_(Enemy|Ally)",
                });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Gragas",
                SpellName = "GragasW",
                Slot = SpellSlot.W,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Channel },
                SpellTags = new[] { SpellTags.DefensiveBuff, SpellTags.DamageAmplifier },
                Delay = 1250
            });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl, SpellTags.AoE },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockback },
                    Delay = 0,
                    Range = 950,
                    Radius = 200,
                    MissileSpeed = 1200,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GragasE",
                    CanBeRemoved = true,
                    CollisionObjects = new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Gragas",
                    SpellName = "GragasR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl, SpellTags.AoE },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockback },
                    Delay = 250,
                    Range = 1050,
                    Radius = 375,
                    MissileSpeed = 1800,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "GragasRBoom",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            #endregion Gragas

            #region Graves

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Graves",
                    SpellName = "GravesQLineSpell",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Delay = 250,
                    Range = 808,
                    Radius = 40,
                    MissileSpeed = 3000,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "GravesQLineMis",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Graves",
                    SpellName = "GravesClusterShot",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCone,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Delay = 650,
                    Range = 950,
                    Radius = 100
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Graves",
                    SpellName = "GravesSmokeGrenade",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.NearSight },
                    Delay = 250,
                    Range = 950,
                    Radius = 250
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Graves",
                    SpellName = "GravesMove",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Dash, SpellTags.DefensiveBuff },
                    ResetsAutoAttackTimer = true,
                    Delay = 250,
                    Range = 425,
                    Radius = 100
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Graves",
                    SpellName = "GravesChargeShot",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Delay = 250,
                    Range = 1100,
                    Radius = 100,
                    MissileSpeed = 2100,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "GravesChargeShotShot",
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            #endregion Graves

            #region Hecarim
            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Hecarim",
                SpellName = "HecarimRapidSlash",
                Slot = SpellSlot.Q,
                SpellType = SpellType.SkillshotCircle,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.Damage },
                Delay = 250,
                Range = 350,
                Radius = 350
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Hecarim",
                SpellName = "HecarimW",
                Slot = SpellSlot.W,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.Heal },
                Delay = 250,
                Range = 525,
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Hecarim",
                SpellName = "HecarimRamp",
                Slot = SpellSlot.E,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.MovementSpeedAmplifier, SpellTags.CrowdControl },
                AppliedBuffsOnEnemies = new[] { BuffType.Knockback },
                Delay = 250,
                Range = 325,
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Hecarim",
                SpellName = "HecarimUlt",
                Slot = SpellSlot.R,
                SpellType = SpellType.SkillshotLine,
                CastType = new[] { CastType.Position },
                SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.MovementSpeedAmplifier, SpellTags.CrowdControl },
                AppliedBuffsOnEnemies = new[] { BuffType.Knockback },
                Delay = 250,
                Range = 1000,
                Radius = 400
            });
            #endregion Hecarim

            #region Heimerdinger

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Heimerdinger",
                SpellName = "HeimerdingerQ",
                Slot = SpellSlot.Q,
                SpellType = SpellType.SkillshotCircle,
                CastType = new[] { CastType.Position },
                SpellTags = new[] { SpellTags.Damage },
                Delay = 250,
                Range = 450,
                Radius = 450
            });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Heimerdinger",
                    SpellName = "HeimerdingerW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 450,
                    Radius = 70,
                    MissileSpeed = 1800,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "HeimerdingerWAttack2",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Heimerdinger",
                    SpellName = "HeimerdingerE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun, BuffType.Slow },
                    Delay = 250,
                    Range = 1100,
                    Radius = 100,
                    MissileSpeed = 1200,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "heimerdingerespell",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Heimerdinger",
                    SpellName = "HeimerdingerR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    Delay = 250,
                    Range = int.MaxValue
                });

            #endregion Heimerdinger

            #region Illaoi

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Illaoi",
                    SpellName = "IllaoiQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 750,
                    Range = 850,
                    Radius = 100,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "illaoiemis",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Illaoi",
                    SpellName = "IllaoiW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Dash, SpellTags.AttackRangeModifier, SpellTags.AttackRangeModifier, SpellTags.DamageAmplifier },
                    Delay = 250,
                    Range = 350
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Illaoi",
                    SpellName = "IllaoiE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl, SpellTags.LeavesMark },
                    ResetsAutoAttackTimer = true,
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 950,
                    Radius = 50,
                    MissileSpeed = 1900,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "illaoiemis",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Illaoi",
                    SpellName = "IllaoiR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 500,
                    Range = 0,
                    Radius = 450,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            #endregion Illaoi

            #region Irelia
            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Irelia",
                SpellName = "IreliaGatotsu",
                Slot = SpellSlot.Q,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                SpellTags = new[] { SpellTags.Dash, SpellTags.Damage },
                Delay = 150,
                Range = 650,
                Radius = 350

            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Irelia",
                SpellName = "IreliaHitenStyle",
                Slot = SpellSlot.W,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.Heal },
                Delay = 250,
                Range = int.MaxValue
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Irelia",
                SpellName = "IreliaEquilibriumStrike",
                Slot = SpellSlot.E,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                AppliedBuffsOnEnemies = new[] { BuffType.Slow, BuffType.Stun },
                Delay = 150,
                Range = 650,
                Radius = 350

            });

            Spells.Add(
            new SpellDatabaseEntry
            {
                ChampionName = "Irelia",
                SpellName = "IreliaTranscendentBlades",
                Slot = SpellSlot.R,
                SpellType = SpellType.SkillshotMissileLine,
                CastType = new[] { CastType.Position },
                SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                Delay = 0,
                Range = 1200,
                Radius = 65,
                MissileSpeed = 1600,
                DangerValue = 2,
                IsDangerous = false,
                MissileSpellName = "IreliaTranscendentBlades",
                CollisionObjects = new[] { CollisionableObjects.YasuoWall },
            });

            #endregion Irelia

            #region Janna

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Janna",
                    SpellName = "HowlingGale",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockup },
                    Delay = 250,
                    Range = 1700,
                    Radius = 120,
                    MissileSpeed = 900,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "HowlingGaleSpell",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Janna",
                SpellName = "SowTheWind",
                Slot = SpellSlot.W,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                Delay = 250,
                Range = 600
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Janna",
                SpellName = "EyeOfTheStorm",
                Slot = SpellSlot.E,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.AllyChampions, CastType.AllyTurrets, CastType.Self },
                SpellTags = new[] { SpellTags.Shield },
                Delay = 250,
                Range = 800
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Janna",
                SpellName = "ReapTheWhirlwind",
                Slot = SpellSlot.R,
                CastType = new[] { CastType.Channel },
                SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Heal },
                AppliedBuffsOnEnemies = new[] { BuffType.Knockback },
                Delay = 250,
                Range = 875,
                Radius = 725
            });

            #endregion Janna

            #region JarvanIV

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "JarvanIV",
                    SpellName = "JarvanIVDragonStrike",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 600,
                    Range = 770,
                    Radius = 70,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 3,
                    IsDangerous = false,
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "JarvanIV",
                    SpellName = "JarvanIVEQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 880,
                    Radius = 70,
                    MissileSpeed = 1450,
                    DangerValue = 3,
                    IsDangerous = true,
                });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "JarvanIV",
                SpellName = "JarvanIVGoldenAegis",
                Slot = SpellSlot.W,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.Shield },
                Delay = 250,
                Range = 300
            });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "JarvanIV",
                    SpellName = "JarvanIVDemacianStandard",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 500,
                    Range = 860,
                    Radius = 175,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "JarvanIVDemacianStandard",
                });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "JarvanIV",
                SpellName = "JarvanIVCataclysm",
                Slot = SpellSlot.R,
                SpellType = SpellType.SkillshotCircle,
                CastType = new[] { CastType.EnemyChampions },
                SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                Delay = 500,
                Range = 650,
                Radius = 325
            });

            #endregion JarvanIV

            #region Jayce

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Jayce",
                    SpellName = "JayceToTheSkies",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Dash, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 600,
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Jayce",
                    SpellName = "JayceShockBlast",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 1300,
                    Radius = 70,
                    MissileSpeed = 1450,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "JayceShockBlastMis",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Jayce",
                     SpellName = "JayceStaticField",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.Activated,
                     CastType = new[] { CastType.Activate },
                     SpellTags = new[] { SpellTags.Damage },
                     Delay = 250,
                     Range = 285,
                 });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Jayce",
                    SpellName = "JayceHyperCharge",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.AttackSpeedAmplifier }
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Jayce",
                     SpellName = "JayceThunderingBlow",
                     Slot = SpellSlot.E,
                     SpellType = SpellType.Targeted,
                     CastType = new[] { CastType.EnemyChampions },
                     SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                     AppliedBuffsOnEnemies = new[] { BuffType.Knockback },
                     Range = 240
                 });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Jayce",
                     SpellName = "JayceAccelerationGate",
                     Slot = SpellSlot.E,
                     SpellType = SpellType.SkillshotLine,
                     CastType = new[] { CastType.Position },
                     SpellTags = new[] { SpellTags.MovementSpeedAmplifier },
                     Range = 600,
                     Radius = 100,
                     Width = 900
                 });

            Spells.Add(
                  new SpellDatabaseEntry
                  {
                      ChampionName = "Jayce",
                      SpellName = "JayceStanceHtG",
                      Slot = SpellSlot.R,
                      SpellType = SpellType.Activated,
                      CastType = new[] { CastType.Activate },
                      SpellTags = new[] { SpellTags.Transformation },
                  });

            Spells.Add(
                  new SpellDatabaseEntry
                  {
                      ChampionName = "Jayce",
                      SpellName = "JayceStanceGtH",
                      Slot = SpellSlot.R,
                      SpellType = SpellType.Activated,
                      CastType = new[] { CastType.Activate },
                      SpellTags = new[] { SpellTags.Transformation },
                  });

            #endregion Jayce

            #region Jinx

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Jinx",
                    SpellName = "JinxQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Toggled,
                    CastType = new[] { CastType.Toggle },
                    SpellTags = new[] { SpellTags.AttackRangeModifier, SpellTags.AttackSpeedAmplifier },
                    Range = 150
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Jinx",
                    SpellName = "JinxW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 600,
                    Range = 1500,
                    Radius = 60,
                    MissileSpeed = 3300,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "JinxWMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Jinx",
                    SpellName = "JinxE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Snare },
                    Delay = 1500,
                    Range = 900,
                    Radius = 50,
                    Width = 450
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Jinx",
                    SpellName = "JinxR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 600,
                    Range = 20000,
                    Radius = 140,
                    MissileSpeed = 1700,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "JinxR",
                    CanBeRemoved = true,
                    CollisionObjects = new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall },
                });

            #endregion Jinx

            #region Kalista

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Kalista",
                    SpellName = "KalistaMysticShot",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    ResetsAutoAttackTimer = true,
                    Delay = 250,
                    Range = 1200,
                    Radius = 40,
                    MissileSpeed = 1700,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "kalistamysticshotmis",
                    ExtraMissileNames = new[] { "kalistamysticshotmistrue" },
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Kalista",
                    SpellName = "KalistaW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.GrantsVision },
                    Delay = 250,
                    Range = 5000,
                    Radius = 160
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Kalista",
                    SpellName = "KalistaExpungeWrapper",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 150,
                    Range = 1000
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Kalista",
                    SpellName = "KalistaRx",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    Delay = 450,
                    Range = 1400
                });



            #endregion Kalista

            #region Karma

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Karma",
                    SpellName = "KarmaQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 1050,
                    Radius = 60,
                    MissileSpeed = 1700,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KarmaQMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            //TODO: add the circle at the end.
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Karma",
                    SpellName = "KarmaQMantra",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 250,
                    Range = 950,
                    Radius = 80,
                    MissileSpeed = 1700,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KarmaQMissileMantra",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Karma",
                SpellName = "KarmaSpiritBind",
                Slot = SpellSlot.W,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions },
                SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                AppliedBuffsOnEnemies = new[] { BuffType.Snare },
                Delay = 2000,
                Range = 675
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Karma",
                SpellName = "KarmaWMantra",
                Slot = SpellSlot.W,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyChampions },
                SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl, SpellTags.Heal },
                AppliedBuffsOnEnemies = new[] { BuffType.Snare },
                Delay = 2000,
                Range = 675
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Karma",
                SpellName = "KarmaSolKimShield",
                Slot = SpellSlot.E,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.AllyChampions, CastType.Self },
                SpellTags = new[] { SpellTags.Shield },
                Delay = 400,
                Range = 800
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Karma",
                SpellName = "KarmaSolKimShieldLocket",
                Slot = SpellSlot.E,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.AllyChampions, CastType.Self },
                SpellTags = new[] { SpellTags.Shield, SpellTags.AoE },
                Delay = 400,
                Range = 800
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Karma",
                SpellName = "KarmaMantra",
                Slot = SpellSlot.R,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
            });

            #endregion Karma

            #region Karthus

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Karthus",
                    SpellName = "KarthusLayWasteA2",
                    ExtraSpellNames =
                        new[]
                        {
                            "KarthusLayWasteA1", "KarthusLayWasteA2", "KarthusLayWasteA3", "KarthusLayWasteDeadA1",
                            "KarthusLayWasteDeadA2", "KarthusLayWasteDeadA3"
                        },
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.AoE},
                    Delay = 625,
                    Range = 875,
                    Radius = 160,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Karthus",
                    SpellName = "KarthusWallOfPain",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl, SpellTags.AoE},
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                    Delay = 250,
                    Range = 1000,
                    Radius = 100,
                    Width = 800
                });
            
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Karthus",
                    SpellName = "KarthusDefile",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] {CastType.Toggle},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.AoE},
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                    Delay = 250,
                    Range = 425,
                    Radius = 425,
                });

            Spells.Add(new SpellDatabaseEntry 
            {
                ChampionName = "Karthus",
                SpellName = "KarthusFallenOne",
                ExtraSpellNames = new[] {"KarthusFallenOne2", "KarthusFallenOneExtra", "KarthusFallenOneExtra2"},
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Channel },
                SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                Delay = 3000,
                Range = int.MaxValue
            });

            #endregion Karthus

            #region Kassadin

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Kassadin",
                    SpellName = "NullLance",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyMinions, CastType.EnemyChampions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] {BuffType.Silence},
                    Delay = 250,
                    Range = 650,
                }); 
            
            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Kassadin",
                     SpellName = "NetherBlade",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.Activated,
                     CastType = new[] { CastType.Activate },
                     SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.AttackRangeModifier },
                     ResetsAutoAttackTimer = true,
                     Delay = 0,
                     Range = 200,
                 });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Kassadin",
                     SpellName = "ForcePulse",
                     Slot = SpellSlot.E,
                     SpellType = SpellType.SkillshotCone,
                     CastType = new[] { CastType.Position },
                     SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                     AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                     Delay = 400,
                     Range = 700,
                     Radius = 350,
                     Angle = 80
                 });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Kassadin",
                    SpellName = "RiftWalk",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.Dash},
                    Delay = 250,
                    Range = 450,
                    Radius = 270,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "RiftWalk",
                });

            #endregion Kassadin

            #region Katarina

            Spells.Add(new SpellDatabaseEntry("Katarina", "KatarinaQ", SpellSlot.Q, SpellType.Targeted, new[] { CastType.EnemyChampions, CastType.EnemyMinions }, new[] { SpellTags.Damage, SpellTags.LeavesMark }, false, 675));
            Spells.Add(new SpellDatabaseEntry("Katarina", "KatarinaW", SpellSlot.W, SpellType.SkillshotCircle, new[] { CastType.Activate }, new[] { SpellTags.Damage, SpellTags.CanDetonateMark }, false, 375));
            Spells.Add(new SpellDatabaseEntry("Katarina", "KatarinaE", SpellSlot.E, SpellType.Targeted, new[] { CastType.EnemyChampions, CastType.EnemyMinions, CastType.AllyChampions, CastType.AllyMinions }, new[] { SpellTags.CanDetonateMark, SpellTags.Dash }, true, 700));
            Spells.Add(new SpellDatabaseEntry("Katarina", "KatarinaR", SpellSlot.R, SpellType.SkillshotCircle, new[] { CastType.Channel }, new[] { SpellTags.Damage, SpellTags.CanDetonateMark }, false, 550, 250, 550));

            #endregion Katarina

            #region Kennen

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Kennen",
                    SpellName = "KennenShurikenHurlMissile1",
                    ExtraSpellNames = new[] {"KennenShurikenHurl1"},
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.LeavesMark},
                    Delay = 125,
                    Range = 1050,
                    Radius = 50,
                    MissileSpeed = 1700,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KennenShurikenHurlMissile1",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Kennen",
                    SpellName = "KennenBringTheLight",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CanDetonateMark },
                    Range = 800,
                    Radius = 750
                });

            Spells.Add(
            new SpellDatabaseEntry
            {
                ChampionName = "Kennen",
                SpellName = "KennenLightningRush",
                Slot = SpellSlot.E,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.MovementSpeedAmplifier, SpellTags.DefensiveBuff, SpellTags.LeavesMark },
                Range = int.MaxValue
            });

            Spells.Add(new SpellDatabaseEntry
                {
                    ChampionName = "Kennen",
                    SpellName = "KennenShurikenStorm",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE, SpellTags.LeavesMark },
                    Range = 550,
                    Radius = 500
                });
            #endregion Kennen

            #region Khazix
            Spells.Add(new SpellDatabaseEntry("Khazix", "KhazixQ", SpellSlot.Q, SpellType.Targeted, new[] { CastType.EnemyChampions, CastType.EnemyMinions }, new[] { SpellTags.DamageAmplifier, SpellTags.Damage }, false, 325));

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Khazix",
                    SpellName = "KhazixW",
                    ExtraSpellNames = new[] { "khazixwlong" },
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                    Delay = 250,
                    Range = 1025,
                    Radius = 73,
                    MissileSpeed = 1700,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KhazixWMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Khazix",
                    SpellName = "KhazixE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.Dash},
                    Delay = 250,
                    Range = 600,
                    Radius = 300,
                    MissileSpeed = 1500,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KhazixE",
                });

            Spells.Add(
                new SpellDatabaseEntry 
                {
                    ChampionName = "Khazix",
                    SpellName = "KhazixR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.MovementSpeedAmplifier },
                    AppliedBuffsOnSelf = new[] {BuffType.Invisibility}
                });
            #endregion Khazix

            #region Kindred
            Spells.Add(new SpellDatabaseEntry("Kindred", "KindredQ", SpellSlot.Q, SpellType.SkillshotLine, new[] { CastType.Position }, new[] { SpellTags.Dash, SpellTags.Damage }, false, 340, 250, 500));
            Spells.Add(new SpellDatabaseEntry("Kindred", "KindredW", SpellSlot.W, SpellType.SkillshotCircle, new[] { CastType.Activate }, new[] { SpellTags.Damage }, false, 800, 250, 800));
            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Kindred",
                SpellName = "KindredEWrapper",
                Slot = SpellSlot.E,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.EnemyMinions, CastType.EnemyChampions },
                SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                Range = 500
            });
            Spells.Add(new SpellDatabaseEntry 
            {
                ChampionName = "Kindred",
                SpellName = "KindredR",
                Slot = SpellSlot.R,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.Self, CastType.EnemyChampions },
                SpellTags = new[] { SpellTags.Stasis, SpellTags.Heal },
                Range = 400
            });
            #endregion Kindred

            #region Kogmaw

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Kogmaw",
                    SpellName = "KogMawQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.DamageAmplifier},
                    Delay = 250,
                    Range = 1200,
                    Radius = 70,
                    MissileSpeed = 1650,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KogMawQ",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(new SpellDatabaseEntry("Kogmaw", "KogMawBioArcaneBarrage", SpellSlot.W, SpellType.Activated, new[] { CastType.Activate }, new[] { SpellTags.AttackRangeModifier, SpellTags.AttackSpeedAmplifier }));

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Kogmaw",
                    SpellName = "KogMawVoidOoze",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                    Delay = 250,
                    Range = 1360,
                    Radius = 120,
                    MissileSpeed = 1400,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KogMawVoidOozeMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Kogmaw",
                    SpellName = "KogMawLivingArtillery",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 1200,
                    Range = 1800,
                    Radius = 225,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "KogMawLivingArtillery",
                });

            #endregion Kogmaw

            #region Leblanc

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancChaosOrb",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.LeavesMark, SpellTags.CanDetonateMark },
                    Range = 700
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancChaosOrbM",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CanDetonateMark },
                    Range = 700
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancSlide",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.CanDetonateMark, SpellTags.Dash, SpellTags.Damage},
                    Delay = 0,
                    Range = 600,
                    Radius = 220,
                    MissileSpeed = 1450,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LeblancSlide",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancSlideM",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.CanDetonateMark, SpellTags.Dash, SpellTags.Damage },
                    Delay = 0,
                    Range = 600,
                    Radius = 220,
                    MissileSpeed = 1450,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LeblancSlideM",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancSoulShackle",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.CanDetonateMark, SpellTags.CrowdControl, SpellTags.Damage },
                    AppliedBuffsOnEnemies = new[] {BuffType.Snare},
                    Delay = 250,
                    Range = 950,
                    Radius = 70,
                    MissileSpeed = 1600,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "LeblancSoulShackle",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Leblanc",
                    SpellName = "LeblancSoulShackleM",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.CanDetonateMark, SpellTags.CrowdControl, SpellTags.Damage },
                    AppliedBuffsOnEnemies = new[] { BuffType.Snare },
                    Delay = 250,
                    Range = 950,
                    Radius = 70,
                    MissileSpeed = 1600,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "LeblancSoulShackleM",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            #endregion Leblanc

            #region LeeSin

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "LeeSin",
                    SpellName = "BlindMonkQOne",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 65,
                    MissileSpeed = 1800,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "BlindMonkQOne",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                }); 
            Spells.Add(new SpellDatabaseEntry("LeeSin", "BlindMonkQTwo", SpellSlot.Q, SpellType.Activated, new[] { CastType.Activate }, new[] { SpellTags.Dash }));

            Spells.Add(new SpellDatabaseEntry("LeeSin", "BlindMonkWOne", SpellSlot.W, SpellType.Targeted, new[] { CastType.AllyMinions, CastType.AllyChampions, CastType.Self }, new[] { SpellTags.Dash, SpellTags.DefensiveBuff, SpellTags.Shield }, false, 700));
            Spells.Add(new SpellDatabaseEntry("LeeSin", "BlindMonkWTwo", SpellSlot.W, SpellType.Activated, new[] { CastType.Activate }, new[] { SpellTags.DefensiveBuff }));

            Spells.Add(new SpellDatabaseEntry("LeeSin", "BlindMonkEOne", SpellSlot.E, SpellType.SkillshotCircle, new[] { CastType.Activate }, new[] { SpellTags.Damage, SpellTags.LeavesMark}, false, 350, 250, 125));
            Spells.Add(new SpellDatabaseEntry{
                ChampionName = "LeeSin", 
                SpellName = "BlindMonkETwo", 
                Slot = SpellSlot.E,
                SpellType = SpellType.Activated,
                CastType = new[] {CastType.Activate},
                SpellTags = new[] {SpellTags.CrowdControl},
                Range = 500});

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "LeeSin", 
                SpellName = "BlindMonkRKick", 
                Slot = SpellSlot.R, 
                SpellType = SpellType.SkillshotLine, 
                CastType = new[] {CastType.EnemyChampions}, 
                SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                AppliedBuffsOnEnemies = new[] {BuffType.Knockup},
                Range = 375,
                Width = 1200});

            #endregion LeeSin

            #region Leona
            Spells.Add(
            new SpellDatabaseEntry
            {
                ChampionName = "Leona",
                SpellName = "LeonaShieldOfDaybreak",
                Slot = SpellSlot.Q,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.CrowdControl, SpellTags.DamageAmplifier, SpellTags.AppliesOnHitEffects },
                AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                ResetsAutoAttackTimer = true
            });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Leona",
                    SpellName = "LeonaSolarBarrier",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DefensiveBuff }
                });
           
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Leona",
                    SpellName = "LeonaZenithBlade",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Dash, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Snare},
                    Delay = 250,
                    Range = 905,
                    Radius = 70,
                    MissileSpeed = 2000,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "LeonaZenithBladeMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Leona",
                    SpellName = "LeonaSolarFlare",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun, BuffType.Slow },
                    Delay = 1000,
                    Range = 1200,
                    Radius = 300,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "LeonaSolarFlare",
                });

            #endregion Leona

            #region Lissandra

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lissandra",
                    SpellName = "LissandraQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 700,
                    Radius = 75,
                    MissileSpeed = 2200,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LissandraQMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lissandra",
                    SpellName = "LissandraQShards",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 700,
                    Radius = 90,
                    MissileSpeed = 2200,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "lissandraqshards",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lissandra",
                    SpellName = "LissandraW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Snare},
                    Range = 450,
                    Radius = 225
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lissandra",
                    SpellName = "LissandraE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1025,
                    Radius = 125,
                    MissileSpeed = 850,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LissandraEMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });


            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lissandra",
                    SpellName = "LissandraR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.Self, CastType.EnemyChampions },
                    SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Damage, SpellTags.Stasis, SpellTags.Heal },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                    AppliedBuffsOnSelf = new[] { BuffType.Invulnerability },
                    Range = 550
                });

            #endregion Lissandra

            #region Lucian

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] {CastType.EnemyChampions, CastType.EnemyMinions},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.AoE},
                    Delay = 500,
                    Range = 1300,
                    Radius = 65,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LucianQ",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Delay = 250,
                    Range = 1000,
                    Radius = 55,
                    MissileSpeed = 1600,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "lucianwmissile",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Range = 425,
                    Width = 425
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Direction},
                    SpellTags = new[] {SpellTags.Damage},
                    Delay = 500,
                    Range = 1400,
                    Radius = 110,
                    MissileSpeed = 2800,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "lucianrmissileoffhand",
                    ExtraMissileNames = new[] { "lucianrmissile" },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lucian",
                    SpellName = "LucianRMis",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Direction },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 500,
                    Range = 1400,
                    Radius = 110,
                    MissileSpeed = 2800,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "lucianrmissileoffhand",
                    ExtraMissileNames = new[] { "lucianrmissile" },
                });

            #endregion Lucian

            #region Lulu

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lulu",
                    SpellName = "LuluQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1450,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LuluQMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lulu",
                    SpellName = "LuluQPix",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1450,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LuluQMissileTwo",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(new SpellDatabaseEntry
                {
                    ChampionName = "Lulu",
                    SpellName = "LuluW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.AllyChampions, CastType.Self },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl, SpellTags.MovementSpeedAmplifier },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow, BuffType.Polymorph },
                    Range = 650
                });

            Spells.Add(new SpellDatabaseEntry
                {
                    ChampionName = "Lulu",
                    SpellName = "LuluE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.AllyChampions, CastType.Self },
                    SpellTags = new[] { SpellTags.Shield },
                    Range = 650
                });

            Spells.Add(new SpellDatabaseEntry
                {
                    ChampionName = "Lulu",
                    SpellName = "LuluR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.AllyChampions, CastType.Self },
                    SpellTags = new[] { SpellTags.Heal, SpellTags.DefensiveBuff, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockup },
                    Range = 900,
                    Radius = 150
                });

            #endregion Lulu

            #region Lux

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lux",
                    SpellName = "LuxLightBinding",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] {BuffType.Snare},
                    Delay = 250,
                    Range = 1300,
                    Radius = 70,
                    MissileSpeed = 1200,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "LuxLightBindingMis",
                    CollisionObjects = new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall, },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lux",
                    SpellName = "LuxPrismaticWave",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Shield },
                    Range = 1075,
                    Width = 900,
                    Radius = 150
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lux",
                    SpellName = "LuxLightStrikeKugel",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                    Delay = 250,
                    Range = 1100,
                    Radius = 275,
                    MissileSpeed = 1300,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "LuxLightStrikeKugel",
                    ToggleParticleName = "Lux_.+_E_tar_aoe_",
                    CanBeRemoved = true,
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Lux",
                    SpellName = "LuxMaliceCannon",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 1000,
                    Range = 3500,
                    Radius = 190,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "LuxMaliceCannon",
                });

            #endregion Lux

            #region Malphite

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Malphite",
                    SpellName = "SeismicShard",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.MovementSpeedAmplifier, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Range = 625
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Malphite",
                    SpellName = "Obduracy",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier },
                    Range = 225
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Malphite",
                    SpellName = "Landslide",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage },
                    Range = 400,
                    Radius = 200
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Malphite",
                    SpellName = "UFSlash",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 0,
                    Range = 1000,
                    Radius = 270,
                    MissileSpeed = 1500,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "UFSlash",
                });

            #endregion Malphite

            #region Malzahar

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Malzahar",
                    SpellName = "AlZaharCalloftheVoid",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.AoE, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Silence},
                    Delay = 1000,
                    Range = 900,
                    Radius = 85,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "AlZaharCalloftheVoid",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Malzahar",
                    SpellName = "AlzaharNullZone",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Range = 800,
                    Radius = 250
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Malzahar",
                    SpellName = "AlzaharMaleficVisions",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags=  new[] {SpellTags.Damage},
                    Range = 650
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Malzahar",
                    SpellName = "AlzaharNetherGrasp",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Suppression },
                    Range = 700
                });

            #endregion Malzahar

            #region Maokai
            Spells.Add(new SpellDatabaseEntry
                {
                    ChampionName = "Maokai",
                    SpellName = "MaokaiTrunkLine",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockback, BuffType.Slow },
                    Range = 600,
                    Radius = 100,
                    Width = 110
                });

            Spells.Add(new SpellDatabaseEntry
                {
                    ChampionName = "Maokai",
                    SpellName = "MaokaiUnstableGrowth",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Dash, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Snare },
                    Delay = 450,
                    Range = 525
                });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Maokai",
                SpellName = "MaokaiSapling2",
                Slot = SpellSlot.E,
                SpellType = SpellType.SkillshotCircle,
                CastType = new[] { CastType.Position },
                SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Damage },
                AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                Range = 1100,
                Radius = 250
            });

            Spells.Add(new SpellDatabaseEntry
                {
                    ChampionName = "Maokai",
                    SpellName = "MaokaiDrain3",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DefensiveBuff },
                    Range = 475
                });
            #endregion Maokai

            #region MasterYi
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "MasterYi",
                    SpellName = "AlphaStrike",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Stasis },
                    Range = 600
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "MasterYi",
                    SpellName = "Meditate",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DefensiveBuff, SpellTags.Heal }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "MasterYi",
                    SpellName = "WujuStyle",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.AttackSpeedAmplifier, SpellTags.DamageAmplifier }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "MasterYi",
                    SpellName = "Highlander",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.MovementSpeedAmplifier }
                });
            #endregion MasterYi

            #region MissFortune
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "MissFortune",
                    SpellName = "MissFortuneRichochetShot",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCone,
                    CastType = new[] { CastType.EnemyMinions, CastType.EnemyChampions },
                    SpellTags = new[] { SpellTags.Damage },
                    Range = 650,
                    Radius = 250,
                    Angle = 60
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "MissFortune",
                    SpellName = "MissFortuneViciousStrikes",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.AttackSpeedAmplifier, SpellTags.MovementSpeedAmplifier}
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "MissFortune",
                    SpellName = "MissFortuneScattershot",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Delay = 300,
                    Range = 1000,
                    Radius = 200
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "MissFortune",
                    SpellName = "MissFortuneBulletTime",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCone,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Range = 1400,
                    Width = 1400,
                    Angle = 17
                });
            #endregion MissFortune

            #region Wukong
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "MonkeyKing",
                    SpellName = "MonkeyKingDoubleAttack",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Activated,
                    CastType = new[] {CastType.Activate},
                    SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.AttackSpeedAmplifier }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "MonkeyKing",
                    SpellName = "MonkeyKingDecoy",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage },
                    AppliedBuffsOnSelf = new[] { BuffType.Invisibility }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "MonkeyKing",
                    SpellName = "MonkeyKingNimbus",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Dash },
                    Range = 650
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "MonkeyKing",
                    SpellName = "MonkeyKingSpinToWin",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockup },
                    Range = 660,
                    Radius = 450
                });
            #endregion Wukong

            #region Morgana

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Morgana",
                    SpellName = "DarkBindingMissile",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    Delay = 250,
                    Range = 1300,
                    Radius = 80,
                    MissileSpeed = 1200,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "DarkBindingMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Morgana",
                    SpellName = "TormentedSoil",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.AoE },
                    Range = 900,
                    Radius = 175
                });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Morgana",
                SpellName = "BlackShield",
                Slot = SpellSlot.E,
                SpellType = SpellType.Targeted,
                CastType = new[] { CastType.Self, CastType.AllyChampions },
                SpellTags = new[] { SpellTags.SpellShield },
                Range = 750
            });

            Spells.Add(new SpellDatabaseEntry
            {
                ChampionName = "Morgana",
                SpellName = "SoulShackles",
                Slot = SpellSlot.R,
                SpellType = SpellType.Activated,
                CastType = new[] { CastType.Activate },
                SpellTags = new[] { SpellTags.Damage, SpellTags.AoE, SpellTags.CrowdControl },
                Delay = 500,
                Range = 600
            });

            #endregion Morgana

            #region Nami

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nami",
                    SpellName = "NamiQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage,SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Stun},
                    Delay = 950,
                    Range = 1625,
                    Radius = 150,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "namiqmissile",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nami",
                    SpellName = "NamiW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.Self, CastType.AllyChampions, CastType.EnemyChampions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Heal },
                    Range = 725
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nami",
                    SpellName = "NamiE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.Self, CastType.AllyChampions },
                    SpellTags = new[] { SpellTags.DamageAmplifier },
                    Range = 800
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nami",
                    SpellName = "NamiR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Stun},
                    Delay = 500,
                    Range = 2750,
                    Radius = 260,
                    MissileSpeed = 850,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "NamiRMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            #endregion Nami

            #region Nasus
            Spells.Add(new SpellDatabaseEntry
                {
                    ChampionName = "Nasus",
                    SpellName = "NasusQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.AttackRangeModifier },
                    Range = 150
                });

            Spells.Add(new SpellDatabaseEntry
                {
                    ChampionName = "Nasus",
                    SpellName = "NasusW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions },
                    SpellTags = new[] { SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow },
                    Range = 600
                });

            Spells.Add(new SpellDatabaseEntry
                {
                    ChampionName = "Nasus",
                    SpellName = "NasusE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage},
                    Range = 650,
                    Radius = 400
                });

            Spells.Add(new SpellDatabaseEntry
                {
                    ChampionName = "Nasus",
                    SpellName = "NasusR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DefensiveBuff }
                });
            #endregion Nasus

            #region Nautilus

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nautilus",
                    SpellName = "NautilusAnchorDrag",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl, SpellTags.Dash},
                    AppliedBuffsOnEnemies = new[] {BuffType.Stun},
                    Delay = 250,
                    Range = 1250,
                    Radius = 90,
                    MissileSpeed = 2000,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "NautilusAnchorDragMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall, CollisionableObjects.Walls },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nautilus",
                    SpellName = "NautilusPiercingGaze",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.Shield },
                    ResetsAutoAttackTimer = true
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nautilus",
                    SpellName = "NautilusSplashZone",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    Range = 600,
                    Radius = 450
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nautilus",
                    SpellName = "NautilusGrandLine",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Targeted,
                    CastType = new[] {CastType.EnemyChampions},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Knockup},
                    Range = 825
                });

            #endregion Nautilus

            #region Nocturne

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nocturne",
                    SpellName = "NocturneDuskbringer",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.MovementSpeedAmplifier},
                    Delay = 250,
                    Range = 1125,
                    Radius = 60,
                    MissileSpeed = 1400,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "NocturneDuskbringer",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nocturne",
                    SpellName = "NocturneShroudofDarkness",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.SpellShield }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nocturne",
                    SpellName = "NocturneUnspeakableHorror",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Damage },
                    AppliedBuffsOnEnemies = new[] { BuffType.Taunt },
                    Range = 425,
                    Radius = 300
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nocturne",
                    SpellName = "NocturneParanoia",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.Charging, CastType.EnemyChampions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Dash },
                    Range = 2500
                });

            #endregion Nocturne

            #region Nidalee

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nidalee",
                    SpellName = "JavelinToss",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.LeavesMark},
                    Delay = 250,
                    Range = 1500,
                    Radius = 40,
                    MissileSpeed = 1300,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "JavelinToss",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nidalee",
                    SpellName = "Takedown",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nidalee",
                    SpellName = "Bushwhack",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.LeavesMark },
                    Delay = 1000,
                    Range = 900,
                    Radius = 100
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nidalee",
                    SpellName = "Pounce",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Dash, SpellTags.CanDetonateMark },
                    Range = 375,
                    Radius = 75,
                    Width = 375
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nidalee",
                    SpellName = "PrimalSurge",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.Self, CastType.AllyChampions },
                    SpellTags = new[] { SpellTags.Heal },
                    Range = 600
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nidalee",
                    SpellName = "Swipe",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCone,
                    CastType = new[] { CastType.Direction },
                    SpellTags = new[] { SpellTags.Damage },
                    Range = 300,
                    Width = 250,
                    Angle = 180
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Nidalee",
                    SpellName = "AspectOfTheCougar",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Toggled,
                    CastType = new[] { CastType.Toggle },
                    SpellTags = new[] { SpellTags.Transformation }
                });

            #endregion Nidalee

            #region Olaf

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Olaf",
                    SpellName = "OlafAxeThrowCast",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                    Delay = 250,
                    Range = 1000,
                    Radius = 105,
                    MissileSpeed = 1600,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "olafaxethrow",
                    CanBeRemoved = true,
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Olaf",
                    SpellName = "OlafFrenziedStrikes",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Heal, SpellTags.DefensiveBuff }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Olaf",
                    SpellName = "OlafRecklessStrike",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.Heal },
                    Range = 325
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Olaf",
                    SpellName = "OlafRagnarok",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.SpellShield, SpellTags.DefensiveBuff }
                });

            #endregion Olaf

            #region Orianna

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Orianna",
                    SpellName = "OriannasQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage},
                    Delay = 0,
                    Range = 1500,
                    Radius = 80,
                    MissileSpeed = 1200,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "orianaizuna",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Orianna",
                    SpellName = "OrianaDissonanceCommand-",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] {CastType.Activate},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl, SpellTags.MovementSpeedAmplifier},
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                    Delay = 250,
                    Range = 0,
                    Radius = 255,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "OrianaDissonanceCommand-",
                    FromObject = "yomu_ring_",
                    SourceObjectName = "w_dissonance_ball" //Orianna_Base_W_Dissonance_ball_green.troy & Orianna_Base_W_Dissonance_cas_green.troy
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Orianna",
                    SpellName = "OriannasE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.AllyChampions, CastType.Self},
                    SpellTags = new[] {SpellTags.Shield, SpellTags.Damage, SpellTags.DefensiveBuff},
                    Delay = 0,
                    Range = 1500,
                    Radius = 85,
                    MissileSpeed = 1850,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "orianaredact",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Orianna",
                    SpellName = "OrianaDetonateCommand-",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] {CastType.Activate},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Flee},
                    Delay = 700,
                    Range = 0,
                    Radius = 410,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "OrianaDetonateCommand-",
                    FromObject = "yomu_ring_",
                    SourceObjectName = "r_vacuumindicator", //Orianna_Base_R_VacuumIndicator.troy
                });

            #endregion Orianna

            #region Quinn

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Quinn",
                    SpellName = "QuinnQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.NearSight},
                    Delay = 313,
                    Range = 1050,
                    Radius = 60,
                    MissileSpeed = 1550,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "QuinnQ",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Quinn",
                    SpellName = "QuinnW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.GrantsVision },
                    Range = 2100,
                    Radius = 2100
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Quinn",
                    SpellName = "QuinnE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] {SpellTags.DamageAmplifier},
                    Range = 675
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Quinn",
                    SpellName = "QuinnR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Transformation }
                });

            #endregion Quinn

            #region Pantheon

            Spells.Add(new SpellDatabaseEntry("Pantheon", "PantheonQ", SpellSlot.Q, SpellType.Targeted, new[] { CastType.EnemyMinions, CastType.EnemyChampions }, new[] { SpellTags.Damage }, false, 600));
            Spells.Add(new SpellDatabaseEntry("Pantheon", "PantheonW", SpellSlot.W, SpellType.Targeted, new[] { CastType.EnemyChampions, CastType.EnemyMinions }, new[] { SpellTags.Dash, SpellTags.CrowdControl, SpellTags.DefensiveBuff}, false, 600 ));
            Spells.Add(new SpellDatabaseEntry("Pantheon", "PantheonE", SpellSlot.E, SpellType.SkillshotCone, new[] { CastType.Channel }, new[] { SpellTags.Damage }, false, 600, 250, 210, 500, 1400, 35));
            Spells.Add(new SpellDatabaseEntry("Pantheon", "PantheonRJump", SpellSlot.R, SpellType.SkillshotCircle, new[] { CastType.Position }, new[] { SpellTags.Teleport, SpellTags.Damage }, false, 5500, 2000, 700));

            #endregion Pantheon

            #region Poppy

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Poppy",
                    SpellName = "PoppyQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] {CastType.Direction},
                    SpellTags = new[] {SpellTags.Damage},
                    Delay = 500,
                    Range = 430,
                    Radius = 100,
                    MissileSpeed = int.MaxValue,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "PoppyQ",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Poppy",
                    SpellName = "PoppyW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DefensiveBuff },
                    Range = 400,
                    Radius = 400
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Poppy",
                    SpellName = "PoppyE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Dash, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                    Range = 375
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Poppy",
                    SpellName = "PoppyRSpell",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 300,
                    Range = 1200,
                    Radius = 100,
                    MissileSpeed = 1600,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "PoppyRMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            #endregion Poppy

            #region Rengar
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Rengar",
                    SpellName = "RengarQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier },
                    ResetsAutoAttackTimer = true
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Rengar",
                    SpellName = "RengarW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Heal, SpellTags.DefensiveBuff, SpellTags.Damage },
                    Range = 500,
                    Radius = 500
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Rengar",
                    SpellName = "RengarE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                    Delay = 250,
                    Range = 1000,
                    Radius = 70,
                    MissileSpeed = 1500,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "RengarEFinal",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Rengar",
                    SpellName = "RengarR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier },
                    AppliedBuffsOnSelf = new[] { BuffType.Invisibility }
                });

            #endregion Rengar

            #region RekSai

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "RekSai",
                    SpellName = "RekSaiQBurrowed",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage},
                    Delay = 500,
                    Range = 1625,
                    Radius = 60,
                    MissileSpeed = 1950,
                    DangerValue = 3,
                    IsDangerous = false,
                    MissileSpellName = "RekSaiQBurrowedMis",
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "RekSai",
                    SpellName = "RekSaiQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier },
                    Range = 325,
                    ResetsAutoAttackTimer = true
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "RekSai",
                    SpellName = "RekSaiW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Transformation },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "RekSai",
                    SpellName = "RekSaiWBurrowed",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Transformation, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] {BuffType.Knockup},
                    Radius = 200
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "RekSai",
                    SpellName = "RekSaiE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage },
                    Range = 250
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "RekSai",
                    SpellName = "RekSaiEBurrowed",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] {CastType.Position},
                    Range = 750,
                    Width = 750
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "RekSai",
                    SpellName = "RekSaiR",
                    ExtraSpellNames = new[] {"RekSaiR2"},
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.HeroPets },
                    SpellTags = new[] { SpellTags.Teleport }
                });

            #endregion RekSai

            #region Riven
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Riven",
                    SpellName = "RivenTriCleave",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Dash, SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Knockup },
                    Range = 260,
                    Radius = 150
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Riven",
                    SpellName = "RivenMartyr",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.CrowdControl, SpellTags.Damage },
                    AppliedBuffsOnEnemies = new[] { BuffType.Stun },
                    Range = 125,
                    Radius = 125
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Riven",
                    SpellName = "RivenFeint",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Dash, SpellTags.Shield },
                    Range = 325,
                    Radius = 80,
                    Width = 325
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Riven",
                    SpellName = "RivenFengShuiEngine",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Channel, CastType.Position},
                    Delay = 250,
                    Range = 1100,
                    Radius = 125,
                    MissileSpeed = 1600,
                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "RivenLightsaberMissile",
                    ExtraMissileNames = new[] { "RivenLightsaberMissileSide" }
                });

            #endregion Riven

            #region Rumble
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Rumble",
                    SpellName = "RumbleFlameThrower",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCone,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage },
                    Range = 600,
                    Radius = 600,
                    Angle = 32
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Rumble",
                    SpellName = "RumbleShield",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Shield }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Rumble",
                    SpellName = "RumbleGrenade",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 2000,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "RumbleGrenade",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Rumble",
                    SpellName = "RumbleCarpetBomb",
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 400,
                    MissileDelayed = true,
                    Range = 1200,
                    Radius = 200,
                    MissileSpeed = 1600,
                    DangerValue = 4,
                    IsDangerous = false,
                    MissileSpellName = "RumbleCarpetBombMissile",
                    CanBeRemoved = false,
                    CollisionObjects = new CollisionableObjects[] { },
                });

            #endregion Rumble

            #region Ryze

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ryze",
                    SpellName = "RyzeQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage},
                    Delay = 250,
                    Range = 900,
                    Radius = 50,
                    MissileSpeed = 1700,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "RyzeQ",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ryze",
                    SpellName = "RyzeW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Taunt },
                    Range = 600
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ryze",
                    SpellName = "RyzeE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage },
                    Range = 600
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ryze",
                    SpellName = "RyzeR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.MovementSpeedAmplifier, SpellTags.DefensiveBuff, SpellTags.Transformation }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ryze",
                    SpellName = "RyzeRQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] { CastType.Position },
                    SpellTags = new[] { SpellTags.Damage },
                    Delay = 250,
                    Range = 900,
                    Radius = 50,
                    MissileSpeed = 1700,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ryzerq",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Ryze",
                     SpellName = "RyzeRW",
                     Slot = SpellSlot.W,
                     SpellType = SpellType.Targeted,
                     CastType = new[] { CastType.EnemyChampions },
                     SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                     AppliedBuffsOnEnemies = new[] { BuffType.Taunt },
                     Range = 600
                 });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ryze",
                    SpellName = "RyzeRE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage },
                    Range = 600
                });
            #endregion

            #region Sejuani

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sejuani",
                    SpellName = "SejuaniArcticAssault",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Knockup},
                    Delay = 0,
                    Range = 900,
                    Radius = 70,
                    MissileSpeed = 1600,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.Minions, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sejuani",
                    SpellName = "SejuaniNorthernWinds",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier },
                    ResetsAutoAttackTimer = true
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sejuani",
                    SpellName = "SejuaniWintersClaw",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] { BuffType.Slow }
                });
            //TODO: fix?
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sejuani",
                    SpellName = "SejuaniGlacialPrisonStart",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage,SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Stun},
                    Delay = 250,
                    Range = 1100,
                    Radius = 110,
                    MissileSpeed = 1600,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "sejuaniglacialprison",
                    CanBeRemoved = true,
                    CollisionObjects = new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall },
                });

            #endregion Sejuani

            #region Sion
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sion",
                    SpellName = "SionQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCone,
                    CastType = new[] { CastType.Direction },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.CrowdControl },
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow, BuffType.Knockup},
                    Delay = 2000,
                    Range = 300,
                    Radius = 45
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sion",
                    SpellName = "SionW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Shield, SpellTags.Damage },
                    Range = 550,
                    Radius = 400
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sion",
                    SpellName = "SionE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Slow},
                    Delay = 250,
                    Range = 800,
                    Radius = 80,
                    MissileSpeed = 1800,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "SionEMissile",
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sion",
                    SpellName = "SionR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Direction},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Stun},
                    Delay = 500,
                    Range = 800,
                    Radius = 120,
                    MissileSpeed = 1000,
                    DangerValue = 3,
                    IsDangerous = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Heroes },
                });

            #endregion Sion

            #region Soraka

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Soraka",
                    SpellName = "SorakaQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage},
                    Delay = 500,
                    Range = 800,
                    Radius = 300,
                    MissileSpeed = 1750,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Soraka",
                    SpellName = "SorakaW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.AllyChampions },
                    SpellTags = new[] { SpellTags.Heal },
                    Range = 550
                });
            
            Spells.Add(
                 new SpellDatabaseEntry
                 {
                     ChampionName = "Soraka",
                     SpellName = "SorakaE",
                     Slot = SpellSlot.E,
                     SpellType = SpellType.SkillshotCircle,
                     CastType = new[] { CastType.Position },
                     SpellTags = new[] { SpellTags.CrowdControl },
                     AppliedBuffsOnEnemies = new[] {BuffType.Silence, BuffType.Taunt},
                     Delay = 500,
                     Range = 925,
                     Radius = 300,
                     MissileSpeed = 1750,
                     DangerValue = 2,
                     IsDangerous = false,
                     MissileSpellName = "",
                     CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                 });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Soraka",
                    SpellName = "SorakaR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Heal }
                });

            #endregion Soraka

            #region Shen

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Shen",
                    SpellName = "ShenVorpalStar",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.EnemyChampions, CastType.EnemyMinions },
                    SpellTags = new[] { SpellTags.Damage, SpellTags.LeavesMark },
                    Range = 475
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Shen",
                    SpellName = "ShenFeint",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Shield }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Shen",
                    SpellName = "ShenShadowDash",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.CrowdControl, SpellTags.CanDetonateMark},
                    AppliedBuffsOnEnemies = new[] {BuffType.Taunt},
                    Delay = 0,
                    Range = 650,
                    Radius = 50,
                    MissileSpeed = 1600,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ShenShadowDash",
                    CollisionObjects =
                        new[] { CollisionableObjects.Minions, CollisionableObjects.Heroes, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Shen",
                    SpellName = "ShenStandUnited",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Targeted,
                    CastType = new[] { CastType.AllyChampions },
                    SpellTags = new[] { SpellTags.Shield }
                });

            #endregion Shen

            #region Shyvana
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Shyvana",
                    SpellName = "ShyvanaDoubleAttack",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Shyvana",
                    SpellName = "ShyvanaImmolationAura",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.Damage },
                    Range = 160,
                    Radius = 160
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Shyvana",
                    SpellName = "ShyvanaFireball",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage},
                    Delay = 250,
                    Range = 950,
                    Radius = 60,
                    MissileSpeed = 1700,
                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ShyvanaFireballMissile",
                    CollisionObjects =
                        new[] { CollisionableObjects.Minions, CollisionableObjects.Heroes, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Shyvana",
                    SpellName = "ShyvanaTransformCast",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.Damage, SpellTags.DefensiveBuff, SpellTags.CrowdControl},
                    AppliedBuffsOnEnemies = new[] {BuffType.Flee},
                    Delay = 250,
                    Range = 1000,
                    Radius = 150,
                    MissileSpeed = 1500,
                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ShyvanaTransformCast",
                });

            #endregion Shyvana

            #region Sivir
            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sivir",
                    SpellName = "SivirQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    CastType = new[] {CastType.Position},
                    SpellTags = new[] {SpellTags.AoE, SpellTags.Damage},
                    Delay = 250,
                    Range = 1250,
                    Radius = 90,
                    MissileSpeed = 1350,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "SivirQMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sivir",
                    SpellName = "SivirW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.DamageAmplifier, SpellTags.AoE }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sivir",
                    SpellName = "SivirE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.SpellShield }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sivir",
                    SpellName = "SivirR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.Activated,
                    CastType = new[] { CastType.Activate },
                    SpellTags = new[] { SpellTags.AttackSpeedAmplifier, SpellTags.MovementSpeedAmplifier }
                });

            #endregion Sivir

            #region Skarner

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Skarner",
                    SpellName = "SkarnerFracture",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 70,
                    MissileSpeed = 1500,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "SkarnerFractureMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            #endregion Skarner

            #region Sona

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Sona",
                    SpellName = "SonaR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 140,
                    MissileSpeed = 2400,


                    DangerValue = 5,
                    IsDangerous = true,
                    MissileSpellName = "SonaR",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            #endregion Sona

            #region Swain

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Swain",
                    SpellName = "SwainShadowGrasp",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 1100,
                    Range = 900,
                    Radius = 180,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "SwainShadowGrasp",
                });

            #endregion Swain

            #region Syndra

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Syndra",
                    SpellName = "SyndraQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 600,
                    Range = 800,
                    Radius = 150,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "SyndraQ",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Syndra",
                    SpellName = "syndrawcast",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 250,
                    Range = 950,
                    Radius = 210,
                    MissileSpeed = 1450,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "syndrawcast",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Syndra",
                    SpellName = "syndrae5",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 950,
                    Radius = 100,
                    MissileSpeed = 2000,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "syndrae5",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Syndra",
                    SpellName = "SyndraE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 950,
                    Radius = 100,
                    MissileSpeed = 2000,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "SyndraE",
                });

            #endregion Syndra

            #region Talon

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Talon",
                    SpellName = "TalonRake",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 800,
                    Radius = 80,
                    MissileSpeed = 2300,


                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "talonrakemissileone",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Talon",
                    SpellName = "TalonRakeReturn",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 800,
                    Radius = 80,
                    MissileSpeed = 1850,


                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "talonrakemissiletwo",
                });

            #endregion Riven

            #region Tahm Kench

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "TahmKench",
                    SpellName = "TahmKenchQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 951,
                    Radius = 90,
                    MissileSpeed = 2800,


                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "tahmkenchqmissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Minions, CollisionableObjects.Heroes, CollisionableObjects.YasuoWall },
                });

            #endregion Tahm Kench

            #region Thresh

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Thresh",
                    SpellName = "ThreshQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1100,
                    Radius = 70,
                    MissileSpeed = 1900,


                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ThreshQMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Minions, CollisionableObjects.Heroes, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Thresh",
                    SpellName = "ThreshEFlay",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 125,
                    Range = 1075,
                    Radius = 110,
                    MissileSpeed = 2000,


                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ThreshEMissile1",
                });

            #endregion Thresh

            #region Tristana

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Tristana",
                    SpellName = "RocketJump",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 500,
                    Range = 900,
                    Radius = 270,
                    MissileSpeed = 1500,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "RocketJump",
                });

            #endregion Tristana

            #region Tryndamere

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Tryndamere",
                    SpellName = "slashCast",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 0,
                    Range = 660,
                    Radius = 93,
                    MissileSpeed = 1300,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "slashCast",
                });

            #endregion Tryndamere

            #region TwistedFate

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "TwistedFate",
                    SpellName = "WildCards",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1450,
                    Radius = 40,
                    MissileSpeed = 1000,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "SealFateMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            #endregion TwistedFate

            #region Twitch

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Twitch",
                    SpellName = "TwitchVenomCask",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 250,
                    Range = 900,
                    Radius = 275,
                    MissileSpeed = 1400,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "TwitchVenomCaskMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            #endregion Twitch

            #region Urgot

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Urgot",
                    SpellName = "UrgotHeatseekingLineMissile",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 125,
                    Range = 1000,
                    Radius = 60,
                    MissileSpeed = 1600,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "UrgotHeatseekingLineMissile",
                    CanBeRemoved = true,
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Urgot",
                    SpellName = "UrgotPlasmaGrenade",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 250,
                    Range = 1100,
                    Radius = 210,
                    MissileSpeed = 1500,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "UrgotPlasmaGrenadeBoom",
                });

            #endregion Urgot

            #region Varus

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Varus",
                    SpellName = "VarusQMissilee",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1800,
                    Radius = 70,
                    MissileSpeed = 1900,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VarusQMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Varus",
                    SpellName = "VarusE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 1000,
                    Range = 925,
                    Radius = 235,
                    MissileSpeed = 1500,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VarusE",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Varus",
                    SpellName = "VarusR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 120,
                    MissileSpeed = 1950,


                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "VarusRMissile",
                    CanBeRemoved = true,
                    CollisionObjects = new[] { CollisionableObjects.Heroes, CollisionableObjects.YasuoWall },
                });

            #endregion Varus

            #region Veigar

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Veigar",
                    SpellName = "VeigarBalefulStrike",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 950,
                    Radius = 70,
                    MissileSpeed = 2000,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VeigarBalefulStrikeMis",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Veigar",
                    SpellName = "VeigarDarkMatter",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 1350,
                    Range = 900,
                    Radius = 225,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Veigar",
                    SpellName = "VeigarEventHorizon",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotRing,
                    Delay = 500,
                    Range = 700,
                    Radius = 80,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "",
                });

            #endregion Veigar

            #region Velkoz

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 50,
                    MissileSpeed = 1300,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VelkozQMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Minions, CollisionableObjects.Heroes, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozQSplit",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1100,
                    Radius = 55,
                    MissileSpeed = 2100,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VelkozQMissileSplit",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Minions, CollisionableObjects.Heroes, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1200,
                    Radius = 88,
                    MissileSpeed = 1700,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VelkozWMissile",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Velkoz",
                    SpellName = "VelkozE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 500,
                    Range = 800,
                    Radius = 225,
                    MissileSpeed = 1500,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "VelkozEMissile",
                });

            #endregion Velkoz

            #region Vi

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Vi",
                    SpellName = "Vi-q",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1000,
                    Radius = 90,
                    MissileSpeed = 1500,


                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ViQMissile",
                });

            #endregion Vi

            #region Viktor

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Viktor",
                    SpellName = "Laser",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1500,
                    Radius = 80,
                    MissileSpeed = 780,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ViktorDeathRayMissile",
                    ExtraMissileNames = new[] { "viktoreaugmissile" },
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            #endregion Viktor

            #region Xerath

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Xerath",
                    SpellName = "xeratharcanopulse2",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotLine,
                    Delay = 600,
                    Range = 1600,
                    Radius = 100,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "xeratharcanopulse2",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Xerath",
                    SpellName = "XerathArcaneBarrage2",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 700,
                    Range = 1000,
                    Radius = 200,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "XerathArcaneBarrage2",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Xerath",
                    SpellName = "XerathMageSpear",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 200,
                    Range = 1150,
                    Radius = 60,
                    MissileSpeed = 1400,


                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "XerathMageSpearMissile",
                    CanBeRemoved = true,
                    CollisionObjects =
                        new[] { CollisionableObjects.Minions, CollisionableObjects.Heroes, CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Xerath",
                    SpellName = "xerathrmissilewrapper",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 700,
                    Range = 5600,
                    Radius = 120,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "xerathrmissilewrapper",
                });

            #endregion Xerath

            #region Yasuo

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Yasuo",
                    SpellName = "yasuoq2",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotLine,
                    Delay = 400,
                    Range = 550,
                    Radius = 20,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "yasuoq2",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Yasuo",
                    SpellName = "yasuoq3w",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1150,
                    Radius = 90,
                    MissileSpeed = 1500,


                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "yasuoq3w",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Yasuo",
                    SpellName = "yasuoq",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotLine,
                    Delay = 400,
                    Range = 550,
                    Radius = 20,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 2,
                    IsDangerous = true,
                    MissileSpellName = "yasuoq",
                });

            #endregion Yasuo

            #region Zac

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Zac",
                    SpellName = "ZacQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotLine,
                    Delay = 500,
                    Range = 550,
                    Radius = 120,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZacQ",
                });

            #endregion Zac

            #region Zed

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Zed",
                    SpellName = "ZedQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 925,
                    Radius = 50,
                    MissileSpeed = 1700,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZedQMissile",
                    //FromObjects = new[] { "Zed_Clone_idle.troy", "Zed_Clone_Idle.troy" },
                    FromObjects = new[] { "Zed_Base_W_tar.troy", "Zed_Base_W_cloneswap_buf.troy" },
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            #endregion Zed

            #region Ziggs

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 250,
                    Range = 850,
                    Radius = 140,
                    MissileSpeed = 1700,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZiggsQSpell",
                    CanBeRemoved = false,
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsQBounce1",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 250,
                    Range = 850,
                    Radius = 140,
                    MissileSpeed = 1700,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZiggsQSpell2",
                    ExtraMissileNames = new[] { "ZiggsQSpell2" },
                    CanBeRemoved = false,
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsQBounce2",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 250,
                    Range = 850,
                    Radius = 160,
                    MissileSpeed = 1700,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZiggsQSpell3",
                    ExtraMissileNames = new[] { "ZiggsQSpell3" },
                    CanBeRemoved = false,
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsW",
                    Slot = SpellSlot.W,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 250,
                    Range = 1000,
                    Radius = 275,
                    MissileSpeed = 1750,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZiggsW",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsE",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 500,
                    Range = 900,
                    Radius = 235,
                    MissileSpeed = 1750,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZiggsE",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Ziggs",
                    SpellName = "ZiggsR",
                    Slot = SpellSlot.R,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 0,
                    Range = 5300,
                    Radius = 500,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZiggsR",
                });

            #endregion Ziggs

            #region Zilean

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Zilean",
                    SpellName = "ZileanQ",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 300,
                    Range = 900,
                    Radius = 210,
                    MissileSpeed = 2000,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZileanQMissile",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall }
                });

            #endregion Zilean

            #region Zyra

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Zyra",
                    SpellName = "ZyraQFissure",
                    Slot = SpellSlot.Q,
                    SpellType = SpellType.SkillshotCircle,
                    Delay = 850,
                    Range = 800,
                    Radius = 220,
                    MissileSpeed = int.MaxValue,


                    DangerValue = 2,
                    IsDangerous = false,
                    MissileSpellName = "ZyraQFissure",
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Zyra",
                    SpellName = "ZyraGraspingRoots",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 250,
                    Range = 1150,
                    Radius = 70,
                    MissileSpeed = 1150,


                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "ZyraGraspingRoots",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            Spells.Add(
                new SpellDatabaseEntry
                {
                    ChampionName = "Zyra",
                    SpellName = "zyrapassivedeathmanager",
                    Slot = SpellSlot.E,
                    SpellType = SpellType.SkillshotMissileLine,
                    Delay = 500,
                    Range = 1474,
                    Radius = 70,
                    MissileSpeed = 2000,


                    DangerValue = 3,
                    IsDangerous = true,
                    MissileSpellName = "zyrapassivedeathmanager",
                    CollisionObjects = new[] { CollisionableObjects.YasuoWall },
                });

            #endregion Zyra

            //Console.WriteLine("Added " + Spells.Count + " spells.");
        }


        #region Methods


        /// <summary>
        /// Search the spell database for a spell you need right in the moment of the query, for example if you need a heal.
        /// </summary>
        /// <param name="spellTag">The Spell Tags you are looking for, see <cref="SpellTags">SpellTags</cref></param>
        /// <param name="championName">The associated champion name, it's your champion's name by default.</param>
        /// <returns></returns>
        public static List<SpellDatabaseEntry> GetBySpellTag(SpellTags spellTag, string championName = "")
        {
            var actualChampionName = championName == "" ? ObjectManager.Player.CharData.BaseSkinName : championName;
            var matches = new List<SpellDatabaseEntry>();
            foreach (var spellData in Spells)
            {
                if (spellData.SpellTags.Any(tag => tag == spellTag))
                {
                    matches.Add(spellData);
                }
            }
            return matches;
        }

        /// <summary>
        /// Search the spell database by the spell name. You can use the core SData.Name property to figure out a spellslot's name.
        /// </summary>
        /// <param name="spellName">The name of the spell you're looking for.</param>
        /// <returns></returns>
        public static SpellDatabaseEntry GetByName(string spellName)
        {
            spellName = spellName.ToLower();
            foreach (var spellData in Spells)
            {
                if (spellData.SpellName.ToLower() == spellName || spellData.ExtraSpellNames.Contains(spellName))
                {
                    return spellData;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns all the available and known spells on the specified slot
        /// </summary>
        /// <param name="slot">The slot you are looking for</param>
        /// <param name="championName">The associated champion name, it's your champion's by default.</param>
        /// <returns></returns>
        public static List<SpellDatabaseEntry> GetBySpellSlot(SpellSlot slot, string championName = "")
        {
            var actualChampionName = championName.Equals("") ? ObjectManager.Player.CharData.BaseSkinName.ToLower() : championName.ToLower();
            var matches = new List<SpellDatabaseEntry>();
            foreach (var spellData in Spells)
            {
                if (spellData.ChampionName.ToLower() == actualChampionName && spellData.Slot == slot)
                {
                    matches.Add(spellData);
                }
            }
            return matches;
        }

        /// <summary>
        /// If you know that your spell leaves a missile and you also know it's name, use this to search the spell database.
        /// </summary>
        /// <param name="missileSpellName">The name of the missile you're looking for.</param>
        /// <returns></returns>
        public static SpellDatabaseEntry GetByMissileName(string missileSpellName)
        {
            missileSpellName = missileSpellName.ToLower();
            foreach (var spellData in Spells)
            {
                if (spellData.MissileSpellName != null && spellData.MissileSpellName.ToLower() == missileSpellName ||
                    spellData.ExtraMissileNames.Contains(missileSpellName))
                {
                    return spellData;
                }
            }

            return null;
        }
        #endregion Methods
    }
}
