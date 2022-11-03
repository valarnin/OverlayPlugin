using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game.Object;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Graphics;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game
{

    [StructLayout(LayoutKind.Explicit, Size = 0x810)]
    public unsafe struct ActionManager
    {
        [FieldOffset(0x13C)] public fixed uint BlueMageActions[24];





















    }

    [StructLayout(LayoutKind.Explicit, Size = 0x14)]
    public struct RecastDetail
    {
        [FieldOffset(0x0)] public byte IsActive;
        [FieldOffset(0x4)] public uint ActionID;
        [FieldOffset(0x8)] public float Elapsed;
        [FieldOffset(0xC)] public float Total;
    }

    public enum ActionType : byte
    {
        None = 0x00,
        Spell = 0x01,
        Item = 0x02,
        KeyItem = 0x03,
        Ability = 0x04,
        General = 0x05,
        Companion = 0x06,
        Unk_7 = 0x07,
        Unk_8 = 0x08, //something with Leve?
        CraftAction = 0x09,
        MainCommand = 0x0A,
        PetAction = 0x0B,
        Unk_12 = 0x0C,
        Mount = 0x0D,
        PvPAction = 0x0E,
        Waymark = 0x0F,
        ChocoboRaceAbility = 0x10,
        ChocoboRaceItem = 0x11,
        Unk_18 = 0x12,
        SquadronAction = 0x13,
        Accessory = 0x14
    }
}