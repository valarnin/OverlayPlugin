using System;
using System.Runtime.InteropServices;

namespace FFXIVClientStructs.Global.FFXIV.Client.Game.Object
{
    // Size = 0x1A0
    public unsafe partial struct GameObject
    {
        // `Name` already present, size is 4 bytes smaller (64 bytes)
        public const int NameBytes = 64;

        // New field
        [FieldOffset(0x94)]
        public byte Status;
    }
}