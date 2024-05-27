using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors;
using RainbowMage.OverlayPlugin.NetworkProcessors.PacketHelper;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    using RPH = RegionalizedPacketHelper<
            Server_MessageHeader_Global, LineBattleTalk2.BattleTalk2_v655,
            Server_MessageHeader_CN, LineBattleTalk2.BattleTalk2_v655,
            Server_MessageHeader_KR, LineBattleTalk2.BattleTalk2_v655>;

    public class LineBattleTalk2
    {
        [StructLayout(LayoutKind.Explicit, Size = structSize, Pack = 1)]
        internal unsafe struct BattleTalk2_v655 : IPacketStruct
        {
            // 00|2024-02-25T15:13:29.0000000-05:00|0044|Whiskerwall Kupdi Koop|Mogglesguard, assemble! We must drive them out together, kupo!|e9f836e9767bed2e
            // Pre-processed data from packet (sorry, no raw packet for this one, instead it's my scuffed debugging packet dump's data)
            // 00000000|00000000|80034E2B|000002CE|33804|5000|0|2|0|0
            // first 4 bytes are actor ID, not always set
            // 0x80034E2B = instance content ID
            // 0x2CE = entry on `BNpcName` table
            // 33804 = entry on `InstanceContentTextData` table
            // 5000 = display time in ms
            // 2 = some sort of flags for display settings?

            public const int structSize = 40;
            [FieldOffset(0x0)]
            public uint actorID;
            [FieldOffset(0x8)]
            public uint instanceContentID;
            [FieldOffset(0xC)]
            public uint npcNameID;
            [FieldOffset(0x10)]
            public uint instanceContentTextID;
            [FieldOffset(0x14)]
            public uint displayMS;
            [FieldOffset(0x18)]
            public uint param1;
            [FieldOffset(0x1C)]
            public uint param2;
            [FieldOffset(0x20)]
            public uint param3;
            [FieldOffset(0x24)]
            public uint param4;

            public string ToString(long epoch, uint ActorID)
            {
                return
                    $"{actorID:X8}|" +
                    $"{instanceContentID:X8}|" +
                    $"{npcNameID:X4}|" +
                    $"{instanceContentTextID:X4}|" +
                    $"{displayMS}|" +
                    $"{param1:X}|" +
                    $"{param2:X}|" +
                    $"{param3:X}|" +
                    $"{param4:X}";
            }
        }

        public const uint LogFileLineID = 267;

        private readonly FFXIVRepository ffxiv;

        private Func<string, DateTime, bool> logWriter;
        private RPH packetHelper;
        private GameRegion? currentRegion;

        public LineBattleTalk2(TinyIoCContainer container)
        {
            ffxiv = container.Resolve<FFXIVRepository>();
            ffxiv.RegisterNetworkParser(MessageReceived);
            ffxiv.RegisterProcessChangedHandler(ProcessChanged);

            var opcodeConfig = container.Resolve<OverlayPluginLogLineConfig>();

            packetHelper = RPH.CreateFromOpcodeConfig(opcodeConfig, "BattleTalk2");

            if (packetHelper == null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Log(LogLevel.Error, "Failed to initialize LineBattleTalk2: Failed to create BattleTalk2 packet helper from opcode configs and native structs");
                return;
            }
            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            this.logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Name = "BattleTalk2",
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
