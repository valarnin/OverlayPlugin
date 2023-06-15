using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using FFXIVClientStructs.Global.FFXIV.Client.Game.Group;
using RainbowMage.OverlayPlugin.MemoryProcessors.FFXIVClientStructs;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.Party
{
    interface IPartyMemory64 : IPartyMemory { }

    class PartyMemory64 : PartyMemory, IPartyMemory64
    {
        private static long GetPartySingletonAddress(TinyIoCContainer container, int instance)
        {
            var data = container.Resolve<Data>();
            return (long)data.GetClassInstanceAddress(DataNamespace.Global, "Client::Game::Group::GroupManager", instance);
        }

        public PartyMemory64(TinyIoCContainer container) : base(container, GetPartySingletonAddress(container, 0), GetPartySingletonAddress(container, 1)) { }

        public override Version GetVersion()
        {
            return new Version(6, 4);
        }


        public unsafe PartyListsStruct GetPartyLists()
        {
            if (!IsValid())
            {
                return new PartyListsStruct();
            }

            if (party1InstanceAddress.ToInt64() == 0)
            {
                return new PartyListsStruct();
            }
            var groupManager1 = ManagedType<GroupManager>.GetManagedTypeFromIntPtr(party1InstanceAddress, memory).ToType();
            var groupManager2 = ManagedType<GroupManager>.GetManagedTypeFromIntPtr(party2InstanceAddress, memory).ToType();

            // `PartyMembers` is a standard array, members are moved up/down as they're added/removed.
            // As such, limit extracting members to the current count to avoid "ghost" members
            var partyMembers = extractPartyMembers(groupManager1.PartyMembers, Math.Min((int)groupManager1.MemberCount, 8));

            // `AllianceMembers` is a fixed-position array, with removed elements being mostly zero'd out
            // Easiest way to check if an entry is still active is to check for `Flags != 0`
            var allianceAMembers = extractAllianceMembers(groupManager1.AllianceMembers, 20, 0, 8);
            var allianceBMembers = extractAllianceMembers(groupManager1.AllianceMembers, 20, 8, 8);
            // TOOD: Actually verify C/D/E alliance info?
            var allianceCMembers = extractAllianceMembers(groupManager2.PartyMembers, 8, 0, 8);
            var allianceDMembers = extractAllianceMembers(groupManager2.AllianceMembers, 20, 0, 8);
            var allianceEMembers = extractAllianceMembers(groupManager2.AllianceMembers, 20, 8, 8);

            return new PartyListsStruct()
            {
                partyId = groupManager1.PartyId,
                partyId_2 = groupManager1.PartyId_2,
                partyLeaderIndex = groupManager1.PartyLeaderIndex,
                memberCount = groupManager1.MemberCount,
                allianceFlags = groupManager1.AllianceFlags,

                Unk_3D40 = groupManager1.Unk_3D40,

                partyMembers = partyMembers,
                allianceAMembers = allianceAMembers,
                allianceBMembers = allianceBMembers,
                allianceCMembers = allianceCMembers,
                allianceDMembers = allianceDMembers,
                allianceEMembers = allianceEMembers,
            };
        }

        private unsafe PartyListEntry[] extractAllianceMembers(byte* allianceMembers, int elementCount, int start, int count)
        {
            var allMembers = extractPartyMembers(allianceMembers, elementCount);
            var retMembers = new PartyListEntry[count];
            for (var i = start; i < start + count && i < allMembers.Length; ++i)
            {
                var member = allMembers[i];
                if (member.flags == 0)
                {
                    continue;
                }

                retMembers[i - start] = member;
            }
            return retMembers;
        }

        private unsafe PartyListEntry[] extractPartyMembers(byte* ptr, int count)
        {
            var ret = new PartyListEntry[count];
            for (int i = 0; i < count; ++i)
            {
                var member = Marshal.PtrToStructure<PartyMember>(new IntPtr(ptr + (i * sizeof(PartyMember))));
                ret[i] = new PartyListEntry()
                {
                    x = member.X,
                    y = member.Y,
                    z = member.Z,
                    contentID = member.ContentID,
                    objectID = member.ObjectID,
                    currentHP = member.CurrentHP,
                    maxHP = member.MaxHP,
                    currentMP = member.CurrentMP,
                    maxMP = member.MaxMP,
                    territoryType = member.TerritoryType,
                    homeWorld = member.HomeWorld,
                    name = FFXIVMemory.GetStringFromBytes(member.Name, 0x40),
                    sex = member.Sex,
                    classJob = member.ClassJob,
                    level = member.Level,
                    flags = member.Flags,
                };
            }
            return ret;
        }

    }
}
