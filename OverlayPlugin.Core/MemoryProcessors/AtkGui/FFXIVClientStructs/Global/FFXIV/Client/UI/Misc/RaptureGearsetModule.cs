using System;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.System.Framework;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.UI.Misc
{

    [StructLayout(LayoutKind.Explicit, Size = 0xB670)]
    public unsafe struct RaptureGearsetModule
    {
        [FieldOffset(0x0000)] public void* vtbl;
        [FieldOffset(0x0030)] public fixed byte ModuleName[16];
        [FieldOffset(0x0048)] public Gearsets Gearset;

        /// <summary>
        /// Return a pointer to a <see cref="GearsetEntry"/> by index/ID.
        /// </summary>
        /// <param name="gearsetId">The index of the gearset to look up.</param>
        /// <returns>Returns a pointer to a GearsetEntry</returns>

        /// <summary>
        /// Check if a gearset at a specific index is valid.
        /// </summary>
        /// <remarks>
        /// Will return 0 if the gearset index is higher than the player's max allowed gearset number.
        /// </remarks>
        /// <param name="gearsetId">The index of the gearset to look up.</param>
        /// <returns>Returns 1 if the gearset is valid, 0 otherwise.</returns>

        /// <summary>
        /// Attempt to equip a gearset, with certain safety checks in place.
        /// </summary>
        /// <param name="gearsetId">The gearset ID to attempt to equip.</param>
        /// <param name="glamourPlateId">The glamour plate to attempt to equip alongside this gearset. Passing 0 will use the
        /// linked gearset (if any).</param>
        /// <returns>Returns 0 if the equip succeeded, -1 otherwise (???).</returns>

        /// <summary>
        /// Save the player's current inventory to a new gearset at the next possible ID.
        /// </summary>
        /// <returns>Returns the ID of the created gearset, or 255 if the creation attempt fails.</returns>

        /// <summary>
        /// Delete the gearset at the specified ID.
        /// </summary>
        /// <param name="gearsetId">The gearset ID to delete.</param>

        /// <summary>
        /// Link a glamour plate to a specific gearset.
        /// </summary>
        /// <param name="gearsetId">The gearset ID to link a glamour plate to </param>
        /// <param name="glamourPlateId">The glamour plate ID to link. 0 resets the linked glamour plate.</param>

        /// <summary>
        /// Check if a specific gearset has a linked glamour plate.
        /// </summary>
        /// <param name="gearsetId">The ID of the gearset to check.</param>
        /// <returns>Returns 0 if a gearset is invalid or does not have a linked plate, 1 otherwise.</returns>


        [StructLayout(LayoutKind.Sequential, Size = 0xAF2C)]
        public struct Gearsets
        {
            private fixed byte data[0x1C0 * 100];

        }

        [Flags]
        public enum GearsetFlag : byte
        {
            None = 0x00,
            Exists = 0x01,
            Unknown02 = 0x02,
            Unknown04 = 0x04,
            HeadgearVisible = 0x08,
            WeaponsVisible = 0x10,
            VisorEnabled = 0x20,
            Unknown40 = 0x40,
            Unknown80 = 0x80
        }

        [StructLayout(LayoutKind.Explicit, Size = Size)]
        public struct GearsetItem
        {
            public const int Size = 0x1C;

            [FieldOffset(0x0)] public uint ItemID;
            [FieldOffset(0x4)] public uint GlamourId;
            [FieldOffset(0x8)] public byte Stain;

            [FieldOffset(0xA)] public fixed ushort Materia[5];
            [FieldOffset(0x14)] public fixed byte MateriaGrade[5];
        }

        [StructLayout(LayoutKind.Explicit, Size = 0x1C0)]
        public struct GearsetEntry
        {
            [FieldOffset(0x000)] public byte ID;    // This may actually be set number, which is not _quite_ ID.
            [FieldOffset(0x001)] public fixed byte Name[0x2F];
            [FieldOffset(0x31)] public byte ClassJob;
            [FieldOffset(0x32)] public byte GlamourSetLink;
            [FieldOffset(0x34)] public short ItemLevel;
            // [FieldOffset(0x36)] public byte BannerId; // ?
            [FieldOffset(0x37)] public GearsetFlag Flags;

            private const int ItemDataOffset = 0x38;
            [FieldOffset(ItemDataOffset)] public fixed byte ItemsData[GearsetItem.Size * 14];
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 00)] public GearsetItem MainHand;
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 01)] public GearsetItem OffHand;
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 02)] public GearsetItem Head;
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 03)] public GearsetItem Body;
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 04)] public GearsetItem Hands;
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 05)] public GearsetItem Belt;
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 06)] public GearsetItem Legs;
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 07)] public GearsetItem Feet;
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 08)] public GearsetItem Ears;
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 09)] public GearsetItem Neck;
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 10)] public GearsetItem Wrists;
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 11)] public GearsetItem RingRight;
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 12)] public GearsetItem RightLeft; // ?!
            [FieldOffset(ItemDataOffset + GearsetItem.Size * 13)] public GearsetItem SoulStone;
        }
    }
}