using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    [StructLayout(LayoutKind.Explicit, Size = 0x298)]
    public struct AddonContextMenu
    {
        [FieldOffset(0x0)] public AtkUnitBase AtkUnitBase;
        [FieldOffset(0x160)] public unsafe AtkValue* AtkValues;
        [FieldOffset(0x1CA)] public ushort AtkValuesCount;
    }
}