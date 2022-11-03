using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    [StructLayout(LayoutKind.Explicit, Size = 0x2B8)]
    public unsafe struct AddonActionBar
    {
        [FieldOffset(0x00)] public AddonActionBarX AddonActionBarX;
    }
}