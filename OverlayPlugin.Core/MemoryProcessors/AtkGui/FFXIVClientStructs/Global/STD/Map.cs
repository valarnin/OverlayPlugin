using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.STD
{

    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    public unsafe struct StdMap
    {
        public void* Head;
        public ulong Count;
    }
}