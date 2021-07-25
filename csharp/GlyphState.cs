namespace Refterm
{
    public class GlyphState
    {
        public GpuGlyphIndex GPUIndex { get; set; }

        // NOTE(casey): Technically these two values can be whatever you want.
        public GlyphEntryState FilledState { get; set; }
        public uint DimX { get; set; }
        public uint DimY { get; set; }

        public GlyphEntry Entry { get; set; }
    }
}
