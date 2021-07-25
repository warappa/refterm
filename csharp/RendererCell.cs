using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Refterm
{
    [DebuggerDisplay("{GlyphIndex}")]
    [StructLayout(LayoutKind.Sequential)]
    public struct RendererCell
    {
        public uint GlyphIndex { get; set; }
        public uint Foreground { get; set; }
        public uint Background { get; set; } // NOTE(casey): The top bit of the background flag indicates blinking
    }
}
