using System;

using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.STD
{

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StdVector
    {
        public void* First;
        public void* Last;
        public void* End;
    }
}