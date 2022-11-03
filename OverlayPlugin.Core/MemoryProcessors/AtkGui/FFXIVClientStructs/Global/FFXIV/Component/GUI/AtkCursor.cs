using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.GUI
{

    [StructLayout(LayoutKind.Explicit, Size = 0x10)]
    public struct AtkCursor
    {
        [FieldOffset(0x00)] public CursorType Type;
        [FieldOffset(0x0E)] public byte Visible;




        public enum CursorType : byte
        {
            Arrow,
            Boot,
            Search,
            ChatPointer,
            Interact,
            Attack,
            Hand,
            ResizeWE,
            ResizeNS,
            ResizeNWSR,
            ResizeNESW,
            Clickable,
            TextInput,
            TextClick,
            Grab,
            ChatBubble,
            NoAccess,
            Hidden
        }
    }
}