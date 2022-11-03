using System.Runtime.InteropServices;
using System.Text;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Configuration;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Common.Lua;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.Excel;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.Exd;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Framework
{
    // Client::System::Framework::Framework

    // size=0x35C8
    // ctor E8 ? ? ? ? 48 8B C8 48 89 05 ? ? ? ? EB 0A 48 8B CE 
    [StructLayout(LayoutKind.Explicit, Size = 0x35C8)]
    public unsafe struct Framework
    {
        [FieldOffset(0x10)] public SystemConfig SystemConfig;
        [FieldOffset(0x460)] public DevConfig DevConfig;

        [FieldOffset(0x1678)] public bool IsNetworkModuleInitialized;
        [FieldOffset(0x1679)] public bool EnableNetworking;
        [FieldOffset(0x1680)] public long ServerTime;
        [FieldOffset(0x16B8)] public float FrameDeltaTime;
        [FieldOffset(0x1770)] public long EorzeaTime;
        [FieldOffset(0x17C4)] public float FrameRate;
        [FieldOffset(0x17D0)] public bool WindowInactive;

        [FieldOffset(0x220C)] private fixed char userPath[260]; // WideChar Array

        [FieldOffset(0x2B30)] public ExcelModuleInterface* ExcelModuleInterface;
        [FieldOffset(0x2B38)] public ExdModule* ExdModule;

        [FieldOffset(0x2B60)] public UIModule* UIModule;
        [FieldOffset(0x2BC8)] public LuaState LuaState;

        [FieldOffset(0x2BF0)] public GameVersion GameVersion;




    }
}