using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RainbowMage.OverlayPlugin.NetworkProcessors.PacketHelper
{
    internal static class MachinaMap
    {
        public static readonly Type HeaderType_Global;
        public static readonly Type HeaderType_CN;
        public static readonly Type HeaderType_KR;

        public static readonly ReadOnlyDictionary<GameRegion, ReadOnlyDictionary<string, Type>> packetTypeMap;

        static MachinaMap()
        {
            var machina = Assembly.Load("Machina.FFXIV");
            var allMachinaTypes = machina.GetTypes();
            var globalDict = new Dictionary<string, Type>();
            var chineseDict = new Dictionary<string, Type>();
            var koreanDict = new Dictionary<string, Type>();

            foreach (var mType in allMachinaTypes)
            {
                var parentNamespaceName = mType.Namespace;

                // Wrong namespace
                if (!parentNamespaceName.StartsWith("Machina.FFXIV.Headers.")) continue;
                // Not a struct or enum or primitive
                if (!mType.IsValueType) continue;
                // Check for enum
                if (mType.IsEnum) continue;
                // Check for primitive
                if (mType.IsPrimitive) continue;

                // Only allow structs that are fixed layout of some sort, this avoids potential exceptions when marshaling
                if (!mType.IsExplicitLayout && !mType.IsLayoutSequential) continue;

                switch (parentNamespaceName)
                {
                    case "Machina.FFXIV.Headers.Global":
                        globalDict.Add(mType.Name, mType);
                        break;
                    case "Machina.FFXIV.Headers.Chinese":
                        chineseDict.Add(mType.Name, mType);
                        break;
                    case "Machina.FFXIV.Headers.Korean":
                        koreanDict.Add(mType.Name, mType);
                        break;
                }

                packetTypeMap = new ReadOnlyDictionary<GameRegion, ReadOnlyDictionary<string, Type>>(new Dictionary<GameRegion, ReadOnlyDictionary<string, Type>>() {
                    { GameRegion.Global, new ReadOnlyDictionary<string, Type>(globalDict) },
                    { GameRegion.Chinese, new ReadOnlyDictionary<string, Type>(chineseDict) },
                    { GameRegion.Korean, new ReadOnlyDictionary<string, Type>(koreanDict) },
                });
            }
        }

        public static bool GetPacketType(GameRegion region, string name, out Type packetType)
        {
            packetType = null;
            if (!packetTypeMap.TryGetValue(region, out var map)) return false;
            if (!map.TryGetValue(name, out packetType)) return false;

            return true;
        }
    }

    class MachinaRegionalizedPacketHelper<
        HeaderStruct_Global,
        HeaderStruct_CN,
        HeaderStruct_KR,
        PacketType>
        where HeaderStruct_Global : struct, IHeaderStruct
        where HeaderStruct_CN : struct, IHeaderStruct
        where HeaderStruct_KR : struct, IHeaderStruct
        where PacketType : MachinaPacketWrapper, new()
    {
        public readonly MachinaPacketHelper<HeaderStruct_Global, PacketType> global;
        public readonly MachinaPacketHelper<HeaderStruct_CN, PacketType> cn;
        public readonly MachinaPacketHelper<HeaderStruct_KR, PacketType> kr;

        // @TODO: Might make more sense to just throw an exception instead
        public readonly bool Valid;

        public MachinaRegionalizedPacketHelper(string packetTypeName)
        {
            var opcodes = FFXIVRepository.GetMachinaOpcodes();
            if (opcodes == null)
            {
                Valid = false;
                return;
            }

            if (!opcodes.TryGetValue(GameRegion.Global, out var globalOpcodes))
            {
                Valid = false;
                return;
            }
            if (!opcodes.TryGetValue(GameRegion.Chinese, out var cnOpcodes))
            {
                Valid = false;
                return;
            }
            if (!opcodes.TryGetValue(GameRegion.Korean, out var krOpcodes))
            {
                Valid = false;
                return;
            }

            if (!MachinaMap.GetPacketType(GameRegion.Global, packetTypeName, out var globalPacketType))
            {
                Valid = false;
                return;
            }
            if (!MachinaMap.GetPacketType(GameRegion.Chinese, packetTypeName, out var cnPacketType))
            {
                Valid = false;
                return;
            }
            if (!MachinaMap.GetPacketType(GameRegion.Korean, packetTypeName, out var krPacketType))
            {
                Valid = false;
                return;
            }

            if (!globalOpcodes.TryGetValue(packetTypeName, out var globalOpcode))
            {
                globalOpcode = 0;
            }
            if (!cnOpcodes.TryGetValue(packetTypeName, out var cnOpcode))
            {
                cnOpcode = 0;
            }
            if (!krOpcodes.TryGetValue(packetTypeName, out var krOpcode))
            {
                krOpcode = 0;
            }

            global = new MachinaPacketHelper<HeaderStruct_Global, PacketType>(globalOpcode, globalPacketType);
            cn = new MachinaPacketHelper<HeaderStruct_CN, PacketType>(cnOpcode, cnPacketType);
            kr = new MachinaPacketHelper<HeaderStruct_KR, PacketType>(krOpcode, krPacketType);
        }

        public IPacketHelper this[GameRegion gameRegion]
        {
            get
            {
                switch (gameRegion)
                {
                    case GameRegion.Global: return global;
                    case GameRegion.Chinese: return cn;
                    case GameRegion.Korean: return kr;

                    default: return global;
                }
            }
        }
    }

    class MachinaPacketHelper<HeaderStruct, PacketType> : IPacketHelper
        where HeaderStruct : struct, IHeaderStruct
        where PacketType : MachinaPacketWrapper, new()
    {
        public readonly ushort Opcode;
        public readonly int headerSize;
        public readonly int packetSize;

        public readonly Type packetType;

        public MachinaPacketHelper(ushort opcode, Type packetType)
        {
            Opcode = opcode;
            headerSize = Marshal.SizeOf(typeof(HeaderStruct));
            packetSize = Marshal.SizeOf(packetType);
            this.packetType = packetType;
        }

        /// <summary>
        /// Construct a string representation of a packet from a byte array
        /// </summary>
        /// <param name="epoch">epoch timestamp from FFXIV_ACT_Plugin's NetworkReceivedDelegate</param>
        /// <param name="message">Message byte array received from FFXIV_ACT_Plugin's NetworkReceivedDelegate</param>
        /// <returns>null for invalid packet, otherwise a constructed packet</returns>
        public string ToString(long epoch, byte[] message)
        {
            if (ToStructs(message, out var header, out var packet) == false)
            {
                return null;
            }

            return ToString(epoch, header, packet);
        }

        /// <summary>
        /// Construct a string representation of a packet from a byte array
        /// </summary>
        /// <param name="epoch">epoch timestamp from FFXIV_ACT_Plugin's NetworkReceivedDelegate</param>
        /// <param name="message">Message byte array received from FFXIV_ACT_Plugin's NetworkReceivedDelegate</param>
        /// <returns>null for invalid packet, otherwise a constructed packet</returns>
        public string ToString(long epoch, HeaderStruct header, MachinaPacketWrapper packet)
        {
            return packet.ToString(epoch, header.ActorID);
        }

        public unsafe bool ToStructs(byte[] message, out HeaderStruct header, out PacketType packet)
        {
            // Message is too short to contain this packet
            if (message.Length < headerSize + packetSize)
            {
                header = default;
                packet = null;

                return false;
            }


            fixed (byte* messagePtr = message)
            {
                var ptr = new IntPtr(messagePtr);
                header = Marshal.PtrToStructure<HeaderStruct>(ptr);

                if (header.Opcode != Opcode)
                {
                    header = default;
                    packet = null;

                    return false;
                }

                var packetObj = Marshal.PtrToStructure(ptr, packetType);

                packet = new PacketType
                {
                    packetType = packetType,
                    packetValue = packetObj
                };

                return true;
            }
        }
    }

    abstract class MachinaPacketWrapper : IPacketStruct
    {
        public Type packetType;
        public object packetValue;

        private static Dictionary<Type, Dictionary<string, FieldInfo>> typePropertyMap = new Dictionary<Type, Dictionary<string, FieldInfo>>();

        private Dictionary<string, FieldInfo> propMap = null;

        public static void InitTypePropertyMap(Type type)
        {
            var dict = new Dictionary<string, FieldInfo>();
            foreach (var fieldInfo in type.GetFields())
            {
                dict.Add(fieldInfo.Name, fieldInfo);
            }
            typePropertyMap.Add(type, dict);
        }

        public abstract string ToString(long epoch, uint ActorID);

        public T Get<T>(string name)
        {
            // For performance, we don't check that the dictionary entries exist here
            // The only times they would not exist are a compile-time error, or Machina itself removing/renaming a field/struct
            // which would require an OverlayPlugin version bump anyways

            // Cache the map locally for subsequent calls
            if (propMap == null) propMap = typePropertyMap[packetType];
            return (T)propMap[name].GetValue(packetValue);
        }
    }
}
