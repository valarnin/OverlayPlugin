using System.Runtime.InteropServices;
using System.Text;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI
{

    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public unsafe struct StringArrayData
    {
        [FieldOffset(0x0)] public AtkArrayData AtkArrayData;
        [FieldOffset(0x20)] public byte** StringArray; // char * *
        [FieldOffset(0x28)] public byte* UnkString; // char *



    }
}