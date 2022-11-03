using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkGui.FFXIVClientStructs
{
    public class ManagedType<T> : DynamicObject where T : unmanaged
    {
        public static ManagedType<T> GetManagedTypeFromIntPtr(IntPtr ptr, FFXIVMemory memory, Dictionary<IntPtr, object> readPtrMap = null)
        {
            if (readPtrMap == null)
            {
                readPtrMap = new Dictionary<IntPtr, object>();
            }
            if (readPtrMap.ContainsKey(ptr))
            {
                return (ManagedType<T>)readPtrMap[ptr];
            }
            return new ManagedType<T>(ptr, memory, readPtrMap);
        }

        public static ManagedType<T> GetManagedTypeFromBaseType(T baseObj, FFXIVMemory memory, Dictionary<IntPtr, object> readPtrMap = null)
        {
            if (readPtrMap == null)
            {
                readPtrMap = new Dictionary<IntPtr, object>();
            }
            return new ManagedType<T>(baseObj, memory, readPtrMap);
        }

        public readonly IntPtr ptr;
        private readonly FFXIVMemory memory;
        private bool haveReadBaseObj = false;
        private Dictionary<string, IntPtr> ptrMap = new Dictionary<string, IntPtr>();
        private Dictionary<string, object> objMap = new Dictionary<string, object>();
        private Dictionary<string, object> valMap = new Dictionary<string, object>();
        private Dictionary<IntPtr, object> readPtrMap = new Dictionary<IntPtr, object>();
        private JObject jObject = null;

        private ManagedType(IntPtr ptr, FFXIVMemory memory, Dictionary<IntPtr, object> readPtrMap)
        {
            this.ptr = ptr;
            this.memory = memory;
            this.readPtrMap = readPtrMap;
            if (ptr.ToInt64() == 0)
            {
                throw new Exception($"NPE! {ptr.ToInt64():X}, {typeof(T).Name}");
            }
            if (readPtrMap.ContainsKey(ptr))
            {
                throw new Exception($"Already read this object! {ptr.ToInt64():X}, {typeof(T).Name}");
            }
            readPtrMap[ptr] = this;
        }

        private ManagedType(T baseObj, FFXIVMemory memory, Dictionary<IntPtr, object> readPtrMap)
        {
            haveReadBaseObj = true;
            ReadBaseObj(baseObj);

            this.memory = memory;
            this.readPtrMap = readPtrMap;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!haveReadBaseObj)
            {
                ReadBaseObj();
            }
            var success = TryGetField(binder.Name, out result);
            if (success)
            {
                return true;
            }
            return base.TryGetMember(binder, out result);
        }
        private bool TryGetField(string name, out object result)
        {
            if (ptrMap.ContainsKey(name))
            {
                if (!objMap.ContainsKey(name))
                {
                    var ptr = ptrMap[name];
                    if (ptr.ToInt64() == 0)
                    {
                        objMap[name] = null;
                        result = null;
                        return true;
                    }
                    if (readPtrMap.ContainsKey(ptr))
                    {
                        result = readPtrMap[ptr];
                        objMap[name] = result;
                        return true;
                    }
                    var obj = typeof(ManagedType<>).MakeGenericType(typeof(T).GetField(name).FieldType.GetElementType())
                        .GetMethod("GetManagedTypeFromIntPtr").Invoke(null, new object[] { ptr, memory, readPtrMap });
                    objMap[name] = obj;
                    result = obj;
                    return true;
                }
                result = objMap[name];
                return true;
            }
            else if (valMap.ContainsKey(name))
            {
                result = valMap[name];
                return true;
            }
            result = null;
            return false;
        }

        public JObject ToJObject()
        {
            if (jObject != null)
            {
                return jObject;
            }
            if (!haveReadBaseObj)
            {
                ReadBaseObj();
            }
            jObject = new JObject();

            foreach (var key in ptrMap.Keys)
            {
                var success = TryGetField(key, out object result);
                if (success && result != null)
                {
                    jObject[key] = ((dynamic)result).ToJObject();
                }
            }

            foreach (var key in valMap.Keys)
            {
                var success = TryGetField(key, out object result);
                if (success)
                {
                    if (result.GetType().IsPrimitive || result.GetType().IsArray)
                    {
                        jObject[key] = JToken.FromObject(result);
                    }
                    else if (result.GetType().IsEnum)
                    {
                        jObject[key] = JToken.FromObject(result.ToString());
                    }
                    else
                    {
                        var t = typeof(ManagedType<>);
                        var dynamicT = t.MakeGenericType(typeof(T).GetField(key).FieldType);
                        var method = dynamicT.GetMethod("GetManagedTypeFromBaseType");
                        dynamic obj = 
                            method.Invoke(null, new object[] { result, memory, readPtrMap });
                        jObject[key] = obj.ToJObject();
                    }
                }
            }

            return jObject;
        }

        private unsafe void ReadBaseObj()
        {
            haveReadBaseObj = true;
            var objSize = Marshal.SizeOf(typeof(T));

            if (ptr.ToInt64() == 0)
            {
                return;
            }

            byte[] source = memory.GetByteArray(ptr, objSize);

            fixed (byte* p = source)
            {
                var memory = *(T*)&p[0];

                ReadBaseObj(memory);
            }
        }

        private unsafe void ReadBaseObj(T memory)
        {
            var p = (byte*)&memory;
            foreach (var field in typeof(T).GetFields())
            {
                FixedBufferAttribute fixedBuffer = (FixedBufferAttribute)field.GetCustomAttribute(typeof(FixedBufferAttribute));
                if (field.FieldType.IsPointer)
                {
                    var value = Pointer.Unbox((Pointer)field.GetValue(memory));
                    // Treat some pointers differently by just returning the memory address
                    if (
                        // <*>** pointers are multidimensional arrays
                        field.FieldType.GetElementType().IsPointer ||
                        // void* pointers are unknown data types
                        field.FieldType.GetElementType() == typeof(void) ||
                        // <T>* pointers are linked lists, attempting to read them leads to a stack overflow exception
                        field.FieldType.GetElementType() == typeof(T)
                    )
                    {
                        valMap[field.Name] = (ulong)value;
                    }
                    else
                    {
                        ptrMap.Add(field.Name, new IntPtr((long)value));
                    }
                }
                else if (fixedBuffer != null)
                {
                    // Fixed buffers get dealt with slightly differently.
                    var offset = GetOffset(typeof(T), field.Name);
                    var elementTypeSize = Marshal.SizeOf(fixedBuffer.ElementType);
                    var elementCount = fixedBuffer.Length / elementTypeSize;
                    var array = Array.CreateInstance(fixedBuffer.ElementType, elementCount);
                    byte* fixedPtr = &p[offset];
                    for (int i = 0; i < elementCount; ++i)
                    {
                        array.SetValue(DynamicCast(fixedBuffer.ElementType, &fixedPtr[i]), i);
                    }
                    valMap[field.Name] = array;
                }
                else
                {
                    valMap[field.Name] = field.GetValue(memory);
                }
            }
        }

        private unsafe object DynamicCast(Type elementType, byte* v)
        {
            switch(elementType.Name)
            {
                case "Boolean":
                    return *(Boolean*)v;
                case "Byte":
                    return *(Byte*)v;
                case "SByte":
                    return *(SByte*)v;
                case "Int16":
                    return *(Int16*)v;
                case "UInt16":
                    return *(UInt16*)v;
                case "Int32":
                    return *(Int32*)v;
                case "UInt32":
                    return *(UInt32*)v;
                case "Int64":
                    return *(Int64*)v;
                case "UInt64":
                    return *(UInt64*)v;
                case "IntPtr":
                    return *(IntPtr*)v;
                case "UIntPtr":
                    return *(UIntPtr*)v;
                case "Char":
                    return *(Char*)v;
                case "Double":
                    return *(Double*)v;
                case "Single":
                    return *(Single*)v;
                default:
                    // Should be impossible
                    return null;
            }
        }

        private int GetOffset(Type type, string property)
        {
            int offset = 0;

            foreach (var prop in type.GetFields())
            {
                var customOffset = prop.GetCustomAttribute<FieldOffsetAttribute>();
                if (customOffset != null)
                {
                    offset = customOffset.Value;
                }

                if (prop.Name == property)
                {
                    break;
                }

                if (prop.FieldType.IsEnum)
                {
                    offset += Marshal.SizeOf(Enum.GetUnderlyingType(prop.FieldType));
                }
                else
                {
                    offset += Marshal.SizeOf(prop.FieldType);
                }
            }

            return offset;
        }
    }
}
