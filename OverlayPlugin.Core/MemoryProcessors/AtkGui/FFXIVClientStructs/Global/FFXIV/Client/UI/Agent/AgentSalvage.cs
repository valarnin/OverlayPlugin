using System;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Framework;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Agent
{

    [StructLayout(LayoutKind.Explicit, Size = 0x3E0)]
    public unsafe struct AgentSalvage
    {
        [FieldOffset(0x00)] public AgentInterface AgentInterface;

        [FieldOffset(0x30)] public SalvageItemCategory SelectedCategory;
        [FieldOffset(0x38)] public void* ItemList; // Maybe?
        [FieldOffset(0x40)] public Utf8String TextCarpenter;
        [FieldOffset(0xA8)] public Utf8String TextBlacksmith;
        [FieldOffset(0x110)] public Utf8String TextArmourer;
        [FieldOffset(0x178)] public Utf8String TextGoldsmith;
        [FieldOffset(0x1E0)] public Utf8String TextLeatherworker;
        [FieldOffset(0x248)] public Utf8String TextWeaver;
        [FieldOffset(0x2B0)] public Utf8String TextAlchemist;
        [FieldOffset(0x318)] public Utf8String TextCulinarian;
        [FieldOffset(0x380)] public uint ItemCount;

        [FieldOffset(0x398)] public InventoryItem* DesynthItemSlot;

        [FieldOffset(0x3B0)] public SalvageResult DesynthItem;
        [FieldOffset(0x3BC)] public uint DesynthItemId;





        public enum SalvageItemCategory
        {
            InventoryEquipment,
            InventoryHousing,
            ArmouryMainOff,
            ArmouryHeadBodyHands,
            ArmouryLegsFeet,
            ArmouryNeckEars,
            ArmouryWristsRings,
            Equipped,
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct SalvageResult
    {
        [FieldOffset(0x00)] public uint ItemId;
        [FieldOffset(0x04)] public int Quantity;
    }
}