using System;
using System.Runtime.InteropServices;
using System.Text;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Common.Log;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.Excel;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Misc
{

    // Client::UI::Misc::RaptureLogModule
    // ctor E8 ?? ?? ?? ?? 4C 8D AF ?? ?? ?? ?? 49 8B CD
    [StructLayout(LayoutKind.Explicit, Size = 0x3480)]
    public unsafe struct RaptureLogModule
    {
        [FieldOffset(0x00)] public LogModule LogModule;

        [FieldOffset(0xE8)] public ExcelModuleInterface* ExcelModuleInterface;
        [FieldOffset(0xF0)] public RaptureTextModule* RaptureTextModule;

        [FieldOffset(0x528)] public fixed byte ChatTabs[5 * 0x928]; // array of 5 RaptureLogModuleTab

        [FieldOffset(0x3470)] public LogMessageSource* MsgSourceArray;
        [FieldOffset(0x3478)] public int MsgSourceArrayLength;






    }

    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public struct LogMessageSource
    {
        [FieldOffset(0x00)] public ulong ContentId;
        [FieldOffset(0x08)] public int LogMessageIndex;
        [FieldOffset(0x0C)] public short World;
        [FieldOffset(0x0E)] public short ChatType;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x928)]
    public struct RaptureLogModuleTab
    {
        [FieldOffset(0x00)] public Utf8String Name;
        [FieldOffset(0x68)] public Utf8String VisibleLogLines;
    }
}