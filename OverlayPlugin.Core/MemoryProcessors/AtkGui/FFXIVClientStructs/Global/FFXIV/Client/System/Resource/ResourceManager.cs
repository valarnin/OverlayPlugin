using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Resource.Handle;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Resource
{
    // Client::System::Resource::ResourceManager
    // no vtbl

    // size = 0x1728
    // ctor E8 ? ? ? ? 48 89 05 ? ? ? ? 48 8B 08
    [StructLayout(LayoutKind.Explicit, Size = 0x1728)]
    public unsafe struct ResourceManager
    {
        [FieldOffset(0x8)] public ResourceGraph* ResourceGraph;



    }
}