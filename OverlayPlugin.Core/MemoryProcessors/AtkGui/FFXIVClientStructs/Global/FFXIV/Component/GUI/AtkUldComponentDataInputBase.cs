using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Graphics;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI
{

    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public struct AtkUldComponentDataInputBase
    {
        [FieldOffset(0x00)] public AtkUldComponentDataBase Base;
        [FieldOffset(0x0C)] public ByteColor FocusColor;
    }
}