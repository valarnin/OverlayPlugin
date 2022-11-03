using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.UI
{

    //ctor 40 53 48 83 EC 20 48 8B D9 48 8D 81 ?? ?? ?? ?? BA
    [StructLayout(LayoutKind.Explicit, Size = 0x798)]
    public unsafe struct PlayerState
    {
        [FieldOffset(0x00)] public byte IsLoaded;
        [FieldOffset(0x01)] public fixed byte CharacterName[64];
        [FieldOffset(0x54)] public uint ObjectId;
        [FieldOffset(0x58)] public ulong ContentId;

        [FieldOffset(0x6E)] public byte CurrentClassJobId;
        [FieldOffset(0x78)] public short CurrentLevel;

        [FieldOffset(0x7A)] public fixed short ClassJobLevelArray[30];
        [FieldOffset(0xB8)] public fixed int ClassJobExpArray[30];

        [FieldOffset(0x130)] public short SyncedLevel;
        [FieldOffset(0x132)] public byte IsLevelSynced;

        [FieldOffset(0x154)] public int BaseStrength;
        [FieldOffset(0x158)] public int BaseDexterity;
        [FieldOffset(0x15C)] public int BaseVitality;
        [FieldOffset(0x160)] public int BaseIntelligence;
        [FieldOffset(0x164)] public int BaseMind;
        [FieldOffset(0x168)] public int BasePiety;

        [FieldOffset(0x16C)] public fixed int Attributes[74];

        [FieldOffset(0x294)] public byte GrandCompany;
        [FieldOffset(0x295)] public byte GCRankMaelstrom;
        [FieldOffset(0x296)] public byte GCRankTwinAdders;
        [FieldOffset(0x297)] public byte GCRankImmortalFlames;

        [FieldOffset(0x298)] public byte HomeAetheryteId;
        [FieldOffset(0x299)] public byte FavouriteAetheryteCount;
        [FieldOffset(0x29A)] public fixed byte FavouriteAetheryteArray[4];
        [FieldOffset(0x29E)] public byte FreeAetheryteId;

        [FieldOffset(0x2A0)] public uint BaseRestedExperience;

        [FieldOffset(0x414)] public uint FishingBait;

        [FieldOffset(0x45C)] public short PlayerCommendations;

        [FieldOffset(0x712)] public fixed ushort DesynthesisLevels[8];







        /// <summary>
        /// Check if a specific mount has been unlocked by the player.
        /// </summary>
        /// <param name="mountId">The ID of the mount to look up.</param>
        /// <returns>Returns true if the mount has been unlocked.</returns>

        /// <summary>
        /// Check if a specific ornament (fashion accessory) has been unlocked by the player.
        /// </summary>
        /// <param name="ornamentId">The ID of the ornament to look up.</param>
        /// <returns>Returns true if the ornament has been unlocked.</returns>

        /// <summary>
        /// Check if a specific orchestrion roll has been unlocked by the player.
        /// </summary>
        /// <param name="rollId">The ID of the roll to look up.</param>
        /// <returns>Returns true if the roll has been unlocked.</returns>

        /// <summary>
        /// Check if a Secret Recipe Book (DoH Master Tome) is unlocked and (indirectly) if the player can craft recipes
        /// from that specific book.
        /// </summary>
        /// <param name="tomeId">The ID of the book to check for. Can be retrieved from the SecretRecipeBook sheet.</param>
        /// <returns>Returns true if the book is unlocked.</returns>

        /// <summary>
        /// Check if a Folklore Book (DoL Master Tome) is unlocked and (indirectly) if the player can find legendary nodes
        /// revealed by that book.
        /// </summary>
        /// <param name="tomeId">The ID of the book to check for. Can be retrieved from GatheringSubCategory.Division</param>
        /// <returns>Returns true if the book is unlocked.</returns>

        /// <summary>
        /// Check if a specific McGuffin (Collectible/Curiosity) has been unlocked by the player.
        /// </summary>
        /// <param name="mcGuffinId">The ID of the McGuffin to look up, generally from the McGuffin sheet.</param>
        /// <returns>Returns true if the McGuffin has been unlocked.</returns>

        /// <summary>
        /// Check if a particular Framer's Kit is unlocked and can be used.
        /// </summary>
        /// <remarks>
        /// How IDs are located is a bit weird and not necessarily fully understood at time of writing. They appear on Framer
        /// Kit items in the AdditionalData field, and at +0 in BannerCondition EXDs when +0xE == 9.
        /// </remarks>
        /// <param name="kitId">The kit ID to check for.</param>
        /// <returns>Returns true if the framer's kit is unlocked.</returns>
    }
}