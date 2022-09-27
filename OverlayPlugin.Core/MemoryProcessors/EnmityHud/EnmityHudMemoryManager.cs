using System.Collections.Generic;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.EnmityHud
{
    public interface IEnmityHudMemory
    {
        List<EnmityHudEntry> GetEnmityHudEntries();
    }

    public class EnmityHudMemoryManager : IEnmityHudMemory
    {
        private readonly TinyIoCContainer container;
        private readonly FFXIVRepository repository;
        private EnmityHudMemory memory = null;

        public EnmityHudMemoryManager(TinyIoCContainer container)
        {
            this.container = container;
            container.Register(new EnmityHudMemory60(container));
            container.Register(new EnmityHudMemory61(container));
            container.Register(new EnmityHudMemory62(container));
            repository = container.Resolve<FFXIVRepository>();
        }

        private void FindMemory()
        {
            List<EnmityHudMemory> candidates = new List<EnmityHudMemory>();
            // For CN/KR, try the lang-specific candidate first, then fall back to intl
            if (repository.GetLanguage() == FFXIV_ACT_Plugin.Common.Language.Chinese)
            {
                candidates.Add(container.Resolve<EnmityHudMemory61>());
            }
            else if (repository.GetLanguage() == FFXIV_ACT_Plugin.Common.Language.Korean)
            {
                candidates.Add(container.Resolve<EnmityHudMemory60>());
            }
            candidates.Add(container.Resolve<EnmityHudMemory62>());

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


        public List<EnmityHudEntry> GetEnmityHudEntries()
        {
            if (!IsValid())
            {
                return null;
            }
            return memory.GetEnmityHudEntries();
        }
    }
}
