using System;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Event;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Fate;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Component.Exd;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.UI
{
    // this is a large object holding most of the other objects in the Client::Game::UI namespace
    // all data in here is used for UI display

    // ctor E8 ? ? ? ? 48 8D 0D ? ? ? ? 48 83 C4 28 E9 ? ? ? ? 48 83 EC 28 33 D2 
    [StructLayout(LayoutKind.Explicit, Size = 0x16AE4)] // its at least this big, may be a few bytes bigger
    public unsafe struct UIState
    {
        [FieldOffset(0x00)] public Hotbar Hotbar;
        [FieldOffset(0x08)] public Hate Hate;
        [FieldOffset(0x110)] public Hater Hater;
        [FieldOffset(0xA18)] public WeaponState WeaponState;
        [FieldOffset(0xA38)] public PlayerState PlayerState;
        [FieldOffset(0x11D0)] public Revive Revive;
        [FieldOffset(0x1468)] public Telepo Telepo;
        [FieldOffset(0x14C0)] public Cabinet Cabinet;
        [FieldOffset(0x1A50)] public Buddy Buddy;
        [FieldOffset(0x2A60)] public RelicNote RelicNote;

        [FieldOffset(0xA828)] public Director* ActiveDirector;
        [FieldOffset(0xA970)] public FateDirector* FateDirector;

        [FieldOffset(0xAAB8)] public Map Map;

        [FieldOffset(0xEAA0)] public MarkingController MarkingController;

        [FieldOffset(0x11AF0)] public ContentsFinder ContentsFinder;

        // No idea why this isn't its own thing, but I can't find any trace of any member functions or anything worthy of
        // making this its own struct . 
        // Ref: g_Client::Game::UI::UnlockedCompanionsMask
        //      direct ref: 48 8D 0D ?? ?? ?? ?? 0F B6 04 08 84 D0 75 10 B8 ?? ?? ?? ?? 48 8B 5C 24
        //      relative to uistate: E8 ?? ?? ?? ?? 84 C0 75 A6 32 C0 (case for 0x355)
        [FieldOffset(0x16A62)] public fixed byte UnlockedCompanionsBitmask[0x3A];



        /// <summary>
        /// Checks to see if an unlock link is unlocked, or if the passed value is greater than 0x10000, checks if the quest
        /// is completed.
        /// </summary>
        /// <param name="unlockLinkOrQuestId">The unlock link or quest ID to check</param>
        /// <param name="a3">Unknown, but appears to always be 1.</param>
        /// <returns>Returns true if the unlock link is unlocked or if the quest is completed.</returns>

        /// <summary>
        /// Check an item (by EXD row) to see if the action associated with the item is unlocked or "obtained/registered."
        /// </summary>
        /// <remarks>
        /// This method populates offset 324 of *something*, which is then used in turn to display the "checked" flag on
        /// unlockable items. It's used in a few other places, but this appears to be the primary use.
        /// <br /><br />
        /// This method **will** call EXD, so the usual caveats there apply. Where possible, use a by-ID check as they're
        /// generally faster or rely less on EXD.
        /// </remarks>
        /// <param name="itemExdPtr">A pointer to an EXD row for an item, generally retrieved from <see cref="ExdModule.GetItemRowById"/>.</param>
        /// <returns>Returns a number (byte?) based on the item's unlock status. For reasons unknown, this is a long but
        /// no value above 4 has ever been noted.
        /// <list type="table">
        /// <listheader><term>Value</term><description>Meaning</description></listheader>
        /// <item><term>1</term><description>The item is unlocked/registered.</description></item>
        /// <item><term>2</term><description>The item is not unlocked/registered, but can be.</description></item>
        /// <item><term>3</term><description>Can be returned, but not clear as to when.</description></item>
        /// <item><term>4</term><description>The item has no unlock action, or an unlock action was not found.</description></item>
        /// </list>
        /// </returns>

        /// <summary>
        /// Check if a Triple Triad card is obtained by the character.
        /// </summary>
        /// <param name="cardId">The ID of the card (technically, of TripleTriadCardResident) to check against.</param>
        /// <returns>Returns true if the card is unlocked.</returns>

        /// <summary>
        /// Check if an emote (by ID) is unlocked.
        /// </summary>
        /// <remarks>
        /// This method *will* perform an EXD get. If the unlock link is known it is better to call
        /// <see cref="IsUnlockLinkUnlockedOrQuestCompleted"/> in order to prevent the extra call, as it performs
        /// functionally the same checks.
        /// </remarks>
        /// <param name="emoteId">The ID of the emote to check for.</param>
        /// <returns>Returns true if the emote is unlocked.</returns>

        /// <summary>
        /// Check if a companion (minion) is unlocked for the current character.
        /// </summary>
        /// <remarks>
        /// WARNING: This method is NOT BOUNDED on IDs. While *one* function seems to set an upper bound on this, this
        /// method is a pain in the neck to find *and*, frustratingly, cannot be sigged. 
        /// </remarks>
        /// <param name="companionId">The ID of the companion/minion to check for.</param>
        /// <returns>Returns true if the specified minion is unlocked.</returns>

    }
}