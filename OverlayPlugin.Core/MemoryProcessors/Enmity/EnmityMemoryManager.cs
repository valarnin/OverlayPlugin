using System.Collections.Generic;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.Enmity
{
    public interface IEnmityMemory
    {
        List<EnmityEntry> GetEnmityEntryList(List<Combatant.Combatant> combatantList);
    }

    public class EnmityMemoryManager : IEnmityMemory
    {
        private readonly TinyIoCContainer container;
        private readonly FFXIVRepository repository;
        private EnmityMemory memory = null;

        public EnmityMemoryManager(TinyIoCContainer container)
        {
            this.container = container;
            container.Register(new EnmityMemory60(container));
            container.Register(new EnmityMemory61(container));
            container.Register(new EnmityMemory62(container));
            repository = container.Resolve<FFXIVRepository>();
        }

        private void FindMemory()
        {
            List<EnmityMemory> candidates = new List<EnmityMemory>();
            // For CN/KR, try the lang-specific candidate first, then fall back to intl
            if (repository.GetLanguage() == FFXIV_ACT_Plugin.Common.Language.Chinese)
            {
                candidates.Add(container.Resolve<EnmityMemory61>());
            }
            else if (repository.GetLanguage() == FFXIV_ACT_Plugin.Common.Language.Korean)
            {
                candidates.Add(container.Resolve<EnmityMemory60>());
            }
            candidates.Add(container.Resolve<EnmityMemory62>());

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


        public List<EnmityEntry> GetEnmityEntryList(List<Combatant.Combatant> combatantList)
        {
            if (!IsValid())
            {
                return null;
            }
            return memory.GetEnmityEntryList(combatantList);
        }
    }
}
