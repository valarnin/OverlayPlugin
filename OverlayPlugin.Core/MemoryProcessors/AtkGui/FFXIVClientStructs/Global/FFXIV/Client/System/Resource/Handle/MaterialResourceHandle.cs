using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Resource.Handle
{
    // Client::System::Resource::Handle::MaterialResourceHandle
    //   Client::System::Resource::Handle::ResourceHandle
    //     Client::System::Common::NonCopyable

    // ctor 40 53 48 83 EC ?? 48 8B 44 24 ?? 48 8B D9 48 89 44 24 ?? 48 8B 44 24 ?? 48 89 44 24 ?? E8 ?? ?? ?? ?? 33 C9 
    [StructLayout(LayoutKind.Explicit, Size = 0x108)]
    public unsafe struct MaterialResourceHandle
    {
        [FieldOffset(0x0)] public ResourceHandle ResourceHandle;


    }
}