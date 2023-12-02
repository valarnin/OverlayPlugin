﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using RainbowMage.OverlayPlugin.MemoryProcessors.Combatant;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.ContentFinderSettings
{
    public abstract class ContentFinderSettingsMemory : IContentFinderSettingsMemory
    {
        private struct ContentFinderSettingsImpl : ContentFinderSettings
        {
            public byte unrestrictedParty { get; set; }

            public byte minimalItemLevel { get; set; }

            public byte silenceEcho { get; set; }

            public byte explorerMode { get; set; }

            public byte levelSync { get; set; }
        }

        private FFXIVMemory memory;
        private ILogger logger;

        private IntPtr settingsAddress = IntPtr.Zero;
        private IntPtr inContentFinderAddress = IntPtr.Zero;

        private string settingsSignature;
        private string inContentFinderSignature;
        private int inContentSettingsOffset;

        public ContentFinderSettingsMemory(TinyIoCContainer container, string settingsSignature, string inContentFinderSignature, int inContentSettingsOffset)
        {
            this.settingsSignature = settingsSignature;
            this.inContentFinderSignature = inContentFinderSignature;
            this.inContentSettingsOffset = inContentSettingsOffset;
            logger = container.Resolve<ILogger>();
            memory = container.Resolve<FFXIVMemory>();
        }

        private void ResetPointers()
        {
            settingsAddress = IntPtr.Zero;
            inContentFinderAddress = IntPtr.Zero;
        }

        private bool HasValidPointers()
        {
            if (settingsAddress == IntPtr.Zero)
                return false;
            if (inContentFinderAddress == IntPtr.Zero)
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

            if (fail.Count == 0)
            {
                logger.Log(LogLevel.Info, $"Found content finder settings memory via {GetType().Name}.");
                return;
            }

            logger.Log(LogLevel.Error, $"Failed to find content finder settings memory via {GetType().Name}: {string.Join(", ", fail)}.");
            return;
        }

        public abstract Version GetVersion();

        public bool GetInContentFinderContent()
        {
            var bytes = memory.GetByteArray(inContentFinderAddress, 1);
            return bytes[0] != 0;
        }

        public ContentFinderSettings GetContentFinderSettings()
        {
            var bytes = memory.GetByteArray(settingsAddress, 5);
            var settings = new ContentFinderSettingsImpl();
            settings.unrestrictedParty = bytes[0];
            settings.minimalItemLevel = bytes[1];
            settings.silenceEcho = bytes[3];
            settings.explorerMode = bytes[4];
            settings.levelSync = bytes[2];
            return settings;
        }
    }
}
