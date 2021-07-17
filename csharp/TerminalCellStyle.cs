using System;

namespace Refterm
{
    [Flags]
    public enum TerminalCellStyle
    {
        Bold = 0x1,
        Dim = 0x2,
        Italic = 0x4,
        Underline = 0x8,
        Blinking = 0x10,
        ReverseVideo = 0x20,
        Invisible = 0x40,
        Strikethrough = 0x80,
    };
}
