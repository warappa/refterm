using System;
using System.Runtime.InteropServices;

namespace Refterm
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct RendererConstBuffer
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] CellSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] TermSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] TopLeftMargin;
        [MarshalAs(UnmanagedType.U4)]
        public uint BlinkModulate;
        [MarshalAs(UnmanagedType.U4)]
        public uint MarginColor;
        [MarshalAs(UnmanagedType.U4)]
        public uint StrikeMin;
        [MarshalAs(UnmanagedType.U4)]
        public uint StrikeMax;
        [MarshalAs(UnmanagedType.U4)]
        public uint UnderlineMin;
        [MarshalAs(UnmanagedType.U4)]
        public uint UnderlineMax;
    }
}
