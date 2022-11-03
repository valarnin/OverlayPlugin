using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Framework;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Agent
{

    [StructLayout(LayoutKind.Explicit, Size = 0x1AE8)]
    public unsafe struct AgentLobby
    {

        [FieldOffset(0x0)] public AgentInterface AgentInterface;
        [FieldOffset(0xEC0)] public ulong SelectedCharacterId;
        [FieldOffset(0xEC8)] public byte DataCenter;
        [FieldOffset(0xECC)] public ushort WorldId;
        [FieldOffset(0xEE8)] public uint IdleTime;
    }
}