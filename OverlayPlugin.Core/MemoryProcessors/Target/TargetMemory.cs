using RainbowMage.OverlayPlugin.MemoryProcessors.Combatant;
using System;
using System.Collections.Generic;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.Target
{
    public abstract class TargetMemory {
        private FFXIVMemory memory;
        private ILogger logger;
        private MemoryProcessors.Combatant.CombatantMemoryManager combatantMemory;
        private uint loggedScanErrors = 0;

        private IntPtr targetAddress = IntPtr.Zero;

        private string targetSignature;

        // Offsets from the targetAddress to find the correct target type.
        private int targetTargetOffset;
        private int focusTargetOffset;
        private int hoverTargetOffset;

        public TargetMemory(TinyIoCContainer container, string targetSignature, int targetTargetOffset, int focusTargetOffset, int hoverTargetOffset)
        {
            this.targetSignature = targetSignature;
            this.targetTargetOffset = targetTargetOffset;
            this.focusTargetOffset = focusTargetOffset;
            this.hoverTargetOffset = hoverTargetOffset;
            memory = new FFXIVMemory(container);
            memory.OnProcessChange += ResetPointers;
            logger = container.Resolve<ILogger>();
            combatantMemory = container.Resolve<MemoryProcessors.Combatant.CombatantMemoryManager>();
            GetPointerAddress();
        }

        private void ResetPointers(object sender, EventArgs _)
        {
            targetAddress = IntPtr.Zero;
        }

        private bool HasValidPointers()
        {
            if (targetAddress == IntPtr.Zero)
                return false;
            return true;
        }

        public bool IsValid()
        {
            if (!memory.IsValid())
                return false;

            if (!HasValidPointers())
                GetPointerAddress();

            if (!HasValidPointers())
                return false;

            return true;
        }

        private bool GetPointerAddress()
        {
            if (!memory.IsValid())
                return false;

            bool success = true;

            List<string> fail = new List<string>();

            List<IntPtr> list = memory.SigScan(targetSignature, 0, true);
            if (list != null && list.Count > 0)
            {
                targetAddress = list[0];
            }
            else
            {
                targetAddress = IntPtr.Zero;
                fail.Add(nameof(targetAddress));
                success = false;
            }

            logger.Log(LogLevel.Debug, "targetAddress: 0x{0:X}", targetAddress.ToInt64());

            if (!success)
            {
                if (loggedScanErrors < 10)
                {
                    logger.Log(LogLevel.Error, $"Failed to find target memory via {GetType().Name}: {string.Join(", ", fail)}.");
                    loggedScanErrors++;

                    if (loggedScanErrors == 10)
                    {
                        logger.Log(LogLevel.Error, "Further target memory errors won't be logged.");
                    }
                }
            }
            else
            {
                logger.Log(LogLevel.Info, $"Found target memory via {GetType().Name}.");
                loggedScanErrors = 0;
            }

            return success;
        }

        private Combatant.Combatant GetTargetRelativeCombatant(int offset)
        {
            IntPtr address = memory.ReadIntPtr(IntPtr.Add(targetAddress, offset));
            if (address == IntPtr.Zero)
                return null;

            return combatantMemory.GetCombatantFromAddress(address, 0);
        }

        public Combatant.Combatant GetTargetCombatant()
        {
            return GetTargetRelativeCombatant(targetTargetOffset);
        }

        public Combatant.Combatant GetFocusCombatant()
        {
            return GetTargetRelativeCombatant(focusTargetOffset);
        }

        public Combatant.Combatant GetHoverCombatant()
        {
            return GetTargetRelativeCombatant(hoverTargetOffset);
        }

    }
}
