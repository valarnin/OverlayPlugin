using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.STD;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Common.Log
{

    //Component::Log::LogModule
    //  Component::Log::LogModuleInterface
    [StructLayout(LayoutKind.Explicit, Size = 0x80)]
    public struct LogModule
    {
        [FieldOffset(0x08)] public ulong LocalPlayerContentId;

        [FieldOffset(0x14)] public int LogMessageCount;

        [FieldOffset(0x48)] public StdVector LogMessageIndex;
        [FieldOffset(0x60)] public StdVector LogMessageData;
    }
}