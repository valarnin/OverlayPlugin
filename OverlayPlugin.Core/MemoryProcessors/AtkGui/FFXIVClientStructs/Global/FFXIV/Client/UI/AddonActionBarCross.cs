
using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    [StructLayout(LayoutKind.Explicit, Size = 0x710)]
    public struct AddonActionCross
    {
        [FieldOffset(0x000)] public AddonActionBarBase ActionBarBase;
        [FieldOffset(0x6E8)] public int ExpandedHoldControlsLTRT;
        [FieldOffset(0x6EC)] public int ExpandedHoldControlsRTLT;

    }
}
