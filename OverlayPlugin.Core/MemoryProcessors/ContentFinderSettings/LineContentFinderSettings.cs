using System;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.ContentFinderSettings
{
    class LineContentFinderSettings
    {
        public const uint LogFileLineID = 265;
        private readonly FFXIVRepository ffxiv;

        private Func<string, DateTime, bool> logWriter;

        private IContentFinderSettingsMemory contentFinderSettingsMemory;

        public LineContentFinderSettings(TinyIoCContainer container)
        {
            ffxiv = container.Resolve<FFXIVRepository>();
            if (!ffxiv.IsFFXIVPluginPresent())
                return;
            contentFinderSettingsMemory = container.Resolve<IContentFinderSettingsMemory>();
            var customLogLines = container.Resolve<FFXIVCustomLogLines>();
            this.logWriter = customLogLines.RegisterCustomLogLine(new LogLineRegistryEntry()
            {
                Name = "ContentFinderSettings",
                Source = "OverlayPlugin",
                ID = LogFileLineID,
                Version = 1,
            });
            // Get the current zone ID before we subscribe
            var currentZoneId = ffxiv.GetCurrentTerritoryID();
            ffxiv.RegisterZoneChangeDelegate(OnZoneChange);

            // If we already had a zone ID, manually trigger a line
            if (currentZoneId.HasValue)
            {
                var currentZoneName = Advanced_Combat_Tracker.ActGlobals.oFormActMain.CurrentZone;
                OnZoneChange(currentZoneId.Value, currentZoneName);
            }
        }

        private void OnZoneChange(uint zoneId, string zoneName)
        {
            if (!contentFinderSettingsMemory.IsValid())
                return;
            WriteInContentFinderSettingsLine(DateTime.Now, $"{zoneId:X}", zoneName);
        }

        private void WriteInContentFinderSettingsLine(DateTime dateTime, string zoneID, string zoneName)
        {
            var settings = contentFinderSettingsMemory.GetContentFinderSettings();

            logWriter.Invoke(
                $"{settings.inContentFinderContent}|" +
                $"{zoneID}|" +
                $"{zoneName}|" +
                $"{settings?.ilvlSync ?? 0}|" +
                $"{settings?.unrestrictedParty ?? 0}|" +
                $"{settings?.minimalItemLevel ?? 0}|" +
                $"{settings?.silenceEcho ?? 0}|" +
                $"{settings?.explorerMode ?? 0}|" +
                $"{settings?.levelSync ?? 0}",
                dateTime);
        }
    }
}
