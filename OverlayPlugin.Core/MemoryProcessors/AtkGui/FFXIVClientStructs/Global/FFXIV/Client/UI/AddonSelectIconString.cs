using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    // Client::UI::AddonSelectIconString
    //   Component::GUI::AtkUnitBase
    //     Component::GUI::AtkEventListener
    [StructLayout(LayoutKind.Explicit, Size = 0x2A0)]
    public struct AddonSelectIconString
    {
        [FieldOffset(0x0)] public AtkUnitBase AtkUnitBase;
        [FieldOffset(0x238)] public PopupMenuDerive PopupMenu;

        [StructLayout(LayoutKind.Explicit, Size = 0x68)]
        public struct PopupMenuDerive
        {
            [FieldOffset(0x0)] public PopupMenu PopupMenu;
        }
    }
}