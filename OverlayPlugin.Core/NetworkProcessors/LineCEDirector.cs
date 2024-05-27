using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.NetworkProcessors.PacketHelper;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    using RPH = RegionalizedPacketHelper<
            Server_MessageHeader_Global, LineCEDirector.CEDirector_v62,
            Server_MessageHeader_CN, LineCEDirector.CEDirector_v62,
            Server_MessageHeader_KR, LineCEDirector.CEDirector_v62>;

    public class LineCEDirector
    {
        [StructLayout(LayoutKind.Explicit)]
        internal struct CEDirector_v62 : IPacketStruct
        {
            [FieldOffset(0x0)]
            public uint popTime;
            [FieldOffset(0x4)]
            public ushort timeRemaining;
            [FieldOffset(0x6)]
            public ushort unk9;
            [FieldOffset(0x8)]
            public byte ceKey;
            [FieldOffset(0x9)]
            public byte numPlayers;
            [FieldOffset(0xA)]
            public byte status;
            [FieldOffset(0xB)]
            public byte unk10;
            [FieldOffset(0xC)]
            public byte progress;
            [FieldOffset(0xD)]
            public byte unk11;
            [FieldOffset(0xE)]
            public byte unk12;
            [FieldOffset(0xF)]
            public byte unk13;

            public string ToString(long epoch, uint ActorID)
            {
                string line =
                    $"{popTime:X8}|" +
                    $"{timeRemaining:X4}|" +
                    $"{unk9:X4}|" +
                    $"{ceKey:X2}|" +
                    $"{numPlayers:X2}|" +
                    $"{status:X2}|" +
                    $"{unk10:X2}|" +
                    $"{progress:X2}|" +
                    $"{unk11:X2}|" +
                    $"{unk12:X2}|" +
                    $"{unk13:X2}";

                var isBeingRemoved = status == 0;
                if (isBeingRemoved)
                {
                    if (!ces.Remove(ceKey))
                    {
                        return null;
                    }
                }
                else
                {
                    string oldData;
                    if (ces.TryGetValue(ceKey, out oldData))
                    {
                        if (oldData == line)
                        {
                            return null;
                        }
                    }
                    ces[ceKey] = line;
                }

                return line;
            }
        }
        public const uint LogFileLineID = 259;

        private readonly FFXIVRepository ffxiv;

        private Func<string, DateTime, bool> logWriter;
        private RPH packetHelper;
        private GameRegion? currentRegion;

        // Used to reduce spam of these packets to log file
        // Only emit a line if it doesn't match the last line for this CE ID
        private static Dictionary<byte, string> ces = new Dictionary<byte, string>();

        public LineCEDirector(TinyIoCContainer container)
        {
            ffxiv = container.Resolve<FFXIVRepository>();
            ffxiv.RegisterNetworkParser(MessageReceived);
            ffxiv.RegisterProcessChangedHandler(ProcessChanged);

            var opcodeConfig = container.Resolve<OverlayPluginLogLineConfig>();

            packetHelper = RPH.CreateFromOpcodeConfig(opcodeConfig, "CEDirector");

            if (packetHelper == null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Log(LogLevel.Error, "Failed to initialize LineCEDirector: Failed to create CEDirector packet helper from opcode configs and native structs");
                return;
            }

            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            this.logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Name = "CEDirector",
                Source = "OverlayPlugin",
                ID = LogFileLineID,
                Version = 1,
            });

            ffxiv.RegisterZoneChangeDelegate((zoneID, zoneName) => ces.Clear());
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
