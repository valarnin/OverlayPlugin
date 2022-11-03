using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Graphics.Scene
{
    // Client::Graphics::Scene::Object
    // base class for all graphics objects

    // size = 0x80
    // ctor - inlined in all derived class ctors
    [StructLayout(LayoutKind.Explicit, Size = 0x80)]
    public unsafe struct Object
    {
        [FieldOffset(0x18)] public Object* ParentObject;
        [FieldOffset(0x20)] public Object* PreviousSiblingObject;
        [FieldOffset(0x28)] public Object* NextSiblingObject;
        [FieldOffset(0x30)] public Object* ChildObject; // for humans this is a weapon

        [FieldOffset(0x50)] public Vector3 Position;
        [FieldOffset(0x60)] public Quarternion Rotation;
        [FieldOffset(0x70)] public Vector3 Scale;

    }

    public enum ObjectType
    {
        Object = 0,
        Terrain = 1,
        BgObject = 2,
        CharacterBase = 3,
        VfxObject = 4,
        Light = 5,
        Unk_Type6 = 6,
        EnvSpace = 7,
        EnvLocation = 8,
        Unk_Type9 = 9
    }
}