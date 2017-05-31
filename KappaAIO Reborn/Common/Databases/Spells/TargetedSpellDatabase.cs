using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using KappAIO_Reborn.Common.Databases.SpellData;

namespace KappAIO_Reborn.Common.Databases.Spells
{
    public static class TargetedSpellDatabase
    {
        public static TargetedSpellData[] Current;

        static TargetedSpellDatabase()
        {
            if (Current != null)
                return;

            Current = List.Where(s => s.hero == Champion.Unknown || EntityManager.Heroes.AllHeroes.Any(h => s.hero.Equals(h.Hero))).ToArray();
        }

        private static readonly TargetedSpellData[] List =
             {
              new TargetedSpellData
                 {
                     hero = Champion.Akali,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "AkaliMota" },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Speed = 1000,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Akali,
                     slot = SpellSlot.R,
                     DangerLevel = 2,
                     CastDelay = 250,
                     Speed = 2200
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Alistar,
                     slot = SpellSlot.W,
                     DangerLevel = 3,
                     CastDelay = 0,
                     Speed = 2000
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Anivia,
                     slot = SpellSlot.E,
                     MissileNames = new[] { "Frostbite" },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Speed = 1600,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Annie,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "Disintegrate" },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Speed = 1400,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Brand,
                     slot = SpellSlot.E,
                     DangerLevel = 2,
                     CastDelay = 250,
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Brand,
                     slot = SpellSlot.R,
                     MissileNames = new[] { "BrandR", "BrandRMissile" },
                     DangerLevel = 5,
                     CastDelay = 0,
                     Speed = 1000,
                     FastEvade = true,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Caitlyn,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 2000,
                     Speed = 3200,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Camille,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250,
                     Speed = 1200
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Cassiopeia,
                     slot = SpellSlot.E,
                     MissileNames = new[] { "CassiopeiaE" },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Speed = 2500,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Chogath,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Darius,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 366.7f,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Diana,
                     slot = SpellSlot.R,
                     DangerLevel = 2,
                     CastDelay = 250,
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Elise,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "EliseHumanQ" },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Speed = 2200,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Evelynn,
                     slot = SpellSlot.E,
                     DangerLevel = 1,
                     CastDelay = 250
                 },
              new TargetedSpellData
                 {
                     hero = Champion.FiddleSticks,
                     slot = SpellSlot.Q,
                     DangerLevel = 3,
                     CastDelay = 100
                 },
              new TargetedSpellData
                 {
                     hero = Champion.FiddleSticks,
                     slot = SpellSlot.E,
                     MissileNames = new[] { "FiddlesticksDarkWind", "FiddleSticksDarkWindMissile"},
                     DangerLevel = 3,
                     CastDelay = 250,
                     Speed = 1100,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Gangplank,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "GangplankQProceed", "GangplankQProceedCrit" },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Speed = 2600,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Garen,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Irelia,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 0,
                     Speed = 2200
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Irelia,
                     slot = SpellSlot.E,
                     DangerLevel = 3,
                     CastDelay = 200,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Janna,
                     slot = SpellSlot.W,
                     MissileNames = new[] { "SowTheWind" },
                     DangerLevel = 2,
                     CastDelay = 245,
                     Speed = 1600,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.JarvanIV,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Jax,
                     slot = SpellSlot.Q,
                     DangerLevel = 1,
                     CastDelay = 250
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Jayce,
                     slot = SpellSlot.Q,
                     DangerLevel = 1,
                     CastDelay = 250
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Jayce,
                     slot = SpellSlot.E,
                     DangerLevel = 3,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Jhin,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "JhinQ", "JhinQMisBounce" },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Speed = 1800,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Kassadin,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "NullLance" },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Speed = 1400,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Katarina,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "KatarinaQ", "KatarinaQMis" },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Speed = 1800,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Katarina,
                     slot = SpellSlot.R,
                     MissileNames = new[] { "KatarinaRMis" },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Speed = 2400,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Katarina,
                     slot = SpellSlot.E,
                     MissileNames = new[] { "KatarinaETrail" },
                     DangerLevel = 1,
                     CastDelay = 150
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Kayle,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "JudicatorReckoning" },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Speed = 1500,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Kled,
                     slot = SpellSlot.R,
                     MissileNames = new[] { "Kled_Base_R_target_beam.troy" },
                     DangerLevel = 4,
                     CastDelay = 0,
                     Speed = 1337,
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Khazix,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Leblanc,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "LeblancQ" },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Speed = 2000,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Lulu,
                     slot = SpellSlot.W,
                     MissileNames = new[] { "LuluWTwo" },
                     DangerLevel = 3,
                     CastDelay = 241.9f,
                     Speed = 2250,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Lulu,
                     slot = SpellSlot.E,
                     DangerLevel = 1,
                     CastDelay = 100,
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Lissandra,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.LeeSin,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Malphite,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "SeismicShard" },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Speed = 1200,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Malzahar,
                     slot = SpellSlot.E,
                     DangerLevel = 1,
                     CastDelay = 100
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Malzahar,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Maokai,
                     slot = SpellSlot.W,
                     DangerLevel = 3,
                     CastDelay = 0,
                     Speed = 1000
                 },
              new TargetedSpellData
                 {
                     hero = Champion.MasterYi,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "AlphaStrikeMissile" },
                     DangerLevel = 1,
                     CastDelay = 100,
                     Speed = 4000
                 },
              new TargetedSpellData
                 {
                     hero = Champion.MissFortune,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "MissFortuneRicochetShot", "MissFortuneRShotExtra" },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Speed = 1400,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Mordekaiser,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 100,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Nami,
                     slot = SpellSlot.W,
                     MissileNames = new[] { "NamiWEnemy", "NamiWMissileEnemy" },
                     DangerLevel = 2,
                     CastDelay = 250,
                     Speed = 1500,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Nautilus,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 500,
                     Speed = 600
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Nocturne,
                     slot = SpellSlot.R,
                     DangerLevel = 3,
                     CastDelay = 0,
                     Speed = 2000
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Nunu,
                     slot = SpellSlot.E,
                     MissileNames = new[] { "IceBlast" },
                     DangerLevel = 3,
                     CastDelay = 0,
                     Speed = 1000,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Olaf,
                     slot = SpellSlot.E,
                     DangerLevel = 2,
                     CastDelay = 250
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Pantheon,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "PantheonQ" },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Speed = 1500,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Pantheon,
                     slot = SpellSlot.W,
                     DangerLevel = 4,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Poppy,
                     slot = SpellSlot.E,
                     DangerLevel = 4,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Quinn,
                     slot = SpellSlot.E,
                     DangerLevel = 4,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Rammus,
                     slot = SpellSlot.E,
                     DangerLevel = 5,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Ryze,
                     slot = SpellSlot.W,
                     DangerLevel = 3,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Ryze,
                     slot = SpellSlot.E,
                     MissileNames = new[] { "RyzeE" },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Speed = 3500,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Shaco,
                     slot = SpellSlot.E,
                     MissileNames = new[] { "TwoShivPoison" },
                     DangerLevel = 2,
                     CastDelay = 0,
                     Speed = 1500,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Singed,
                     slot = SpellSlot.E,
                     DangerLevel = 4,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Skarner,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Swain,
                     slot = SpellSlot.E,
                     MissileNames = new[] { "SwainTorment" },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Speed = 1400
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Syndra,
                     slot = SpellSlot.R,
                     MissileNames = new[] { "SyndraRTrigger" },
                     DangerLevel = 5,
                     CastDelay = 250,
                     Speed = 1600,
                     FastEvade = true,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Talon,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 0,
                     Speed = 2000
                 },
              new TargetedSpellData
                 {
                     hero = Champion.TahmKench,
                     slot = SpellSlot.W,
                     DangerLevel = 4,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Teemo,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "BlindingDart" },
                     DangerLevel = 3,
                     CastDelay = 0,
                     Speed = 1500,
                     WindWall = true,
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Tristana,
                     slot = SpellSlot.R,
                     MissileNames = new[] { "TristanaR" },
                     DangerLevel = 5,
                     CastDelay = 0,
                     Speed = 2000,
                     FastEvade = true,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Urgot,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "UrgotHeatseekingHomeMissile" },
                     DangerLevel = 1,
                     CastDelay = 0,
                     Speed = 1800,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Urgot,
                     slot = SpellSlot.R,
                     DangerLevel = 4,
                     CastDelay = 250,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Vi,
                     slot = SpellSlot.R,
                     DangerLevel = 5,
                     CastDelay = 250,
                     Speed = 1400,
                     FastEvade = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Vayne,
                     slot = SpellSlot.E,
                     MissileNames = new[] { "VayneCondemnMissile" },
                     DangerLevel = 4,
                     CastDelay = 0,
                     Speed = 2200,
                     FastEvade = true,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Veigar,
                     slot = SpellSlot.R,
                     MissileNames = new[] { "VeigarR" },
                     DangerLevel = 5,
                     CastDelay = 250,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Viktor,
                     slot = SpellSlot.Q,
                     MissileNames = new[] { "ViktorPowerTransfer" },
                     DangerLevel = 1,
                     CastDelay = 250,
                     Speed = 2000,
                     WindWall = true
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Vladimir,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Volibear,
                     slot = SpellSlot.W,
                     DangerLevel = 2,
                     CastDelay = 250
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Warwick,
                     slot = SpellSlot.Q,
                     DangerLevel = 2,
                     CastDelay = 250
                 },
              new TargetedSpellData
                 {
                     hero = Champion.MonkeyKing,
                     slot = SpellSlot.E,
                     DangerLevel = 1,
                     CastDelay = 0,
                     Speed = 2200
                 },
              new TargetedSpellData
                 {
                     hero = Champion.XinZhao,
                     slot = SpellSlot.E,
                     DangerLevel = 2,
                     CastDelay = 250
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Yasuo,
                     slot = SpellSlot.E,
                     DangerLevel = 1,
                     CastDelay = 200
                 },
              new TargetedSpellData
                 {
                     hero = Champion.Zilean,
                     slot = SpellSlot.E,
                     DangerLevel = 2,
                     CastDelay = 200
                 },
             };
    }
}
