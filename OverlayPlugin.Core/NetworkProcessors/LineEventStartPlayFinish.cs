using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors;

/*
Example lines from fishing:
cast line was done before capturing.
exclamation point animation:
275|2024-05-12T00:43:34.1040000-04:00|1|10001234|0|150001|5|40001|0|1|125|4927e2b6b7d6d118
reeling animation:
275|2024-05-12T00:43:34.8510000-04:00|4|10001234|0|150001|6|40001|0|4|11C|120|10F|0|b0042cd36facf644
finish reeling animation:
275|2024-05-12T00:43:44.2490000-04:00|1|10001234|0|150001|2|40001|0|2|0|359f9119b64af7fd
capture animation:
275|2024-05-12T00:43:44.5690000-04:00|8|10001234|0|150001|4|40001|0|5|10F|0|1|1|7|0|0|0|157dc732c1530126
 */
namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct EventPlayBase
    {
        fixed byte header[32];

        uint actorID;
        uint unknown1;
        uint eventID;
        ushort scene;
        ushort padding;
        uint flags;
        uint param3;
        byte param4;
        byte padding1;
        ushort padding2;

        public unsafe string ToString(uint* eventParams, int eventParamsCount)
        {
            var retString = $"{actorID:X8}|{unknown1:X}|{eventID:X}|{scene:X}|{flags:X}|{param3:X}|{param4:X}";

            for (var i = 0; i < eventParamsCount; ++i)
            {
                retString += $"|{eventParams[i]:X}";
            }

            return retString;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct EventPlay1
    {
        EventPlayBase eventPlayBase;

        const int eventParamsCount = 1;

        fixed uint eventParams[eventParamsCount];

        public unsafe override string ToString()
        {
            fixed (uint* paramsPtr = eventParams)
            {
                return eventPlayBase.ToString(paramsPtr, eventParamsCount);
            }
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct EventPlay4
    {
        EventPlayBase eventPlayBase;

        const int eventParamsCount = 4;

        fixed uint eventParams[eventParamsCount];

        public unsafe override string ToString()
        {
            fixed (uint* paramsPtr = eventParams)
            {
                return eventPlayBase.ToString(paramsPtr, eventParamsCount);
            }
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct EventPlay8
    {
        EventPlayBase eventPlayBase;

        const int eventParamsCount = 8;

        fixed uint eventParams[eventParamsCount];

        public unsafe override string ToString()
        {
            fixed (uint* paramsPtr = eventParams)
            {
                return eventPlayBase.ToString(paramsPtr, eventParamsCount);
            }
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct EventPlay16
    {
        EventPlayBase eventPlayBase;

        const int eventParamsCount = 16;

        fixed uint eventParams[eventParamsCount];

        public unsafe override string ToString()
        {
            fixed (uint* paramsPtr = eventParams)
            {
                return eventPlayBase.ToString(paramsPtr, eventParamsCount);
            }
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct EventPlay32
    {
        EventPlayBase eventPlayBase;

        const int eventParamsCount = 32;

        fixed uint eventParams[eventParamsCount];

        public unsafe override string ToString()
        {
            fixed (uint* paramsPtr = eventParams)
            {
                return eventPlayBase.ToString(paramsPtr, eventParamsCount);
            }
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct EventPlay64
    {
        EventPlayBase eventPlayBase;

        const int eventParamsCount = 64;

        fixed uint eventParams[eventParamsCount];

        public unsafe override string ToString()
        {
            fixed (uint* paramsPtr = eventParams)
            {
                return eventPlayBase.ToString(paramsPtr, eventParamsCount);
            }
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct EventPlay128
    {
        EventPlayBase eventPlayBase;

        const int eventParamsCount = 128;

        fixed uint eventParams[eventParamsCount];

        public unsafe override string ToString()
        {
            fixed (uint* paramsPtr = eventParams)
            {
                return eventPlayBase.ToString(paramsPtr, eventParamsCount);
            }
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct EventPlay255
    {
        EventPlayBase eventPlayBase;

        const int eventParamsCount = 255;

        fixed uint eventParams[eventParamsCount];

        public unsafe override string ToString()
        {
            fixed (uint* paramsPtr = eventParams)
            {
                return eventPlayBase.ToString(paramsPtr, eventParamsCount);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct EventStart
    {
        fixed byte header[32];

        uint actorID;
        uint unknown1;
        uint eventID;
        byte param1;
        byte param2;
        ushort padding;
        uint param3;
        uint padding1;

        public override string ToString()
        {
            return $"{actorID:X8}|{unknown1:X8}|{eventID:X8}|{param1:X2}|{param2:X2}|{param3:X8}";
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct EventFinish
    {
        fixed byte header[32];

        uint eventID;
        byte param1;
        byte param2;
        ushort padding;
        uint param3;
        uint padding1;

        public override string ToString()
        {
            return $"{eventID:X8}|{param1:X2}|{param2:X2}|{param3:X8}";
        }
    };

    public class LineEventStartPlayFinish
    {
        internal class EventMap
        {
            public Type type;
            public string lineName;
            public int packetSize;
        }

        private static readonly Dictionary<string, Type> eventMapClasses = new Dictionary<string, Type>() {
            { "1", typeof(EventPlay1) },
            { "4", typeof(EventPlay4) },
            { "8", typeof(EventPlay8) },
            { "16", typeof(EventPlay16) },
            { "32", typeof(EventPlay32) },
            { "64", typeof(EventPlay64) },
            { "128", typeof(EventPlay128) },
            { "255", typeof(EventPlay255) },
            { "S", typeof(EventStart) },
            { "F", typeof(EventFinish) },
        };

        public const uint LogFileLineID = 275;
        private ILogger logger;
        private OverlayPluginLogLineConfig opcodeConfig;
        private Dictionary<ushort, EventMap> opcodes = null;
        private readonly int offsetMessageType;
        private readonly int offsetPacketData;
        private readonly FFXIVRepository ffxiv;

        private Func<string, DateTime, bool> logWriter;

        public LineEventStartPlayFinish(TinyIoCContainer container)
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
                Name = "LineEventStartPlayFinish",
                Source = "OverlayPlugin",
                ID = LogFileLineID,
                Version = 1,
            });
        }

        private void initOpcodes()
        {
            Dictionary<ushort, EventMap> tmpOpcodes = new Dictionary<ushort, EventMap>();
            foreach (var entry in eventMapClasses)
            {
                var lineName = entry.Key;
                var opcodeType = entry.Value;
                var opcodeName = opcodeType.Name;
                var opcode = opcodeConfig[opcodeName];
                if (opcode == null) return;
                tmpOpcodes.Add((ushort)opcode.opcode, new EventMap()
                {
                    type = opcodeType,
                    lineName = lineName,
                    packetSize = Marshal.SizeOf(opcodeType),
                });
            }
            // Only set the class variable if all opcodes are found
            opcodes = tmpOpcodes;
        }

        private unsafe void MessageReceived(string id, long epoch, byte[] message)
        {
            // Wait for network data to actually fetch opcode info from file and register log line
            // This is because the FFXIV_ACT_Plugin's `GetGameVersion` method only returns valid data
            // if the player is currently logged in/a network connection is active
            if (opcodes == null)
            {
                initOpcodes();
                if (opcodes == null)
                {
                    return;
                }
            }

            // Initial check to make sure we have a full header
            if (message.Length < 32)
                return;

            fixed (byte* buffer = message)
            {
                var messageType = *(ushort*)&buffer[offsetMessageType];
                if (opcodes.TryGetValue(messageType, out var eventMapEntry))
                {
                    if (message.Length < eventMapEntry.packetSize)
                        return;

                    DateTime serverTime = ffxiv.EpochToDateTime(epoch);
                    var packet = Marshal.PtrToStructure(new IntPtr(buffer), eventMapEntry.type);
                    logWriter($"{eventMapEntry.lineName}|" + packet.ToString(), serverTime);

                    return;
                }
            }
        }
    }
}
