using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    [StructLayout(LayoutKind.Explicit, Size = 1280)]
    public struct AddonCastBar
    {
        [FieldOffset(0x000)] public AtkUnitBase AtkUnitBase;
        [FieldOffset(0x220)] public Utf8String CastName;
        [FieldOffset(0x2BC)] public ushort CastTime;
        [FieldOffset(0x2C0)] public float CastPercent;

    }
}