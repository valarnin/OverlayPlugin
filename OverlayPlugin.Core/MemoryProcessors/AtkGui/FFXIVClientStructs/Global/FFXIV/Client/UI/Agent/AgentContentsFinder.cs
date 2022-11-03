using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Framework;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Agent
{

    // Client::UI::Agent::AgentContentsFinder
    //   Client::UI::Agent::AgentInterface
    //     Component::GUI::AtkModuleInterface::AtkEventInterface
    [StructLayout(LayoutKind.Explicit, Size = 0x2050)]
    public unsafe struct AgentContentsFinder
    {
        [FieldOffset(0x0)] public AgentInterface AgentInterface;


    }
}
