using System.Runtime.InteropServices;

namespace Refterm
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct GlyphDim
    {
        public uint TileCount;
        public float XScale;
        public float YScale;
    };
}
