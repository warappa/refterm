using System;
using System.Runtime.InteropServices;

namespace Refterm
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 0, Size = 48)]
    public struct RendererConstBuffer
    {
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint CellSizeX;
        public uint CellSizeY;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint TermSizeX;
        public uint TermSizeY;
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint TopMargin;
        public uint LeftMargin;
        //[MarshalAs(UnmanagedType.U4)]
        public uint BlinkModulate;
        //[MarshalAs(UnmanagedType.U4)]
        public uint MarginColor;
        //[MarshalAs(UnmanagedType.U4)]
        public uint StrikeMin;
        //[MarshalAs(UnmanagedType.U4)]
        public uint StrikeMax;
        //[MarshalAs(UnmanagedType.U4)]
        public uint UnderlineMin;
        //[MarshalAs(UnmanagedType.U4)]
        public uint UnderlineMax;
    }
    //[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Pack = 0, Size = 48)]
    //public struct RendererConstBuffer
    //{
    //    [FieldOffset(0)]
    //    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    //    public uint[] CellSize;
    //    [FieldOffset(8)]
    //    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    //    public uint[] TermSize;
    //    [FieldOffset(16)]
    //    //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    //    public uint[] TopLeftMargin;
    //    [FieldOffset(24)]
    //    //[MarshalAs(UnmanagedType.U4)]
    //    public uint BlinkModulate;
    //    [FieldOffset(28)]
    //    //[MarshalAs(UnmanagedType.U4)]
    //    public uint MarginColor;
    //    [FieldOffset(32)]
    //    //[MarshalAs(UnmanagedType.U4)]
    //    public uint StrikeMin;
    //    [FieldOffset(36)]
    //    //[MarshalAs(UnmanagedType.U4)]
    //    public uint StrikeMax;
    //    [FieldOffset(40)]
    //    //[MarshalAs(UnmanagedType.U4)]
    //    public uint UnderlineMin;
    //    [FieldOffset(44)]
    //    //[MarshalAs(UnmanagedType.U4)]
    //    public uint UnderlineMax;
    //}
}
