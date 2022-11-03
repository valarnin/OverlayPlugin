using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Graphics.Animation;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Graphics.Scene;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Resource.Handle;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Graphics.Render
{
    // Client::Graphics::Render::Skeleton
    //   Client::Graphics::ReferencedClassBase

    [StructLayout(LayoutKind.Explicit, Size = 0x100)]
    public unsafe struct Skeleton
    {
        [FieldOffset(0x0)] public ReferencedClassBase ReferencedClassBase;

        // all Skeleton objects are in a global linked list enforced by base class RenderObjectList
        [FieldOffset(0x10)] public Skeleton* LinkedListPrevious;
        [FieldOffset(0x18)] public Skeleton* LinkedListNext;
        [FieldOffset(0x20)] public Transform Transform;
        [FieldOffset(0x50)] public ushort SkeletonCount;

        // Client::System::Resource::Handle::SkeletonResourceHandle pointer array, size =SkeletonCount
        [FieldOffset(0x58)] public void** SkeletonResourceHandles;

        [FieldOffset(0x60)] public AnimationResourceHandle** AnimationResourceHandles;

        // Client::Graphics::Animation::PartialSkeleton array, size =SkeletonCount
        [FieldOffset(0x68)] public PartialSkeleton* Skeletons;

        [FieldOffset(0xB8)] public CharacterBase* Owner;
    }
}