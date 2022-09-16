using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    internal static class MapEffectOpcodes
    {
        public const uint LogFileLineID = 257;

        public const ushort OPCODE_2022_05_27 = 0x2E0;
        public const ushort SIZE_2022_05_27 = 0x0B;

        public const ushort OPCODE_2022_06_21 = 0x27B;
        public const ushort SIZE_2022_06_21 = 0x0B;

        public const ushort OPCODE_2022_07_08 = 0x7B;
        public const ushort SIZE_2022_07_08 = 0x0B;

        public const ushort OPCODE_2022_08_17 = 0x195;
        public const ushort SIZE_2022_08_17 = 0x0B;

        public const ushort OPCODE_2022_08_20 = 0x9A;
        public const ushort SIZE_2022_08_20 = 0x0B;

        public const ushort OPCODE_2022_08_25 = 0x24A;
        public const ushort SIZE_2022_08_25 = 0x0B;
    }

    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    internal struct MapEffect_v62
    {
        [FieldOffset(0x0)]
        public uint instanceContentID;
        [FieldOffset(0x4)]
        public uint flags;
        [FieldOffset(0x8)]
        public byte index;
        [FieldOffset(0x9)]
        public byte unknown1;
        [FieldOffset(0x10)]
        public ushort unknown2;

        public override string ToString()
        {
            return $"{instanceContentID:X8}|{flags:X8}|{index:X2}|{unknown1:X2}|{unknown2:X4}";
        }
    }

    public class LineMapEffect
    {
        private ILogger logger;
        private readonly ushort packetOpcode;
        private readonly ushort packetSize;
        private readonly int offsetMessageType;
        private readonly int offsetPacketData;

        private Func<string, bool> logWriter;

        [Serializable]
        [StructLayout(LayoutKind.Explicit)]
        public struct MapEffectData
        {

            [FieldOffset(0x20)]
            public uint popTime;
            [FieldOffset(0x24)]
            public ushort timeRemaining;
            [FieldOffset(0x28)]
            public byte ceKey;
            [FieldOffset(0x29)]
            public byte numPlayers;
            [FieldOffset(0x2A)]
            public byte status;
            [FieldOffset(0x2C)]
            public byte progress;
        };

        public LineMapEffect(TinyIoCContainer container)
        {
            logger = container.Resolve<ILogger>();
            var ffxiv = container.Resolve<FFXIVRepository>();
            var netHelper = container.Resolve<NetworkParser>();
            if (!ffxiv.IsFFXIVPluginPresent())
                return;

            switch (ffxiv.GetLocaleString())
            {
                // @TODO: Someone with access to these clients, map these
                case "ko":
                case "cn":
                default:
                    packetOpcode = MapEffectOpcodes.OPCODE_2022_08_25;
                    packetSize = MapEffectOpcodes.SIZE_2022_08_25;
                    break;
            }
            try
            {
                var mach = Assembly.Load("Machina.FFXIV");
                var msgHeaderType = mach.GetType("Machina.FFXIV.Headers.Server_MessageHeader");
                offsetMessageType = netHelper.GetOffset(msgHeaderType, "MessageType");
                offsetPacketData = Marshal.SizeOf(msgHeaderType);
                ffxiv.RegisterNetworkParser(MessageReceived);
            }
            catch (System.IO.FileNotFoundException)
            {
                logger.Log(LogLevel.Error, Resources.NetworkParserNoFfxiv);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, Resources.NetworkParserInitException, e);
            }
            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            this.logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Source = "OverlayPlugin",
                ID = MapEffectOpcodes.LogFileLineID,
                Version = 1,
            });
        }

        private unsafe void MessageReceived(string id, long epoch, byte[] message)
        {
            if (message.Length < packetSize)
                return;

            fixed (byte* buffer = message)
            {
                if (*(ushort*)&buffer[offsetMessageType] == packetOpcode)
                {
                    MapEffect_v62 mapEffectPacket = *(MapEffect_v62*)&buffer[offsetPacketData];
                    logWriter(mapEffectPacket.ToString());

                    return;
                }
            }
        }

    }
}
