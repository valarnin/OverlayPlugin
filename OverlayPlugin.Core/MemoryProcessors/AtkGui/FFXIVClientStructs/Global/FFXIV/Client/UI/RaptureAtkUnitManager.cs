using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{
    // Client::UI::RaptureAtkUnitManager
    //   Component::GUI::AtkUnitManager
    //     Component::GUI::AtkEventListener

    // size = 0x9D2C
    // ctor 40 53 48 83 EC 20 48 8B D9 E8 ? ? ? ? C6 83 ? ? ? ? ? 48 8D 8B ? ? ? ?

    [StructLayout(LayoutKind.Explicit, Size = 0x9D10)]
    public unsafe struct RaptureAtkUnitManager
    {
        [FieldOffset(0x0)] public AtkUnitManager AtkUnitManager;


    }
}