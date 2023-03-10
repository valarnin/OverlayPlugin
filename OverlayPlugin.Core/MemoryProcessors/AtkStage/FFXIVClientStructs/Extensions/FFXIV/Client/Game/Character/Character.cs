using System;
using System.Runtime.InteropServices;
using FFXIVClientStructs.Global.FFXIV.Client.Game.Object;

namespace FFXIVClientStructs.Global.FFXIV.Client.Game.Character
{
    // Size = 0x1B20
    public unsafe partial struct Character
    {
        // TODO: this is incorrect in 6.3, please fix
        [FieldOffset(0x19C3)]
        public byte MonsterType;

        // TODO: this is incorrect in 6.3, please fix
        [FieldOffset(0x19DF)]
        public byte AggressionStatus;

        // FFXIVClientStructs has this struct defined but doesn't use it for some reason :shrug:
        [FieldOffset(0x1A88)]
        public GameObjectID TargetObject;

        // New field
        [FieldOffset(0x1B0A)]
        public byte WeaponId;
    }
}