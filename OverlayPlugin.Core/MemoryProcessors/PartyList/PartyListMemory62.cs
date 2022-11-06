using System;
using FFXIVClientStructs.Global.FFXIV.Client.UI;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkGui.FFXIVClientStructs;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.PartyList
{
    interface IPartyListMemory62 : IPartyListMemory { }

    class PartyListMemory62 : PartyListMemory, IPartyListMemory62
    {
        public PartyListMemory62(TinyIoCContainer container) : base(container) { }

        public override Version GetVersion()
        {
            return new Version(6, 2);
        }

        public unsafe ManagedType<AddonPartyList> GetPartyList()
        {
            if (!IsValid())
            {
                return null;
            }

            if (!atkStageMemory.IsValid())
            {
                return null;
            }

            var partyListPtr = atkStageMemory.GetAddonAddress("_PartyList");

            if (partyListPtr != IntPtr.Zero)
            {
                return ManagedType<AddonPartyList>.GetManagedTypeFromIntPtr(partyListPtr, memory);
            }

            return null;
        }
    }
}
