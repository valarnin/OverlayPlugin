using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Event;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.InstanceContent
{

    [StructLayout(LayoutKind.Explicit, Size = 0x770)]
    public struct ContentDirector
    {
        [FieldOffset(0x00)] public Director Director;

        [FieldOffset(0x740)] public float ContentTimeLeft;
    }
}