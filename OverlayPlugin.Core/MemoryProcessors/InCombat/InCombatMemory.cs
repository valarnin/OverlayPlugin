using RainbowMage.OverlayPlugin.MemoryProcessors.Combatant;
using System;
using System.Collections.Generic;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.InCombat
{
    public abstract class InCombatMemory {
        protected FFXIVMemory memory;
        protected ILogger logger;
        private uint loggedScanErrors = 0;

        protected IntPtr inCombatAddress = IntPtr.Zero;

        private string inCombatSignature;

        private int inCombatSignatureOffset;
        private int inCombatRIPOffset;

        public InCombatMemory(TinyIoCContainer container, string inCombatSignature, int inCombatSignatureOffset, int inCombatRIPOffset)
        {
            this.inCombatSignature = inCombatSignature;
            this.inCombatSignatureOffset = inCombatSignatureOffset;
            this.inCombatRIPOffset = inCombatRIPOffset;
            memory = new FFXIVMemory(container);
            memory.OnProcessChange += ResetPointers;
            logger = container.Resolve<ILogger>();
            GetPointerAddress();
        }

        private void ResetPointers(object sender, EventArgs _)
        {
            inCombatAddress = IntPtr.Zero;
        }

        private bool HasValidPointers()
        {
            if (inCombatAddress == IntPtr.Zero)
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

        protected virtual bool GetPointerAddress()
        {
            if (!memory.IsValid())
                return false;

            bool success = true;

            List<string> fail = new List<string>();

            List<IntPtr> list = memory.SigScan(inCombatSignature, inCombatSignatureOffset, true, inCombatRIPOffset);

            if (list != null && list.Count > 0)
            {
                inCombatAddress = list[0];
            }
            else
            {
                inCombatAddress = IntPtr.Zero;
                fail.Add(nameof(inCombatAddress));
                success = false;
            }


            logger.Log(LogLevel.Debug, "inCombatAddress: 0x{0:X}", inCombatAddress.ToInt64());

            if (!success)
            {
                if (loggedScanErrors < 10)
                {
                    logger.Log(LogLevel.Error, $"Failed to find in combat memory via {GetType().Name}: {string.Join(", ", fail)}.");
                    loggedScanErrors++;

                    if (loggedScanErrors == 10)
                    {
                        logger.Log(LogLevel.Error, "Further in combat memory errors won't be logged.");
                    }
                }
            }
            else
            {
                logger.Log(LogLevel.Info, $"Found in combat memory via {GetType().Name}.");
                loggedScanErrors = 0;
            }

            return success;
        }

        public bool GetInCombat()
        {
            if (inCombatAddress == IntPtr.Zero)
                return false;
            byte[] bytes = memory.Read8(inCombatAddress, 1);
            return bytes[0] != 0;
        }
    }
}
