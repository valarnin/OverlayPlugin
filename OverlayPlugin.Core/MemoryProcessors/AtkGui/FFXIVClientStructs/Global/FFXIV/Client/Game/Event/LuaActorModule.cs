using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.STD;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Event
{

    [StructLayout(LayoutKind.Explicit, Size = 0x50)]
    public unsafe struct LuaActorModule
    {
        [FieldOffset(0x00)] public ModuleBase ModuleBase;
        [FieldOffset(0x40)] public StdMap ActorMap;
    }
}