namespace Refterm
{
    public class GlyphTableStats
    {
        public int HitCount; // NOTE(casey): Number of times FindGlyphEntryByHash hit the cache
        public int MissCount; // NOTE(casey): Number of times FindGlyphEntryByHash misses the cache
        public int RecycleCount;  // NOTE(casey): Number of times an entry had to be recycled to fill a cache miss
    }
}
