using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    public class LineActorCastExtra
    {
        public const uint LogFileLineID = 263;
        private ILogger logger;
        private OverlayPluginLogLineConfig opcodeConfig;
        private readonly int packetSize;
        private readonly int packetOpcode;
        private readonly int offsetMessageType;
        private readonly FFXIVRepository ffxiv;
        private readonly Type actorCastType;

        private Func<string, DateTime, bool> logWriter;

        public LineActorCastExtra(TinyIoCContainer container)
        {
            logger = container.Resolve<ILogger>();
            ffxiv = container.Resolve<FFXIVRepository>();
            var netHelper = container.Resolve<NetworkParser>();
            if (!ffxiv.IsFFXIVPluginPresent())
                return;
            opcodeConfig = container.Resolve<OverlayPluginLogLineConfig>();
            try
            {
                var mach = Assembly.Load("Machina.FFXIV");
                var msgHeaderType = mach.GetType("Machina.FFXIV.Headers.Server_MessageHeader");
                actorCastType = mach.GetType("Machina.FFXIV.Headers.Server_ActorCast");
                packetOpcode = netHelper.GetOpcode("ActorCast");
                packetSize = Marshal.SizeOf(actorCastType);
                offsetMessageType = netHelper.GetOffset(msgHeaderType, "MessageType");
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
                Name = "ActorCastExtra",
                Source = "OverlayPlugin",
                ID = LogFileLineID,
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
                    object packet = actorCastType.GetConstructor(new Type[] { }).Invoke(new object[] { });
                    Marshal.PtrToStructure(new IntPtr(buffer), packet);
                    DateTime serverTime = ffxiv.EpochToDateTime(epoch);

                    string someData = "";

                    someData += actorCastType.GetField("PosX") + "|";
                    someData += actorCastType.GetField("PosY") + "|";
                    someData += actorCastType.GetField("PosZ") + "|";
                    someData += actorCastType.GetField("Heading");

                    logWriter(someData, serverTime);
                }
            }
        }

    }
}
