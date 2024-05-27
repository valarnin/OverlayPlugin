using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors;
using RainbowMage.OverlayPlugin.NetworkProcessors.PacketHelper;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    using RPH = RegionalizedPacketHelper<
            Server_MessageHeader_Global, LineActorSetPos.ActorSetPos_v655,
            Server_MessageHeader_CN, LineActorSetPos.ActorSetPos_v655,
            Server_MessageHeader_KR, LineActorSetPos.ActorSetPos_v655>;

    public class LineActorSetPos
    {
        [StructLayout(LayoutKind.Explicit, Size = structSize, Pack = 1)]
        internal unsafe struct ActorSetPos_v655 : IPacketStruct
        {
            // 6.5.5 packet data (minus header):
            // 6AD3 0F  02  00000000 233E3BC1 00000000 D06AF840 00000000
            // AAAA BB  CC  DDDDDDDD EEEEEEEE FFFFFFFF GGGGGGGG HHHHHHHH
            // 0x0  0x2 0x3 0x4      0x8      0xC      0x10     0x14
            // Rot  unk unk unk      X        Y        Z        unk

            // Have never seen data in 0x4 or 0x14, probably just padding?

            public const int structSize = 24;
            [FieldOffset(0x0)]
            public ushort rotation;

            [FieldOffset(0x2)]
            public byte unknown1;

            [FieldOffset(0x3)]
            public byte unknown2;

            // Yes, these are actually floats, and not some janky ushort that needs converted through ConvertUInt16Coordinate
            [FieldOffset(0x8)]
            public float x;
            [FieldOffset(0xC)]
            public float y;
            [FieldOffset(0x10)]
            public float z;

            public string ToString(long epoch, uint ActorID)
            {
                return $"{ActorID:X8}|" +
                    $"{FFXIVRepository.ConvertHeading(rotation):F4}|" +
                    $"{unknown1:X2}|" +
                    $"{unknown2:X2}|" +
                    $"{x:F4}|" +
                    // y and z are intentionally flipped to match other log lines
                    $"{z:F4}|" +
                    $"{y:F4}";
            }
        }

        public const uint LogFileLineID = 271;

        private readonly FFXIVRepository ffxiv;

        private Func<string, DateTime, bool> logWriter;
        private RPH packetHelper;
        private GameRegion? currentRegion;

        public LineActorSetPos(TinyIoCContainer container)
        {
            ffxiv = container.Resolve<FFXIVRepository>();
            ffxiv.RegisterNetworkParser(MessageReceived);
            ffxiv.RegisterProcessChangedHandler(ProcessChanged);

            var opcodeConfig = container.Resolve<OverlayPluginLogLineConfig>();

            packetHelper = RPH.CreateFromOpcodeConfig(opcodeConfig, "ActorSetPos");

            if (packetHelper == null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Log(LogLevel.Error, "Failed to initialize LineActorSetPos: Failed to create ActorSetPos packet helper from opcode configs and native structs");
                return;
            }

            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            this.logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Name = "ActorSetPos",
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
