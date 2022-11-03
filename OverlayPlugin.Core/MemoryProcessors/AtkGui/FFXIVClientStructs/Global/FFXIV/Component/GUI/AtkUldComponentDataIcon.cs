using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI
{

    [StructLayout(LayoutKind.Explicit, Size = 0x2C)]
    public unsafe struct AtkUldComponentDataIcon
    {
        [FieldOffset(0x00)] public AtkUldComponentDataBase Base;
        [FieldOffset(0x0C)] public fixed uint Nodes[8];
    }
}