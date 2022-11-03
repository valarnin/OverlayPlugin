using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI
{

    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    public unsafe struct NumberArrayData
    {
        [FieldOffset(0x0)] public AtkArrayData AtkArrayData;
        [FieldOffset(0x20)] public int* IntArray;

    }
}