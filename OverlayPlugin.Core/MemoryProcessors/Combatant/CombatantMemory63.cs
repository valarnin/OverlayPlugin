using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FFXIVClientStructs.Global.FFXIV.Client.Game;
using FFXIVClientStructs.Global.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Global.FFXIV.Client.Game.Object;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.Combatant
{
    interface ICombatantMemory63 : ICombatantMemory { }

    class CombatantMemory63 : CombatantMemory, ICombatantMemory63
    {
        private const string charmapSignature = "488B5720B8000000E0483BD00F84????????488D0D";

        public CombatantMemory63(TinyIoCContainer container)
            : base(container, charmapSignature, Marshal.SizeOf<BattleChara>(), EffectMemory.Size)
        {

        }

        public override Version GetVersion()
        {
            return new Version(6, 3);
        }

        // Returns a combatant if the combatant is a mob or a PC.
        protected override unsafe Combatant GetMobFromByteArray(byte[] source, uint mycharID)
        {
            fixed (byte* p = source)
            {
                BattleChara mem = *(BattleChara*)&p[0];
                if (mem.Character.GameObject.ObjectID == 0 || mem.Character.GameObject.ObjectID == emptyID)
                    return null;
            }
            return GetCombatantFromByteArray(source, mycharID, false);
        }

        // Will return any kind of combatant, even if not a mob.
        // This function always returns a combatant object, even if empty.
        protected override unsafe Combatant GetCombatantFromByteArray(byte[] source, uint mycharID, bool isPlayer, bool exceptEffects = false)
        {
            fixed (byte* p = source)
            {
                BattleChara mem = *(BattleChara*)&p[0];

                if (isPlayer)
                {
                    mycharID = mem.Character.GameObject.ObjectID;
                }

                Combatant combatant = new Combatant()
                {
                    Name = FFXIVMemory.GetStringFromBytes(mem.Character.GameObject.Name, GameObject.NameBytes),
                    Job = mem.Character.ClassJob,
                    ID = mem.Character.GameObject.ObjectID,
                    OwnerID = mem.Character.GameObject.OwnerID == emptyID ? 0 : mem.Character.GameObject.OwnerID,
                    Type = (ObjectType)mem.Character.GameObject.ObjectKind,
                    MonsterType = (MonsterType)mem.Character.MonsterType,
                    Status = (ObjectStatus)mem.Character.GameObject.Status,
                    ModelStatus = (ModelStatus)mem.Character.GameObject.RenderFlags,
                    // Normalize all possible aggression statuses into the basic 4 ones.
                    AggressionStatus = (AggressionStatus)(mem.Character.AggressionStatus - (mem.Character.AggressionStatus / 4) * 4),
                    NPCTargetID = mem.Character.TargetObject.ObjectID,
                    RawEffectiveDistance = mem.Character.GameObject.YalmDistanceFromPlayerZ,
                    PosX = mem.Character.GameObject.Position.X,
                    // Y and Z are deliberately swapped to match FFXIV_ACT_Plugin's data model
                    PosY = mem.Character.GameObject.Position.Z,
                    PosZ = mem.Character.GameObject.Position.Y,
                    Heading = mem.Character.GameObject.Rotation,
                    Radius = mem.Character.GameObject.HitboxRadius,
                    // In-memory there are separate values for PC's current target and NPC's current target
                    TargetID = (ObjectType)mem.Character.GameObject.ObjectKind == ObjectType.PC ? mem.Character.PlayerTargetObjectID : mem.Character.TargetObject.ObjectID,
                    CurrentHP = (int)mem.Character.Health,
                    MaxHP = (int)mem.Character.MaxHealth,
                    Effects = exceptEffects ? new List<EffectEntry>() : GetEffectEntries(mem.StatusManager.Status, (ObjectType)mem.Character.GameObject.ObjectKind, mycharID),

                    BNpcID = mem.Character.GameObject.DataID,
                    CurrentMP = (int)mem.Character.Mana,
                    MaxMP = (int)mem.Character.MaxMana,
                    CurrentGP = mem.Character.GatheringPoints,
                    MaxGP = mem.Character.MaxGatheringPoints,
                    CurrentCP = mem.Character.CraftingPoints,
                    MaxCP = mem.Character.MaxCraftingPoints,
                    Level = mem.Character.Level,
                    PCTargetID = mem.Character.PlayerTargetObjectID,

                    BNpcNameID = mem.Character.NameID,

                    WorldID = mem.Character.HomeWorld,
                    CurrentWorldID = mem.Character.CurrentWorld,

                    IsCasting1 = mem.SpellCastInfo.IsCasting,
                    IsCasting2 = (byte)mem.SpellCastInfo.ActionType,
                    CastBuffID = mem.SpellCastInfo.ActionID,
                    CastTargetID = mem.SpellCastInfo.CastTargetID,
                    CastDurationCurrent = mem.SpellCastInfo.CurrentCastTime,
                    CastDurationMax = mem.SpellCastInfo.TotalCastTime,

                    TransformationId = mem.Character.TransformationId,
                    WeaponId = mem.Character.WeaponId
                };
                combatant.IsTargetable =
                    (combatant.ModelStatus == ModelStatus.Visible)
                    && ((combatant.Status == ObjectStatus.NormalActorStatus) || (combatant.Status == ObjectStatus.NormalSubActorStatus));
                if (combatant.Type != ObjectType.PC && combatant.Type != ObjectType.Monster)
                {
                    // Other types have garbage memory for hp.
                    combatant.CurrentHP = 0;
                    combatant.MaxHP = 0;
                }
                return combatant;
            }
        }
    }
}
