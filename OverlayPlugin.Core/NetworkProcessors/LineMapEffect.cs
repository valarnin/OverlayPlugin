using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.NetworkProcessors.PacketHelper;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    using RPH = RegionalizedPacketHelper<
            Server_MessageHeader_Global, LineMapEffect.MapEffect_v62,
            Server_MessageHeader_CN, LineMapEffect.MapEffect_v62,
            Server_MessageHeader_KR, LineMapEffect.MapEffect_v62>;

    public class LineMapEffect
    {
        [StructLayout(LayoutKind.Explicit)]
        internal struct MapEffect_v62 : IPacketStruct
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

            public string ToString(long epoch, uint ActorID)
            {
                return $"{instanceContentID:X8}|{flags:X8}|{index:X2}|{unknown1:X2}|{unknown2:X4}";
            }
        }

        public const uint LogFileLineID = 257;

        private readonly FFXIVRepository ffxiv;

        private Func<string, DateTime, bool> logWriter;
        private RPH packetHelper;
        private GameRegion? currentRegion;

        public LineMapEffect(TinyIoCContainer container)
        {
            ffxiv = container.Resolve<FFXIVRepository>();
            ffxiv.RegisterNetworkParser(MessageReceived);
            ffxiv.RegisterProcessChangedHandler(ProcessChanged);

            var opcodeConfig = container.Resolve<OverlayPluginLogLineConfig>();

            packetHelper = RPH.CreateFromOpcodeConfig(opcodeConfig, "MapEffect");

            if (packetHelper == null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Log(LogLevel.Error, "Failed to initialize LineMapEffect: Failed to create MapEffect packet helper from opcode configs and native structs");
                return;
            }

            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            this.logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Name = "MapEffect",
                Source = "OverlayPlugin",
                ID = LogFileLineID,
                Version = 1,
            });
        }

        private void ProcessChanged(Process process)
        {
            if (!ffxiv.IsFFXIVPluginPresent())
                return;

            currentRegion = null;
        }

        private unsafe void MessageReceived(string id, long epoch, byte[] message)
        {
            if (packetHelper == null)
                return;

            if (currentRegion == null)
                currentRegion = ffxiv.GetMachinaRegion();

            if (currentRegion == null)
                return;

            var line = packetHelper[currentRegion.Value].ToString(epoch, message);

            if (line != null)
            {
                DateTime serverTime = ffxiv.EpochToDateTime(epoch);
                logWriter(line, serverTime);
            }
        }

    }
}
