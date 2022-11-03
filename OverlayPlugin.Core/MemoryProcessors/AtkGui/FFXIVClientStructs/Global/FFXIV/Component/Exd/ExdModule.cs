using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.Excel;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.Exd
{

    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    public unsafe struct ExdModule
    {
        [FieldOffset(0x20)] public ExcelModule* ExcelModule;



    }
}