using System.Collections.Generic;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.InCombat
{
    public interface IInCombatMemory
    {
        bool GetInCombat();
    }

    class InCombatMemoryManager : IInCombatMemory
    {
        private readonly TinyIoCContainer container;
        private readonly FFXIVRepository repository;
        private InCombatMemory memory = null;

        public InCombatMemoryManager(TinyIoCContainer container)
        {
            this.container = container;
            container.Register(new InCombatMemory60(container));
            container.Register(new InCombatMemory61(container));
            container.Register(new InCombatMemory62(container));
            repository = container.Resolve<FFXIVRepository>();
        }

        private void FindMemory()
        {
            List<InCombatMemory> candidates = new List<InCombatMemory>();
            // For CN/KR, try the lang-specific candidate first, then fall back to intl
            if (repository.GetLanguage() == FFXIV_ACT_Plugin.Common.Language.Chinese)
            {
                candidates.Add(container.Resolve<InCombatMemory61>());
            }
            else if (repository.GetLanguage() == FFXIV_ACT_Plugin.Common.Language.Korean)
            {
                candidates.Add(container.Resolve<InCombatMemory60>());
            }
            candidates.Add(container.Resolve<InCombatMemory62>());

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

        public bool GetInCombat()
        {
            if (!IsValid())
            {
                return false;
            }
            return memory.GetInCombat();
        }
    }
}
