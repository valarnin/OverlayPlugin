using System;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Agent
{

    [StructLayout(LayoutKind.Explicit, Size = 0x3B0)]
    public unsafe struct AgentReadyCheck
    {
        [FieldOffset(0x00)] public AgentInterface AgentInterface;

        [FieldOffset(0xB0)] public fixed byte ReadyCheckEntries[16 * 48];


        [StructLayout(LayoutKind.Explicit, Size = 0x10)]
        public struct ReadyCheckEntry
        {
            [FieldOffset(0x00)] public long ContentID;
            [FieldOffset(0x08)] public ReadyCheckStatus Status;
        }

        public enum ReadyCheckStatus : byte
        {
            Unknown = 0,
            AwaitingResponse = 1,
            Ready = 2,
            NotReady = 3,
            MemberNotPresent = 4
        }
    }
}
