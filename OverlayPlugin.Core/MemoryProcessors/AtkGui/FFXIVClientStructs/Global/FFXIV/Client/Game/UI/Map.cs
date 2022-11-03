using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.UI
{

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Map
    {
        [FieldOffset(0x88)] public QuestMarkerArray QuestMarkers;

        [StructLayout(LayoutKind.Sequential, Size = 0x10E0)]
        public struct QuestMarkerArray
        {
            private fixed byte data[30 * 0x90];

        }

        [StructLayout(LayoutKind.Explicit, Size = 0x90)]
        public struct MapMarkerInfo
        {
            [FieldOffset(0x04)] public uint QuestID;
            [FieldOffset(0x08)] public Utf8String Name;
            [FieldOffset(0x8B)] public byte ShouldRender;
            [FieldOffset(0x88)] public ushort RecommendedLevel;
        }

    }
}