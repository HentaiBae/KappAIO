using System;
using System.Linq;
using EloBuddy;

namespace KappAIO_Reborn.Common.Databases.SpellData
{
    public class SkillshotData
    {
        public Type type;
        public Type Explodetype;
        public SpellSlot[] Slots;
        public GameType GameType;
        public string[] CasterNames;
        public string[] SpellNames;
        public string[] MissileNames;
        public string[] ParticleNames;
        public string DisplayName;
        public string RemoveOnBuffLose;
        public string TargetName;
        public string StartParticalName;
        public string MidParticalName;
        public string EndParticalName;
        public string ParticalObjectName;
        public RequireBuff[] RequireBuffs;
        public int CollideCount;
        public int DangerLevel;
        public float ExplodeWidth;
        public float MoveSpeedScaleMod;
        public float MissileAccel;
        public float MissileMaxSpeed;
        public float MissileMinSpeed;
        public float ExtraDuration;
        public float RingRadius;
        public float Range;
        public float ExtraRange;
        public float Angle;
        public float Width;
        public float Speed;
        public float CastDelay;
        public bool IsMoving;
        public bool DecaySpeedWithLessRange;
        public bool EndSticksToTarget;
        public bool DetectAsOne;
        public bool AddEndExplode;
        public bool DontRemoveWithMissile;
        public bool AllowDuplicates;
        public bool HasExplodingEnd;
        public bool StaticStart;
        public bool StaticEnd;
        public bool EndSticksToMissile;
        public bool RangeScaleWithMoveSpeed;
        public bool DontCross;
        public bool DontAddExtraDuration;
        public bool TakeClosestPath;
        public bool EndIsCasterDirection;
        public bool IsFixedRange;
        public bool DetectByMissile;
        public bool EndSticksToCaster;
        public bool SticksToCaster;
        public bool SticksToMissile;
        public bool FastEvade;
        public bool StartsFromTarget;
        public Collision[] Collisions;

        public bool IsCasterName(string name)
        {
            return this.CasterNames != null && this.CasterNames.Any(s => s.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool IsCasterName(Champion name)
        {
            return this.CasterNames != null && this.CasterNames.Any(s => s.Equals(name.ToString(), StringComparison.CurrentCultureIgnoreCase));
        }

        public bool IsSlot(SpellSlot slot)
        {
            return this.Slots != null && this.Slots.Any(s => s.Equals(slot));
        }

        public bool IsDisplayName(string name)
        {
            return !string.IsNullOrEmpty(this.DisplayName) && this.DisplayName.Equals(name, StringComparison.CurrentCultureIgnoreCase);
        }

        public bool IsSpellName(string name)
        {
            return this.SpellNames != null && this.SpellNames.Any(s => s.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool IsMissileName(string name)
        {
            return this.MissileNames != null && this.MissileNames.Any(s => s.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public bool IsParticleName(string name)
        {
            return this.ParticleNames != null && this.ParticleNames.Any(name.StartsWith) && name.EndsWith(".troy");
        }

        public bool HasBuff(Obj_AI_Base caster)
        {
            if (this.RequireBuffs == null || caster == null)
                return true;

            return this.RequireBuffs.Any(b => !string.IsNullOrEmpty(b.Name) && caster.GetBuffCount(b.Name) >= b.Count);
        }

        public string MenuItemName => $"{(CasterNames != null ? CasterNames[0] : "")} {(Slots != null ? Slots.All(s => s.Equals(SpellSlot.Unknown)) ? "Special" : Slots[0].ToString() : "")} ({(!string.IsNullOrEmpty(DisplayName) ? DisplayName : SpellNames != null ? SpellNames[0] : this.MissileNames != null ? MissileNames[0] : ParticleNames != null ? ParticleNames[0] : "")})";
        public bool HasRange => (this.Range < int.MaxValue && this.Range < float.MaxValue && this.Range > 0) && this.Range != 25000f;
        public bool HasWidth => this.Width < int.MaxValue && this.Width < float.MaxValue && this.Width > 0;
        public bool HasAngle => this.Angle < int.MaxValue && this.Angle < float.MaxValue && this.Angle > 0;
        public bool HasExtraRange => this.ExtraRange < int.MaxValue && this.ExtraRange < float.MaxValue && this.ExtraRange > 0;
        public bool HasRingRadius => this.RingRadius < int.MaxValue && this.RingRadius < float.MaxValue && this.RingRadius > 0;
        public bool CanCollide => this.CollideCount > 0 && this.CollideCount < int.MaxValue;
        public bool ClickRemove => this.CastDelay > 1250 && this.CastDelay < int.MaxValue || this.ExtraDuration > 1000 && this.ExtraDuration < int.MaxValue;

        public class RequireBuff
        {
            public RequireBuff(string name, int count)
            {
                this.Name = name;
                this.Count = count;
            }
            public string Name;
            public int Count;
        }
    }

    public enum Type
    {
        LineMissile,
        CircleMissile,
        Cone,
        Arc,
        Ring
    }

    public enum Collision
    {
        YasuoWall,
        Minions,
        Heros,
        Walls,
        Caster
    }
}
