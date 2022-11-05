using System;
using System.Collections.Generic;
using System.Diagnostics;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkGui.FFXIVClientStructs;
using FFXIVClientStructs.FFXIV.Client.UI;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.PartyList
{
    public interface IPartyListMemory : IVersionedMemory
    {
        ManagedType<AddonPartyList> GetPartyList();
    }

    class PartyListMemoryManager : IPartyListMemory
    {
        private readonly TinyIoCContainer container;
        private readonly FFXIVRepository repository;
        private IPartyListMemory memory = null;

        public PartyListMemoryManager(TinyIoCContainer container)
        {
            this.container = container;
            container.Register<IPartyListMemory62, PartyListMemory62>();
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
            List<IPartyListMemory> candidates = new List<IPartyListMemory>();
            candidates.Add(container.Resolve<IPartyListMemory62>());
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

        public ManagedType<AddonPartyList> GetPartyList()
        {
            if (!IsValid())
            {
                return null;
            }
            return memory.GetPartyList();
        }
    }
}
