using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Advanced_Combat_Tracker;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.ContentFinderSettings
{
    class LineContentFinderSettings
    {
        public const uint LogFileLineID = 265;
        private ILogger logger;
        private readonly FFXIVRepository ffxiv;

        private Func<string, DateTime, bool> logWriter;

        private IContentFinderSettingsMemory contentFinderSettingsMemory;

        private bool wroteFirstLine = false;

        public LineContentFinderSettings(TinyIoCContainer container)
        {
            logger = container.Resolve<ILogger>();
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
            ActGlobals.oFormActMain.BeforeLogLineRead += LogLineHandler;
        }

        private void LogLineHandler(bool isImport, LogLineEventArgs args)
        {
            if (isImport)
            {
                return;
            }

            if (!contentFinderSettingsMemory.IsValid())
                return;

            try
            {
                LogMessageType lineType = (LogMessageType)args.detectedType;

                if (lineType != LogMessageType.ChangeZone && wroteFirstLine)
                    return;

                var line = args.originalLogLine.Split('|');
                var zoneID = line[2];
                var zoneName = line[3];

                wroteFirstLine = true;
                WriteInContentFinderSettingsLine(args.detectedTime, zoneID, zoneName);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, "Failed to process log line: " + e.ToString());
            }
        }

        private void WriteInContentFinderSettingsLine(DateTime dateTime, string zoneID, string zoneName)
        {
            var inContentFinderContent = contentFinderSettingsMemory.GetInContentFinderContent();

            // If we're not in a content finder content instance, set this to null
            // So that we can default to all 0's later
            var settings = inContentFinderContent ? contentFinderSettingsMemory.GetContentFinderSettings() : null;

            logWriter.Invoke(
                $"{inContentFinderContent}|" +
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
