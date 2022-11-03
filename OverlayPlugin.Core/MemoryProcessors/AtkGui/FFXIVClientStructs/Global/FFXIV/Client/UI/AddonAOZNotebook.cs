using System;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    // Client::UI::AddonAOZNotebook
    //   Component::GUI::AtkUnitBase
    //     Component::GUI::AtkEventListener
    [StructLayout(LayoutKind.Explicit, Size = 0xCD0)]
    public unsafe struct AddonAOZNotebook
    {
        [FieldOffset(0x0)] public AtkUnitBase AtkUnitBase;

        [FieldOffset(0x308)] public SpellbookBlock SpellbookBlock01;
        [FieldOffset(0x350)] public SpellbookBlock SpellbookBlock02;
        [FieldOffset(0x398)] public SpellbookBlock SpellbookBlock03;
        [FieldOffset(0x3E0)] public SpellbookBlock SpellbookBlock04;
        [FieldOffset(0x428)] public SpellbookBlock SpellbookBlock05;
        [FieldOffset(0x470)] public SpellbookBlock SpellbookBlock06;
        [FieldOffset(0x4B8)] public SpellbookBlock SpellbookBlock07;
        [FieldOffset(0x500)] public SpellbookBlock SpellbookBlock08;
        [FieldOffset(0x548)] public SpellbookBlock SpellbookBlock09;
        [FieldOffset(0x590)] public SpellbookBlock SpellbookBlock10;
        [FieldOffset(0x5D8)] public SpellbookBlock SpellbookBlock11;
        [FieldOffset(0x620)] public SpellbookBlock SpellbookBlock12;
        [FieldOffset(0x668)] public SpellbookBlock SpellbookBlock13;
        [FieldOffset(0x6B0)] public SpellbookBlock SpellbookBlock14;
        [FieldOffset(0x6F8)] public SpellbookBlock SpellbookBlock15;
        [FieldOffset(0x740)] public SpellbookBlock SpellbookBlock16;

        [FieldOffset(0x820)] public ActiveActions ActiveActions01;
        [FieldOffset(0x840)] public ActiveActions ActiveActions02;
        [FieldOffset(0x860)] public ActiveActions ActiveActions03;
        [FieldOffset(0x880)] public ActiveActions ActiveActions04;
        [FieldOffset(0x8A0)] public ActiveActions ActiveActions05;
        [FieldOffset(0x8C0)] public ActiveActions ActiveActions06;
        [FieldOffset(0x8E0)] public ActiveActions ActiveActions07;
        [FieldOffset(0x900)] public ActiveActions ActiveActions08;
        [FieldOffset(0x920)] public ActiveActions ActiveActions09;
        [FieldOffset(0x940)] public ActiveActions ActiveActions10;
        [FieldOffset(0x960)] public ActiveActions ActiveActions11;
        [FieldOffset(0x980)] public ActiveActions ActiveActions12;
        [FieldOffset(0x9A0)] public ActiveActions ActiveActions13;
        [FieldOffset(0x9C0)] public ActiveActions ActiveActions14;
        [FieldOffset(0x9E0)] public ActiveActions ActiveActions15;
        [FieldOffset(0xA00)] public ActiveActions ActiveActions16;
        [FieldOffset(0xA20)] public ActiveActions ActiveActions17;
        [FieldOffset(0xA40)] public ActiveActions ActiveActions18;
        [FieldOffset(0xA60)] public ActiveActions ActiveActions19;
        [FieldOffset(0xA80)] public ActiveActions ActiveActions20;
        [FieldOffset(0xAA0)] public ActiveActions ActiveActions21;
        [FieldOffset(0xAC0)] public ActiveActions ActiveActions22;
        [FieldOffset(0xAE0)] public ActiveActions ActiveActions23;
        [FieldOffset(0xB00)] public ActiveActions ActiveActions24;

        [StructLayout(LayoutKind.Explicit, Size = 0x48)]
        public unsafe struct SpellbookBlock
        {
            [FieldOffset(0x0)] public AtkComponentBase* AtkComponentBase;
            [FieldOffset(0x8)] public AtkCollisionNode* AtkCollisionNode;
            [FieldOffset(0x10)] public AtkComponentCheckBox* AtkComponentCheckBox;
            [FieldOffset(0x18)] public AtkComponentIcon* AtkComponentIcon;
            [FieldOffset(0x20)] public AtkTextNode* AtkTextNode;
            [FieldOffset(0x28)] public AtkResNode* AtkResNode1;
            [FieldOffset(0x30)] public AtkResNode* AtkResNode2;
            [FieldOffset(0x38)] public char* Name;
            [FieldOffset(0x40)] public uint ActionID;
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x20)]
        public struct ActiveActions
        {
            [FieldOffset(0x0)] public AtkComponentDragDrop* AtkComponentDragDrop;
            [FieldOffset(0x8)] public AtkTextNode* AtkTextNode;
            [FieldOffset(0x10)] public char* Name;
            [FieldOffset(0x18)] public int ActionID;
        }


    }
}