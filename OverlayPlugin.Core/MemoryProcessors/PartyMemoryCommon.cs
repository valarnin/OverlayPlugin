using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainbowMage.OverlayPlugin.MemoryProcessors
{
    public class PartyList
    {
        public ulong Address;
        public List<PartyEntry> Entries = new List<PartyEntry>();
        public byte Flags;
    }

    public struct PartyEntry
    {
        public uint ID;
        public string Name;
        public byte Flags;
        public JObject RawEntry;
    }

    public abstract class PartyMemoryCommon
    {
        abstract public bool IsValid();
        abstract public unsafe PartyList GetPartyList();
    }
}
