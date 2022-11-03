using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.STD;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Agent
{

    [StructLayout(LayoutKind.Explicit, Size = 0x38)]
    public unsafe struct AgentMJIPouch
    {
        [FieldOffset(0x00)] public AgentInterface AgentInterface;

        [FieldOffset(0x28)] public PouchIndexInfo* InventoryIndex;
        [FieldOffset(0x30)] public PouchInventoryData* InventoryData;


        [StructLayout(LayoutKind.Explicit, Size = 0x8)]
        public struct PouchIndexInfo
        {
            [FieldOffset(0x00)] public int CurrentIndex;
            [FieldOffset(0x04)] public int MaxIndex;
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x1A0)]
        public struct PouchInventoryData
        {
            [FieldOffset(0x78)] public StdVector Inventory;
            [FieldOffset(0x90)] public StdVector Materials;
            [FieldOffset(0xA8)] public StdVector Produce;
            [FieldOffset(0xC0)] public StdVector StockStores;
            [FieldOffset(0xD8)] public StdVector Tools;
            [FieldOffset(0xF0)] public StdVector InventoryNames;
            [FieldOffset(0x108)] public uint MJIItemPouchItemCount;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x80)]
    public struct PouchInventoryItem
    {
        [FieldOffset(0x00)] public uint ItemId;
        [FieldOffset(0x04)] public uint IconId;
        [FieldOffset(0x08)] public int SlotIndex;
        [FieldOffset(0x0C)] public int StackSize;
        [FieldOffset(0x10)] public int MaxStackSize;
        [FieldOffset(0x14)] public byte InventoryIndex;
        [FieldOffset(0x15)] public byte ItemCategory;
        [FieldOffset(0x16)] public byte Undiscovered;

        [FieldOffset(0x18)] public Utf8String Name;
    }
}