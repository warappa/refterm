using System.Collections.Generic;

namespace Refterm
{
    public class GlyphTable
    {
        public GlyphTableStats Stats;

        public uint HashMask;
        public uint HashCount;
        public uint EntryCount;

        public uint[] HashTable;
        public GlyphEntry[] Entries;

        public Dictionary<int, GlyphEntry> Dictionary = new Dictionary<int, GlyphEntry>();
    }
}
