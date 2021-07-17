using System.Runtime.InteropServices;

namespace Refterm
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RendererConstBuffer
    {
        //[MarshalAs(UnmanagedType.U4, SizeConst = 2)]
        public uint[] CellSize;
        //[MarshalAs(UnmanagedType.U4, SizeConst = 2)]
        public uint[] TermSize;
        //[MarshalAs(UnmanagedType.U4, SizeConst = 2)]
        public uint[] TopLeftMargin;
        //[MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public uint BlinkModulate;
        //[MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public uint MarginColor;
        //[MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public uint StrikeMin;
        //[MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public uint StrikeMax;
        //[MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public uint UnderlineMin;
        //[MarshalAs(UnmanagedType.U4, SizeConst = 1)]
        public uint UnderlineMax;
    }
}
