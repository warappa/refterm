namespace Refterm
{
    public class GlyphState
    {
        //public uint ID;
        public GpuGlyphIndex GPUIndex;

        // NOTE(casey): Technically these two values can be whatever you want.
        public GlyphEntryState FilledState;
        public uint DimX;
        public uint DimY;

        public GlyphEntry Entry { get; internal set; }
    }
}
