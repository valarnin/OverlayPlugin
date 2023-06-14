using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.Party
{
    public class PartyListEntry
    {
        public float x;
        public float y;
        public float z;
        public long contentID;
        public uint objectID;
        public uint currentHP;
        public uint maxHP;
        public ushort currentMP;
        public ushort maxMP;
        public ushort territoryType;
        public ushort homeWorld;
        public string name;
        public byte sex;
        public byte classJob;
        public byte level;
        public byte flags;
    }

    public class PartyListsStruct
    {
        public long partyId;
        public long partyId_2;
        public uint partyLeaderIndex;
        public byte memberCount;
        public byte allianceFlags;

        public PartyListEntry[] partyMembers;
        public PartyListEntry[] allianceAMembers;
        public PartyListEntry[] allianceBMembers;
        public PartyListEntry[] allianceCMembers;
        public PartyListEntry[] allianceDMembers;
        public PartyListEntry[] allianceEMembers;
    }

    public abstract class PartyMemory
    {
        protected FFXIVMemory memory;
        protected ILogger logger;

        protected IntPtr party1InstanceAddress = IntPtr.Zero;
        protected IntPtr party2InstanceAddress = IntPtr.Zero;

        private long partySingletonAddressInstance1;
        private long partySingletonAddressInstance2;

        public PartyMemory(TinyIoCContainer container, long partySingletonAddressInstance1, long partySingletonAddressInstance2)
        {
            this.partySingletonAddressInstance1 = partySingletonAddressInstance1;
            this.partySingletonAddressInstance2 = partySingletonAddressInstance2;
            logger = container.Resolve<ILogger>();
            memory = container.Resolve<FFXIVMemory>();
        }

        private void ResetPointers()
        {
            party1InstanceAddress = IntPtr.Zero;
        }

        private bool HasValidPointers()
        {
            if (party1InstanceAddress == IntPtr.Zero)
                return false;
            if (party2InstanceAddress == IntPtr.Zero)
                return false;
            return true;
        }

        public bool IsValid()
        {
            if (!memory.IsValid())
                return false;

            if (!HasValidPointers())
                return false;

            return true;
        }

        public void ScanPointers()
        {
            ResetPointers();
            if (!memory.IsValid())
                return;

            List<string> fail = new List<string>();

            // These addresses aren't pointers, they're static memory structures. Therefore we don't need to resolve nested pointers.
            long instanceAddress = memory.GetBaseAddress().ToInt64() + partySingletonAddressInstance1;

            if (instanceAddress != 0)
            {
                party1InstanceAddress = new IntPtr(instanceAddress);
            }
            else
            {
                party1InstanceAddress = IntPtr.Zero;
                fail.Add(nameof(party1InstanceAddress));
            }

            instanceAddress = memory.GetBaseAddress().ToInt64() + partySingletonAddressInstance2;

            if (instanceAddress != 0)
            {
                party2InstanceAddress = new IntPtr(instanceAddress);
            }
            else
            {
                party2InstanceAddress = IntPtr.Zero;
                fail.Add(nameof(party2InstanceAddress));
            }

            logger.Log(LogLevel.Debug, "party1InstanceAddress: 0x{0:X}", party1InstanceAddress.ToInt64());
            logger.Log(LogLevel.Debug, "party2InstanceAddress: 0x{0:X}", party2InstanceAddress.ToInt64());

            if (fail.Count == 0)
            {
                logger.Log(LogLevel.Info, $"Found party memory via {GetType().Name}.");
                return;
            }

            // @TODO: Change this from Debug to Error once we're actually using party
            logger.Log(LogLevel.Debug, $"Failed to find party memory via {GetType().Name}: {string.Join(", ", fail)}.");
            return;
        }

        public abstract Version GetVersion();

        public IntPtr GetPointer()
        {
            if (!IsValid())
                return IntPtr.Zero;
            return party1InstanceAddress;
        }
    }
}
