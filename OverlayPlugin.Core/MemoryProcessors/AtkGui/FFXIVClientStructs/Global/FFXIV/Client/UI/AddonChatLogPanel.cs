using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    // Client::UI::AddonChatLogPanel
    //   Component::GUI::AtkUnitBase
    //     Component::GUI::AtkEventListener
    [StructLayout(LayoutKind.Explicit, Size = 0x3D0)]
    public unsafe struct AddonChatLogPanel
    {
        [FieldOffset(0x0)] public AtkUnitBase AtkUnitBase;
        [FieldOffset(0x280)] public AtkTextNode* ChatText;
        [FieldOffset(0x2B0)] public byte FontSize;
        [FieldOffset(0x2B4)] public uint FirstLineVisible;
        [FieldOffset(0x2B8)] public uint LastLineVisible;
        [FieldOffset(0x2C0)] public uint Unknown2C0;
        [FieldOffset(0x2C4)] public uint TotalLineCount;
        [FieldOffset(0x2F8)] public uint MessagesAboveCurrent;
        [FieldOffset(0x341)] public byte IsScrolledBottom;
    }
}