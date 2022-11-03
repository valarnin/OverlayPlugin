using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.STD;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI
{

    // Component::GUI::AtkToolTipManager
    //      Component::GUI::AtkEventListener
    [StructLayout(LayoutKind.Explicit, Size = 0x150)]
    public unsafe struct AtkTooltipManager
    {
        public enum AtkTooltipType : byte
        {
            Text = 1,
            Item = 2,
            TextItem = 3,
            Action = 4
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x18)]
        public struct AtkTooltipArgs
        {
            [FieldOffset(0x0)] public byte* Text;
            [FieldOffset(0x8)] public ulong TypeSpecificID;
            [FieldOffset(0x10)] public uint Flags;
            [FieldOffset(0x14)] public short Unk_14;
            [FieldOffset(0x16)] public byte Unk_16;
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x20)]
        public struct AtkTooltipInfo
        {
            [FieldOffset(0x0)] public AtkTooltipArgs AtkTooltipArgs;
            [FieldOffset(0x18)] public ushort ParentID; // same as IDs in addons
            [FieldOffset(0x1A)] public AtkTooltipType Type;
        }

        [FieldOffset(0x0)] public AtkEventListener AtkEventListener;
        [FieldOffset(0x8)] public StdMap TooltipMap;
        [FieldOffset(0x18)] public AtkStage* AtkStage;


    }
}