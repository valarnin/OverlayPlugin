using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    // Client::UI::AddonRetainerTaskAsk
    //   Component::GUI::AtkUnitBase
    //     Component::GUI::AtkEventListener
    [StructLayout(LayoutKind.Explicit, Size = 0x2B8)]
    public unsafe struct AddonRetainerTaskAsk
    {
        [FieldOffset(0x0)] public AtkUnitBase AtkUnitBase;
        [FieldOffset(0x2A8)] public AtkComponentButton* AssignButton;
        [FieldOffset(0x2B0)] public AtkComponentButton* ReturnButton;
    }
}