using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI
{

    // Component::GUI::AtkModule
    //   Component::GUI::AtkModuleInterface
    [StructLayout(LayoutKind.Explicit, Size = 0x8228)]
    public unsafe struct AtkModule
    {
        [FieldOffset(0x0)] public void* vtbl;
        [FieldOffset(0x1B48)] public AtkArrayDataHolder AtkArrayDataHolder;

    }
}
