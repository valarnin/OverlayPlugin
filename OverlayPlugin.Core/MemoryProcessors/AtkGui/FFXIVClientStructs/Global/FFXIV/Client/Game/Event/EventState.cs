using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Object;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Event
{

    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public struct EventState
    {
        [FieldOffset(0x10)] public GameObjectID ObjectId;
    }
}