using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Object;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.STD;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Fate
{

    // This is a struct of some sort, likely part of the FateDirector.
    // Size taken from dtor, no vtbl
    [StructLayout(LayoutKind.Explicit, Size = 0xB0)]
    public unsafe struct FateManager
    {
        [FieldOffset(0x00)] public StdVector Unk_Vector;
        [FieldOffset(0x18)] public Utf8String Unk_String;

        [FieldOffset(0x80)] public FateDirector* FateDirector;
        [FieldOffset(0x88)] public FateContext* CurrentFate;
        [FieldOffset(0x90)] public StdVector Fates;
        [FieldOffset(0xA8)] public ushort SyncedFateId;
        [FieldOffset(0xAC)] public byte FateJoined;

    }
}