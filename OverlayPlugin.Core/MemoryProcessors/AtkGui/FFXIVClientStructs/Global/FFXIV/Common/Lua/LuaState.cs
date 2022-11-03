using System;

using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Common.Lua
{

    //ctor 48 8D 05 ?? ?? ?? ?? C6 41 10 01 48 89 01 33 C0
    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    public unsafe struct LuaState
    {
        [FieldOffset(0x08)] public lua_State* State;
        [FieldOffset(0x10)] public bool GCEnabled;
        [FieldOffset(0x18)] public long LastGCRestart;
        [FieldOffset(0x20)] public void* db_errorfb;

    }

    [StructLayout(LayoutKind.Explicit, Size = 0xB0)]
    public unsafe struct lua_State
    {
























    }

    public enum LuaType
    {
        None = -1,
        Nil,
        Boolean,
        LightUserData,
        Number,
        String,
        Table,
        Function,
        UserData,
        Thread,
        Proto,
        Upval
    }
}