using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Framework;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Misc
{

    // Client::UI::Misc::RaptureMacroModule
    // ctor E8 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 4C 8B C7 49 8B D5 E8 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 4C 8B C7
    [StructLayout(LayoutKind.Explicit, Size = 0x51AA8)]
    public unsafe struct RaptureMacroModule
    {
        [StructLayout(LayoutKind.Sequential, Size = 0x688)]
        public struct Macro
        {
            public uint IconId;
            public uint Unk; // MacroIcon, exclusive of /micon or similar. Oddly, off by one from Lumina's table.
            public Utf8String Name;
            public Lines Line;

            [StructLayout(LayoutKind.Sequential, Size = 0x618)]
            public struct Lines
            {
                private fixed byte data[0x618];

            }
        }

        [StructLayout(LayoutKind.Sequential, Size = 0x28D20)]
        public struct MacroPage
        {
            private fixed byte data[0x28D20];

        }

        [FieldOffset(0x58)] public MacroPage Individual;
        [FieldOffset(0x28D78)] public MacroPage Shared;





    }
}