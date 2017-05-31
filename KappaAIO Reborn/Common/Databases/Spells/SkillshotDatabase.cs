using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Databases.SpellData;

namespace KappAIO_Reborn.Common.Databases.Spells
{
    public static class SkillshotDatabase
    {
        public static SkillshotData[] Current;

        static SkillshotDatabase()
        {
            if (Current == null || !Current.Any())
                Current = List.Where(s => (s.GameType.Equals(GameType.Normal) || s.GameType.Equals(Game.Type)) && (s.IsCasterName("all") || s.IsCasterName("jungle") || EntityManager.Heroes.AllHeroes.Any(h => s.IsCasterName(h.ChampionName)))).ToArray();
        }

        private static readonly SkillshotData[] List =
            {
				#region All
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "AscXerath", "jungle" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 530,
                     Range = 1025,
                     Speed = int.MaxValue,
					 GameType = GameType.Ascension,
                     Width = 100,
                     SpellNames = new[] { "AscXerathArcanopulse2" },
                     MissileNames = new[] { "AscXerathArcanopulse2" },
                     StaticStart = true,
					 IsDangerous = true,
					 AllowDuplicates = true,
					 IsFixedRange = true
                   },
               new SkillshotData
                  {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Baron", "SRU_Baron", "jungle" },
                     Slots = new[] { SpellSlot.E },
                     GameType = GameType.Normal,
                     DangerLevel = 3,
                     CastDelay = 1250,
                     Range = int.MaxValue,
                     Speed = int.MaxValue,
                     Width = 200,
					 DisplayName = "Tail",
					 ParticleNames = new[] { "SRU_Baron_Tail_Target.troy" }
                  },
               new SkillshotData
                  {
                     type = Type.LineMissile,
                     CasterNames = new[] { "all" },
                     Slots = new[] { SpellSlot.Unknown },
                     GameType = (GameType)911,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1800,
                     Speed = 500,
                     Width = 70,
                     ExtraRange = 75,
                     SpellNames = new[] { "testskill" },
                     Collisions = new [] { Collision.YasuoWall, Collision.Heros },
					 IsFixedRange = true
                  },
               new SkillshotData
                  {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "all" },
                     Slots = new[] { SpellSlot.Unknown },
                     GameType = (GameType)911,
                     DangerLevel = 5,
                     CastDelay = 2000,
                     Range = 1200,
                     Speed = int.MaxValue,
                     Width = 300,
                     SpellNames = new[] { "testskillc" },
                  },
               new SkillshotData
                  {
                     type = Type.LineMissile,
                     CasterNames = new[] { "all" },
                     Slots = new[] { SpellSlot.Unknown },
                     GameType = (GameType)7,
                     DangerLevel = 2,
                     CastDelay = 750,
                     Range = 2825,
                     Speed = int.MaxValue,
                     Width = 70,
                     SpellNames = new[] { "SiegeLaserAffixShot" },
                     IsFixedRange = true,
                     StaticStart = true
                  },
               new SkillshotData
                  {
                     type = Type.LineMissile,
                     CasterNames = new[] { "all" },
                     Slots = new[] { SpellSlot.Unknown },
                     GameType = GameType.ARAM,
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1600,
                     Speed = 1200,
                     Width = 80,
                     SpellNames = new[] { "SummonerSnowball" },
                     IsFixedRange = true,
                     Collisions = new[] { Collision.Heros, Collision.Minions, Collision.YasuoWall }
                  },
				  
				#endregion All
				
				#region Aatrox
				
