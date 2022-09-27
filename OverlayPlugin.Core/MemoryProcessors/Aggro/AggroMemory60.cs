namespace RainbowMage.OverlayPlugin.MemoryProcessors.Aggro
{
    class AggroMemory60 : AggroMemory
    {
        private const int aggroEnmityOffset = -2336;

        // Aggro uses the same signature as Enmity
        public AggroMemory60(TinyIoCContainer container)
            : base(container, Enmity.EnmityMemory60.enmitySignature, aggroEnmityOffset)
        { }
    }
}
