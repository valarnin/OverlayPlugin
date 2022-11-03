using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.STD;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.UI
{
    // Client::Game::UI::Telepo

    // size = 0x58
    // ctor E8 ? ? ? ? 89 B3 ? ? ? ? 48 8D 8B ? ? ? ? 48 8D 05
    [StructLayout(LayoutKind.Explicit, Size = 0x58)]
    public unsafe struct Telepo
    {
        [FieldOffset(0x00)] public void* vtbl;
        [FieldOffset(0x10)] public StdVector TeleportList;
        [FieldOffset(0x28)] public SelectUseTicketInvoker UseTicketInvoker;



    }

    [StructLayout(LayoutKind.Explicit, Size = 0x20)]
    public struct TeleportInfo
    {
        [FieldOffset(0x00)] public uint AetheryteId;
        [FieldOffset(0x04)] public uint GilCost;
        [FieldOffset(0x08)] public ushort TerritoryId;

        [FieldOffset(0x18)] public byte Ward;
        [FieldOffset(0x19)] public byte Plot;
        [FieldOffset(0x1A)] public byte SubIndex;
        [FieldOffset(0x1B)] public byte IsFavourite;

        public bool IsSharedHouse => Ward > 0 && Plot > 0;
        public bool IsAppartment => SubIndex == 128 && !IsSharedHouse;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    public unsafe struct SelectUseTicketInvoker
    {
        [FieldOffset(0x00)] public void* vtbl;
        [FieldOffset(0x10)] public Telepo* Telepo;

    }
}