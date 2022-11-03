using System.Runtime.InteropServices;
using System.Text;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Object;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Framework;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Misc
{

    // Client::UI::Misc::RetainerModule
    // ctor 48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 33 ED 48 89 51 10 48 8D 05 ?? ?? ?? ?? 48 89 69 08 48 8B F1
    [StructLayout(LayoutKind.Explicit, Size = 0x450)]
    public unsafe struct RetainerCommentModule
    {
        [FieldOffset(0x40)] public RetainerCommentList Retainers;



        [StructLayout(LayoutKind.Sequential, Size = 0x410)]
        public struct RetainerCommentList
        {
            private fixed byte data[0x68 * 10];

        }

        [StructLayout(LayoutKind.Sequential, Size = 0x68)]
        public struct RetainerComment
        {
            public ulong ID;
        }
    }
}
