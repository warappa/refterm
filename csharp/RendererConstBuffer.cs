using System;
using System.Runtime.InteropServices;

namespace Refterm
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 0, Size = 48)]
    public struct RendererConstBuffer
    {
        public uint CellSizeX { get; set; }
        public uint CellSizeY { get; set; }
        public uint TermSizeX { get; set; }
        public uint TermSizeY { get; set; }
        public uint TopMargin { get; set; }
        public uint LeftMargin { get; set; }
        public uint BlinkModulate { get; set; }
        public uint MarginColor { get; set; }
        public uint StrikeMin { get; set; }
        public uint StrikeMax { get; set; }
        public uint UnderlineMin { get; set; }
        public uint UnderlineMax { get; set; }
    }
}
