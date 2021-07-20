using System;
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

        public GlyphTableParams Params { get; internal set; }

        private int lastKnownEntry = 0;
        public GpuGlyphIndex PickNextFreeGpuIndex()
        {
            for (var i = lastKnownEntry; i < Entries.Length; i++)
            {
                if (!Entries[i].Used)
                {
                    lastKnownEntry = i;
                    Entries[i].Used = true;
                    var X = i % Params.CacheTileCountInX;
                    var Y = i / Params.CacheTileCountInX;
                    return new GpuGlyphIndex { Value = (uint)((Y << 16) | X) };
                }
            }
            throw new Exception("No free glyph index");
        }
    }
}
