using System;
using System.Collections.Generic;

namespace Refterm
{
    public class GlyphTable
    {
        private int lastKnownEntry = 0;

        public GlyphTableStats Stats;

        public uint HashMask { get; set; }
        public uint HashCount { get; set; }
        public uint EntryCount { get; set; }

        public uint[] HashTable { get; set; }
        public GlyphEntry[] Entries { get; set; }

        public Dictionary<int, GlyphEntry> Dictionary { get; set; } = new Dictionary<int, GlyphEntry>();

        public GlyphTableParams Params { get; set; }

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
