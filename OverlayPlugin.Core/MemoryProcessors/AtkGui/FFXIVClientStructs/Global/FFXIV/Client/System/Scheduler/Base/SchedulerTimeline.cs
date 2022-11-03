using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Scheduler.Base
{

    // Client::System::Scheduler::Base::SchedulerTimeline
    //   Client::System::Scheduler::Base::TimelineController
    //     Client::System::Scheduler::Base::SchedulerState

    // Size at least 0x248
    // ctor E8 ?? ?? ?? ?? 48 89 43 ?? 48 89 98
    [StructLayout(LayoutKind.Explicit, Size = 0x248)]
    public struct SchedulerTimeline
    {
        [FieldOffset(0)] public TimelineController TimelineController;


    }
}