using System.Runtime.InteropServices;
using System.Text;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Framework;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Misc
{
    // Client::UI::Misc::ConfigModule
    // ctor E8 ?? ?? ?? ?? 48 8B 97 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? 4C 8B CF

    [StructLayout(LayoutKind.Explicit, Size = 0xD9E8)]
    public unsafe struct ConfigModule
    {
        public const int ConfigOptionCount = 685;
        [FieldOffset(0x28)] public UIModule* UIModule;
        [FieldOffset(0x2C8)] private fixed byte options[Option.Size * ConfigOptionCount];
        [FieldOffset(0x5878)] private fixed byte values[0x10 * ConfigOptionCount];















        [StructLayout(LayoutKind.Explicit, Size = Size)]
        public struct Option
        {
            public const int Size = 0x20;
            [FieldOffset(0x00)] public void* Unk00;
            [FieldOffset(0x08)] public ulong Unk08;
            [FieldOffset(0x10)] public ConfigOption OptionID;
            [FieldOffset(0x14)] public uint Unk14;
            [FieldOffset(0x18)] public uint Unk18;
            [FieldOffset(0x1C)] public ushort Unk1C;

        }
    }
}
