using System;

using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Info
{

    [StructLayout(LayoutKind.Explicit, Size = 0x14A0)]
    public unsafe struct InfoProxyCrossRealm
    {
        [FieldOffset(0x00)] public void** Vtbl;

        [FieldOffset(0x08)] public UIModule* UiModule;
        // memset((void *)(a1 + 0x30),  0, 0x358ui64);
        // memset((void *)(a1 + 0x3A0), 0, 0xF30ui64);

        [FieldOffset(0x38D)] public byte LocalPlayerGroupIndex;
        [FieldOffset(0x38E)] public byte GroupCount;

        [FieldOffset(0x390)] public byte IsCrossRealm; //i guess?
        [FieldOffset(0x391)] public byte IsInAllianceRaid;
        [FieldOffset(0x392)] public byte IsPartyLeader;
        [FieldOffset(0x393)] public byte IsInCrossRealmParty;

        [FieldOffset(0x3A0)] public fixed byte CrossRealmGroupArray[6 * 0x288];











    }

    [StructLayout(LayoutKind.Explicit, Size = 0x288)]
    public unsafe struct CrossRealmGroup
    {
        [FieldOffset(0x00)] public byte GroupMemberCount;
        [FieldOffset(0x08)] public fixed byte GroupMembers[8 * 0x50];

    }

    [StructLayout(LayoutKind.Explicit, Size = 0x50)]
    public unsafe struct CrossRealmMember
    {
        [FieldOffset(0x00)] public ulong ContentId;
        [FieldOffset(0x10)] public uint ObjectId;
        [FieldOffset(0x18)] public byte Level;
        [FieldOffset(0x1A)] public short HomeWorld;
        [FieldOffset(0x1C)] public short CurrentWorld;
        [FieldOffset(0x1E)] public byte ClassJobId;
        [FieldOffset(0x22)] public fixed byte Name[30];
        [FieldOffset(0x48)] public byte MemberIndex;
        [FieldOffset(0x49)] public byte GroupIndex;
        [FieldOffset(0x4B)] public byte IsPartyLeader;
    }
}
