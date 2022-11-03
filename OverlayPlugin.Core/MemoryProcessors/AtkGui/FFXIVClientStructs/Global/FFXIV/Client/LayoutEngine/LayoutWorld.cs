using System.ComponentModel.Design.Serialization;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.STD;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.LayoutEngine
{

    [StructLayout(LayoutKind.Explicit, Size = 0x228)]
    public unsafe struct LayoutWorld
    {

        [FieldOffset(0x20)] public LayoutManager* ActiveLayout;
        [FieldOffset(0x218)] public StdMap RsvMap;
    }
}