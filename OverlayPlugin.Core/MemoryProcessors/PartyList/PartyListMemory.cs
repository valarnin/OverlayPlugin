using System;
using System.Collections.Generic;
using System.Diagnostics;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.PartyList
{
    public abstract class PartyListMemory
    {
        protected FFXIVMemory memory;
        protected ILogger logger;

        protected IAtkStageMemory atkStageMemory;

        public PartyListMemory(TinyIoCContainer container)
        {
            logger = container.Resolve<ILogger>();
            memory = container.Resolve<FFXIVMemory>();
            atkStageMemory = container.Resolve<IAtkStageMemory>();
        }

        public bool IsValid()
        {
            if (!memory.IsValid())
                return false;

            if (!atkStageMemory.IsValid())
                return false;

            return true;
        }

        public void ScanPointers()
        {
            //noop
        }

        public abstract Version GetVersion();
    }
}
