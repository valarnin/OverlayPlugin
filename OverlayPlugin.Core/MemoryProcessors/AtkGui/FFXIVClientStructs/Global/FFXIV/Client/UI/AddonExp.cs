using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    // Client::UI::AddonExp
    //   Component::GUI::AtkUnitBase
    //     Component::GUI::AtkEventListener
    [StructLayout(LayoutKind.Explicit, Size = 0x290)]
    public struct AddonExp
    {
        [FieldOffset(0x0)] public AtkUnitBase AtkUnitBase;

        [FieldOffset(0x270)] public byte ClassJob;

        [FieldOffset(0x278)] public uint CurrentExp;
        [FieldOffset(0x27C)] public uint RequiredExp;
        [FieldOffset(0x280)] public uint RestedExp;

    }
}