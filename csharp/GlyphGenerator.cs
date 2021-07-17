using System;
using System.Runtime.InteropServices;

namespace Refterm
{
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class GlyphGenerator
    {
        public uint FontWidth;
        public uint FontHeight;
        public uint Pitch;
        public IntPtr Pixels;

        public uint TransferWidth;
        public uint TransferHeight;

        // NOTE(casey): For DWrite-based generation:
        public SharpDX.DirectWrite.Factory DWriteFactory;
        public IntPtr FontFace;
        public SharpDX.DirectWrite.TextFormat TextFormat;
    }
}
