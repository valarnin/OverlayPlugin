﻿using System;
using System.Runtime.InteropServices;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    // This is a copy/paste from Machina, and hasn't changed basically ever.
    // We don't pull this reference directly from Machina to avoid linking and runtime DLL loading issues due to version differences.
    // While this has never changed in the past, we shouldn't assume it won't change in the future, so allow for regional differences
    // if it changes at some point.
    [StructLayout(LayoutKind.Explicit)]
    public struct Server_MessageHeader
    {
        [FieldOffset(0)]
        public uint MessageLength;
        [FieldOffset(4)]
        public uint ActorID;
        [FieldOffset(8)]
        public uint LoginUserID;
        [FieldOffset(12)]
        public uint Unknown1;
        [FieldOffset(16)]
        public ushort Unknown2;
        [FieldOffset(18)]
        public ushort MessageType;
        [FieldOffset(20)]
        public uint Unknown3;
        [FieldOffset(24)]
        public uint Seconds;
        [FieldOffset(28)]
        public uint Unknown4;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Server_MessageHeader_Global : IHeaderStruct
    {
        public Server_MessageHeader header;

        public uint ActorID => header.ActorID;

        public uint Opcode => header.MessageType;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Server_MessageHeader_CN : IHeaderStruct
    {
        public Server_MessageHeader header;

        public uint ActorID => header.ActorID;

        public uint Opcode => header.MessageType;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Server_MessageHeader_KR : IHeaderStruct
    {
        public Server_MessageHeader header;

        public uint ActorID => header.ActorID;

        public uint Opcode => header.MessageType;
    }

    class RegionalizedPacketHelper<
        HeaderStruct_Global, PacketStruct_Global,
        HeaderStruct_CN, PacketStruct_CN,
        HeaderStruct_KR, PacketStruct_KR>
        where HeaderStruct_Global : struct, IHeaderStruct
        where PacketStruct_Global : struct, IPacketStruct
        where HeaderStruct_CN : struct, IHeaderStruct
        where PacketStruct_CN : struct, IPacketStruct
        where HeaderStruct_KR : struct, IHeaderStruct
        where PacketStruct_KR : struct, IPacketStruct
    {
        public readonly PacketHelper<HeaderStruct_Global, PacketStruct_Global> global;
        public readonly PacketHelper<HeaderStruct_CN, PacketStruct_CN> cn;
        public readonly PacketHelper<HeaderStruct_KR, PacketStruct_KR> kr;

        public RegionalizedPacketHelper(ushort globalOpcode, ushort cnOpcode, ushort krOpcode)
        {
            global = new PacketHelper<HeaderStruct_Global, PacketStruct_Global>(globalOpcode);
            cn = new PacketHelper<HeaderStruct_CN, PacketStruct_CN>(cnOpcode);
            kr = new PacketHelper<HeaderStruct_KR, PacketStruct_KR>(krOpcode);
        }

        public IPacketHelper this[GameRegion gameRegion]
        {
            get
            {
                switch (gameRegion)
                {
                    case GameRegion.Global: return global;
                    case GameRegion.Chinese: return global;
                    case GameRegion.Korean: return global;

                    default: return global;
                }
            }
        }

        public static RegionalizedPacketHelper<HeaderStruct_Global, PacketStruct_Global, HeaderStruct_CN, PacketStruct_CN, HeaderStruct_KR, PacketStruct_KR>
            CreateFromMachina(string opcodeName)
        {
            var opcodes = FFXIVRepository.GetMachinaOpcodes();
            if (opcodes == null)
            {
                return null;
            }

            if (!opcodes.TryGetValue(GameRegion.Global, out var globalOpcodes))
            {
                return null;
            }
            if (!opcodes.TryGetValue(GameRegion.Chinese, out var cnOpcodes))
            {
                return null;
            }
            if (!opcodes.TryGetValue(GameRegion.Korean, out var krOpcodes))
            {
                return null;
            }

            if (!globalOpcodes.TryGetValue(opcodeName, out var globalOpcode))
            {
                globalOpcode = 0;
            }
            if (!cnOpcodes.TryGetValue(opcodeName, out var cnOpcode))
            {
                cnOpcode = 0;
            }
            if (!krOpcodes.TryGetValue(opcodeName, out var krOpcode))
            {
                krOpcode = 0;
            }

            return new RegionalizedPacketHelper<HeaderStruct_Global, PacketStruct_Global, HeaderStruct_CN, PacketStruct_CN, HeaderStruct_KR, PacketStruct_KR>
                (globalOpcode, cnOpcode, krOpcode);
        }

        public static RegionalizedPacketHelper<HeaderStruct_Global, PacketStruct_Global, HeaderStruct_CN, PacketStruct_CN, HeaderStruct_KR, PacketStruct_KR>
            CreateFromOpcodeConfig(OverlayPluginLogLineConfig opcodeConfig, string opcodeName)
        {
            var globalOpcodeConfigEntry = opcodeConfig[opcodeName, GameRegion.Global.ToString()];
            var cnOpcodeConfigEntry = opcodeConfig[opcodeName, GameRegion.Chinese.ToString()];
            var krOpcodeConfigEntry = opcodeConfig[opcodeName, GameRegion.Korean.ToString()];

            ushort globalOpcode = (ushort)(globalOpcodeConfigEntry?.opcode ?? 0);
            ushort cnOpcode = (ushort)(cnOpcodeConfigEntry?.opcode ?? 0);
            ushort krOpcode = (ushort)(krOpcodeConfigEntry?.opcode ?? 0);

            return new RegionalizedPacketHelper<HeaderStruct_Global, PacketStruct_Global, HeaderStruct_CN, PacketStruct_CN, HeaderStruct_KR, PacketStruct_KR>
                (globalOpcode, cnOpcode, krOpcode);
        }
    }

    interface IPacketHelper
    {
        string ToString(long epoch, byte[] message);
    }

    class PacketHelper<HeaderStruct, PacketStruct> : IPacketHelper
        where HeaderStruct : struct, IHeaderStruct
        where PacketStruct : struct, IPacketStruct
    {
        public readonly ushort Opcode;

        public PacketHelper(ushort opcode)
        {
            Opcode = opcode;
        }

        /// <summary>
        /// Construct a string representation of a packet from a byte array
        /// </summary>
        /// <param name="epoch">epoch timestamp from FFXIV_ACT_Plugin's NetworkReceivedDelegate</param>
        /// <param name="message">Message byte array received from FFXIV_ACT_Plugin's NetworkReceivedDelegate</param>
        /// <returns>null for invalid packet, otherwise a constructed packet</returns>
        public string ToString(long epoch, byte[] message)
        {
            if (ToStructs(message, out var header, out var packet) == false)
            {
                return null;
            }

            return ToString(epoch, header, packet);
        }

        /// <summary>
        /// Construct a string representation of a packet from a byte array
        /// </summary>
        /// <param name="epoch">epoch timestamp from FFXIV_ACT_Plugin's NetworkReceivedDelegate</param>
        /// <param name="message">Message byte array received from FFXIV_ACT_Plugin's NetworkReceivedDelegate</param>
        /// <returns>null for invalid packet, otherwise a constructed packet</returns>
        public string ToString(long epoch, HeaderStruct header, PacketStruct packet)
        {
            return packet.ToString(epoch, header.ActorID);
        }

        public unsafe bool ToStructs(byte[] message, out HeaderStruct header, out PacketStruct packet)
        {
            int headerSize = Marshal.SizeOf(typeof(HeaderStruct));
            int packetSize = Marshal.SizeOf(typeof(PacketStruct));

            // Message is too short to contain this packet
            if (message.Length < headerSize + packetSize)
            {
                header = default;
                packet = default;

                return false;
            }


            fixed (byte* messagePtr = message)
            {
                var headerPtr = new IntPtr(messagePtr);
                header = Marshal.PtrToStructure<HeaderStruct>(headerPtr);

                if (header.Opcode != Opcode)
                {
                    header = default;
                    packet = default;

                    return false;
                }

                var packetPtr = new IntPtr(messagePtr + headerSize);
                packet = Marshal.PtrToStructure<PacketStruct>(packetPtr);

                return true;
            }
        }
    }

    interface IPacketStruct
    {
        string ToString(long epoch, uint ActorID);
    }

    interface IHeaderStruct
    {
        uint ActorID { get; }
        uint Opcode { get; }
    }
}
