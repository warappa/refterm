using SharpDX.DirectWrite;

namespace Refterm
{
    public class GlyphGenerator
    {
        public uint FontWidth { get; set; }
        public uint FontHeight { get; set; }

        public uint TransferWidth { get; set; }
        public uint TransferHeight { get; set; }

        // NOTE(casey): For DWrite-based generation:
        public Factory DWriteFactory { get; set; }
        public TextFormat TextFormat { get; set; }
    }
}
