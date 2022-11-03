using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.Exd;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.Excel
{

    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public unsafe struct ExcelModuleInterface
    {
        [FieldOffset(0x08)] public ExdModule* ExdModule;


    }
}