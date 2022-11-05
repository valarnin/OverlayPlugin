using System;
using System.Reflection;
using System.Runtime.InteropServices;
using RainbowMage.OverlayPlugin.MemoryProcessors.AtkGui.FFXIVClientStructs;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace RainbowMage.OverlayPlugin.MemoryProcessors.AtkStage
{
    using AtkStage = FFXIVClientStructs.FFXIV.Component.GUI.AtkStage;
    interface IAtkStageMemory62 : IAtkStageMemory { }

    class AtkStageMemory62 : AtkStageMemory, IAtkStageMemory62
    {
        // Offset to global for atkStage singleton instance. ghidra shows `14203c540`, base address of ffxiv_dx11.exe is `140000000`
        private const int atkStageSingletonAddress = 0x203c540;
        public AtkStageMemory62(TinyIoCContainer container) : base(container, atkStageSingletonAddress) { }

        public override Version GetVersion()
        {
            return new Version(6, 2);
        }

        public unsafe IntPtr GetAddonAddress(string name)
        {
            if (!IsValid())
            {
                return IntPtr.Zero;
            }

            // Our current address points to an instance of AtkStage
            // We need to traverse the object to AtkUnitManager, then check each pointer to see if it's the addon we're looking for

            if (atkStageInstanceAddress.ToInt64() == 0)
            {
                return IntPtr.Zero;
            }
            dynamic atkStage = ManagedType<AtkStage>.GetManagedTypeFromIntPtr(atkStageInstanceAddress, memory);
            dynamic raptureAtkUnitManager = atkStage.RaptureAtkUnitManager;
            dynamic unitMgr = raptureAtkUnitManager.AtkUnitManager;
            AtkUnitList list = unitMgr.AllLoadedUnitsList;
            long* entries = (long*)&list.AtkUnitEntries;

            for (var i = 0; i < list.Count; ++i)
            {
                var ptr = new IntPtr(entries[i]);
                dynamic atkUnit = ManagedType<AtkUnitBase>.GetManagedTypeFromIntPtr(ptr, memory);
                byte[] atkUnitName = atkUnit.Name;

                var atkUnitNameValue = FFXIVMemory.GetStringFromBytes(atkUnitName, 0, atkUnitName.Length);
                if (atkUnitNameValue.Equals(name))
                {
                    return atkUnit.ptr;
                }
            }

            return IntPtr.Zero;
        }
    }
}
