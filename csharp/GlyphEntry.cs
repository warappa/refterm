namespace Refterm
{
    public class GlyphEntry
    {
        public GlyphHash HashValue { get; set; }

        public GpuGlyphIndex GPUIndex { get; set; }

        // NOTE(casey): For user use:
        public GlyphEntryState FilledState { get; set; }
        public uint DimX { get; set; }
        public uint DimY { get; set; }

        public bool Used { get; set; }
    }
}
