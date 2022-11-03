using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.UI
{

    // Client::Game::UI::RelicNote
    // size = 0x18
    // ctor inlined in UIState
    [StructLayout(LayoutKind.Explicit, Size = 0x18)]
    public unsafe struct RelicNote
    {
        [FieldOffset(0x08)] public byte RelicID;
        [FieldOffset(0x09)] public byte RelicNoteID;
        [FieldOffset(0x0A)] public fixed byte MonsterProgress[10];
        [FieldOffset(0x14)] public int ObjectiveProgress;





    }
}