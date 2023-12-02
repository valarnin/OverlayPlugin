﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.ContentFinderSettings
{
    public interface ContentFinderSettings
    {
        byte unrestrictedParty { get; }
        byte minimalItemLevel { get; }
        byte silenceEcho { get; }
        byte explorerMode { get; }
        byte levelSync { get; }
        // TODO: Maybe we can track these down if they're ever actually needed?
        // byte lootRules { get; }
        // byte limitedLevelingRoulette { get; }
    }

    public interface IContentFinderSettingsMemory : IVersionedMemory
    {
        bool GetInContentFinderContent();

        ContentFinderSettings GetContentFinderSettings();
    }

    class ContentFinderSettingsMemoryManager : IContentFinderSettingsMemory
    {
        private readonly TinyIoCContainer container;
        private readonly FFXIVRepository repository;
        private IContentFinderSettingsMemory memory = null;

        public ContentFinderSettingsMemoryManager(TinyIoCContainer container)
        {
            this.container = container;
            container.Register<IContentFinderSettingsMemory651, ContentFinderSettingsMemory651>();
            repository = container.Resolve<FFXIVRepository>();

            var memory = container.Resolve<FFXIVMemory>();
            memory.RegisterOnProcessChangeHandler(FindMemory);
        }

        private void FindMemory(object sender, Process p)
        {
            memory = null;
            if (p == null)
            {
                return;
            }
            ScanPointers();
        }

        public void ScanPointers()
        {
            List<IContentFinderSettingsMemory> candidates = new List<IContentFinderSettingsMemory>();
            candidates.Add(container.Resolve<IContentFinderSettingsMemory651>());
            memory = FFXIVMemory.FindCandidate(candidates, repository.GetMachinaRegion());
        }

        public bool IsValid()
        {
            if (memory == null || !memory.IsValid())
            {
                return false;
            }
            return true;
        }

        Version IVersionedMemory.GetVersion()
        {
            if (!IsValid())
                return null;
            return memory.GetVersion();
        }

        public bool GetInContentFinderContent()
        {
            if (!IsValid())
                return false;
            return memory.GetInContentFinderContent();
        }

        public ContentFinderSettings GetContentFinderSettings()
        {
            if (!IsValid())
                return null;
            return memory.GetContentFinderSettings();
        }
    }
}
