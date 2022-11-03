using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Object;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Control
{
    // Client::Game::Control::TargetSystem

    [StructLayout(LayoutKind.Explicit, Size = 0x5290)]
    public unsafe struct TargetSystem
    {
        [FieldOffset(0x80)] public GameObject* Target;
        [FieldOffset(0x88)] public GameObject* SoftTarget;
        [FieldOffset(0x98)] public GameObject* GPoseTarget;
        [FieldOffset(0xD0)] public GameObject* MouseOverTarget;
        [FieldOffset(0xE0)] public GameObject* MouseOverNameplateTarget;
        [FieldOffset(0xF8)] public GameObject* FocusTarget;
        [FieldOffset(0x110)] public GameObject* PreviousTarget;
        [FieldOffset(0x140)] public uint TargetObjectId;

        [FieldOffset(0x148)] public GameObjectArray ObjectFilterArray0;

        [FieldOffset(0x1A00)] public GameObjectArray ObjectFilterArray1;
        [FieldOffset(0x2CA8)] public GameObjectArray ObjectFilterArray2;
        [FieldOffset(0x3F50)] public GameObjectArray ObjectFilterArray3;








    }

    [StructLayout(LayoutKind.Explicit, Size = 0x12A8)]
    public unsafe struct GameObjectArray
    {
        [FieldOffset(0x00)] public int Length;
        [FieldOffset(0x08)] public fixed long Objects[596];

    }
}