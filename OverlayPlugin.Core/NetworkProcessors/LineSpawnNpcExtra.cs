using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    public class LineSpawnNpcExtra
    {
        public const uint LogFileLineID = 265;
        private ILogger logger;
        private readonly FFXIVRepository ffxiv;

        private readonly Func<string, DateTime, bool> logWriter;
        private readonly NetworkParser netHelper;

        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct Server_NpcSpawn
        {
            [FieldOffset(0x78)]
            public uint parentActorId;

            [FieldOffset(0x9A)]
            public ushort tetherId;
        }

        private class RegionalizedInfo
        {
            public readonly int packetSize;
            public readonly int packetOpcode;
            public readonly int offsetMessageType;
            public readonly Type headerType;
            public readonly FieldInfo fieldCastSourceId;

            public RegionalizedInfo(Type headerType, NetworkParser netHelper)
            {
                this.headerType = headerType;
                fieldCastSourceId = headerType.GetField("ActorID");
                packetOpcode = netHelper.GetOpcode("NpcSpawn");
                packetSize = Marshal.SizeOf(typeof(Server_NpcSpawn));
                offsetMessageType = netHelper.GetOffset(headerType, "MessageType");
            }
        }

        private RegionalizedInfo regionalized;

        public LineSpawnNpcExtra(TinyIoCContainer container)
        {
            logger = container.Resolve<ILogger>();
            ffxiv = container.Resolve<FFXIVRepository>();
            netHelper = container.Resolve<NetworkParser>();
            ffxiv.RegisterNetworkParser(MessageReceived);
            ffxiv.RegisterProcessChangedHandler(ProcessChanged);

            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Name = "NpcSpawnExtra",
                Source = "OverlayPlugin",
                ID = LogFileLineID,
                Version = 1,
            });
        }

        private void ProcessChanged(Process process)
        {
            if (!ffxiv.IsFFXIVPluginPresent())
                return;

            try
            {
                Assembly mach = Assembly.Load("Machina.FFXIV");
                Type headerType = mach.GetType("Machina.FFXIV.Headers.Server_MessageHeader");
                regionalized = new RegionalizedInfo(headerType, netHelper);
            }
            catch
            {
            }
        }

        private unsafe void MessageReceived(string id, long epoch, byte[] message)
        {
            var info = regionalized;
            if (info == null)
              return;

            if (message.Length < info.packetSize)
                return;

            fixed (byte* buffer = message)
            {
                if (*(ushort*)&buffer[info.offsetMessageType] == info.packetOpcode)
                {
                    object header = Marshal.PtrToStructure(new IntPtr(buffer), info.headerType);
                    UInt32 sourceId = (UInt32)info.fieldCastSourceId.GetValue(header);

                    var packet = Marshal.PtrToStructure<Server_NpcSpawn>(new IntPtr(buffer));
                    var parentActorId = packet.parentActorId;
                    var tetherId = packet.tetherId;

                    string line = string.Format(CultureInfo.InvariantCulture,
                        "{0:X8}|{1:X8}|{2:X4}",
                        sourceId, parentActorId, tetherId);

                    DateTime serverTime = ffxiv.EpochToDateTime(epoch);
                    logWriter(line, serverTime);
                }
            }
        }
    }
}