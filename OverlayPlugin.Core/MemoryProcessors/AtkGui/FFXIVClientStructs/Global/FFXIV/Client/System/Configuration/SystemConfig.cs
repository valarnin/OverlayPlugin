using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Configuration
{
    // Client::System::Configuration::SystemConfig
    //   Common::Configuration::SystemConfig
    //     Common::Configuration::ConfigBase
    //       Client::System::Common::NonCopyable

    // size = 0x450
    // ctor inlined in Framework ctor
    [StructLayout(LayoutKind.Explicit, Size = 0x450)]
    public struct SystemConfig
    {
        [FieldOffset(0x0)] public Common.Configuration.SystemConfig CommonSystemConfig;

    }
}