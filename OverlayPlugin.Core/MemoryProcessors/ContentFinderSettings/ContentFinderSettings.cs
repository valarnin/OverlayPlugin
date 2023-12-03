using System;
using System.Collections.Generic;
using System.Diagnostics;
using RainbowMage.OverlayPlugin.MemoryProcessors.Combatant;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.ContentFinderSettings
{
    public abstract class ContentFinderSettingsMemory : IContentFinderSettingsMemory
    {
        private struct ContentFinderSettingsImpl : ContentFinderSettings
        {
            public bool inContentFinderContent { get; set; }

            public byte unrestrictedParty { get; set; }

            public byte minimalItemLevel { get; set; }

            public byte silenceEcho { get; set; }

            public byte explorerMode { get; set; }

            public byte levelSync { get; set; }

            public ushort ilvlSync { get; set; }
        }

        private FFXIVMemory memory;
        private ILogger logger;
        private ICombatantMemory combatantMemoryManager;

        private IntPtr settingsAddress = IntPtr.Zero;
        private IntPtr inContentFinderAddress = IntPtr.Zero;
        private IntPtr contentDirectorAddress = IntPtr.Zero;

        private string settingsSignature;
        private string inContentFinderSignature;
        private string contentDirectorSignature;
        private int inContentSettingsOffset;

        public ContentFinderSettingsMemory(TinyIoCContainer container, string settingsSignature, string inContentFinderSignature, string contentDirectorSignature, int inContentSettingsOffset)
        {
            this.settingsSignature = settingsSignature;
            this.inContentFinderSignature = inContentFinderSignature;
            this.inContentSettingsOffset = inContentSettingsOffset;
            this.contentDirectorSignature = contentDirectorSignature;
            logger = container.Resolve<ILogger>();
            memory = container.Resolve<FFXIVMemory>();
            combatantMemoryManager = container.Resolve<ICombatantMemory>();
        }

        private void ResetPointers()
        {
            settingsAddress = IntPtr.Zero;
            inContentFinderAddress = IntPtr.Zero;
            contentDirectorAddress = IntPtr.Zero;
        }

        private bool HasValidPointers()
        {
            if (settingsAddress == IntPtr.Zero)
                return false;
            if (inContentFinderAddress == IntPtr.Zero)
                return false;
            if (contentDirectorAddress == IntPtr.Zero)
                return false;
            return true;
        }

        public bool IsValid()
        {
            if (!memory.IsValid())
                return false;

            if (!HasValidPointers())
                return false;

            return true;
        }

        public void ScanPointers()
        {
            ResetPointers();
            if (!memory.IsValid())
                return;

            List<string> fail = new List<string>();

            List<IntPtr> list = memory.SigScan(settingsSignature, -35, true);
            if (list != null && list.Count > 0)
            {
                settingsAddress = list[0] + inContentSettingsOffset;
            }
            else
            {
                settingsAddress = IntPtr.Zero;
                fail.Add(nameof(settingsAddress));
            }

            logger.Log(LogLevel.Debug, "settingsAddress: 0x{0:X}", settingsAddress.ToInt64());

            list = memory.SigScan(inContentFinderSignature, -34, true, 1);
            if (list != null && list.Count > 0)
            {
                inContentFinderAddress = list[0];
            }
            else
            {
                inContentFinderAddress = IntPtr.Zero;
                fail.Add(nameof(inContentFinderAddress));
            }

            logger.Log(LogLevel.Debug, "inContentFinderAddress: 0x{0:X}", inContentFinderAddress.ToInt64());

            list = memory.SigScan(contentDirectorSignature, -32, true, 1);
            if (list != null && list.Count > 0)
            {
                contentDirectorAddress = list[0];
            }
            else
            {
                contentDirectorAddress = IntPtr.Zero;
                fail.Add(nameof(contentDirectorAddress));
            }

            logger.Log(LogLevel.Debug, "contentDirectorAddress: 0x{0:X}", contentDirectorAddress.ToInt64());

            if (fail.Count == 0)
            {
                logger.Log(LogLevel.Info, $"Found content finder settings memory via {GetType().Name}.");
                return;
            }

            logger.Log(LogLevel.Error, $"Failed to find content finder settings memory via {GetType().Name}: {string.Join(", ", fail)}.");
            return;
        }

        public abstract Version GetVersion();

        private bool GetInContentFinderContent()
        {
            var bytes = memory.GetByteArray(inContentFinderAddress, 1);
            return bytes[0] != 0;
        }

        public ContentFinderSettings GetContentFinderSettings()
        {
            var settings = new ContentFinderSettingsImpl();
            settings.inContentFinderContent = GetInContentFinderContent();

            // Don't bother fetching other info if we're not in a valid ContentFinder scope
            if (!settings.inContentFinderContent)
            {
                return settings;
            }

            var bytes = memory.GetByteArray(settingsAddress, 5);
            settings.unrestrictedParty = bytes[0];
            settings.minimalItemLevel = bytes[1];
            settings.silenceEcho = bytes[3];
            settings.explorerMode = bytes[4];
            settings.levelSync = bytes[2];
            var directorPtr = memory.GetInt64(contentDirectorAddress);

            // Wrap the ilvlSync in a try/catch, because I'm not sure if this code will persist very well across major game updates
            // Should work fine for 6.51 and 6.51-hotfix, but I don't really have a good way to check older versions for this logic
            try
            {
                // Technically, the InstanceContentDirector is 0x1CB2 in length, but grab the minimal length required here.
                var directorBytes = memory.GetByteArray(new IntPtr(directorPtr), 0x1CA7);
                // Offsets and logic in `GetItemLevelSync` are based on
                // https://github.com/Kouzukii/ffxiv-characterstatus-refined/blob/master/CharacterPanelRefined/IlvlSync.cs
                var ilvlSyncValue1 = (ushort)(directorBytes[0x524] + (directorBytes[0x525] << 8)); // 1316
                var ilvlSyncValue2 = (ushort)(directorBytes[0x526] + (directorBytes[0x527] << 8)); // 1318
                var flags1 = directorBytes[0xCE4]; // 3300
                var flags2 = directorBytes[0x33C]; // 828
                var minIlvlFlags1 = directorBytes[0x1CA6]; // 7334
                settings.ilvlSync = GetItemLevelSync(ilvlSyncValue1, ilvlSyncValue2, flags1, flags2, minIlvlFlags1, settings.levelSync, settings.inContentFinderContent);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, $"Failed to get memory info for ilvlSync calculations: " + e.ToString());
            }
            return settings;
        }

        private ushort GetItemLevelSync(ushort ilvlSyncValue1, ushort ilvlSyncValue2, byte flags1, byte flags2, byte minIlvlFlags1, byte levelSync, bool inContentFinderContent)
        {
            if (inContentFinderContent)
            {
                if (flags1 != 8 && (flags2 & 1) == 0)
                {
                    // min ilvl
                    if (minIlvlFlags1 >= 0x80 && ilvlSyncValue1 > 0)
                    {
                        return ilvlSyncValue1;
                    }

                    // duty is sync'd
                    if (((minIlvlFlags1 & 0x40) == 0 || levelSync == 1) && ilvlSyncValue2 > 0)
                    {
                        return ilvlSyncValue2;
                    }
                }
            }

            if (levelSync == 1)
            {
                var syncedLevel = combatantMemoryManager.GetSelfCombatant().Level;
                ushort ilvl;
                if (syncedLevel == 90)
                    ilvl = 660;
                else if (syncedLevel >= 83)
                    ilvl = (ushort)(530 + (syncedLevel - 83) * 3);
                else if (syncedLevel >= 81)
                    ilvl = (ushort)(520 + (syncedLevel - 81) * 5);
                else if (syncedLevel == 80)
                    ilvl = 530;
                else if (syncedLevel >= 73)
                    ilvl = (ushort)(400 + (syncedLevel - 73) * 3);
                else if (syncedLevel >= 71)
                    ilvl = (ushort)(390 + (syncedLevel - 71) * 5);
                else if (syncedLevel == 70)
                    ilvl = 400;
                else if (syncedLevel >= 63)
                    ilvl = (ushort)(270 + (syncedLevel - 63) * 3);
                else if (syncedLevel >= 61)
                    ilvl = (ushort)(260 + (syncedLevel - 61) * 5);
                else if (syncedLevel == 60)
                    ilvl = 270;
                else if (syncedLevel >= 53)
                    ilvl = (ushort)(130 + (syncedLevel - 53) * 3);
                else if (syncedLevel >= 51)
                    ilvl = (ushort)(120 + (syncedLevel - 51) * 5);
                else if (syncedLevel == 50)
                    ilvl = 130;
                else
                    ilvl = syncedLevel;
                return ilvl;
            }
            return 0;
        }
    }
}
