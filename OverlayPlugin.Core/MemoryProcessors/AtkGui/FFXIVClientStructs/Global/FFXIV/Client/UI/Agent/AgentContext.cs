using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Object;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Framework;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.String;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Agent
{

    [StructLayout(LayoutKind.Explicit, Size = 0x1748)]
    public unsafe struct AgentContext
    {

        [FieldOffset(0x00)] public AgentInterface AgentInterface;

        [FieldOffset(0x28)] public fixed byte ContextMenuArray[0x678 * 2];
        [FieldOffset(0x28)] public ContextMenu MainContextMenu;
        [FieldOffset(0x6A0)] public ContextMenu SubContextMenu;

        [FieldOffset(0xD18)] public ContextMenu* CurrentContextMenu;
        [FieldOffset(0xD20)] public Utf8String ContextMenuTitle;
        [FieldOffset(0xD88)] public Point Position;
        [FieldOffset(0xD90)] public uint OwnerAddon;

        [FieldOffset(0xDA0)] public ContextMenuTarget ContextMenuTarget;
        [FieldOffset(0xE00)] public ContextMenuTarget* CurrentContextMenuTarget;
        [FieldOffset(0xE08)] public Utf8String TargetName;
        [FieldOffset(0xE70)] public Utf8String YesNoTargetName;

        [FieldOffset(0xEE0)] public ulong TargetContentId;
        [FieldOffset(0xEE8)] public ulong YesNoTargetContentId;
        [FieldOffset(0xEF0)] public GameObjectID TargetObjectId;
        [FieldOffset(0xEF8)] public GameObjectID YesNoTargetObjectId;
        [FieldOffset(0xF00)] public short TargetHomeWorldId;
        [FieldOffset(0xF02)] public short YesNoTargetHomeWorldId;
        [FieldOffset(0xF04)] public byte YesNoEventId;

        [FieldOffset(0xF08)] public int TargetGender;
        [FieldOffset(0xF0C)] public uint TargetMountSeats;

        [FieldOffset(0x1730)]
        public void* UpdateChecker; // AgentContextUpdateChecker*, if handler returns false the menu closes

        [FieldOffset(0x1738)]
        public long UpdateCheckerParam; //objectid of the target or list index of an addon or other things

        [FieldOffset(0x1740)] public byte ContextMenuIndex;
        [FieldOffset(0x1741)] public byte OpenAtPosition; // if true menu opens at Position else at cursor location















    }

    [StructLayout(LayoutKind.Explicit, Size = 0x678)]
    public unsafe struct ContextMenu
    {
        [FieldOffset(0x00)] public short CurrentEventIndex;
        [FieldOffset(0x02)] public short CurrentEventId;

        [FieldOffset(0x08)]
        public fixed byte EventParams[0x10 * 33]; // 32 * AtkValue + 1 * AtkValue for submenus with title


        [FieldOffset(0x428)] public fixed byte EventIdArray[32];
        [FieldOffset(0x450)] public fixed long EventHandlerArray[32];
        [FieldOffset(0x558)] public fixed long EventHandlerParamArray[32];

        [FieldOffset(0x660)] public uint ContextItemDisabledMask;
        [FieldOffset(0x664)] public uint ContextSubMenuMask;
        [FieldOffset(0x668)] public byte* ContextTitleString;
        [FieldOffset(0x670)] public byte SelectedContextItemIndex;
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x60)]
    public unsafe struct ContextMenuTarget
    {
        [FieldOffset(0x00)] public ulong ContentId;
        [FieldOffset(0x14)] public byte AddonListIndex;
        [FieldOffset(0x16)] public ushort CurrentWorldId;
        [FieldOffset(0x18)] public ushort HomeWorldId;
        [FieldOffset(0x1A)] public ushort TerritoryTypeId;
        [FieldOffset(0x1C)] public byte GrandCompany;
        [FieldOffset(0x1D)] public byte ClientLanguage;
        [FieldOffset(0x1E)] public byte LanguageBitmask;
        [FieldOffset(0x20)] public byte Gender;
        [FieldOffset(0x21)] public byte ClassJobId;
        [FieldOffset(0x22)] public fixed byte Name[32];
        [FieldOffset(0x42)] public fixed byte FcName[14];
        [FieldOffset(0x50)] public void* Unk_Info_Ptr;
    }
}