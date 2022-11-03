using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Event
{

    [StructLayout(LayoutKind.Explicit, Size = 0x210)]
    public unsafe struct EventHandler
    {
        [FieldOffset(0x18)] public void* EventSceneModule;
    }
}