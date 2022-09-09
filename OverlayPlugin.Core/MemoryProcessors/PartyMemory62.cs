using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace RainbowMage.OverlayPlugin.MemoryProcessors
{
    public class PartyMemory62 : PartyMemoryCommon
    {
        private FFXIVMemory memory;
        private ILogger logger;
        private DateTime lastSigScan = DateTime.MinValue;
        private uint loggedScanErrors = 0;

        private IntPtr partyAddress = IntPtr.Zero;

        private const string partyListSignature = "8B7C24440FB66C2440400FB6D5488D0D";

        // Offsets from the signature to find the correct address.
        private const int partyListSignatureOffset = 0;

        // Constants.
        private const uint emptyID = 0xE0000000;

        public PartyMemory62(TinyIoCContainer container)
        {
            this.memory = new FFXIVMemory(container);
            this.memory.OnProcessChange += ResetPointers;
            this.logger = container.Resolve<ILogger>();
            GetPointerAddress();
        }

        private void ResetPointers(object sender, EventArgs _)
        {
            partyAddress = IntPtr.Zero;
        }

        private bool HasValidPointers()
        {
            if (partyAddress == IntPtr.Zero)
                return false;
            return true;
        }

        public override bool IsValid()
        {
            if (!memory.IsValid())
                return false;

            if (!HasValidPointers())
                GetPointerAddress();

            if (!HasValidPointers())
                return false;

            return true;
        }

        private bool GetPointerAddress()
        {
            if (!memory.IsValid())
                return false;

            // Don't scan too often to avoid excessive CPU load
            if ((DateTime.Now - lastSigScan) < TimeSpan.FromSeconds(5))
                return false;

            lastSigScan = DateTime.Now;
            bool success = true;
            bool bRIP = true;

            List<IntPtr> list = memory.SigScan(partyListSignature, 0, bRIP);
            foreach (var entry in list)
            {
                logger.Log(LogLevel.Warning, "partyAddress: 0x{0:X}", entry.ToInt64());
            }
            if (list != null && list.Count > 0)
            {
                partyAddress = list[0] + partyListSignatureOffset;
            }
            else
            {
                partyAddress = IntPtr.Zero;
                logger.Log(LogLevel.Warning, $"{list != null}, {list != null && list.Count > 0}");
                success = false;
            }

            logger.Log(LogLevel.Debug, "partyAddress: 0x{0:X}", partyAddress.ToInt64());

            return success;
        }

        public override unsafe PartyList GetPartyList()
        {
            var result = new PartyList();
            var seen = new HashSet<uint>();

            if (!IsValid())
            {
                return result;
            }

            byte[] source = memory.GetByteArray(partyAddress, PartyListMemory.Size);
            if (source == null || source.Length == 0)
                return result;

            fixed (byte* p = source)
            {
                PartyListMemory list = *(PartyListMemory*)&p[0];

                if (list.PartySize > 27 || list.PartySize < 0)
                    return result;
                
                for (var i = 0; i < list.PartySize; ++i)
                {
                    PartyListEntryMemory entry = *(PartyListEntryMemory*)&list.Entries[PartyListEntryMemory.Size * i];
                    result.Entries.Add(new PartyEntry() {
                        ID = entry.ObjectID,
                        Name = FFXIVMemory.GetStringFromBytes(entry.Name, PartyListEntryMemory.nameBytes),
                        Flags = entry.Flags,
                        RawEntry = GetRawEntry(entry),
                    });
                }

                result.Address = (ulong)partyAddress.ToInt64();
            }

            return result;
        }

        private unsafe JObject GetRawEntry(PartyListEntryMemory entry)
        {
            JObject ret = JObject.FromObject(entry);
            List<EffectEntry> effects = new List<EffectEntry>();

            for (int i = 0; i < 30; ++i)
            {
                EffectEntry effect = *(EffectEntry*)&entry.Effects[i * 0xC];
                if (effect.BuffID > 0 &&
                    effect.Stack >= 0 &&
                    effect.Timer >= 0.0f &&
                    effect.ActorID > 0)
                {
                    effects.Add(effect);
                }
            }

            ret["Effects"] = JArray.FromObject(effects);

            return ret;
        }

        [StructLayout(LayoutKind.Explicit, Size = Size, Pack = 1)]
        public unsafe struct PartyListEntryMemory
        {
            public const int Size = 0x230;
            public const int nameBytes = 64;

            [FieldOffset(0)]
            public ulong CharacterPointer;

            [FieldOffset(8)]
            public fixed byte Effects[EffectEntry.Size * 30];

            [FieldOffset(0x170)]
            public uint Unk_170;

            [FieldOffset(0x174)]
            public ushort Unk_174;

            [FieldOffset(0x178)]
            public long Unk_178;

            [FieldOffset(0x180)]
            public byte Unk_180;

            [FieldOffset(0x190)]
            public float X;
            [FieldOffset(0x194)]
            public float Y;
            [FieldOffset(0x198)]
            public float Z;
            [FieldOffset(0x1A0)]
            public long ContentID;
            [FieldOffset(0x1A8)]
            public uint ObjectID;
            [FieldOffset(0x1AC)]
            public uint Unk_ObjectID_1;
            [FieldOffset(0x1B0)]
            public uint Unk_ObjectID_2;
            [FieldOffset(0x1B4)]
            public uint CurrentHP;
            [FieldOffset(0x1B8)]
            public uint MaxHP;
            [FieldOffset(0x1BC)]
            public ushort CurrentMP;
            [FieldOffset(0x1BE)]
            public ushort MaxMP;
            [FieldOffset(0x1C0)]
            public ushort TerritoryType;
            [FieldOffset(0x1C2)]
            public ushort HomeWorld;
            [FieldOffset(0x1C4)]
            public fixed byte Name[0x40];
            [FieldOffset(0x204)]
            public byte Sex;
            [FieldOffset(0x205)]
            public byte ClassJob;
            [FieldOffset(0x206)]
            public byte Level;

            // 0x18 byte struct at 0x208
            [FieldOffset(0x208)]
            public byte Unk_208;
            [FieldOffset(0x20C)]
            public uint Unk_20C;
            [FieldOffset(0x210)]
            public ushort Unk_210;
            [FieldOffset(0x214)]
            public uint Unk_214;
            [FieldOffset(0x218)]
            public ushort Unk_218;
            [FieldOffset(0x21A)]
            public ushort Unk_21A;
            [FieldOffset(0x220)]
            public byte Flags;
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct PartyListMemory
        {
            public static int Size => Marshal.SizeOf(typeof(PartyListMemory));

            [FieldOffset(0)]
            public unsafe fixed byte Entries[PartyListEntryMemory.Size * 28];
            [FieldOffset(15700)]
            public byte PartyFlag1;
            [FieldOffset(15708)]
            public byte PartySize;
        }

        [StructLayout(LayoutKind.Explicit, Size = Size, Pack = 1)]
        public unsafe struct EffectEntry
        {
            public const int Size = 0xC;
            [FieldOffset(0)]
            public ushort BuffID;
            [FieldOffset(2)]
            public ushort Stack;
            [FieldOffset(4)]
            public float Timer;
            [FieldOffset(8)]
            public uint ActorID;
        }
    }
}
