using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI
{
    // Component::GUI::AtkComponentNumericInput
    //   Component::GUI::AtkComponentInputBase
    //     Component::GUI::AtkComponentBase
    //       Component::GUI::AtkEventListener

    [StructLayout(LayoutKind.Explicit, Size = 0x338)]
    public unsafe struct AtkComponentNumericInput
    {
        [FieldOffset(0x0)] public AtkComponentInputBase AtkComponentInputBase;
        [FieldOffset(0x2F8)] public AtkUldComponentDataNumericInput Data;

    }
}