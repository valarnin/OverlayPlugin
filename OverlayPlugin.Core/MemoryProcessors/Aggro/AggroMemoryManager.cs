using System;
using System.Collections.Generic;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.Aggro
{
    public interface IAggroMemory
    {
        List<AggroEntry> GetAggroList(List<Combatant.Combatant> combatantList);
    }

    public class AggroMemoryManager : IAggroMemory
    {
        private readonly TinyIoCContainer container;
        private readonly FFXIVRepository repository;
        private AggroMemory memory = null;

        public AggroMemoryManager(TinyIoCContainer container)
        {
            this.container = container;
            container.Register(new AggroMemory60(container));
            container.Register(new AggroMemory61(container));
            container.Register(new AggroMemory62(container));
            repository = container.Resolve<FFXIVRepository>();
        }

        private void FindMemory()
        {
            List<AggroMemory> candidates = new List<AggroMemory>();
            // For CN/KR, try the lang-specific candidate first, then fall back to intl
            if (repository.GetLanguage() == FFXIV_ACT_Plugin.Common.Language.Chinese)
            {
                candidates.Add(container.Resolve<AggroMemory61>());
            }
            else if (repository.GetLanguage() == FFXIV_ACT_Plugin.Common.Language.Korean)
            {
                candidates.Add(container.Resolve<AggroMemory60>());
            }
            candidates.Add(container.Resolve<AggroMemory62>());

            foreach (var c in candidates)
            {
                if (c.IsValid())
                {
                    memory = c;
                    break;
                }
            }
        }

        public bool IsValid()
        {
            if (memory == null)
            {
                FindMemory();
            }
            if (memory == null || !memory.IsValid())
            {
                return false;
            }
            return true;
        }


        public List<AggroEntry> GetAggroList(List<Combatant.Combatant> combatantList)
        {
            if (!IsValid())
            {
                return null;
            }
            return memory.GetAggroList(combatantList);
        }
    }
}
