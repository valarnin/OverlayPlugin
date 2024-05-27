using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors;
using RainbowMage.OverlayPlugin.NetworkProcessors.PacketHelper;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    using RPH = RegionalizedPacketHelper<
            Server_MessageHeader_Global, LineCountdownCancel.CountdownCancel_v655,
            Server_MessageHeader_CN, LineCountdownCancel.CountdownCancel_v655,
            Server_MessageHeader_KR, LineCountdownCancel.CountdownCancel_v655>;

    public class LineCountdownCancel
    {
        [StructLayout(LayoutKind.Explicit, Size = structSize, Pack = 1)]
        internal unsafe struct CountdownCancel_v655 : IPacketStruct
        {
            // 6.5.5 packet data (minus header):
            // 34120010 4F00 0000 0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F20
            // AAAAAAAA BBBB CCCC DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD
            // 0x0      0x4  0x6  0x8
            // Actor ID Wrld Unk  Name

            public const int structSize = 40;
            [FieldOffset(0x0)]
            public uint countdownCancellerActorID;
            [FieldOffset(0x4)]
            public ushort countdownCancellerWorldId;

            [FieldOffset(0x8)]
            public fixed byte countdownCancellerName[32];

            public string ToString(long epoch, uint ActorID)
            {
                fixed (byte* name = countdownCancellerName)
                {
                    return
                        $"{countdownCancellerActorID:X8}|" +
                        $"{countdownCancellerWorldId:X4}|" +
                        $"{FFXIVMemory.GetStringFromBytes(name, 32)}";
                }
            }
        }
        public const uint LogFileLineID = 269;

        private readonly FFXIVRepository ffxiv;

        private Func<string, DateTime, bool> logWriter;
        private RPH packetHelper;
        private GameRegion? currentRegion;

        public LineCountdownCancel(TinyIoCContainer container)
        {
            ffxiv = container.Resolve<FFXIVRepository>();
            ffxiv.RegisterNetworkParser(MessageReceived);
            ffxiv.RegisterProcessChangedHandler(ProcessChanged);

            var opcodeConfig = container.Resolve<OverlayPluginLogLineConfig>();

            packetHelper = RPH.CreateFromOpcodeConfig(opcodeConfig, "CountdownCancel");

            if (packetHelper == null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Log(LogLevel.Error, "Failed to initialize LineCountdownCancel: Failed to create CountdownCancel packet helper from opcode configs and native structs");
                return;
            }

            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            this.logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Name = "CountdownCancel",
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
