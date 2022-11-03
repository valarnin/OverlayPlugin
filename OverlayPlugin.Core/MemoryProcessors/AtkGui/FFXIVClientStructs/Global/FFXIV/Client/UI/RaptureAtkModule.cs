using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Object;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI
{

    // Client::UI::RaptureAtkModule
    //   Component::GUI::AtkModule
    //     Component::GUI::AtkModuleInterface
    [StructLayout(LayoutKind.Explicit, Size = 0x28770)]
    public struct RaptureAtkModule
    {
        [FieldOffset(0x0)] public AtkModule AtkModule;

        [FieldOffset(0x11408)] public RaptureAtkUnitManager RaptureAtkUnitManager;

        [FieldOffset(0x1B390)] public int NameplateInfoCount;
        [FieldOffset(0x1B398)] public NamePlateInfo NamePlateInfoArray; // 0-50, &NamePlateInfoArray[i]


        [StructLayout(LayoutKind.Explicit, Size = 0x248)]
        public struct NamePlateInfo
        {
            [FieldOffset(0x00)] public GameObjectID ObjectID;
            [FieldOffset(0x30)] public Utf8String Name;
            [FieldOffset(0x98)] public Utf8String FcName;
            [FieldOffset(0x100)] public Utf8String Title;
            [FieldOffset(0x168)] public Utf8String DisplayTitle;
            [FieldOffset(0x1D0)] public Utf8String LevelText;
            [FieldOffset(0x240)] public int Flags;

        }
    }
}