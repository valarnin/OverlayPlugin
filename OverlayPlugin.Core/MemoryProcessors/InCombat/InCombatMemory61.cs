namespace RainbowMage.OverlayPlugin.MemoryProcessors.InCombat
{
    class InCombatMemory61 : InCombatMemory
    {
        private const string inCombatSignature = "803D????????000F95C04883C428";
        private const int inCombatSignatureOffset = -12;
        private const int inCombatRIPOffset = 1;
        public InCombatMemory61(TinyIoCContainer container) : base(container, inCombatSignature, inCombatSignatureOffset, inCombatRIPOffset) { }
    }
}
