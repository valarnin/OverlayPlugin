using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    [StructLayout(LayoutKind.Explicit, Size = 0x500)]
    public unsafe struct AddonCharacterInspect
    {
        [FieldOffset(0x000)] public AtkUnitBase AtkUnitBase;
        [FieldOffset(0x430)] public AtkComponentBase* PreviewComponent;
    }
}