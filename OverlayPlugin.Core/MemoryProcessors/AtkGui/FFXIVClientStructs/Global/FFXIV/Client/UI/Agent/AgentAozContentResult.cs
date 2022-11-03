using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Agent
{

    [StructLayout(LayoutKind.Explicit, Size = 0x30)]
    public unsafe struct AgentAozContentResult
    {
        [FieldOffset(0x00)] public AgentInterface AgentInterface;
        [FieldOffset(0x28)] public AozContentResultData* AozContentResultData;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x90)]
    public struct AozContentResultData
    {
        [FieldOffset(0x04)] public uint ClearTime;
        [FieldOffset(0x0C)] public uint Score;
        [FieldOffset(0x28)] public Utf8String StageName;
    }
}