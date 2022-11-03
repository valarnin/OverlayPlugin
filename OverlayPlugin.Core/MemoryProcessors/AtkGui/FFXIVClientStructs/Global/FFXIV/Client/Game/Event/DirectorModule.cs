using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.STD;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Event
{

    [StructLayout(LayoutKind.Explicit, Size = 0xA0)]
    public unsafe struct DirectorModule
    {
        [FieldOffset(0x00)] public ModuleBase ModuleBase;
        [FieldOffset(0x40)] public StdVector DirectorList;
        [FieldOffset(0x98)] public Director* ActiveInstanceDirector;
    }
}