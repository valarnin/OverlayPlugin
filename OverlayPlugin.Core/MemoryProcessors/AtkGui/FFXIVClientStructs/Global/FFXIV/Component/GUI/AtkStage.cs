using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI
{
    // Component::GUI::AtkStage
    //   Component::GUI::AtkEventTarget

    // size = 0x75DF8
    // ctor E8 ? ? ? ? 48 8B F8 48 89 BE ? ? ? ? 48 8B 43 10
    [StructLayout(LayoutKind.Explicit, Size = 0x75DF8)]
    public unsafe struct AtkStage
    {
        [FieldOffset(0x0)] public AtkEventTarget AtkEventTarget;
        [FieldOffset(0x20)] public RaptureAtkUnitManager* RaptureAtkUnitManager;
        [FieldOffset(0x78)] public AtkDragDropManager DragDropManager;
        [FieldOffset(0x168)] public AtkTooltipManager TooltipManager;
        [FieldOffset(0x338)] public AtkCursor AtkCursor;



    }
}