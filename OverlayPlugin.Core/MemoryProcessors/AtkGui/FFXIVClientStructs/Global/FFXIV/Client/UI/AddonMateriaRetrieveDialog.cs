using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    // Client::UI::AddonMateriaRetrieveDialog
    //   Component::GUI::AtkUnitBase
    //     Component::GUI::AtkEventListener
    [StructLayout(LayoutKind.Explicit, Size = 0x220)]
    public struct AddonMateriaRetrieveDialog
    {
        [FieldOffset(0x0)] public AtkUnitBase AtkUnitBase;
    }
}