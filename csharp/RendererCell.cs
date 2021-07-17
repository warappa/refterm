using System.Runtime.InteropServices;

namespace Refterm
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RendererCell
    {
        public uint GlyphIndex;
        public uint Foreground;
        public uint Background; // NOTE(casey): The top bit of the background flag indicates blinking
    }
}