               new SkillshotData
                  {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Aatrox" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 650,
                     Speed = 2000,
                     Width = 250,
                     SpellNames = new[] { "AatroxQ" },
                     ParticleNames = new[] { "Aatrox_Base_Q_Tar_" },
                  },
               new SkillshotData
                  {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Aatrox" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1075,
                     Speed = 1200,
                     Width = 100,
                     SpellNames = new[] { "AatroxE" },
                     MissileNames = new[] { "AatroxEConeMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                  },
				  
				#endregion Aatrox
				
				#region Ahri
				
               new SkillshotData
                  {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ahri" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 2500,
                     Width = 100,
                     MissileAccel = -3200,
                     MissileMaxSpeed = 2500,
                     MissileMinSpeed = 400,
                     SpellNames = new[] { "AhriOrbofDeception" },
                     MissileNames = new[] { "AhriOrbMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                  {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ahri" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 25000,
                     Speed = 60,
                     Width = 100,
                     MissileAccel = 1900,
                     MissileMinSpeed = 60,
                     MissileMaxSpeed = 2600,
                     SpellNames = new[] { "AhriOrbReturn" },
                     MissileNames = new[] { "AhriOrbReturn" },
                     Collisions = new []{ Collision.YasuoWall },
                     EndSticksToCaster = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ahri" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 1550,
                     Width = 60,
                     SpellNames = new[] { "AhriSeduce" },
                     MissileNames = new[] { "AhriSeduceMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Ahri
				
				#region Akali
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Akali" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 325,
                     SpellNames = new[] { "AkaliShadowSwipe" },
                   },
				
				#endregion Akali
				
				#region Alistar
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Alistar" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 365,
                     SpellNames = new[] { "Pulverize" },
                   },
				   
				#endregion Alistar
				   
				#region Amumu
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Amumu" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 2000,
                     Width = 80,
                     SpellNames = new[] { "BandageToss" },
                     MissileNames = new[] { "SadMummyBandageToss" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Amumu" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 350,
                     SpellNames = new[] { "Tantrum" },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Amumu" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 550,
                     SpellNames = new[] { "CurseoftheSadMummy" },
                     IsFixedRange = true
                   },
				   
				#endregion Amumu
				
				#region Anivia
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Anivia" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 1100,
                     ExtraRange = 250,
                     Speed = 850,
                     Width = 150,
                     SpellNames = new[] { "FlashFrostSpell" },
                     MissileNames = new[] { "FlashFrostSpell" },
                     Collisions = new []{ Collision.YasuoWall },
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Anivia" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 700,
                     Speed = int.MaxValue,
                     Width = 90,
                     SpellNames = new[] { "Crystallize" },
                     StaticStart = true,
                     StaticEnd = true
                   },
				   
				#endregion Anivia
				
				#region Annie
				
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Annie" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     Angle = 50,
                     CastDelay = 250,
                     Range = 560,
                     Speed = int.MaxValue,
                     Width = 0,
                     SpellNames = new[] { "Incinerate" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     StaticStart = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Annie" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 200,
                     Range = 600,
                     Speed = int.MaxValue,
                     Width = 250,
                     SpellNames = new[] { "InfernalGuardian" },
                   },
				   
				#endregion Annie
				
				#region Ashe
			
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ashe" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1250,
                     Speed = 1500,
                     Width = 50,
					 DisplayName = "VolleyAttack",
                     SpellNames = new[] { "Volley" },
                     MissileNames = new[] { "VolleyAttack" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
                     DetectByMissile = true,
                     AllowDuplicates = true,
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ashe" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 4000,
                     Speed = 1600,
                     Width = 130,
                     SpellNames = new[] { "EnchantedCrystalArrow" },
                     MissileNames = new[] { "EnchantedCrystalArrow" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 CollideExplode = true,
					 Explodetype = Type.CircleMissile,
					 ExplodeWidth = 250
                   },
				   
				#endregion Ashe
				
				#region AurelionSol
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "AurelionSol" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 550,
                     Speed = 850,
                     Width = 210,
                     SpellNames = new[] { "AurelionSolQ" },
                     MissileNames = new[] { "AurelionSolQMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "AurelionSol" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 300,
                     Range = 1420,
                     Speed = 4500,
                     Width = 120,
                     SpellNames = new[] { "AurelionSolR" },
                     MissileNames = new[] { "AurelionSolRBeamMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion AurelionSol
				
				#region Azir
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Azir", "AzirSoldier" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1750,
                     Speed = 1600,
                     Width = 80,
                     ExtraRange = 175,
                     SpellNames = new[] { "AzirQ" },
                     MissileNames = new[] { "AzirSoldierMissile" },
                     Collisions = new []{ Collision.YasuoWall },
					 IsDangerous = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Azir" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 550,
                     Speed = 1400,
                     Width = 150,
                     SpellNames = new[] { "AzirR" },
                     MissileNames = new[] { "AzirSoldierRMissile" },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Azir
				
				#region Bard
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Bard" },
                     Slots = new[] { SpellSlot.Q },
                     CollideCount = 1,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 950,
                     Speed = 1500,
                     Width = 60,
                     SpellNames = new[] { "BardQ" },
                     MissileNames = new[] { "BardQMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Bard" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 500,
                     Range = 3400,
                     Speed = 2100,
                     Width = 350,
                     SpellNames = new[] { "BardR" },
                     MissileNames = new[] { "BardRMissileFixedTravelTime", "BardRMissileRange1", "BardRMissileRange2", "BardRMissileRange3", "BardRMissileRange4", "BardRMissileRange5" },
                   },
				   
				#endregion Bard
				
				#region Blitzcrank
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Blitzcrank" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 1050,
                     Speed = 1800,
                     Width = 80,
                     SpellNames = new[] { "RocketGrab" },
                     MissileNames = new[] { "RocketGrabMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Blitzcrank" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 600,
                     SpellNames = new[] { "StaticField" },
                     IsFixedRange = true
                   },
				   
				#endregion Blitzcrank
				
				#region Brand
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Brand" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 1600,
                     Width = 60,
                     SpellNames = new[] { "BrandQ" },
                     MissileNames = new[] { "BrandQMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Brand" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 900,
                     Range = 900,
                     Speed = int.MaxValue,
                     Width = 240,
                     SpellNames = new[] { "BrandW" },
                     ParticleNames = new[] { "Brand_Base_W_POF_tar_" },
                   },
				   
				#endregion Brand
				
				#region Braum
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Braum" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1050,
                     Speed = 1700,
                     Width = 60,
                     SpellNames = new[] { "BraumQ" },
                     MissileNames = new[] { "BraumQMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 IsDangerous = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Braum" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 550,
                     Range = 1200,
                     Speed = 1400,
                     Width = 115,
                     SpellNames = new[] { "BraumRWrapper" },
                     MissileNames = new[] { "BraumRMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Braum
				
				#region Caitlyn
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Caitlyn" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 625,
                     Range = 1300,
                     Speed = 2200,
                     Width = 90,
                     SpellNames = new[] { "CaitlynPiltoverPeacemaker", "CaitlynQBehind" },
                     MissileNames = new[] { "CaitlynPiltoverPeacemaker" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Caitlyn" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1300,
                     Speed = 2200,
                     Width = 100,
					 DisplayName = "Q Expand",
                     MissileNames = new[] { "CaitlynPiltoverPeacemaker2" },
                     Collisions = new []{ Collision.YasuoWall },
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Caitlyn" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 90000,
                     Range = 1500,
                     Speed = int.MaxValue,
                     Width = 90,
                     DisplayName = "Caitlyn W Trap",
					 TrapBuff = "CaitlynYordleTrap",
					 TrapBaseSkinNames = new[] { "CaitlynTrap" },
                     AllowDuplicates = true,
                     DontCross = true,
					 IsTrap = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Caitlyn" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 160,
                     Range = 800,
                     Speed = 1600,
                     Width = 70,
                     SpellNames = new[] { "CaitlynEntrapment" },
                     MissileNames = new[] { "CaitlynEntrapmentMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 IsDangerous = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Caitlyn" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 2000,
                     Range = int.MaxValue,
                     Speed = 3200,
                     Width = 70,
                     SpellNames = new[] { "CaitlynAceintheHole" },
                     MissileNames = new[] { "CaitlynAceintheHoleMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     SticksToMissile = true,
					 EndSticksToTarget = true
                   },
				   
				#endregion Caitlyn
				
				#region Camille
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Camille" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 0,
                     Range = 800,
                     Speed = 900,
                     Width = 90,
                     ExtraRange = 150,
                     SpellNames = new[] { "CamilleEDash2" },
                     Collisions = new []{ Collision.Heros },
                     SticksToCaster = true,
                     IsFixedRange = true
                   },
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Camille" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 750,
                     Range = 610,
                     Speed = int.MaxValue,
                     Width = 0,
					 Angle = 75,
                     SpellNames = new[] { "CamilleW" },
                     IsFixedRange = true,
                     SticksToCaster = true,
					 IsMoving = true
                   },
				   
				#endregion Camille
				
				#region Cassiopeia
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Cassiopeia" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 750,
                     Range = 850,
                     Speed = int.MaxValue,
                     Width = 160,
                     SpellNames = new[] { "CassiopeiaQ" },
                     ParticleNames = new[] { "Cassiopeia_Base_Q_Hit_" }
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Cassiopeia" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 900,
                     Speed = 3000,
                     Width = 180,
                     SpellNames = new[] { "CassiopeiaW" },
                     MissileNames = new[] { "CassiopeiaWMissile" },
                     DetectByMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Cassiopeia" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 5000,
                     Range = 900,
                     Speed = int.MaxValue,
                     Width = 180,
                     DisplayName = "Cassiopeia W Placement",
                     ParticleNames = new[] { "Cassiopeia_Base_W_WCircle_tar_" },
                     AllowDuplicates = true
                   },
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Cassiopeia" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 450,
                     Range = 800,
                     Speed = int.MaxValue,
                     Width = 0,
                     Angle = 80,
                     SpellNames = new[] { "CassiopeiaR" },
                     IsFixedRange = true,
                     SticksToMissile = true,
                     StaticStart = true
                   },
				   
				#endregion Cassiopeia
				
				#region Chogath
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Chogath" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 1200,
                     Range = 950,
                     Speed = int.MaxValue,
                     Width = 250,
                     SpellNames = new[] { "Rupture" },
                     ParticleNames = new[] { "rupture_cas_01_" }
                   },
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Chogath" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 3,
                     CastDelay = 500,
                     Range = 650,
                     Speed = int.MaxValue,
                     Width = 0,
                     Angle = 55,
                     SpellNames = new[] { "FeralScream" },
                     StaticStart = true
                   },
				   
				#endregion Chogath
				
				#region Corki
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Corki" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 300,
                     Range = 825,
                     Speed = 1000,
                     Width = 250,
                     SpellNames = new[] { "PhosphorusBomb" },
                     MissileNames = new[] { "PhosphorusBombMissile", "PhosphorusBombMissileMin" },
                     ParticleNames = new[] { "Corki_Base_Q_Indicator_" },
                     Collisions = new []{ Collision.YasuoWall },
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Corki" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 2,
                     CastDelay = 180,
                     Range = 1300,
                     Speed = 2000,
                     Width = 40,
                     SpellNames = new[] { "MissileBarrage" },
                     MissileNames = new[] { "MissileBarrageMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 CollideExplode = true,
					 Explodetype = Type.CircleMissile,
					 ExplodeWidth = 125					 
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Corki" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 180,
                     Range = 1500,
                     Speed = 2000,
                     Width = 40,
                     SpellNames = new[] { "MissileBarrage2" },
                     MissileNames = new[] { "MissileBarrageMissile2" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 CollideExplode = true,
					 Explodetype = Type.CircleMissile,
					 ExplodeWidth = 175
                   },
				   
				#endregion Corki
				
				#region Darius
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Darius" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 750,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 375,
                     SpellNames = new[] { "DariusCleave" },
                     EndSticksToCaster = true,
                     DontCross = true,
					 RemoveOnBuffLose = "dariusqcast"
                   },
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Darius" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 550,
                     Speed = int.MaxValue,
                     Width = 0,
                     Angle = 50,
                     SpellNames = new[] { "DariusAxeGrabCone" },
                     IsFixedRange = true,
                     StaticStart = true
                   },
				   
				#endregion Darius
				   
				#region Diana
				
               new SkillshotData
                   {
                     type = Type.Arc,
                     CasterNames = new[] { "Diana" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 850,
                     Speed = 1400,
                     Width = 50,
                     SpellNames = new[] { "DianaArc" },
                     Collisions = new []{ Collision.YasuoWall },
                     TakeClosestPath = true,
					 AllowDuplicates = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Diana" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 850,
                     Speed = 1400,
                     Width = 250,
					 DisplayName = "Explode End",
                     SpellNames = new[] { "DianaArc" },
                     Collisions = new []{ Collision.YasuoWall },
                     TakeClosestPath = true,
					 AllowDuplicates = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Diana" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 200,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 350,
                     SpellNames = new[] { "DianaVortex" },
                     IsFixedRange = true
                   },
				   
				#endregion Diana
				
				#region DrMundo
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "DrMundo" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1050,
                     Speed = 2000,
                     Width = 60,
                     SpellNames = new[] { "InfectedCleaverMissileCast" },
                     MissileNames = new[] { "InfectedCleaverMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion DrMundo
				
				#region Draven
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Draven" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 1400,
                     Width = 130,
                     SpellNames = new[] { "DravenDoubleShot" },
                     MissileNames = new[] { "DravenDoubleShotMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Draven" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 500,
                     Range = 4500,
                     Speed = 2000,
                     Width = 160,
                     SpellNames = new[] { "DravenRCast" },
                     MissileNames = new[] { "DravenR" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Draven
				
				#region Ekko
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ekko" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 950,
                     Speed = 1650,
                     Width = 60,
                     SpellNames = new[] { "EkkoQ" },
                     MissileNames = new[] { "EkkoQMis" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 IsDangerous = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ekko" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 20000,
                     Speed = 2300,
                     Width = 100,
                     SpellNames = new[] { "EkkoQReturn" },
                     MissileNames = new[] { "EkkoQReturn" },
                     Collisions = new []{ Collision.YasuoWall },
                     EndSticksToCaster = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Ekko" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1700,
                     Speed = int.MaxValue,
                     Width = 375,
                     SpellNames = new[] { "EkkoW" },
                     MissileNames = new[] { "EkkoWMis" },
					 ExtraDuration = 3000,
					 //ParticleNames = new[] { "Ekko_Base_W_Indicator"
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Ekko", "TestCubeRender", "Ekko" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 750,
                     Range = int.MaxValue,
                     Speed = int.MaxValue,
                     Width = 375,
                     SpellNames = new[] { "EkkoR" },
                     MissileNames = new[] { "EkkoR" },
                     //ParticleNames = new[] { "Ekko_Base_R_AOERing",
                   },
				   
				#endregion Ekko
				
				#region Elise
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Elise" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 1600,
                     Width = 80,
                     SpellNames = new[] { "EliseHumanE" },
                     MissileNames = new[] { "EliseHumanE" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Elise
				
				#region Evelynn
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Evelynn" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 650,
                     Speed = 2200,
                     Width = 60,
                     DisplayName = "Evelynn Q",
                     MissileNames = new[] { "HateSpikeLineMissile" },
                     Collisions = new []{ Collision.YasuoWall },
					 IsFixedRange = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Evelynn" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 650,
                     Speed = int.MaxValue,
                     Width = 350,
                     SpellNames = new[] { "EvelynnR" },
                   },
				   
				#endregion Evelynn
				
				#region Ezreal
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ezreal" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1200,
                     Speed = 2000,
                     Width = 60,
                     SpellNames = new[] { "EzrealMysticShot" },
                     MissileNames = new[] { "EzrealMysticShotMissile", "EzrealMysticShotPulseMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 IsDangerous = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ezreal" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 1050,
                     Speed = 1600,
                     Width = 80,
                     SpellNames = new[] { "EzrealEssenceFlux" },
                     MissileNames = new[] { "EzrealEssenceFluxMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ezreal" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 1000,
                     Range = 4500,
                     Speed = 2000,
                     Width = 160,
                     SpellNames = new[] { "EzrealTrueshotBarrage" },
                     MissileNames = new[] { "EzrealTrueshotBarrage" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Ezreal
				
				#region Fiora
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Fiora" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 200,
                     Range = 400,
                     Speed = 3000,
                     Width = 175,
                     SpellNames = new[] { "FioraQ" },
                     SticksToCaster = true,
					 RemoveOnBuffLose = "FioraQ"
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Fiora" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 3,
                     CastDelay = 500,
                     Range = 800,
                     Speed = 3200,
                     Width = 70,
                     SpellNames = new[] { "FioraW" },
                     MissileNames = new[] { "FioraWMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Fiora
				
				#region Fizz
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Fizz" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Range = 550,
                     Speed = 1400,
                     Width = 60,
                     SpellNames = new[] { "FizzQ" },
                     SticksToCaster = true,
                     IsFixedRange = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Fizz" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 350,
                     Speed = int.MaxValue,
                     Width = 200,
                     SpellNames = new[] { "FizzETwo" },
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Fizz" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1275,
                     Speed = 1300,
                     Width = 120,
                     SpellNames = new[] { "FizzR" },
                     MissileNames = new[] { "FizzRMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     SticksToMissile = true,
                     HasExplodingEnd = true,
                     ExplodeWidth = 375,
					 Explodetype = Type.CircleMissile
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Fizz" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 2000,
                     Range = 1275,
                     Speed = int.MaxValue,
                     Width = 375,
                     SpellNames = new[] { "Fizz R Shark" },
                     ParticleNames = new[] { "Fizz_Base_R_Ring_" }
                   },
				   
				#endregion Fizz
				
				#region Galio
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Galio" },
                     Slots = new[] { SpellSlot.Q, (SpellSlot)48 },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 940,
                     Speed = 1400,
                     Width = 200,
					 SpellNames = new[] { "GalioQ" },
                     MissileNames = new[] { "GalioQMissile"/*, "GalioQMissileR"*/ },
                     Collisions = new []{ Collision.YasuoWall },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Galio" },
                     Slots = new[] { SpellSlot.Q, (SpellSlot)50 },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = int.MaxValue,
                     Speed = 60,
                     Width = 200,
					 DisplayName = "Tornado",
                     MissileNames = new[] { "GalioQSuper" },
					 DetectByMissile = true,
					 IsMoving = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Galio" },
                     Slots = new[] { SpellSlot.W, (SpellSlot)45 },
                     DangerLevel = 3,
                     CastDelay = 50,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 400,
                     SpellNames = new[] { "GalioW2" },
					 SticksToCaster = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Galio" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 500,
                     Range = 800,
                     Speed = 1650,
                     Width = 160,
                     SpellNames = new[] { "GalioE" },
                     Collisions = new []{ Collision.Heros, Collision.Walls },
					 SticksToCaster = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Galio" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 2250,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 550,
					 DisplayName = "Galio R",
                     SpellNames = new[] { "GalioR" },
					 ParticleNames = new[] { "Galio_Base_R_Tar_" },
					 StaticEnd = true
                   },
				   
				#endregion Galio
				
				#region Gangplank
				
				/* TODO
               new SkillData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Gangplank" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 300,
                     SpellNames = new[] { "Barrel Explosion",
                     ParticleNames = new[] { "Gangplank_Base_E_Explosion",
                     ParticalObjectName = "Barrel",
                     AllowDuplicates = true
                   },
				   */
				#endregion Gangplank
				
				#region Gnar
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Gnar" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1125,
                     Speed = 2500,
                     Width = 60,
                     MissileAccel = -3000,
                     MissileMaxSpeed = 2500,
                     MissileMinSpeed = 1400,
                     SpellNames = new[] { "GnarQ" },
                     MissileNames = new[] { "GnarQMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Gnar" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 5000,
                     Speed = 60,
                     Width = 90,
                     MissileAccel = 800,
                     MissileMaxSpeed = 2600,
                     MissileMinSpeed = 60,
                     SpellNames = new[] { "GnarQReturn" },
                     MissileNames = new[] { "GnarQMissileReturn" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Caster },
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Gnar" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 500,
                     Range = 1150,
                     Speed = 2100,
                     Width = 90,
                     SpellNames = new[] { "GnarBigQ" },
                     MissileNames = new[] { "GnarBigQMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Minions, Collision.Heros },
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Gnar" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 3,
                     CastDelay = 600,
                     Range = 600,
                     Speed = int.MaxValue,
                     Width = 110,
                     SpellNames = new[] { "GnarBigW" },
                     IsFixedRange = true,
                     FastEvade = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Gnar" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Range = 475,
                     Speed = 900,
                     Width = 150,
                     SpellNames = new[] { "GnarE" },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Gnar" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 475,
                     Speed = 800,
                     Width = 350,
                     SpellNames = new[] { "GnarBigE" },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Gnar" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 275,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 500,
                     SpellNames = new[] { "GnarR" },
					 ParticleNames = new[] { "Gnar_Base_R_Cas_" },
                     FastEvade = true
                   },
				   
				#endregion Gnar
				
				#region Gragas
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Gragas" },
                     Slots = new[] { SpellSlot.Q, (SpellSlot)45 },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 850,
                     Speed = 1000,
                     Width = 270,
                     SpellNames = new[] { "GragasQ" },
                     MissileNames = new[] { "GragasQMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     DontCross = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Gragas" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Range = 850,
                     Speed = int.MaxValue,
                     Width = 270,
                     DisplayName = "Gragas Q Placement",
					 ParticleNames = new[] { "Gragas_Base_Q_" },
                     DontCross = true,
					 ExtraDuration = 4000,
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Gragas" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 4,
                     CastDelay = 0,
                     Range = 600,
                     ExtraRange = 100,
                     Speed = 1000,
                     Width = 200,
                     SpellNames = new[] { "GragasE" },
                     Collisions = new []{ Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToCaster = true,
					 RemoveOnBuffLose = "GragasE"
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Gragas" },
                     Slots = new[] { SpellSlot.R, (SpellSlot)46 },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 1800,
                     Width = 350,
                     SpellNames = new[] { "GragasR" },
                     MissileNames = new[] { "GragasRBoom" },
                     Collisions = new []{ Collision.YasuoWall },
					 DecaySpeedWithLessRange = true
                   },
				   
				#endregion Gragas
				
				#region Graves
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Graves" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 825,
                     Speed = 3000,
                     Width = 60,
                     SpellNames = new[] { "GravesQLineSpell" },
                     MissileNames = new[] { "GravesQLineMis" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Walls },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 HasExplodingEnd = true,
					 ExplodeWidth = 275,
					 DontRemoveWithMissile = true,
					 Explodetype = Type.LineMissile
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Graves" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 825,
                     Speed = 1600,
                     Width = 60,
                     SpellNames = new[] { "GravesQReturn" },
                     MissileNames = new[] { "GravesQReturn" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Graves" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 950,
                     Speed = 1500,
                     Width = 225,
                     SpellNames = new[] { "GravesSmokeGrenade" },
                     MissileNames = new[] { "GravesSmokeGrenadeBoom" },
                     Collisions = new []{ Collision.YasuoWall },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Graves" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 1,
                     CastDelay = 4300,
                     Range = 950,
                     Speed = int.MaxValue,
                     Width = 225,
                     SpellNames = new[] { "Graves W Placement" },
                     ParticleNames = new[] { "Graves_Base_W_tar_" }
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Graves" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 2100,
                     Width = 100,
                     SpellNames = new[] { "GravesChargeShot" },
                     MissileNames = new[] { "GravesChargeShotShot" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 HasExplodingEnd = true,
					 ExplodeWidth = 900,
					 Angle = 65,
					 Explodetype = Type.Cone
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Graves" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 0,
                     Range = 900,
                     Speed = 2000,
                     Width = 100,
                     DisplayName = "Explode",
                     MissileNames = new[] { "GravesChargeShotFxMissile", "GravesChargeShotFxMissile2" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 AllowDuplicates = true
                   },
				   
				#endregion Graves
				
				#region Hecarim
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Hecarim" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 1,
                     CastDelay = 50,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 300,
                     SpellNames = new[] { "HecarimRapidSlash" },
                     //ParticleNames = new[] { "Hecarim_Base_Q_Cas" TODO
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Hecarim" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 0,
                     Range = 1500,
                     Speed = 1100,
                     Width = 240,
                     SpellNames = new[] { "HecarimUlt" },
                     MissileNames = new[] { "HecarimUltMissile" },
                     SticksToMissile = true
                   },
				   
				#endregion Hecarim
				   
				#region Heimerdinger

               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Heimerdinger", "HeimerTYellow" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Range = 1000,
                     Speed = 1650,
                     Width = 65,
                     MissileAccel = -1000,
                     MissileMinSpeed = 1200,
                     MissileMaxSpeed = 1650,
                     SpellNames = new[] { "HeimerdingerQTurretBlast" },
                     MissileNames = new[] { "HeimerdingerTurretEnergyBlast" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Heimerdinger", "HeimerTBlue" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1000,
                     Speed = 1599.4f,
                     Width = 80,
                     MissileAccel = -1000,
                     MissileMinSpeed = 1200,
                     MissileMaxSpeed = 1599.4f,
                     SpellNames = new[] { "HeimerdingerQTurretBigBlast" },
                     MissileNames = new[] { "HeimerdingerTurretBigEnergyBlast" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Heimerdinger" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1350,
                     Speed = 750,
                     Width = 40,
                     MissileAccel = 4000,
                     MissileMinSpeed = 750,
                     MissileMaxSpeed = 3000,
                     SpellNames = new[] { "Heimerdingerwm" },
                     MissileNames = new[] { "HeimerdingerWAttack2" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     DetectByMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Heimerdinger" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 3,
                     CastDelay = 0,
                     Range = 1350,
                     Speed = 750,
                     Width = 40,
                     MissileAccel = 4000,
                     MissileMinSpeed = 750,
                     MissileMaxSpeed = 3000,
                     SpellNames = new[] { "Heimerdingerwm" },
                     MissileNames = new[] { "HeimerdingerWAttack2Ult" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     DetectByMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Heimerdinger" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 1200,
                     Width = 135,
                     SpellNames = new[] { "HeimerdingerE" },
                     MissileNames = new[] { "HeimerdingerESpell" },
                     Collisions = new []{ Collision.YasuoWall },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Heimerdinger" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 4,
                     CastDelay = 0,
                     Range = 1000,
                     Speed = 1200,
                     Width = 135,
                     SpellNames = new[] { "HeimerdingerEUlt", "HeimerdingerEUltBounce" },
                     MissileNames = new[] { "HeimerdingerESpell_ult", "HeimerdingerESpell_ult2", "HeimerdingerESpell_ult3" },
                     Collisions = new []{ Collision.YasuoWall },
                   },
				   
				#endregion Heimerdinger
				
				#region Illaoi
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Illaoi" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 750,
                     Range = 850,
                     Speed = int.MaxValue,
                     Width = 100,
                     SpellNames = new[] { "IllaoiQ" },
                     IsFixedRange = true,
                     StaticStart = true,
					 IsDangerous = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Illaoi" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 950,
                     Speed = 1900,
                     Width = 50,
                     SpellNames = new[] { "IllaoiE" },
                     MissileNames = new[] { "Illaoiemis" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Illaoi" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 3,
                     CastDelay = 850,
                     Range = 850,
                     Speed = int.MaxValue,
                     Width = 100,
                     DisplayName = "Illaoi Tentacles",
                     IsFixedRange = true,
                     StaticStart = true,
                     AllowDuplicates = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Illaoi" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 500,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 450,
                     SpellNames = new[] { "IllaoiR" },
                   },
				   
				#endregion Illaoi
				
				#region Irelia
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Irelia" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1200,
                     Speed = 1600,
                     Width = 120,
                     SpellNames = new[] { "IreliaTranscendentBlades" },
                     MissileNames = new[] { "IreliaTranscendentBlades" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Irelia
				
				#region Ivern
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ivern" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 1300,
                     Width = 65,
                     SpellNames = new[] { "IvernQ" },
                     MissileNames = new[] { "IvernQ" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     SticksToMissile = true,
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ivern" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 0,
                     Range = 800,
                     Speed = 1400,
                     Width = 80,
                     SpellNames = new[] { "Daisy R" },
                     MissileNames = new[] { "IvernRMissile" },
                     SticksToMissile = true,
                     DetectByMissile = true,
					 IsFixedRange = true
                   },
				   
				#endregion Ivern
				
				#region Janna
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Janna" },
                     Slots = new[] { SpellSlot.Q, (SpellSlot)45 },
                     DangerLevel = 3,
                     CastDelay = 0,
                     Range = 1700,
                     Speed = 900,
                     Width = 120,
                     SpellNames = new[] { "HowlingGale" },
                     MissileNames = new[] { "HowlingGaleSpell" },
                     Collisions = new []{ Collision.YasuoWall },
                     SticksToMissile = true,
                     DetectByMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Janna" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 2,
                     CastDelay = 10,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 725,
                     SpellNames = new[] { "ReapTheWhirlwind" },
					 ParticleNames = new[] { "Janna_Base_R_cas_" }
                   },
				   
				#endregion Janna
				
				#region JarvanIV
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "JarvanIV" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 425,
                     Range = 770,
                     Speed = int.MaxValue,
                     Width = 70,
                     SpellNames = new[] { "JarvanIVDragonStrike" },
                     IsFixedRange = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "JarvanIV" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 450,
                     Range = 910,
                     Speed = 2600,
                     Width = 120,
                     SpellNames = new[] { "JarvanIVEQ" },
                     SticksToCaster = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "JarvanIV" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 450,
                     Range = 850,
                     Speed = int.MaxValue,
                     Width = 175,
                     SpellNames = new[] { "JarvanIVDemacianStandard" },
                     MissileNames = new[] { "JarvanIVDemacianStandard" }
                   },
				   
				#endregion JarvanIV
				
				#region Jax
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Jax" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 125,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 300,
                     SpellNames = new[] { "JaxCounterStrike" },
                     RequireBuffs = new [] 
                         {
                             new RequireBuff("JaxEvasion", 1), 
                         },
                   },
				
				#endregion Jax
				
				#region Jayce
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Jayce" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 230,
                     Range = 1050,
                     Speed = 1450,
                     Width = 70,
                     SpellNames = new[] { "jayceshockblast" },
                     MissileNames = new[] { "JayceShockBlastMis" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 HasExplodingEnd = true,
					 ExplodeWidth = 200,
					 Explodetype = Type.CircleMissile
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Jayce" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 0,
                     Range = 2000,
                     Speed = 2350,
                     Width = 70,
                     DisplayName = "Jayce E Q",
                     MissileNames = new[] { "JayceShockBlastWallMis" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     SticksToMissile = true,
					 HasExplodingEnd = true,
					 ExplodeWidth = 250,
					 Explodetype = Type.CircleMissile
                   },
				   
				#endregion Jayce
				
				#region Jhin
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Jhin" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 3,
                     CastDelay = 750,
                     Range = 2550,
                     Speed = int.MaxValue,
                     Width = 45,
                     SpellNames = new[] { "JhinW" },
                     MissileNames = new[] { "JhinWMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     IsFixedRange = true,
                     StaticStart = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Jhin" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 120000,
                     Range = 1500,
                     Speed = int.MaxValue,
                     Width = 150,
                     DisplayName = "Jhin E Trap",
					 TrapBuff = "JhinETrap",
					 TrapBaseSkinNames = new[] { "JhinTrap" },
                     AllowDuplicates = true,
                     DontCross = true,
					 IsTrap = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Jhin" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 3500,
                     Speed = 5000,
                     Width = 80,
                     SpellNames = new[] { "JhinRShotMis" },
                     MissileNames = new[] { "JhinRShotMis", "JhinRShotMis4" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Jhin
				
				#region Jinx
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Jinx" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 3,
                     CastDelay = 600,
                     Range = 1500,
                     Speed = 3300,
                     Width = 65,
                     SpellNames = new[] { "JinxW", "JinxWMissile" },
                     MissileNames = new[] { "JinxWMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Jinx" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 4500,
                     Range = 1500,
                     Speed = int.MaxValue,
                     Width = 90,
                     DisplayName = "Jinx E Placement",
					 ParticleNames = new[] { "Jinx_Base_E_Mine_Ready_" },
                     AllowDuplicates = true,
                     DontCross = true,
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Jinx" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 600,
                     Range = 4000,
                     Speed = 1700,
                     Width = 140,
                     SpellNames = new[] { "JinxR" },
                     MissileNames = new[] { "JinxR" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 CollideExplode = true,
					 ExplodeWidth = 375,
					 Explodetype = Type.CircleMissile
                   },
				   
				#endregion Jinx
				
				#region Kalista
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Kalista" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 350,
                     Range = 1200,
                     Speed = 2400,
                     Width = 70,
                     SpellNames = new[] { "KalistaMysticShot" },
                     MissileNames = new[] { "kalistamysticshotmistrue", "kalistamysticshotmis" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Kalista
				
				#region Karma
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Karma" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 905,
                     Speed = 1700,
                     Width = 70,
                     SpellNames = new[] { "KarmaQ" },
                     MissileNames = new[] { "KarmaQMissile", "KarmaQMissileMantra" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
                     CollideExplode = true,
                     ExplodeWidth = 235,
					 Explodetype = Type.CircleMissile,
					 IsDangerous = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Karma" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 1500,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 235,
                     DisplayName = "Karma Q Explode",
                     MissileNames = new[] { "KarmaQMissileMantra" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
					 AddEndExplode = true,
                   },
				   
				#endregion Karma
				
				#region Karthus
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Karthus" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 850,
                     Range = 875,
                     Speed = int.MaxValue,
                     Width = 160,
                     SpellNames = new[] { "KarthusLayWasteA1", "KarthusLayWasteA2", "KarthusLayWasteA3", "KarthusLayWasteDeadA1", "KarthusLayWasteDeadA2", "KarthusLayWasteDeadA3" },
                   },
				   
				#endregion Karthus
				
				#region Kassadin
				
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Kassadin" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 400,
                     Range = 600,
                     Speed = int.MaxValue,
                     Width = 0,
                     Angle = 70,
                     SpellNames = new[] { "ForcePulse" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     StaticStart = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Kassadin" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 500,
                     Speed = int.MaxValue,
                     Width = 270,
                     SpellNames = new[] { "RiftWalk" },
                     Collisions = new []{ Collision.YasuoWall },
                     StaticStart = true
                   },
				   
				#endregion Kassadin
				
				#region Kennen
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Kennen" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 175,
                     Range = 1050,
                     Speed = 1700,
                     Width = 50,
                     SpellNames = new[] { "KennenShurikenHurlMissile1" },
                     MissileNames = new[] { "KennenShurikenHurlMissile1" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 IsDangerous = true
                   },
				   
				#endregion Kennen
				
				#region Khazix
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Khazix" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1025,
                     Speed = 1700,
                     Width = 70,
                     SpellNames = new[] { "KhazixW", "khazixwlong" },
                     MissileNames = new[] { "KhazixWMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 AllowDuplicates = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Khazix" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 700,
                     Speed = 1250,
                     Width = 300,
                     SpellNames = new[] { "KhazixE" },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Khazix" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 900,
                     Speed = 1250,
                     Width = 300,
                     SpellNames = new[] { "KhazixELong" },
                   },
				   
				#endregion Khazix
				
				#region Kled
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Kled" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 800,
                     Speed = 1600,
                     Width = 45,
                     SpellNames = new[] { "KledQ" },
                     MissileNames = new[] { "KledQMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Kled" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 700,
                     Speed = 3000,
                     Width = 40,
                     SpellNames = new[] { "KledRiderQ" },
                     MissileNames = new[] { "KledRiderQMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Kled" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 550,
                     Speed = 970,
                     Width = 100,
                     SpellNames = new[] { "KledEDash" },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Kled
				   
				#region KogMaw
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "KogMaw" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 1200,
                     Speed = 1650,
                     Width = 70,
                     SpellNames = new[] { "KogMawQ" },
                     MissileNames = new[] { "KogMawQ" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "KogMaw" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1300,
                     Speed = 1400,
                     Width = 120,
                     SpellNames = new[] { "KogMawVoidOoze" },
                     MissileNames = new[] { "KogMawVoidOozeMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "KogMaw" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 1150,
                     Range = 5500,
                     Speed = int.MaxValue,
                     Width = 240,
                     SpellNames = new[] { "KogMawLivingArtillery" },
                   },
				   
				#endregion KogMaw
				
				#region Leblanc
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Leblanc" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 550,
                     Speed = 1600,
                     Width = 220,
                     SpellNames = new[] { "LeblancW" },
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Leblanc" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 950,
                     Speed = 1750,
                     Width = 55,
                     SpellNames = new[] { "LeblancE" },
                     MissileNames = new[] { "LeblancEMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Leblanc
				
				#region LeeSin
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "LeeSin" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1150,
                     Speed = 1800,
                     Width = 75,
					 DisplayName = "Q1",
                     SpellNames = new[] { "BlindMonkQOne" },
                     MissileNames = new[] { "BlindMonkQOne" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "LeeSin" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 430,
                     SpellNames = new[] { "BlindMonkEOne" },
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "LeeSin" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 700,
                     Speed = 1500,
                     Width = 100,
                     SpellNames = new[] { "BlindMonkRKick" },
                     IsFixedRange = true,
                     StartsFromTarget = true
                   },
				   
				#endregion LeeSin
				
				#region Leona
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Leona" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 200,
                     Range = 900,
                     Speed = 2000,
                     Width = 70,
                     SpellNames = new[] { "LeonaZenithBlade" },
                     MissileNames = new[] { "LeonaZenithBladeMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true,
                     TakeClosestPath = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Leona" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 1000,
                     Range = 1200,
                     Speed = int.MaxValue,
                     Width = 300,
                     SpellNames = new[] { "LeonaSolarFlare" },
                     ParticleNames = new[] { "Leona_Base_R_hit_aoe_" }
                   },
				   
				#endregion Leona
				
				#region Lissandra
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Lissandra" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 700,
                     Speed = 2200,
                     Width = 75,
                     SpellNames = new[] { "LissandraQ" },
                     MissileNames = new[] { "LissandraQMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Minions, Collision.Heros },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Lissandra" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 450,
                     Speed = 2200,
                     Width = 90,
                     SpellNames = new[] { "LissandraQShards" },
                     MissileNames = new[] { "LissandraQShards" },
                     Collisions = new []{ Collision.YasuoWall },
                     SticksToMissile = true,
					 AllowDuplicates = true,
					 ExtraRange = 100
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Lissandra" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 1,
                     CastDelay = 10,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 450,
                     SpellNames = new[] { "LissandraW" },
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Lissandra" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1025,
                     Speed = 850,
                     Width = 125,
                     SpellNames = new[] { "LissandraE" },
                     MissileNames = new[] { "LissandraEMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Lissandra" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 475,
                     Speed = int.MaxValue,
                     Width = 500,
                     SpellNames = new[] { "LissandraR" },
					 StartsFromTarget = true
                   },
				   
				#endregion Lissandra
				
				#region Lucian
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Lucian" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 300,
                     Range = 900,
                     Speed = int.MaxValue,
                     Width = 65,
                     SpellNames = new[] { "LucianQ" },
                     IsFixedRange = true,
					 StaticStart = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Lucian" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 900,
                     Speed = 1600,
                     Width = 55,
                     SpellNames = new[] { "LucianW" },
                     MissileNames = new[] { "lucianwmissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     SticksToMissile = true,
					 HasExplodingEnd = true,
					 ExplodeWidth = 200,
					 Explodetype = Type.CircleMissile
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Lucian" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1200,
                     Speed = 2800,
                     Width = 110,
                     SpellNames = new[] { "LucianR", "LucianRMis" },
                     MissileNames = new[] { "lucianrmissileoffhand", "lucianrmissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Lucian
				
				#region Lulu
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Lulu" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 950,
                     Speed = 1450,
                     Width = 80,
                     SpellNames = new[] { "LuluQ", "LuluQPix" },
                     MissileNames = new[] { "LuluQMissile", "LuluQMissileTwo" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },                   
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Lulu" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 1450,
                     Speed = int.MaxValue,
                     Width = 375,
                     SpellNames = new[] { "LuluR" },
					 StartsFromTarget = true
                   },
				   
				#endregion Lulu
				
				#region Lux
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Lux" },
                     Slots = new[] { SpellSlot.Q },
                     CollideCount = 1,
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1300,
                     Speed = 1200,
                     Width = 70,
                     SpellNames = new[] { "LuxLightBinding" },
                     MissileNames = new[] { "LuxLightBindingMis" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Lux" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 1300,
                     Width = 315,
                     SpellNames = new[] { "LuxLightStrikeKugel" },
                     MissileNames = new[] { "LuxLightStrikeKugel" },
                     Collisions = new []{ Collision.YasuoWall },
                     DontCross = true,
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Lux" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1100,
                     Speed = int.MaxValue,
                     Width = 315,
					 ExtraDuration = 5000,
                     DisplayName = "Lux E Placement",
                     ParticleNames = new[] { "Lux_Base_E_tar_aoe_" },
                     DontCross = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Lux" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 1000,
                     Range = 3500,
                     Speed = int.MaxValue,
                     Width = 150,
                     SpellNames = new[] { "LuxMaliceCannon" },
                     StartParticalName = "Lux_Base_R_cas",
                     MidParticalName = "Lux_Base_R_mis_beam_middle",
                     EndParticalName = "Lux_Base_R_mis_beam",
                     IsFixedRange = true,
                     StaticStart = true
                   },
				   
				#endregion Lux
				
				#region Warwick
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Warwick" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 0,
                     Range = 0,
                     Speed = 1600,
                     Width = 100,
                     MoveSpeedScaleMod = 2.5f,
                     SpellNames = new[] { "WarwickR" },
                     FastEvade = true,
                     SticksToCaster = true,
                     RangeScaleWithMoveSpeed = true,
                     IsFixedRange = true,
                     Collisions = new []{ Collision.Heros },
					 RemoveOnBuffLose = "InfiniteDuress"
                   },
				   
				#endregion Warwick
				
				#region Malphite
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Malphite" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 400,
                     SpellNames = new[] { "Landslide" },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Malphite" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 0,
                     Range = 1000,
                     Speed = 1600,
                     Width = 270,
                     SpellNames = new[] { "UFSlash" },
                   },
				   
				#endregion Malphite
				
				#region MonkeyKing
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "MonkeyKing" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 50,
                     ExtraDuration = 3950,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 320,
                     SpellNames = new[] { "MonkeyKingSpinToWin" },
                     EndSticksToCaster = true,
                     FastEvade = true,
					 RemoveOnBuffLose = "MonkeyKingSpinToWin"
                   },
				   
				#endregion MonkeyKing
				
				#region Malzahar
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Malzahar" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 750,
                     Speed = 1600,
                     Width = 90,
                     SpellNames = new[] { "MalzaharQ" },
                     MissileNames = new[] { "MalzaharQMissile" },
                     StaticStart = true,
                     StaticEnd = true
                   },
				   
				#endregion Malzahar
				   
				#region Maokai
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Maokai" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 650,
                     Speed = 1600,
                     Width = 110,
                     SpellNames = new[] { "MaokaiQ" },
                     MissileNames = new[] { "MaokaiQMissile" },
                     IsFixedRange = true,
					 IsDangerous = true
                   },
				   
				#endregion Maokai
				
				#region MissFortune
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "MissFortune" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 1,
                     Range = 1450,
                     Speed = 2000,
                     Width = 60,
                     SpellNames = new[] { "MissFortuneBulletTime" },
                     MissileNames = new[] { "MissFortuneBullets", "MissFortuneBulletEMPTY" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 DetectByMissile = true,
					 DetectAsOne = true
                   },
				   
				#endregion MissFortune
				
				#region Mordekaiser
				
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Mordekaiser" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 640,
                     Speed = int.MaxValue,
                     Width = 0,
                     Angle = 50,
                     SpellNames = new[] { "MordekaiserSyphonOfDestruction" },
                     IsFixedRange = true,
                     StaticStart = true
                   },
				   
				#endregion Mordekaiser
				
				#region Morgana
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Morgana" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 1300,
                     Speed = 1150,
                     Width = 80,
                     SpellNames = new[] { "DarkBindingMissile" },
                     MissileNames = new[] { "DarkBindingMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Morgana" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 1,
                     CastDelay = 0,
                     ExtraDuration = 5000,
                     Range = 900,
                     Speed = int.MaxValue,
                     Width = 250,
                     SpellNames = new[] { "TormentedSoil" },
					 ParticleNames = new[] { "Morgana_Base_W_Tar_" }
                   },
				   
				#endregion Morgana
				
				#region Nami
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Nami" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 4,
                     CastDelay = 700,
                     Range = 850,
                     Speed = 2500,
                     Width = 200,
                     SpellNames = new[] { "NamiQ" },
                     MissileNames = new[] { "NamiQMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Nami" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 500,
                     Range = 2750,
                     Speed = 850,
                     Width = 260,
                     SpellNames = new[] { "NamiR" },
                     MissileNames = new[] { "NamiRMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Nami
				
				#region Nautilus
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Nautilus" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1150,
                     Speed = 2000,
                     Width = 90,
                     SpellNames = new[] { "NautilusAnchorDrag" },
                     MissileNames = new[] { "NautilusAnchorDragMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions, Collision.Walls },
                     IsFixedRange = true,
                     SticksToMissile = true,
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Nautilus" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 500,
                     Range = int.MaxValue,
                     Speed = 600,
                     Width = 140,
                     SpellNames = new[] { "NautilusGrandLine" },
					 EndSticksToTarget = true
                   },
				   
				#endregion Nautilus
				
				#region Nasus
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Nasus" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 650,
                     Speed = int.MaxValue,
                     Width = 400,
                     SpellNames = new[] { "NasusE" },
                   },
				   
				#endregion Nasus
				
				#region Nidalee
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Nidalee" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1500,
                     Speed = 1300,
                     Width = 60,
                     SpellNames = new[] { "JavelinToss" },
                     MissileNames = new[] { "JavelinToss" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 IsDangerous = true
                   },
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Nidalee" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 300,
                     Speed = int.MaxValue,
                     Width = 0,
                     Angle = 180,
                     SpellNames = new[] { "Swipe" },
                     IsFixedRange = true,
                     StaticStart = true
                   },
				   
				#endregion Nidalee
				
				#region Nocturne
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Nocturne" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 1200,
                     Speed = 1400,
                     Width = 60,
                     SpellNames = new[] { "NocturneDuskbringer" },
                     MissileNames = new[] { "NocturneDuskbringer" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Nocturne
				
				#region Nunu
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Nunu" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 10,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 550,
                     SpellNames = new[] { "AbsoluteZero" },
                     ParticleNames = new[] { "Nunu_Base_R_indicator_" },
                     ExtraDuration = 3000,
					 RemoveOnBuffLose = "Absolute Zero"
                   },
				   
				#endregion Nunu
				
				#region Olaf
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Olaf" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1000,
                     ExtraRange = 150,
                     Speed = 1600,
                     Width = 90,
                     SpellNames = new[] { "OlafAxeThrowCast" },
                     MissileNames = new[] { "OlafAxeThrow" },
                     Collisions = new []{ Collision.YasuoWall },
                     SticksToMissile = true
                   },
				   
				#endregion Olaf
				
				#region Orianna
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Orianna" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 2000,
                     Speed = 1200,
                     Width = 80,
                     SpellNames = new[] { "OrianaIzunaCommand" },
                     MissileNames = new[] { "OrianaIzuna" },
                     Collisions = new []{ Collision.YasuoWall },
                     SticksToMissile = true,
					 HasExplodingEnd = true,
					 ExplodeWidth = 175,
					 Explodetype = Type.CircleMissile,
					 DetectByMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Orianna" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 1,
                     CastDelay = 10,
                     Range = 2000,
                     Speed = int.MaxValue,
                     Width = 255,
                     DisplayName = "Orianna W",
                     ParticleNames = new[] { "Orianna_Base_W_Dissonance_" }
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Orianna" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Range = 1500,
                     Speed = 1850,
                     Width = 85,
                     SpellNames = new[] { "OriannasE" },
                     MissileNames = new[] { "orianaredact" },
                     Collisions = new []{ Collision.YasuoWall },
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Orianna", "OriannaBall", "OriannaBall" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 500,
                     Range = 25000,
                     Speed = int.MaxValue,
                     Width = 400,
                     RequireBuffs = new []
                         {
                             new RequireBuff("orianaghost", 1),
                             new RequireBuff("orianaghostself", 1),
                         },
                     ParticalObjectName = "OriannaBall",
                     SpellNames = new[] { "OrianaDetonateCommand" },
					 ParticleNames = new[] { "Orianna_Base_R_VacuumIndicator", "Orianna_Base_R_VacuumIndicatorSelfRing" },
					 EndIsBuffHolderPosition = true
                   },
				   
				#endregion Orianna
				
				#region Pantheon
				
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Pantheon" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 400,
                     Range = 640,
                     Speed = int.MaxValue,
                     Width = 0,
                     Angle = 70,
                     ExtraDuration = 750,
                     SpellNames = new[] { "PantheonE" },
                     IsFixedRange = true,
                     StaticStart = true
                   },
               /*new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Pantheon" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 2,
                     CastDelay = 4700,
                     Range = 5500,
                     Speed = int.MaxValue,
                     Width = 600,
                     SpellNames = new[] { "PantheonRJump" },
                     ParticleNames = new[] { "Pantheon_Base_R_indicator_" },
                     StaticEnd = true
                   },*/
				   
				#endregion Pantheon
				   
				#region Poppy
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Poppy" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 1,
                     CastDelay = 500,
                     ExtraDuration = 900,
                     Range = 430,
                     Speed = int.MaxValue,
                     Width = 100,
                     SpellNames = new[] { "PoppyQ", "PoppyQSpell" },
                     MissileNames = new[] { "PoppyQ" },
                     IsFixedRange = true,
                     StaticStart = true,
                     DontAddExtraDuration = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Poppy" },
                     Slots = new[] { (SpellSlot)50 },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 450,
                     Speed = int.MaxValue,
                     Width = 100,
                     SpellNames = new[] { "PoppyRSpellInstant" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     StaticStart = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Poppy" },
                     Slots = new[] { (SpellSlot)50 },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1200,
                     Speed = 1600,
                     Width = 100,
                     SpellNames = new[] { "PoppyRSpell" },
                     MissileNames = new[] { "PoppyRMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     SticksToMissile = true
                   },
				   
				#endregion Poppy
				
				#region Quinn
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Quinn" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 300,
                     Range = 1050,
                     Speed = 1550,
                     Width = 60,
                     SpellNames = new[] { "QuinnQ" },
                     MissileNames = new[] { "QuinnQ" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 IsDangerous = true
                   },
				   
				#endregion Quinn
				
				
				#region Rakan
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Rakan" },
                     Slots = new[] { SpellSlot.Q, (SpellSlot)50 },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 900,
                     Speed = 1850,
                     Width = 70,
                     SpellNames = new[] { "RakanQ" },
                     MissileNames = new[] { "RakanQMis" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Rakan" },
                     Slots = new[] { SpellSlot.W, (SpellSlot)50 },
                     DangerLevel = 4,
                     CastDelay = 500,
                     Range = 650,
                     Speed = int.MaxValue,
                     Width = 250,
                     SpellNames = new[] { "RakanWCast" },
                   },
				
				#endregion Rakan
				
				#region RekSai
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "RekSai" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 1,
                     CastDelay = 125,
                     Range = 1500,
                     Speed = 1950,
                     Width = 65,
                     SpellNames = new[] { "reksaiqburrowed" },
                     MissileNames = new[] { "RekSaiQBurrowedMis" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "RekSai" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 3,
                     CastDelay = 10,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 200,
                     SpellNames = new[] { "RekSaiW2" },
                   },
				   
				#endregion RekSai
				
				#region Renekton
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Renekton" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 450,
                     Speed = 1100,
                     Width = 100,
                     SpellNames = new[] { "RenektonSliceAndDice", "RenektonDice" },
                     IsFixedRange = true
                   },
				   
				#endregion Renekton
				
				#region Rengar
				
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Rengar" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 300,
                     Range = 400,
                     Speed = int.MaxValue,
                     Width = 0,
                     Angle = 180,
                     SpellNames = new[] { "RengarQ" },
                     MissileNames = new[] { "RengarQ" },
                     IsFixedRange = true,
                     StaticStart = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Rengar" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 600,
                     Speed = int.MaxValue,
                     Width = 80,
                     SpellNames = new[] { "RengarQ2" },
                     MissileNames = new[] { "RengarQ2" },
                     IsFixedRange = true,
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Rengar" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 1500,
                     Width = 70,
                     SpellNames = new[] { "RengarE", "RengarEEmp" },
                     MissileNames = new[] { "RengarEMis", "RengarEEmpMis" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Rengar
				
				#region Riven
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Riven" },
                     Slots = new[] { SpellSlot.Q, (SpellSlot)49 },
                     DangerLevel = 4,
                     CastDelay = 350,
                     Range = 375,
                     Speed = int.MaxValue,
                     Width = 200,
                     RequireBuffs = new []
                         {	
                             new RequireBuff("RivenTriCleave", 2),
                         },
                     SpellNames = new[] { "RivenTriCleave" },
                     EndIsCasterDirection = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Riven" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 4,
                     CastDelay = 10,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 250,
                     SpellNames = new[] { "RivenMartyr" },
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Riven" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1075,
                     Speed = 1600,
                     Width = 130,
                     SpellNames = new[] { "RivenIzunaBlade" },
                     MissileNames = new[] { "RivenLightsaberMissile", "RivenWindslashMissileCenter", "RivenWindslashMissileLeft", "RivenWindslashMissileRight" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 DetectByMissile = true
                   },
				   
				#endregion Riven
				
				#region Rumble
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Rumble" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 950,
                     Speed = 2000,
                     Width = 60,
                     SpellNames = new[] { "RumbleGrenade" },
                     MissileNames = new[] { "RumbleGrenadeMissile", "RumbleGrenadeMissileDangerZone" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Rumble" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 400,
                     Range = 1200,
                     Speed = 1600,
                     Width = 200,
                     SpellNames = new[] { "RumbleCarpetBombM" },
                     MissileNames = new[] { "RumbleCarpetBombMissile" },
                     IsFixedRange = true,
                     StaticStart = true,
					 DontRemoveWithMissile = true,
                     AllowDuplicates = true,
					 ExtraDuration = 4000
                   },
				   
				#endregion Rumble
				
				#region Ryze
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Ryze" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 1700,
                     Width = 55,
                     SpellNames = new[] { "RyzeQ" },
                     MissileNames = new[] { "RyzeQ" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Ryze
				
				#region Sejuani
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Sejuani" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 4,
                     CastDelay = 0,
                     Range = 620,
                     ExtraRange = 200,
                     Speed = 1000,
                     Width = 90,
                     SpellNames = new[] { "SejuaniQ" },
                     Collisions = new []{ Collision.Heros },
                     FastEvade = true,
                     SticksToCaster = true,
					 RemoveOnBuffLose = "SejuaniArcticAssault",
                   },
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Sejuani" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 650,
                     Speed = int.MaxValue,
                     Width = 70,
					 Angle = 70,
                     SpellNames = new[] { "SejuaniW" },
                     IsFixedRange = true,
                     SticksToCaster = true,
					 IsMoving = true,
					 EndStickToDirection = true,
					 AllowDuplicates = true,
					 OnDeleteAdd = new SkillshotData
								   {
									type = Type.LineMissile,
									CasterNames = new[] { "Sejuani" },
									Slots = new[] { SpellSlot.W },
									DangerLevel = 4,
									CastDelay = 500,
									Range = 650,
									Speed = int.MaxValue,
									Width = 70,
									SpellNames = new[] { "SejuaniW" },
									IsFixedRange = true,
									SticksToCaster = true,
									IsMoving = true,
									EndStickToDirection = true,
									AllowDuplicates = true,
									},
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Sejuani" },
                     Slots = new[] { SpellSlot.R, (SpellSlot)46 },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1300,
                     Speed = 1600,
                     Width = 110,
                     SpellNames = new[] { "SejuaniR" },
                     MissileNames = new[] { "SejuaniRMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     IsFixedRange = true,
                     SticksToMissile = true,
                     CollideExplode = true,
                     ExplodeWidth = 235,
					 Explodetype = Type.CircleMissile
                   },
				   
				#endregion Sejuani
				
				#region Shen
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Shen" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1,
                     Speed = 2500,
                     Width = 80,
                     MissileNames = new[] { "ShenQMissile" },
                     EndSticksToCaster = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Shen" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 4,
                     CastDelay = 0,
                     Range = 600,
                     Speed = 1300,
                     Width = 50,
                     SpellNames = new[] { "ShenE" },
                     MissileNames = new[] { "ShenE" },
                     SticksToCaster = true,
					 RemoveOnBuffLose = "shenedash"
                   },
				   
				#endregion Shen
				
				#region Shyvana
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Shyvana" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 950,
                     Speed = 1575,
                     Width = 60,
                     SpellNames = new[] { "ShyvanaFireball" },
                     MissileNames = new[] { "ShyvanaFireballMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Shyvana" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 350,
                     Range = 975,
                     Speed = 1575,
                     Width = 60,
                     SpellNames = new[] { "ShyvanaFireballDragon2" },
                     MissileNames = new[] { "ShyvanaFireballDragonMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     SticksToMissile = true,
					 HasExplodingEnd = true,
					 ExplodeWidth = 250,
					 Explodetype = Type.CircleMissile
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Shyvana" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 300,
                     Range = 950,
                     ExtraRange = 200,
                     Speed = 1100,
                     Width = 200,
                     SpellNames = new[] { "ShyvanaTransformCast" },
                     SticksToCaster = true
                   },
				   
				#endregion Shyvana
				
				#region Sion
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Sion" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 1200,
                     ExtraDuration = 800,
                     Range = 750,
                     Speed = int.MaxValue,
                     Width = 250,
                     SpellNames = new[] { "SionQ" },
                     MissileNames = new[] { "SionQHitParticleMissile2" },
                     IsFixedRange = true,
                     StaticStart = true,
					 RemoveOnBuffLose = "SionQ"
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Sion" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 1,
                     CastDelay = 50,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 325,
                     SpellNames = new[] { "SionWDetonate" },
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Sion" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 800,
                     Speed = 1800,
                     Width = 80,
                     SpellNames = new[] { "SionE" },
                     MissileNames = new[] { "SionEMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Sion" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 0,
                     Range = 1500,
                     Speed = 1000,
                     Width = 200,
                     SpellNames = new[] { "SionR" },
                     Collisions = new []{ Collision.Heros, Collision.Walls },
                     SticksToCaster = true,
                     FastEvade = true,
					 RemoveOnBuffLose = "SionR"
                   },
				   
				#endregion Sion
				
				#region Sivir
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Sivir" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1250,
                     Speed = 1350,
                     Width = 90,
                     SpellNames = new[] { "SivirQ" },
                     MissileNames = new[] { "SivirQMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Sivir" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 20000,
                     Speed = 1350,
                     Width = 90,
                     SpellNames = new[] { "SivirQReturn" },
                     MissileNames = new[] { "SivirQMissileReturn" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = false,
                     EndSticksToCaster = true,
                     SticksToMissile = true
                   },
				   
				#endregion Sivir
				
				#region Skarner
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Skarner" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 1500,
                     Width = 70,
                     SpellNames = new[] { "SkarnerFracture" },
                     MissileNames = new[] { "SkarnerFractureMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 IsDangerous = true
                   },
				   
				#endregion Skarner
				
				#region Sona
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Sona" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 2400,
                     Width = 140,
                     SpellNames = new[] { "SonaR" },
                     MissileNames = new[] { "SonaR" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Sona
				
				#region Soraka
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Soraka" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 800,
                     Speed = 1100,
                     Width = 225,
                     SpellNames = new[] { "SorakaQ" },
                     MissileNames = new[] { "SorakaQMissile" },
                     ParticleNames = new[] { "soraka_base_q_indicator_" },
                     Collisions = new []{ Collision.YasuoWall },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Soraka" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 1770,
                     Range = 920,
                     Speed = int.MaxValue,
                     Width = 250,
                     SpellNames = new[] { "SorakaE" },
                   },
				   
				#endregion Soraka
				
				#region Swain
				
              new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Swain" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 3,
                     CastDelay = 1100,
                     Range = 900,
                     Speed = int.MaxValue,
                     Width = 225,
                     SpellNames = new[] { "SwainShadowGrasp" },
					 ParticleNames = new[] { "Swain_shadowGrasp_warning_" }
                   },
				   
				#endregion Swain
				
				#region Syndra
				
              new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Syndra" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 650,
                     Range = 825,
                     Speed = int.MaxValue,
                     Width = 180,
                     SpellNames = new[] { "SyndraQ" },
                     MissileNames = new[] { "SyndraQSpell" },
					 IsDangerous = true
                   },
              new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Syndra" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 950,
                     Speed = 1500,
                     Width = 210,
                     SpellNames = new[] { "SyndraWCast" },
                   },
              new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Syndra" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 200,
                     Range = 800,
                     Speed = 2500,
                     Width = 0,
					 Angle = 45,
                     SpellNames = new[] { "SyndraE", "SyndraE5" },
                     MissileNames = new[] { "SyndraEMissile", "SyndraEMissile2", "SyndraEMissile3" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
					 StaticStart = true
                   },
              new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Syndra" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 0,
                     Range = 1150,
                     Speed = 2500,
                     Width = 100,
                     SpellNames = new[] { "SyndraEQ" },
                     MissileNames = new[] { "SyndraESphereMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     SticksToMissile = true,
                   },
				   
				#endregion Syndra
				
				#region TahmKench
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "TahmKench" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 850,
                     Speed = 2800,
                     Width = 90,
                     SpellNames = new[] { "TahmKenchQ" },
                     MissileNames = new[] { "tahmkenchqmissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion TahmKench
				
				#region Taliyah
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Taliyah" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 3600,
                     Width = 100,
                     SpellNames = new[] { "TaliyahQ" },
                     MissileNames = new[] { "TaliyahQMis" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Taliyah" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 4,
                     CastDelay = 750,
                     Range = 900,
                     Speed = int.MaxValue,
                     Width = 200,
                     SpellNames = new[] { "TaliyahWVC" },
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Taliyah" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 1000,
                     Range = 25000,
                     Speed = 1700,
                     Width = 200,
                     SpellNames = new[] { "TaliyahR" },
                     MissileNames = new[] { "TaliyahRMis" }
                   },
				   
				#endregion Taliyah
				
				#region Talon
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Talon" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 850,
                     Speed = 2500,
                     Width = 80,
                     Angle = 20,
                     SpellNames = new[] { "TalonW" },
                     MissileNames = new[] { "TalonWMissileOne" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true,
                     DetectByMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Talon" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 2500,
                     Speed = 3000,
                     Width = 80,
                     Angle = 20,
                     SpellNames = new[] { "TalonWReturn" },
                     MissileNames = new[] { "TalonWMissileTwo" },
                     Collisions = new []{ Collision.YasuoWall },
                     EndSticksToCaster = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Talon" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1500,
                     Speed = 2400,
                     Width = 100,
                     SpellNames = new[] { "TalonR" },
                     MissileNames = new[] { "TalonRMisOne" },
                     Collisions = new []{ Collision.YasuoWall },
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Talon" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 2500,
                     Speed = 4000,
                     Width = 100,
                     SpellNames = new[] { "TalonRReturn" },
                     MissileNames = new[] { "TalonRMisTwo" },
                     Collisions = new []{ Collision.YasuoWall },
                     EndSticksToCaster = true,
                     SticksToMissile = true
                   },
				   
				#endregion Talon
				
				#region Taric
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Taric" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 4,
                     CastDelay = 1000,
                     Range = 650,
                     Speed = int.MaxValue,
                     Width = 100,
                     SpellNames = new[] { "TaricE" },
                     IsFixedRange = true,
                     SticksToCaster = true,
					 IsMoving = true,
					 EndStickToDirection = true
                   },
				   
				#endregion Taric
				
				#region Teemo
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Teemo" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 2,
                     CastDelay = 300000,
                     Range = 1500,
                     Speed = int.MaxValue,
                     Width = 150,
                     DisplayName = "Teemo R Trap",
					 TrapBuff = "BantamTrap",
					 TrapBaseSkinNames = new[] { "TeemoMushroom" },
                     AllowDuplicates = true,
                     DontCross = true,
					 IsTrap = true
                   },
				   
				#endregion Teemo
				
				#region Thresh
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Thresh" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 4,
                     CastDelay = 500,
                     Range = 1100,
                     Speed = 1900,
                     Width = 65,
                     SpellNames = new[] { "ThreshQ" },
                     MissileNames = new[] { "ThreshQMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Thresh" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 4,
                     CastDelay = 125,
                     Range = 1075,
                     Speed = 2000,
                     Width = 110,
                     SpellNames = new[] { "ThreshE" },
                     MissileNames = new[] { "ThreshEMissile1" },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.Ring,
                     CasterNames = new[] { "Thresh" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 450,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 55,
                     RingRadius = 350,
                     SpellNames = new[] { "ThreshRPenta" },
					 ParticleNames = new[] { "Thresh_Base_R_warning_" }
                   },
				   
				#endregion Thresh
				
				#region Tristana
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Tristana" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 300,
                     Range = 900,
                     Speed = 1100,
                     Width = 270,
                     SpellNames = new[] { "RocketJump", "TristanaW" },
                     MissileNames = new[] { "RocketJump" },
                   },
				   
				#endregion Tristana
				
				#region Trundle
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Trundle" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = int.MaxValue,
                     Width = 125,
                     SpellNames = new[] { "TrundleCircle" },
                   },
				   
				#endregion Trundle
				
				#region Tryndamere
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Tryndamere" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Range = 650,
                     Speed = 900,
                     Width = 160,
                     SpellNames = new[] { "TryndamereE" },
                     SticksToCaster = true
                   },
				   
				#endregion Tryndamere
				
				#region TwistedFate
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "TwistedFate" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1450,
                     Speed = 1000,
                     Width = 40,
                     SpellNames = new[] { "WildCards" },
                     MissileNames = new[] { "SealFateMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true,
                     DetectByMissile = true
                   },
				   
				#endregion TwistedFate
				
				#region Twitch
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Twitch" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 900,
                     Speed = 1400,
                     Width = 275,
                     SpellNames = new[] { "TwitchVenomCask" },
                     MissileNames = new[] { "TwitchVenomCaskMissile" },
                     Collisions = new []{ Collision.YasuoWall },
					 DontRemoveWithMissile = true,
					 DontAddExtraDuration = true,
                     ExtraDuration = 2850,
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Twitch" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1100,
                     Speed = 4000,
                     Width = 60,
                     SpellNames = new[] { "TwitchSprayAndPrayAttack" },
                     MissileNames = new[] { "TwitchSprayAndPrayAttack" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
					 IsAutoAttack = true
                   },
				   
				#endregion Twitch
				
				#region Urgot
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Urgot" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 1,
                     CastDelay = 150,
                     Range = 1000,
                     Speed = 1600,
                     Width = 60,
                     SpellNames = new[] { "UrgotHeatseekingLineMissile" },
                     MissileNames = new[] { "UrgotHeatseekingLineMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Urgot" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 900,
                     Speed = 1500,
                     Width = 210,
                     SpellNames = new[] { "UrgotPlasmaGrenade" },
                     MissileNames = new[] { "UrgotPlasmaGrenadeBoom" },
                     Collisions = new []{ Collision.YasuoWall },
                   },
				   
				#endregion Urgot
				
				#region Varus
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Varus" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1800,
                     Speed = 1900,
                     Width = 70,
                     ExtraRange = 75,
                     SpellNames = new[] { "VarusQ" },
                     MissileNames = new[] { "VarusQMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     DetectByMissile = true,
                     SticksToMissile = true,
					 IsDangerous = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Varus" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 925,
                     Speed = 1500,
                     Width = 235,
                     SpellNames = new[] { "VarusE" },
                     MissileNames = new[] { "VarusEMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Varus" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Range = 1250,
                     Speed = 1950,
                     Width = 120,
                     SpellNames = new[] { "VarusR" },
                     MissileNames = new[] { "VarusRMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Varus
				
				#region Veigar
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Veigar" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     CollideCount = 1,
                     Range = 950,
                     Speed = 2200,
                     Width = 70,
                     SpellNames = new[] { "VeigarBalefulStrike" },
                     MissileNames = new[] { "VeigarBalefulStrikeMis" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Veigar" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 1250,
                     Range = 900,
                     Speed = int.MaxValue,
                     Width = 200,
                     SpellNames = new[] { "VeigarDarkMatter" },
                     MissileNames = new[] { "VeigarDarkMatter" },
                     ParticleNames = new[] { "Veigar_Base_W_cas_" }
                   },
               new SkillshotData
                   {
                     type = Type.Ring,
                     CasterNames = new[] { "Veigar" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 3,
                     CastDelay = 700,
                     Range = 700,
                     Speed = int.MaxValue,
                     Width = 80,
                     RingRadius = 350,
                     ExtraDuration = 3100,
                     SpellNames = new[] { "VeigarEventHorizon" },
                     DontAddExtraDuration = true,
                     DontCross = true,
                     ParticleNames = new[] { "Veigar_Base_E_Warning_" }
                   },
				   
				#endregion Veigar
				
				#region Velkoz
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Velkoz" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 251,
                     Range = 1100,
                     Speed = 1300,
                     Width = 50,
                     SpellNames = new[] { "VelkozQ" },
                     MissileNames = new[] { "VelkozQMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Velkoz" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 1100,
                     Speed = 2100,
                     Width = 50,
                     SpellNames = new[] { "VelkozQSplit" },
                     MissileNames = new[] { "VelkozQMissileSplit" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Velkoz" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1200,
                     Speed = 1700,
                     Width = 88,
                     SpellNames = new[] { "VelkozW" },
                     MissileNames = new[] { "VelkozWMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
					 StaticStart = true,
					 DontAddExtraDuration = true,
					 DontRemoveWithMissile = true,
					 ExtraDuration = 750
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Velkoz" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 800,
                     Speed = 1500,
                     Width = 225,
                     SpellNames = new[] { "VelkozE" },
                     MissileNames = new[] { "VelkozEMissile" },
					 ParticleNames = new[] { "Velkoz_Base_E_AOE_" }
                   },
				   
				#endregion Velkoz
				
				#region Vi
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Vi" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 750,
                     ExtraRange = 110,
                     Speed = 1500,
                     Width = 95,
                     SpellNames = new[] { "ViQ" },
                     MissileNames = new[] { "ViQMissile" },
                     Collisions = new []{ Collision.Heros },
                     SticksToCaster = true,
                     DetectByMissile = true,
					 RemoveOnBuffLose = "ViQDash"
                   },
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Vi" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 800,
                     Speed = 2250,
                     Width = 0,
					 Angle = 65,
                     SpellNames = new[] { "ViE" },
                     MissileNames = new[] { "ViEFx" },
                     IsFixedRange = true,
                     DetectByMissile = true,
					 StaticStart = true
                   },
				   
				#endregion Vi
				
				#region Viktor
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Viktor" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 710,
                     Speed = 1050,
                     Width = 80,
                     DisplayName = "Laser",
                     SpellNames = new[] { "ViktorDeathRay" },
                     MissileNames = new[] { "ViktorDeathRayMissile", "ViktorEAugMissile", "ViktorDeathRayMissile2" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Viktor" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 1,
                     CastDelay = 1500,
                     Range = 700,
                     Speed = int.MaxValue,
                     Width = 300,
                     SpellNames = new[] { "ViktorGravitonField" },
					 ExtraDuration = 2500,
					 DontAddExtraDuration = true
                   },
				   
				#endregion Viktor
				
				#region Vladimir
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Vladimir" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 550,
                     Speed = 4000,
                     Width = 60,
                     SpellNames = new[] { "VladimirE" },
                     MissileNames = new[] { "VladimirEMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true,
                     DetectByMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Vladimir" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 10,
                     Range = 700,
                     Speed = int.MaxValue,
                     Width = 375,
                     SpellNames = new[] { "VladimirHemoplague" },
                   },
				   
				#endregion Vladimir
				
				#region Xayah
				/*
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Xayah" },
                     Slots = new[] { (SpellSlot)45, },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 4000,
                     Width = 60,
					 DisplayName = "Feathers",
                     SpellNames = new[] { "XayahPassiveAttack" },
                     MissileNames = new[] { "XayahPassiveAttack" },
                     Collisions = new []{ Collision.YasuoWall, },
					 SticksToMissile = true,
					 IsFixedRange = true,
					 IsAutoAttack = true,
					 AllowDuplicates = true
                   },*/
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Xayah" },
                     Slots = new[] { SpellSlot.Q, (SpellSlot)46, (SpellSlot)47 },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 3300,
                     Width = 60,
                     SpellNames = new[] { "XayahQ" },
                     MissileNames = new[] { "XayahQMissile1", "XayahQMissile2" },
                     Collisions = new []{ Collision.YasuoWall, },
					 SticksToMissile = true,
					 IsFixedRange = true,
					 AllowDuplicates = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Xayah" },
                     Slots = new[] { SpellSlot.E, (SpellSlot)48, },
                     DangerLevel = 3,
                     CastDelay = 0,
                     Range = 1100,
                     Speed = 4000,
                     Width = 60,
					 DisplayName = "Xayah E",
                     SpellNames = new[] { "Xayah E" },
                     MissileNames = new[] { "XayahEMissile" },
                     Collisions = new []{ Collision.YasuoWall, },
					 SticksToMissile = true,
					 EndSticksToCaster = true,
					 AllowDuplicates = true
                   },
               new SkillshotData
                   {
                     type = Type.Cone,
                     CasterNames = new[] { "Xayah" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 1000,
                     Range = 1100,
                     Speed = int.MaxValue,
                     Angle = 35,
					 DisplayName = "Xayah R",
                     SpellNames = new[] { "XayahR" },
                     Collisions = new []{ Collision.YasuoWall, },
                     SticksToCaster = true,
					 IsMoving = true
                   },
				   
				#endregion Xayah
				
				#region Xerath
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Xerath" },
                     Slots = new[] { (SpellSlot)45 },
                     DangerLevel = 2,
                     CastDelay = 530,
                     Range = 1600,
                     Speed = int.MaxValue,
                     Width = 100,
                     SpellNames = new[] { "XerathArcanopulse2" },
                     MissileNames = new[] { "XerathArcanopulse2" },
					 //MinionName = "hiu",
					 //MinionBaseSkinName = "TestCubeRender10Vision",
                     StaticStart = true,
					 IsDangerous = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Xerath" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 2,
                     CastDelay = 700,
                     Range = 1100,
                     Speed = int.MaxValue,
                     Width = 260,
                     SpellNames = new[] { "XerathArcaneBarrage2" },
                     ParticleNames = new[] { "Xerath_Base_W_aoe_" }
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Xerath" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 4,
                     CastDelay = 250,
                     Range = 1100,
                     Speed = 1400,
                     Width = 60,
                     SpellNames = new[] { "XerathMageSpear" },
                     MissileNames = new[] { "XerathMageSpearMissile" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Xerath" },
                     Slots = new[] { SpellSlot.R, (SpellSlot)48 },
                     DangerLevel = 3,
                     CastDelay = 650,
                     Range = 25000,
                     Speed = int.MaxValue,
                     Width = 200,
                     SpellNames = new[] { "XerathRMissileWrapper", "XerathLocusPulse" },
                     MissileNames = new[] { "XerathLocusPulse" },
                     ParticleNames = new[] { "Xerath_Base_R_aoe_reticle_" },
					 DontRemoveWithMissile = true
                   },
				   
				#endregion Xerath
				
				#region XinZhao
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "XinZhao" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 347,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 500,
                     SpellNames = new[] { "XenZhaoParry" },
                     StaticStart = true
                   },
				   
				#endregion XinZhao
				
				#region Yasuo
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Yasuo" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 400,
                     Range = 550,
                     Speed = int.MaxValue,
                     Width = 40,
                     SpellNames = new[] { "YasuoQW", "YasuoQ2W" },
                     IsFixedRange = true,
                     StaticStart = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Yasuo" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 400,
                     Range = 440,
                     Speed = int.MaxValue,
                     Width = 275,
                     DisplayName = "E Q",
                     ParticleNames = new[] { "Yasuo_Base_EQ_SwordGlow" },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Yasuo" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 400,
                     Range = 440,
                     Speed = int.MaxValue,
                     Width = 275,
                     DisplayName = "E Q3",
                     ParticleNames = new[] { "Yasuo_Base_EQ_SwordGlow" },
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Yasuo" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 3,
                     CastDelay = 250,
                     Range = 1150,
                     Speed = 1000,
                     Width = 110,
                     SpellNames = new[] { "yasuoq3w" },
                     MissileNames = new[] { "yasuoq3mis" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true
                   },
				   
				#endregion Yasuo
				
				#region Yorick
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Yorick" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 700,
                     Speed = 1800,
                     Width = 100,
                     SpellNames = new[] { "YorickE" },
                     MissileNames = new[] { "YorickEMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                   },
               new SkillshotData
                   {
                     type = Type.Ring,
                     CasterNames = new[] { "Yorick" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 3,
                     CastDelay = 750,
                     Range = 700,
                     Speed = int.MaxValue,
                     Width = 40,
					 RingRadius = 210,
                     SpellNames = new[] { "YorickW" },
                   },
				   
				#endregion Yorick
				
				#region Zac
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Zac" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 850,
                     Speed = 2800,
                     Width = 120,
                     SpellNames = new[] { "ZacQ" },
                     MissileNames = new[] { "ZacQMissile" },
                     IsFixedRange = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Zac" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 2500,
                     Speed = 1500,
                     Width = 250,
                     SpellNames = new[] { "ZacE2" },
                     ParticleNames = new[] { "Zac_Base_E_Tar_" },
					 DontRemoveWithMissile = true,
					 RemoveOnBuffLose = "zacemove",
					 DontHaveBuff = "ZacR",
                     RequireBuffs = new []
                         {	
                             new RequireBuff("zacemove", 1),
                         },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Zac" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 0,
                     Range = 800,
                     Speed = 1800,
                     Width = 300,
                     ParticleNames = new[] { "Zac_Base_E_Tar_" },
					 DisplayName = "R Slam"
                   },
				   
				#endregion Zac
								
				#region Zed
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Zed" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 925,
                     Speed = 1700,
                     Width = 50,
                     SpellNames = new[] { "ZedQ" },
                     MissileNames = new[] { "ZedQMissile" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 AllowDuplicates = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Zed" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 10,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 285,
                     SpellNames = new[] { "ZedE" },
                   },
				   
				#endregion Zed
					
				#region Ziggs
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Ziggs" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 850,
                     Speed = 1700,
                     Width = 125,
                     SpellNames = new[] { "ZiggsQ","ZiggsQBounce1", "ZiggsQBounce2" },
                     MissileNames = new[] { "ZiggsQSpell", "ZiggsQSpell2", "ZiggsQSpell3" },
                     Collisions = new []{ Collision.YasuoWall, Collision.Heros, Collision.Minions },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Ziggs" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 1000,
                     Speed = 1750,
                     Width = 275,
                     SpellNames = new[] { "ZiggsW" },
                     MissileNames = new[] { "ZiggsW" },
                     Collisions = new []{ Collision.YasuoWall },
                     DontCross = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Ziggs" },
                     Slots = new[] { SpellSlot.W },
                     DangerLevel = 1,
                     CastDelay = 4000,
                     Range = 1000,
                     Speed = int.MaxValue,
                     Width = 275,
                     DisplayName = "Ziggs W Placement",
                     ParticleNames = new[] { "Ziggs_Base_W_aoe_" },
                     DontCross = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Ziggs" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Range = 900,
                     Speed = 1550,
                     Width = 235,
                     SpellNames = new[] { "ZiggsE" },
                     MissileNames = new[] { "ZiggsE2" },
                     Collisions = new []{ Collision.YasuoWall },
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Ziggs" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Range = 0,
                     Speed = int.MaxValue,
                     Width = 70,
                     ExtraDuration = 10000,
                     DisplayName = "Mines",
                     ParticleNames = new[] { "Ziggs_Base_E_aoe_" },
                     AllowDuplicates = true,
                     DontCross = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Ziggs" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 3,
                     CastDelay = 400,
                     Range = 5000,
                     Speed = 1550,
                     Width = 500,
                     SpellNames = new[] { "ZiggsR" },
                     MissileNames = new[] { "ZiggsRBoom" },
					 ParticleNames = new[] { "Ziggs_Base_R_flames_", "Ziggs_Base_R_landingZone_", "ZiggsRBoomExtraLong" },
                     DontCross = true,
					 DontRemoveWithMissile = true
                   },
				   
				#endregion Ziggs
				
				#region Zilean
				
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Zilean" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 650,
                     Range = 900,
                     Speed = 2000,
                     Width = 150,
                     SpellNames = new[] { "ZileanQ" },
                     MissileNames = new[] { "ZileanQMissile" },
                     ParticleNames = new[] { "Zilean_Base_Q_Cas_Warning_" },
                     Collisions = new []{ Collision.YasuoWall },
                     DontCross = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Zilean" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Range = 900,
                     Speed = int.MaxValue,
                     Width = 150,
                     ExtraDuration = 3100,
                     DisplayName = "Zilean Q Placement",
                     ParticleNames = new[] { "Zilean_Base_Q_Indicator_" },
                     DontCross = true
                   },
				   
				#endregion Zilean
				
				#region Zyra
				
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Zyra" },
                     Slots = new[] { SpellSlot.Q },
                     DangerLevel = 2,
                     CastDelay = 850,
                     Range = 800,
                     Speed = int.MaxValue,
                     Width = 140,
                     SpellNames = new[] { "ZyraQ" },
                     MissileNames = new[] { "ZyraQ" },
                     Collisions = new []{ Collision.YasuoWall },
					 StaticStart = true,
					 StaticEnd = true
                   },
               new SkillshotData
                   {
                     type = Type.LineMissile,
                     CasterNames = new[] { "Zyra" },
                     Slots = new[] { SpellSlot.E },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Range = 1150,
                     Speed = 1150,
                     Width = 70,
                     SpellNames = new[] { "ZyraE" },
                     MissileNames = new[] { "ZyraE" },
                     Collisions = new []{ Collision.YasuoWall },
                     IsFixedRange = true,
                     SticksToMissile = true,
					 IsDangerous = true
                   },
               new SkillshotData
                   {
                     type = Type.CircleMissile,
                     CasterNames = new[] { "Zyra" },
                     Slots = new[] { SpellSlot.R },
                     DangerLevel = 4,
                     CastDelay = 2200,
                     Range = 700,
                     Speed = int.MaxValue,
                     Width = 520,
                     SpellNames = new[] { "ZyraR" },
                   },
				   
				#endregion Zyra
            };
    }
}
