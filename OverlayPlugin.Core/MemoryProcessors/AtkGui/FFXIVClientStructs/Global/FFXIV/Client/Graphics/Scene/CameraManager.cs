using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Graphics.Scene
{

    [StructLayout(LayoutKind.Explicit, Size = 0x100)]
    public unsafe struct CameraManager
    {

        [FieldOffset(0x50)] public int CameraIndex;
        [FieldOffset(0x58)] public fixed byte CameraArray[14 * 8]; //14 * Camera*


    }
}