using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Common.Lua
{

    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    public struct LuaThread
    {
        [FieldOffset(0x00)] public LuaState LuaState;
    }
}