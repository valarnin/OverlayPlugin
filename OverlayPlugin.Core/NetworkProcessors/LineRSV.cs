using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors;
using RainbowMage.OverlayPlugin.NetworkProcessors.PacketHelper;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    using RPH = RegionalizedPacketHelper<
            Server_MessageHeader_Global, LineRSV.RSV_v62,
            Server_MessageHeader_CN, LineRSV.RSV_v62,
            Server_MessageHeader_KR, LineRSV.RSV_v62>;

    public class LineRSV
    {
        [StructLayout(LayoutKind.Explicit, Size = structSize, Pack = 1)]
        internal unsafe struct RSV_v62 : IPacketStruct
        {
            public const int structSize = 1080;
            public const int keySize = 0x30;
            public const int valueSize = 0x404;
            [FieldOffset(0x0)]
            public int valueByteCount;
            [FieldOffset(0x4)]
            public fixed byte key[keySize];
            [FieldOffset(0x34)]
            public fixed byte value[valueSize];

            public string ToString(long epoch, uint ActorID)
            {
                fixed (byte* key = this.key) fixed (byte* value = this.value)
                {
                    return
                        $"{ffxiv.GetLocaleString()}|" +
                        $"{valueByteCount:X8}|" +
                        $"{FFXIVMemory.GetStringFromBytes(key, keySize).Replace("\r", "\\r").Replace("\n", "\\n")}|" +
                        $"{FFXIVMemory.GetStringFromBytes(value, Math.Min(valueByteCount, valueSize)).Replace("\r", "\\r").Replace("\n", "\\n")}";
                }
            }
        }

        public const uint LogFileLineID = 262;

        private static FFXIVRepository ffxiv;

        private Func<string, DateTime, bool> logWriter;
        private RPH packetHelper;
        private GameRegion? currentRegion;

        public LineRSV(TinyIoCContainer container)
        {
            ffxiv = container.Resolve<FFXIVRepository>();
            ffxiv.RegisterNetworkParser(MessageReceived);
            ffxiv.RegisterProcessChangedHandler(ProcessChanged);

            var opcodeConfig = container.Resolve<OverlayPluginLogLineConfig>();

            packetHelper = RPH.CreateFromOpcodeConfig(opcodeConfig, "ActorMove");

            if (packetHelper == null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Log(LogLevel.Error, "Failed to initialize LineActorMove: Failed to create ActorMove packet helper from opcode configs and native structs");
                return;
            }

            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            this.logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Name = "RSVData",
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
