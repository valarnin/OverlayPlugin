using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Object
{

    [StructLayout(LayoutKind.Explicit, Size = 0x3840)]
    public unsafe struct GameObjectManager
    {
        [FieldOffset(0x04)] public byte Active;
        [FieldOffset(0x18)] public fixed long ObjectList[596]; // size 596 * 8
        [FieldOffset(0x12B8)] public fixed long ObjectListFiltered[596];
        [FieldOffset(0x2558)] public fixed long ObjectList3[596];
        [FieldOffset(0x37F8)] public int ObjectListFilteredCount;
        [FieldOffset(0x37FC)] public int ObjectList3Count;


    }
}
