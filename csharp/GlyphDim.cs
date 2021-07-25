using System.Runtime.InteropServices;

namespace Refterm
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct GlyphDim
    {
        public uint TileCount { get; set; }
        public float XScale { get; set; }
        public float YScale { get; set; }
    };
}
