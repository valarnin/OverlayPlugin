using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Graphics.Scene
{

    // Client::Graphics::Scene::Monster
    //   Client::Graphics::Scene::CharacterBase
    //     Client::Graphics::Scene::DrawObject
    //       Client::Graphics::Scene::Object

    // ctor E8 ?? ?? ?? ?? 4C 8B F0 4C 89 B7 
    [StructLayout(LayoutKind.Explicit, Size = 0x900)]
    public unsafe struct Monster
    {
        [FieldOffset(0x0)] public CharacterBase CharacterBase;

        // Expects at least 8 bytes of data.
    }
}
