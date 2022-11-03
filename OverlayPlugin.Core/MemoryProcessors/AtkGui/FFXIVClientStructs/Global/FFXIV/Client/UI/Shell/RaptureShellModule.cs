using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Framework;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Misc;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Shell
{

    // Client::UI::Shell::RaptureShellModule
    // ctor E8 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 4C 8B CF
    [StructLayout(LayoutKind.Explicit, Size = 0x1208)]
    public unsafe struct RaptureShellModule
    {
        [FieldOffset(0x2C0)] public int MacroCurrentLine;
        [FieldOffset(0x2B3)] public bool MacroLocked;

    }
}