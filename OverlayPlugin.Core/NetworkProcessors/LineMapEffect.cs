using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.NetworkProcessors.PacketHelper;

namespace RainbowMage.OverlayPlugin.NetworkProcessors
{
    class LineMapEffect : LineBaseCustom<
            Server_MessageHeader_Global, LineMapEffect.MapEffect_v62,
            Server_MessageHeader_CN, LineMapEffect.MapEffect_v62,
            Server_MessageHeader_KR, LineMapEffect.MapEffect_v62>
    {
        [StructLayout(LayoutKind.Explicit)]
        internal struct MapEffect_v62 : IPacketStruct
        {
            [FieldOffset(0x0)]
            public uint instanceContentID;
            [FieldOffset(0x4)]
            public uint flags;
            [FieldOffset(0x8)]
            public byte index;
            [FieldOffset(0x9)]
            public byte unknown1;
            [FieldOffset(0x10)]
            public ushort unknown2;

            public string ToString(long epoch, uint ActorID)
            {
                return $"{instanceContentID:X8}|{flags:X8}|{index:X2}|{unknown1:X2}|{unknown2:X4}";
            }
        }

        public const uint LogFileLineID = 257;
        public const string logLineName = "MapEffect";
        public const string MachinaPacketName = "MapEffect";

        public LineMapEffect(TinyIoCContainer container)
            : base(container, LogFileLineID, logLineName, MachinaPacketName) { }
    }
}
