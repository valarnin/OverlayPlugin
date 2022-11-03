using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.InstanceContent;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Common.Lua;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Event
{

    [StructLayout(LayoutKind.Explicit, Size = 0x3BA8)]
    public unsafe struct EventFramework
    {
        [FieldOffset(0x00)] public EventHandlerModule EventHandlerModule;
        [FieldOffset(0xC0)] public DirectorModule DirectorModule;
        [FieldOffset(0x160)] public LuaActorModule LuaActorModule;
        [FieldOffset(0x1B0)] public EventSceneModule EventSceneModule;

        [FieldOffset(0x3358)] public LuaState* LuaState;
        [FieldOffset(0x3360)] public LuaThread LuaThread;

        [FieldOffset(0x33B8)] public EventState EventState1;
        [FieldOffset(0x3418)] public EventState EventState2;


    }
}