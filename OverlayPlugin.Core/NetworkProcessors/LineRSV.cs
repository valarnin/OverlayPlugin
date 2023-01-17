using System;
using System.Reflection;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = structSize, Pack = 1)]
    internal unsafe struct RSV_v62
    {
        public const int structSize = 1080;
        public const int keySize = 0x30;
        public const int valueSize = 0x404;
        [FieldOffset(0x0)]
        public uint unknown1;
        [FieldOffset(0x4)]
        public fixed byte key[keySize];
        [FieldOffset(0x34)]
        public fixed byte value[valueSize];

        public override string ToString()
        {
            fixed (byte* key = this.key) fixed (byte* value = this.value)
            {
                return
                    $"|" +
                    $"{unknown1:X8}|" +
                    $"{FFXIVMemory.GetStringFromBytes(key, keySize)}|" +
                    $"{FFXIVMemory.GetStringFromBytes(value, valueSize)}";
            }
        }

        public string ToString(string locale)
        {
            fixed (byte* key = this.key) fixed (byte* value = this.value)
            {
                return
                    $"{locale}|" +
                    $"{unknown1:X8}|" +
                    $"{FFXIVMemory.GetStringFromBytes(key, keySize)}|" +
                    $"{FFXIVMemory.GetStringFromBytes(value, valueSize)}";
            }
        }
    }

    public class LineRSV
    {
        public const uint LogFileLineID = 262;
        private ILogger logger;
        private readonly int offsetMessageType;
        private readonly int offsetPacketData;

        private Func<string, bool> logWriter;
        private FFXIVRepository ffxiv;

        public LineRSV(TinyIoCContainer container)
        {
            logger = container.Resolve<ILogger>();
            ffxiv = container.Resolve<FFXIVRepository>();
            var netHelper = container.Resolve<NetworkParser>();
            if (!ffxiv.IsFFXIVPluginPresent())
                return;
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
                ID = LogFileLineID,
                Version = 1,
            });
        }

        private unsafe void MessageReceived(string id, long epoch, byte[] message)
        {
            if (message.Length < RSV_v62.structSize + offsetPacketData)
                return;

            fixed (byte* buffer = message)
            {
                RSV_v62 RSVPacket = *(RSV_v62*)&buffer[offsetPacketData];
                if (
                    RSVPacket.key[0] == '_' &&
                    RSVPacket.key[1] == 'r' &&
                    RSVPacket.key[2] == 's' &&
                    RSVPacket.key[3] == 'v'
                )
                {
                    logWriter(RSVPacket.ToString(ffxiv.GetLocaleString()));

                    return;
                }
            }
        }
    }
}
