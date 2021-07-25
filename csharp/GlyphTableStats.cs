namespace Refterm
{
    public class GlyphTableStats
    {
        public int HitCount { get; set; } // NOTE(casey): Number of times FindGlyphEntryByHash hit the cache
        public int MissCount { get; set; } // NOTE(casey): Number of times FindGlyphEntryByHash misses the cache
        public int RecycleCount { get; set; }  // NOTE(casey): Number of times an entry had to be recycled to fill a cache miss
    }
}
