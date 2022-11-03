using System;

using System.Runtime.InteropServices;
namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage.FFXIVClientStructs.Global.FFXIV.Client.Game
{

    // idk if this is a manager, but I don't know what else to call it
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct QuestManager
    {


        public static bool IsQuestComplete(uint questId) => IsQuestComplete((ushort)(questId & 0xFFFF));


        public static bool IsQuestCurrent(uint questId) => IsQuestCurrent((ushort)(questId & 0xFFFF));

        [FieldOffset(0x10)] public QuestListArray Quest;

        [StructLayout(LayoutKind.Explicit)]
        public struct QuestListArray
        {
            [FieldOffset(0x00)] private fixed byte data[0x18 * 30];


            [StructLayout(LayoutKind.Explicit, Size = 0x18)]
            public struct Quest
            {
                [FieldOffset(0x08)] public ushort QuestID;
                [FieldOffset(0x0B)] public QuestFlags Flags; // 1 for Priority, 8 for Hidden

                public bool IsHidden => Flags.HasFlag(QuestFlags.Hidden);

                [Flags]
                public enum QuestFlags : byte
                {
                    None,
                    Priority,
                    Hidden = 0x8
                }
            }
        }
    }
}