using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI
{
    // Component::GUI::AtkComponentDropDownList
    //   Component::GUI::AtkComponentBase
    //     Component::GUI::AtkEventListener

    [StructLayout(LayoutKind.Explicit, Size = 0x224)]
    public struct AtkComponentDropDownList
    {
        [FieldOffset(0x0)] public AtkComponentBase AtkComponentBase;
    }
}