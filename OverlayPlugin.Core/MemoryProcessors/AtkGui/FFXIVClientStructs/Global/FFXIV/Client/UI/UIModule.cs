using System;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Agent;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Misc;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Shell;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.Excel;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    // Client::UI::UIModule
    //   Client::UI::UIModuleInterface
    [StructLayout(LayoutKind.Explicit, Size = 0xE9F30)]
    public unsafe struct UIModule
    {
        [FieldOffset(0x0)] public void* vtbl;
        [FieldOffset(0x0)] public void** vfunc;
        [FieldOffset(0x8)] public Unk1 UnkObj1;
        [FieldOffset(0x10)] public Unk2 UnkObj2;
        [FieldOffset(0x18)] public Unk3 UnkObj3;
        [FieldOffset(0x20)] public void* unk;
        [FieldOffset(0x28)] public void* SystemConfig;

        [Obsolete("Use GetRaptureAtkModule", true)]
        [FieldOffset(0xB9AB0)]
        public RaptureAtkModule RaptureAtkModule; // note: NOT a pointer, the module's a member

        /*
            dq 0                                    ; +0x30
            dq 23000000000h                         ; +0x38
            dq 0                                    ; +0x40
            dq 23000000000h                         ; +0x48
            dq 0                                    ; +0x50
            and so on...
         */



















































        [StructLayout(LayoutKind.Explicit, Size = 0x8)]
        public struct Unk1
        {
            [FieldOffset(0x0)] public void* vtbl;
            [FieldOffset(0x0)] public void** vfunc;
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x8)]
        public struct Unk2
        {
            [FieldOffset(0x0)] public void* vtbl;
            [FieldOffset(0x0)] public void** vfunc;
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x8)] // size?
        public struct Unk3
        {
            [FieldOffset(0x0)] public void* vtbl;
            [FieldOffset(0x0)] public void** vfunc;
        }

        [Flags]
        public enum UiFlags
        {
            Shortcuts = 1, //disable ui shortcuts
            Hud = 2,
            Nameplates = 4,
            Chat = 8,
            ActionBars = 16,
            Unk32 = 32, //same as 1 ?
            TargetInfo = 64 //+disable system menu / ESC key
        }
    }
}