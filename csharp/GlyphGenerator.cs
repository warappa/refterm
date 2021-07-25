using SharpDX.DirectWrite;

namespace Refterm
{
    public class GlyphGenerator
    {
        public uint FontWidth;
        public uint FontHeight;

        public uint TransferWidth;
        public uint TransferHeight;

        // NOTE(casey): For DWrite-based generation:
        public Factory DWriteFactory;
        public TextFormat TextFormat;
    }
}
