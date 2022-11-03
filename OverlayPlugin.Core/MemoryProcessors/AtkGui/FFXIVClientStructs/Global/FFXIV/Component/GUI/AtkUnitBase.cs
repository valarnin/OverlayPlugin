using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI
{
    // Component::GUI::AtkUnitBase
    //   Component::GUI::AtkEventListener

    // base class for all AddonXXX classes (visible UI objects)

    // size = 0x220
    // ctor E8 ? ? ? ? 83 8B ? ? ? ? ? 33 C0

    [StructLayout(LayoutKind.Explicit, Size = 0x220)]
    public unsafe struct AtkUnitBase
    {
        [FieldOffset(0x0)] public AtkEventListener AtkEventListener;
        [FieldOffset(0x8)] public fixed byte Name[0x20];
        [FieldOffset(0x28)] public AtkUldManager UldManager;
        [FieldOffset(0xC8)] public AtkResNode* RootNode;
        [FieldOffset(0xD0)] public AtkCollisionNode* WindowCollisionNode;
        [FieldOffset(0xD8)] public AtkCollisionNode* WindowHeaderCollisionNode;
        [FieldOffset(0xE0)] public AtkResNode* CursorTarget; // Likely always AtkCollisionNode
        [FieldOffset(0x108)] public AtkComponentNode* WindowNode;
        [FieldOffset(0x160)] public AtkValue* AtkValues;
        [FieldOffset(0x1AC)] public float Scale;
        [FieldOffset(0x182)] public byte Flags;
        [FieldOffset(0x1B6)] public byte VisibilityFlags;
        [FieldOffset(0x1BC)] public short X;
        [FieldOffset(0x1BE)] public short Y;
        [FieldOffset(0x1CA)] public ushort AtkValuesCount;
        [FieldOffset(0x1CC)] public ushort ID;
        [FieldOffset(0x1CE)] public ushort ParentID;
        [FieldOffset(0x1D0)] public ushort UnknownID;
        [FieldOffset(0x1D2)] public ushort ContextMenuParentID;
        [FieldOffset(0x1D5)] public byte Alpha;

        [FieldOffset(0x1D8)]
        public AtkResNode**
            CollisionNodeList; // seems to be all collision nodes in tree, may be something else though

        [FieldOffset(0x1E0)] public uint CollisionNodeListCount;
















    }
}