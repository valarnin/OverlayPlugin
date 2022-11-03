using System.Runtime.InteropServices;
using System.Text;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Framework
{

    // Allocates a lot of space for inlined array, even though it only uses 5 strings.
    [StructLayout(LayoutKind.Explicit, Size = 0x900)]
    public unsafe struct GameVersion
    {
        [FieldOffset(0x00)] private fixed byte baseVersion[32];
        // Big unused gap between base and expansions
        [FieldOffset(0xE0)] private fixed byte expansionVersion[32 * 10];

    }
}