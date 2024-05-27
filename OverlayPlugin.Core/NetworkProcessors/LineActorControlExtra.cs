using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.NetworkProcessors.PacketHelper;

// The easiest place to test SetAnimationState is Lunar Subteranne.
// On the first Ruinous Confluence, each staff has this line:
// 273|2023-12-05T10:57:43.4770000-08:00|4000A145|003E|00000001|00000000|00000000|00000000|06e7eff4a949812c
// On the second Ruinous Confluence, each staff has this line:
// 273|2023-12-05T10:58:00.3460000-08:00|4000A144|003E|00000001|00000001|00000000|00000000|a4af9f90928636a3

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    public class LineActorControlExtra
    {
        public const uint LogFileLineID = 273;
        private readonly FFXIVRepository ffxiv;

        // Any category defined in this array will be allowed as an emitted line
        public static readonly Server_ActorControlCategory[] AllowedActorControlCategories = {
            Server_ActorControlCategory.SetAnimationState,
            Server_ActorControlCategory.DisplayPublicContentTextMessage
        };

        internal class ActorControlExtraPacket : MachinaPacketWrapper
        {
            public override string ToString(long epoch, uint ActorID)
            {
                var category = Get<Server_ActorControlCategory>("category");

                if (!AllowedActorControlCategories.Contains(category)) return null;

                var param1 = Get<UInt32>("param1");
                var param2 = Get<UInt32>("param2");
                var param3 = Get<UInt32>("param3");
                var param4 = Get<UInt32>("param4");

                return string.Format(CultureInfo.InvariantCulture,
                            "{0:X8}|{1:X4}|{2:X}|{3:X}|{4:X}|{5:X}",
                            ActorID, (ushort)category, param1, param2, param3, param4);
            }
        }

        private MachinaRegionalizedPacketHelper<ActorControlExtraPacket> packetHelper;
        private GameRegion? currentRegion;

        private readonly Func<string, DateTime, bool> logWriter;

        public LineActorControlExtra(TinyIoCContainer container)
        {
            ffxiv = container.Resolve<FFXIVRepository>();
            ffxiv.RegisterNetworkParser(MessageReceived);
            ffxiv.RegisterProcessChangedHandler(ProcessChanged);

            if (MachinaRegionalizedPacketHelper<ActorControlExtraPacket>.Create("ActorControl", out packetHelper))
            {
                var customLogLines = container.Resolve<FFXIVCustomLogLines>();
                logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
                {
                    Name = "ActorControlExtra",
                    Source = "OverlayPlugin",
                    ID = LogFileLineID,
                    Version = 1,
                });
            }
            else
            {
                var logger = container.Resolve<ILogger>();
                logger.Log(LogLevel.Error, "Failed to initialize LineActorControlExtra: Failed to create ActorControl packet helper from Machina structs");
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