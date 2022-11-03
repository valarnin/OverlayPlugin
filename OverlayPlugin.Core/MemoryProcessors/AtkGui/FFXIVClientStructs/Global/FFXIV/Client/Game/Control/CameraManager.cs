using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Control
{

    [StructLayout(LayoutKind.Explicit, Size = 0x180)]
    public unsafe struct CameraManager
    {
        [FieldOffset(0x00)] public Camera* Camera;
        [FieldOffset(0x08)] public LowCutCamera* LowCutCamera;
        [FieldOffset(0x10)] public LobbyCamera* LobbCamera;
        [FieldOffset(0x18)] public Camera3* Camera3;
        [FieldOffset(0x20)] public Camera4* Camera4;

        [FieldOffset(0x48)] public int ActiveCameraIndex;
        [FieldOffset(0x4C)] public int PreviousCameraIndex;

        [FieldOffset(0x60)] public CameraBase UnkCamera; //not a pointer

    }
}