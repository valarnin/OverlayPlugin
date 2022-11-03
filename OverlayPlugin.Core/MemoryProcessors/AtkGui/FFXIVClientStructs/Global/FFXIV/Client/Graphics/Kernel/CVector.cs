using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.STD;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Graphics.Kernel
{
    // Client::Graphics::Kernel::CVector
    //   std::vector

    // this class inherits std::vector but adds a virtual function, so its just std::vector with a vtbl
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CVector
    {
        public void* vtbl;
        public StdVector Vector;
    }
}