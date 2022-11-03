using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.STD;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Agent
{

    [StructLayout(LayoutKind.Explicit, Size = 0x68)]
    public struct AgentMonsterNote
    {
        [FieldOffset(0x00)] public AgentInterface AgentInterface;
        [FieldOffset(0x28)] public StdVector StringVector;
        [FieldOffset(0x40)] public uint BaseId;
        [FieldOffset(0x44)] public byte ClassId;
        [FieldOffset(0x45)] public byte ClassIndex;
        [FieldOffset(0x46)] public byte MonsterNote;
        [FieldOffset(0x47)] public byte Rank;
        [FieldOffset(0x48)] public byte Filter;

        [FieldOffset(0x5C)] public byte IsLocked;

    }
}