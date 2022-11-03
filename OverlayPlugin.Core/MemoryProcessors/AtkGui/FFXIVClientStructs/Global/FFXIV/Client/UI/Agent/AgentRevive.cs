using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.UI;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Agent
{

    //Client::UI::Agent::AgentRevive
    //   Client::UI::Agent::AgentInterface
    //     Component::GUI::AtkModuleInterface::AtkEventInterface
    [StructLayout(LayoutKind.Explicit, Size = 0xB8)]
    public unsafe struct AgentRevive
    {
        [FieldOffset(0x0)] public AgentInterface AgentInterface;
        [FieldOffset(0x28)] public Revive* Revive; //callback for SelectYesNo
        [FieldOffset(0x38)] public byte ReviveState;
        [FieldOffset(0x40)] public int ResurrectionTimeLeft;
        [FieldOffset(0x44)] public uint ResurrectingPlayerId;
        [FieldOffset(0x48)] public Utf8String ResurrectingPlayerName;
    }
}