namespace Refterm
{
    public class GlyphEntry
    {
        public GlyphHash HashValue;

        uint NextWithSameHash;
        uint NextLRU;
        uint PrevLRU;
        public GpuGlyphIndex GPUIndex;

        // NOTE(casey): For user use:
        public GlyphEntryState FilledState;
        public uint DimX;
        public uint DimY;
    }
}
