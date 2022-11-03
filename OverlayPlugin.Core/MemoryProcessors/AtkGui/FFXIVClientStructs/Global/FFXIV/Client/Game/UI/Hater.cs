using System;

using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.UI
{

    [StructLayout(LayoutKind.Explicit, Size = 0x908)]
    public unsafe struct Hater
    {
        [FieldOffset(0x00)] public fixed byte HaterArray[0x48 * 32];
        [FieldOffset(0x900)] public int HaterArrayLength;

    }

    [StructLayout(LayoutKind.Explicit, Size = 0x48)]
    public unsafe struct HaterInfo
    {
        [FieldOffset(0x00)] public fixed byte Name[64];
        [FieldOffset(0x40)] public uint ObjectId;
        [FieldOffset(0x44)] public int Enmity;
    }
}