using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    // Client::UI::AddonItemInspectionList
    //   Component::GUI::AtkUnitBase
    //     Component::GUI::AtkEventListener
    [StructLayout(LayoutKind.Explicit, Size = 0x1230)]
    public struct AddonItemInspectionList
    {
        [FieldOffset(0x0)] public AtkUnitBase AtkUnitBase;
    }
}