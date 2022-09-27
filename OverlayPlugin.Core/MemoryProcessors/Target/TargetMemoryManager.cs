using System.Collections.Generic;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.Target
{
    public interface ITargetMemory
    {
        Combatant.Combatant GetTargetCombatant();

        Combatant.Combatant GetFocusCombatant();

        Combatant.Combatant GetHoverCombatant();
    }

    class TargetMemoryManager : ITargetMemory
    {
        private readonly TinyIoCContainer container;
        private readonly FFXIVRepository repository;
        private TargetMemory memory = null;

        public TargetMemoryManager(TinyIoCContainer container)
        {
            this.container = container;
            container.Register(new TargetMemory60(container));
            container.Register(new TargetMemory61(container));
            container.Register(new TargetMemory62(container));
            repository = container.Resolve<FFXIVRepository>();
        }

        private void FindMemory()
        {
            List<TargetMemory> candidates = new List<TargetMemory>();
            // For CN/KR, try the lang-specific candidate first, then fall back to intl
            if (repository.GetLanguage() == FFXIV_ACT_Plugin.Common.Language.Chinese)
            {
                candidates.Add(container.Resolve<TargetMemory61>());
            }
            else if (repository.GetLanguage() == FFXIV_ACT_Plugin.Common.Language.Korean)
            {
                candidates.Add(container.Resolve<TargetMemory60>());
            }
            candidates.Add(container.Resolve<TargetMemory62>());

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

        public Combatant.Combatant GetTargetCombatant()
        {
            if (!IsValid())
            {
                return null;
            }
            return memory.GetTargetCombatant();
        }

        public Combatant.Combatant GetFocusCombatant()
        {
            if (!IsValid())
            {
                return null;
            }
            return memory.GetFocusCombatant();
        }

        public Combatant.Combatant GetHoverCombatant()
        {
            if (!IsValid())
            {
                return null;
            }
            return memory.GetHoverCombatant();
        }
    }
}
