using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.NetworkProcessors.PacketHelper;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    public class LineActorCastExtra
    {
        public const uint LogFileLineID = 263;
        private readonly FFXIVRepository ffxiv;

        internal class ActorCastExtraPacket : MachinaPacketWrapper
        {
            public override string ToString(long epoch, uint ActorID)
            {
                UInt16 abilityId = Get<UInt16>("ActionID");

                // for x/y/x, subtract 7FFF then divide by (2^15 - 1) / 100
                float x = FFXIVRepository.ConvertUInt16Coordinate(Get<UInt16>("PosX"));
                // In-game uses Y as elevation and Z as north-south, but ACT convention is to use
                // Z as elevation and Y as north-south.
                float y = FFXIVRepository.ConvertUInt16Coordinate(Get<UInt16>("PosZ"));
                float z = FFXIVRepository.ConvertUInt16Coordinate(Get<UInt16>("PosY"));
                // for rotation, the packet uses '0' as north, and each increment is 1/65536 of a CCW turn, while
                // in-game uses 0=south, pi/2=west, +/-pi=north
                // Machina thinks this is a float but that appears to be incorrect, so we have to reinterpret as
                // a UInt16
                double h = FFXIVRepository.ConvertHeading(FFXIVRepository.InterpretFloatAsUInt16(Get<float>("Rotation")));

                return string.Format(CultureInfo.InvariantCulture,
                    "{0:X8}|{1:X4}|{2:F3}|{3:F3}|{4:F3}|{5:F3}",
                    ActorID, abilityId, x, y, z, h);
            }
        }

        private MachinaRegionalizedPacketHelper<ActorCastExtraPacket> packetHelper;
        private GameRegion? currentRegion;

        private readonly Func<string, DateTime, bool> logWriter;

        public LineActorCastExtra(TinyIoCContainer container)
        {
            ffxiv = container.Resolve<FFXIVRepository>();
            ffxiv.RegisterNetworkParser(MessageReceived);
            ffxiv.RegisterProcessChangedHandler(ProcessChanged);

            if (MachinaRegionalizedPacketHelper<ActorCastExtraPacket>.Create("ActorCast", out packetHelper))
            {
                var customLogLines = container.Resolve<FFXIVCustomLogLines>();
                logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
                {
                    Name = "ActorCastExtra",
                    Source = "OverlayPlugin",
                    ID = LogFileLineID,
                    Version = 1,
                });
            }
            else
            {
                var logger = container.Resolve<ILogger>();
                logger.Log(LogLevel.Error, "Failed to initialize LineActorCastExtra: Failed to create ActorCast packet helper from Machina structs");
            }
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