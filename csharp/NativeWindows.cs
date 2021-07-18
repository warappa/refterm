using System;
using System.Runtime.InteropServices;

namespace Refterm
{
    public class NativeWindows
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(uint threadId, uint msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [StructLayout(LayoutKind.Sequential)]
        public struct WNDCLASSEX
        {
            public uint cbSize;
            public ClassStyles style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }
        public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        [Flags]
        public enum ClassStyles : uint
        {
            /// <summary>Aligns the window's client area on a byte boundary (in the x direction). This style affects the width of the window and its horizontal placement on the display.</summary>
            ByteAlignClient = 0x1000,

            /// <summary>Aligns the window on a byte boundary (in the x direction). This style affects the width of the window and its horizontal placement on the display.</summary>
            ByteAlignWindow = 0x2000,

            /// <summary>
            /// Allocates one device context to be shared by all windows in the class.
            /// Because window classes are process specific, it is possible for multiple threads of an application to create a window of the same class.
            /// It is also possible for the threads to attempt to use the device context simultaneously. When this happens, the system allows only one thread to successfully finish its drawing operation.
            /// </summary>
            ClassDC = 0x40,

            /// <summary>Sends a double-click message to the window procedure when the user double-clicks the mouse while the cursor is within a window belonging to the class.</summary>
            DoubleClicks = 0x8,

            /// <summary>
            /// Enables the drop shadow effect on a window. The effect is turned on and off through SPI_SETDROPSHADOW.
            /// Typically, this is enabled for small, short-lived windows such as menus to emphasize their Z order relationship to other windows.
            /// </summary>
            DropShadow = 0x20000,

            /// <summary>Indicates that the window class is an application global class. For more information, see the "Application Global Classes" section of About Window Classes.</summary>
            GlobalClass = 0x4000,

            /// <summary>Redraws the entire window if a movement or size adjustment changes the width of the client area.</summary>
            HorizontalRedraw = 0x2,

            /// <summary>Disables Close on the window menu.</summary>
            NoClose = 0x200,

            /// <summary>Allocates a unique device context for each window in the class.</summary>
            OwnDC = 0x20,

            /// <summary>
            /// Sets the clipping rectangle of the child window to that of the parent window so that the child can draw on the parent.
            /// A window with the CS_PARENTDC style bit receives a regular device context from the system's cache of device contexts.
            /// It does not give the child the parent's device context or device context settings. Specifying CS_PARENTDC enhances an application's performance.
            /// </summary>
            ParentDC = 0x80,

            /// <summary>
            /// Saves, as a bitmap, the portion of the screen image obscured by a window of this class.
            /// When the window is removed, the system uses the saved bitmap to restore the screen image, including other windows that were obscured.
            /// Therefore, the system does not send WM_PAINT messages to windows that were obscured if the memory used by the bitmap has not been discarded and if other screen actions have not invalidated the stored image.
            /// This style is useful for small windows (for example, menus or dialog boxes) that are displayed briefly and then removed before other screen activity takes place.
            /// This style increases the time required to display the window, because the system must first allocate memory to store the bitmap.
            /// </summary>
            SaveBits = 0x800,

            /// <summary>Redraws the entire window if a movement or size adjustment changes the height of the client area.</summary>
            VerticalRedraw = 0x1
        }

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void GetSystemInfo(ref SYSTEM_INFO Info);

        public enum ShowWindowOption
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11,
        };
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowOption option);

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr handle;
            public WndMsg msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        [Flags]
        public enum QueueStatusFlags : uint
        {
            QS_KEY = 0x1,
            QS_MOUSEMOVE = 0x2,
            QS_MOUSEBUTTON = 0x4,
            QS_MOUSE = (QS_MOUSEMOVE | QS_MOUSEBUTTON),
            QS_INPUT = (QS_MOUSE | QS_KEY),
            QS_POSTMESSAGE = 0x8,
            QS_TIMER = 0x10,
            QS_PAINT = 0x20,
            QS_SENDMESSAGE = 0x40,
            QS_HOTKEY = 0x80,
            QS_REFRESH = (QS_HOTKEY | QS_KEY | QS_MOUSEBUTTON | QS_PAINT),
            QS_ALLEVENTS = (QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY),
            QS_ALLINPUT = (QS_SENDMESSAGE | QS_PAINT | QS_TIMER | QS_POSTMESSAGE | QS_MOUSEBUTTON | QS_MOUSEMOVE | QS_HOTKEY | QS_KEY),
            QS_ALLPOSTMESSAGE = 0x100,
            QS_RAWINPUT = 0x400
        }

        public const UInt32 INFINITE = 0xFFFFFFFF;
        public const UInt32 WAIT_ABANDONED = 0x00000080;
        public const UInt32 WAIT_OBJECT_0 = 0x00000000;
        public const UInt32 WAIT_TIMEOUT = 0x00000102;

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [Flags]
        public enum PeekMessageParams : uint
        {
            PM_NOREMOVE = 0x0000,
            PM_REMOVE = 0x0001,
            PM_NOYIELD = 0x0002,
            PM_QS_INPUT = QueueStatusFlags.QS_INPUT << 16,
            PM_QS_POSTMESSAGE = (QueueStatusFlags.QS_POSTMESSAGE | QueueStatusFlags.QS_HOTKEY | QueueStatusFlags.QS_TIMER) << 16,
            PM_QS_PAINT = QueueStatusFlags.QS_PAINT << 16,
            PM_QS_SENDMESSAGE = QueueStatusFlags.QS_SENDMESSAGE << 16
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(out NativeMessage lpMsg, HandleRef hWnd, uint wMsgFilterMin,
            uint wMsgFilterMax, PeekMessageParams wRemoveMsg);


        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

            public int X
            {
                get { return Left; }
                set { Right -= (Left - value); Left = value; }
            }

            public int Y
            {
                get { return Top; }
                set { Bottom -= (Top - value); Top = value; }
            }

            public int Height
            {
                get { return Bottom - Top; }
                set { Bottom = value + Top; }
            }

            public int Width
            {
                get { return Right - Left; }
                set { Right = value + Left; }
            }

            public System.Drawing.Point Location
            {
                get { return new System.Drawing.Point(Left, Top); }
                set { X = value.X; Y = value.Y; }
            }

            public System.Drawing.Size Size
            {
                get { return new System.Drawing.Size(Width, Height); }
                set { Width = value.Width; Height = value.Height; }
            }

            public static implicit operator System.Drawing.Rectangle(RECT r)
            {
                return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator RECT(System.Drawing.Rectangle r)
            {
                return new RECT(r);
            }

            public static bool operator ==(RECT r1, RECT r2)
            {
                return r1.Equals(r2);
            }

            public static bool operator !=(RECT r1, RECT r2)
            {
                return !r1.Equals(r2);
            }

            public bool Equals(RECT r)
            {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object obj)
            {
                if (obj is RECT)
                    return Equals((RECT)obj);
                else if (obj is System.Drawing.Rectangle)
                    return Equals(new RECT((System.Drawing.Rectangle)obj));
                return false;
            }

            public override int GetHashCode()
            {
                return ((System.Drawing.Rectangle)this).GetHashCode();
            }

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public enum WndMsg
        {
            WM_ACTIVATE = 0x6,
            WM_ACTIVATEAPP = 0x1,
            WM_AFXFIRST = 0x360,
            WM_AFXLAST = 0x37F,
            WM_APP = 0x8000,
            WM_ASKCBFORMATNAME = 0x30,
            WM_CANCELJOURNAL = 0x4B,
            WM_CANCELMODE = 0x1F,
            WM_CAPTURECHANGED = 0x215,
            WM_CHANGECBCHAIN = 0x30D,
            WM_CHANGEUISTATE = 0x127,
            WM_CHAR = 0x102,
            WM_CHARTOITEM = 0x2F,
            WM_CHILDACTIVATE = 0x22,
            WM_CLEAR = 0x303,
            WM_CLOSE = 0x10,
            WM_COMMAND = 0x111,
            WM_COMPACTING = 0x41,
            WM_COMPAREITEM = 0x39,
            WM_CONTEXTMENU = 0x7B,
            WM_COPY = 0x301,
            WM_COPYDATA = 0x4A,
            WM_CREATE = 0x1,
            WM_CTLCOLORBTN = 0x135,
            WM_CTLCOLORDLG = 0x136,
            WM_CTLCOLOREDIT = 0x133,
            WM_CTLCOLORLISTBOX = 0x134,
            WM_CTLCOLORMSGBOX = 0x132,
            WM_CTLCOLORSCROLLBAR = 0x137,
            WM_CTLCOLORSTATIC = 0x138,
            WM_CUT = 0x300,
            WM_DEADCHAR = 0x103,
            WM_DELETEITEM = 0x2D,
            WM_DESTROY = 0x2,
            WM_DESTROYCLIPBOARD = 0x307,
            WM_DEVICECHANGE = 0x219,
            WM_DEVMODECHANGE = 0x1B,
            WM_DISPLAYCHANGE = 0x7E,
            WM_DRAWCLIPBOARD = 0x308,
            WM_DRAWITEM = 0x2B,
            WM_DROPFILES = 0x233,
            WM_ENABLE = 0xA,
            WM_ENDSESSION = 0x16,
            WM_ENTERIDLE = 0x121,
            WM_ENTERMENULOOP = 0x211,
            WM_ENTERSIZEMOVE = 0x231,
            WM_ERASEBKGND = 0x14,
            WM_EXITMENULOOP = 0x212,
            WM_EXITSIZEMOVE = 0x232,
            WM_FONTCHANGE = 0x1D,
            WM_GETDLGCODE = 0x87,
            WM_GETFONT = 0x31,
            WM_GETHOTKEY = 0x33,
            WM_GETICON = 0x7F,
            WM_GETMINMAXINFO = 0x24,
            WM_GETOBJECT = 0x3D,
            WM_GETTEXT = 0xD,
            WM_GETTEXTLENGTH = 0xE,
            WM_HANDHELDFIRST = 0x358,
            WM_HANDHELDLAST = 0x35F,
            WM_HELP = 0x53,
            WM_HOTKEY = 0x312,
            WM_HSCROLL = 0x114,
            WM_HSCROLLCLIPBOARD = 0x30E,
            WM_ICONERASEBKGND = 0x27,
            WM_IME_CHAR = 0x286,
            WM_IME_COMPOSITION = 0x10F,
            WM_IME_COMPOSITIONFULL = 0x284,
            WM_IME_CONTROL = 0x283,
            WM_IME_ENDCOMPOSITION = 0x10E,
            WM_IME_KEYDOWN = 0x290,
            WM_IME_KEYLAST = 0x10F,
            WM_IME_KEYUP = 0x291,
            WM_IME_NOTIFY = 0x282,
            WM_IME_REQUEST = 0x288,
            WM_IME_SELECT = 0x285,
            WM_IME_SETCONTEXT = 0x281,
            WM_IME_STARTCOMPOSITION = 0x10D,
            WM_INITDIALOG = 0x110,
            WM_INITMENU = 0x116,
            WM_INITMENUPOPUP = 0x117,
            WM_INPUTLANGCHANGE = 0x51,
            WM_INPUTLANGCHANGEREQUEST = 0x50,
            WM_KEYDOWN = 0x100,
            WM_KEYFIRST = 0x100,
            WM_KEYLAST = 0x108,
            WM_KEYUP = 0x101,
            WM_KILLFOCUS = 0x8,
            WM_LBUTTONDBLCLK = 0x203,
            WM_LBUTTONDOWN = 0x201,
            WM_LBUTTONUP = 0x202,
            WM_MBUTTONDBLCLK = 0x209,
            WM_MBUTTONDOWN = 0x207,
            WM_MBUTTONUP = 0x208,
            WM_MDIACTIVATE = 0x222,
            WM_MDICASCADE = 0x227,
            WM_MDICREATE = 0x220,
            WM_MDIDESTROY = 0x221,
            WM_MDIGETACTIVE = 0x229,
            WM_MDIICONARRANGE = 0x228,
            WM_MDIMAXIMIZE = 0x225,
            WM_MDINEXT = 0x224,
            WM_MDIREFRESHMENU = 0x234,
            WM_MDIRESTORE = 0x223,
            WM_MDISETMENU = 0x230,
            WM_MDITILE = 0x226,
            WM_MEASUREITEM = 0x2,
            WM_MENUCHAR = 0x120,
            WM_MENUCOMMAND = 0x126,
            WM_MENUDRAG = 0x123,
            WM_MENUGETOBJECT = 0x124,
            WM_MENURBUTTONUP = 0x122,
            WM_MENUSELECT = 0x11F,
            WM_MOUSEACTIVATE = 0x21,
            WM_MOUSEFIRST = 0x200,
            WM_MOUSEHOVER = 0x2A1,
            WM_MOUSELAST = 0x20D,
            WM_MOUSELEAVE = 0x2A3,
            WM_MOUSEMOVE = 0x200,
            WM_MOUSEWHEEL = 0x20A,
            WM_MOUSEHWHEEL = 0x20E,
            WM_MOVE = 0x3,
            WM_MOVING = 0x216,
            WM_NCACTIVATE = 0x86,
            WM_NCCALCSIZE = 0x83,
            WM_NCCREATE = 0x81,
            WM_NCDESTROY = 0x82,
            WM_NCHITTEST = 0x84,
            WM_NCLBUTTONDBLCLK = 0xA3,
            WM_NCLBUTTONDOWN = 0xA1,
            WM_NCLBUTTONUP = 0xA2,
            WM_NCMBUTTONDBLCLK = 0xA9,
            WM_NCMBUTTONDOWN = 0xA7,
            WM_NCMBUTTONUP = 0xA8,
            WM_NCMOUSEHOVER = 0x2A0,
            WM_NCMOUSELEAVE = 0x2A2,
            WM_NCMOUSEMOVE = 0xA0,
            WM_NCPAINT = 0x85,
            WM_NCRBUTTONDBLCLK = 0xA6,
            WM_NCRBUTTONDOWN = 0xA4,
            WM_NCRBUTTONUP = 0xA5,
            WM_NCXBUTTONDBLCLK = 0xAD,
            WM_NCXBUTTONDOWN = 0xAB,
            WM_NCXBUTTONUP = 0xA,
            WM_NCUAHDRAWCAPTION = 0xAE,
            WM_NCUAHDRAWFRAME = 0xAF,
            WM_NEXTDLGCTL = 0x28,
            WM_NEXTMENU = 0x213,
            WM_NOTIFY = 0x4E,
            WM_NOTIFYFORMAT = 0x55,
            WM_NULL = 0x0,
            WM_PAINT = 0xF,
            WM_PAINTCLIPBOARD = 0x309,
            WM_PAINTICON = 0x26,
            WM_PALETTECHANGED = 0x311,
            WM_PALETTEISCHANGING = 0x310,
            WM_PARENTNOTIFY = 0x210,
            WM_PASTE = 0x302,
            WM_PENWINFIRST = 0x380,
            WM_PENWINLAST = 0x38F,
            WM_POWER = 0x48,
            WM_POWERBROADCAST = 0x218,
            WM_PRINT = 0x317,
            WM_PRINTCLIENT = 0x318,
            WM_QUERYDRAGICON = 0x37,
            WM_QUERYENDSESSION = 0x11,
            WM_QUERYNEWPALETTE = 0x30F,
            WM_QUERYOPEN = 0x13,
            WM_QUEUESYNC = 0x23,
            WM_QUIT = 0x12,
            WM_RBUTTONDBLCLK = 0x206,
            WM_RBUTTONDOWN = 0x204,
            WM_RBUTTONUP = 0x205,
            WM_RENDERALLFORMATS = 0x306,
            WM_RENDERFORMAT = 0x305,
            WM_SETCURSOR = 0x20,
            WM_SETFOCUS = 0x7,
            WM_SETFONT = 0x30,
            WM_SETHOTKEY = 0x32,
            WM_SETICON = 0x80,
            WM_SETREDRAW = 0xB,
            WM_SETTEXT = 0x000C,
            WM_SETTINGCHANGE = 0x1A,
            WM_SHOWWINDOW = 0x18,
            WM_SIZE = 0x5,
            WM_SIZECLIPBOARD = 0x30B,
            WM_SIZING = 0x214,
            WM_SPOOLERSTATUS = 0x2A,
            WM_STYLECHANGED = 0x7D,
            WM_STYLECHANGING = 0x7,
            WM_SYNCPAINT = 0x88,
            WM_SYSCHAR = 0x106,
            WM_SYSCOLORCHANGE = 0x15,
            WM_SYSCOMMAND = 0x112,
            WM_SYSDEADCHAR = 0x107,
            WM_SYSKEYDOWN = 0x104,
            WM_SYSKEYUP = 0x105,
            WM_TCARD = 0x52,
            WM_TIMECHANGE = 0x1E,
            WM_TIMER = 0x113,
            WM_UNDO = 0x304,
            WM_UNINITMENUPOPUP = 0x125,
            WM_USER = 0x400,
            WM_USERCHANGED = 0x54,
            WM_VKEYTOITEM = 0x2E,
            WM_VSCROLL = 0x115,
            WM_VSCROLLCLIPBOARD = 0x30A,
            WM_WINDOWPOSCHANGED = 0x47,
            WM_WINDOWPOSCHANGING = 0x46,
            WM_WININICHANGE = 0x1A,
            WM_XBUTTONDBLCLK = 0x20D,
            WM_XBUTTONDOWN = 0x20B,
            WM_XBUTTONUP = 0x20
        }

        public enum SysCommands : int
        {
            SC_SIZE = 0xF000,
            SC_MOVE = 0xF010,
            SC_MINIMIZE = 0xF020,
            SC_MAXIMIZE = 0xF030,
            SC_NEXTWINDOW = 0xF040,
            SC_PREVWINDOW = 0xF050,
            SC_CLOSE = 0xF060,
            SC_VSCROLL = 0xF070,
            SC_HSCROLL = 0xF080,
            SC_MOUSEMENU = 0xF090,
            SC_KEYMENU = 0xF100,
            SC_ARRANGE = 0xF110,
            SC_RESTORE = 0xF120,
            SC_TASKLIST = 0xF130,
            SC_SCREENSAVE = 0xF140,
            SC_HOTKEY = 0xF150,
            //#if(WINVER >= 0x0400) //Win95
            SC_DEFAULT = 0xF160,
            SC_MONITORPOWER = 0xF170,
            SC_CONTEXTHELP = 0xF180,
            SC_SEPARATOR = 0xF00F,
            //#endif /* WINVER >= 0x0400 */

            //#if(WINVER >= 0x0600) //Vista
            SCF_ISSECURE = 0x00000001,
            //#endif /* WINVER >= 0x0600 */

            /*
              * Obsolete names
              */
            SC_ICON = SC_MINIMIZE,
            SC_ZOOM = SC_MAXIMIZE,
        }

        /// <summary>
        /// Enumeration for virtual keys.
        /// </summary>
        public enum VirtualKeys
            : ushort
        {
            LeftButton = 0x01,
            RightButton = 0x02,
            Cancel = 0x03,
            MiddleButton = 0x04,
            ExtraButton1 = 0x05,
            ExtraButton2 = 0x06,
            Back = 0x08,
            Tab = 0x09,
            Clear = 0x0C,
            Return = 0x0D,
            Shift = 0x10,
            Control = 0x11,
            /// <summary></summary>
            Menu = 0x12,
            /// <summary></summary>
            Pause = 0x13,
            /// <summary></summary>
            CapsLock = 0x14,
            /// <summary></summary>
            Kana = 0x15,
            /// <summary></summary>
            Hangeul = 0x15,
            /// <summary></summary>
            Hangul = 0x15,
            /// <summary></summary>
            Junja = 0x17,
            /// <summary></summary>
            Final = 0x18,
            /// <summary></summary>
            Hanja = 0x19,
            /// <summary></summary>
            Kanji = 0x19,
            /// <summary></summary>
            Escape = 0x1B,
            /// <summary></summary>
            Convert = 0x1C,
            /// <summary></summary>
            NonConvert = 0x1D,
            /// <summary></summary>
            Accept = 0x1E,
            /// <summary></summary>
            ModeChange = 0x1F,
            /// <summary></summary>
            Space = 0x20,
            /// <summary></summary>
            Prior = 0x21,
            /// <summary></summary>
            Next = 0x22,
            /// <summary></summary>
            End = 0x23,
            /// <summary></summary>
            Home = 0x24,
            /// <summary></summary>
            Left = 0x25,
            /// <summary></summary>
            Up = 0x26,
            /// <summary></summary>
            Right = 0x27,
            /// <summary></summary>
            Down = 0x28,
            /// <summary></summary>
            Select = 0x29,
            /// <summary></summary>
            Print = 0x2A,
            /// <summary></summary>
            Execute = 0x2B,
            /// <summary></summary>
            Snapshot = 0x2C,
            /// <summary></summary>
            Insert = 0x2D,
            /// <summary></summary>
            Delete = 0x2E,
            /// <summary></summary>
            Help = 0x2F,
            /// <summary></summary>
            N0 = 0x30,
            /// <summary></summary>
            N1 = 0x31,
            /// <summary></summary>
            N2 = 0x32,
            /// <summary></summary>
            N3 = 0x33,
            /// <summary></summary>
            N4 = 0x34,
            /// <summary></summary>
            N5 = 0x35,
            /// <summary></summary>
            N6 = 0x36,
            /// <summary></summary>
            N7 = 0x37,
            /// <summary></summary>
            N8 = 0x38,
            /// <summary></summary>
            N9 = 0x39,
            /// <summary></summary>
            A = 0x41,
            /// <summary></summary>
            B = 0x42,
            /// <summary></summary>
            C = 0x43,
            /// <summary></summary>
            D = 0x44,
            /// <summary></summary>
            E = 0x45,
            /// <summary></summary>
            F = 0x46,
            /// <summary></summary>
            G = 0x47,
            /// <summary></summary>
            H = 0x48,
            /// <summary></summary>
            I = 0x49,
            /// <summary></summary>
            J = 0x4A,
            /// <summary></summary>
            K = 0x4B,
            /// <summary></summary>
            L = 0x4C,
            /// <summary></summary>
            M = 0x4D,
            /// <summary></summary>
            N = 0x4E,
            /// <summary></summary>
            O = 0x4F,
            /// <summary></summary>
            P = 0x50,
            /// <summary></summary>
            Q = 0x51,
            /// <summary></summary>
            R = 0x52,
            /// <summary></summary>
            S = 0x53,
            /// <summary></summary>
            T = 0x54,
            /// <summary></summary>
            U = 0x55,
            /// <summary></summary>
            V = 0x56,
            /// <summary></summary>
            W = 0x57,
            /// <summary></summary>
            X = 0x58,
            /// <summary></summary>
            Y = 0x59,
            /// <summary></summary>
            Z = 0x5A,
            /// <summary></summary>
            LeftWindows = 0x5B,
            /// <summary></summary>
            RightWindows = 0x5C,
            /// <summary></summary>
            Application = 0x5D,
            /// <summary></summary>
            Sleep = 0x5F,
            /// <summary></summary>
            Numpad0 = 0x60,
            /// <summary></summary>
            Numpad1 = 0x61,
            /// <summary></summary>
            Numpad2 = 0x62,
            /// <summary></summary>
            Numpad3 = 0x63,
            /// <summary></summary>
            Numpad4 = 0x64,
            /// <summary></summary>
            Numpad5 = 0x65,
            /// <summary></summary>
            Numpad6 = 0x66,
            /// <summary></summary>
            Numpad7 = 0x67,
            /// <summary></summary>
            Numpad8 = 0x68,
            /// <summary></summary>
            Numpad9 = 0x69,
            /// <summary></summary>
            Multiply = 0x6A,
            /// <summary></summary>
            Add = 0x6B,
            /// <summary></summary>
            Separator = 0x6C,
            /// <summary></summary>
            Subtract = 0x6D,
            /// <summary></summary>
            Decimal = 0x6E,
            /// <summary></summary>
            Divide = 0x6F,
            /// <summary></summary>
            F1 = 0x70,
            /// <summary></summary>
            F2 = 0x71,
            /// <summary></summary>
            F3 = 0x72,
            /// <summary></summary>
            F4 = 0x73,
            /// <summary></summary>
            F5 = 0x74,
            /// <summary></summary>
            F6 = 0x75,
            /// <summary></summary>
            F7 = 0x76,
            /// <summary></summary>
            F8 = 0x77,
            /// <summary></summary>
            F9 = 0x78,
            /// <summary></summary>
            F10 = 0x79,
            /// <summary></summary>
            F11 = 0x7A,
            /// <summary></summary>
            F12 = 0x7B,
            /// <summary></summary>
            F13 = 0x7C,
            /// <summary></summary>
            F14 = 0x7D,
            /// <summary></summary>
            F15 = 0x7E,
            /// <summary></summary>
            F16 = 0x7F,
            /// <summary></summary>
            F17 = 0x80,
            /// <summary></summary>
            F18 = 0x81,
            /// <summary></summary>
            F19 = 0x82,
            /// <summary></summary>
            F20 = 0x83,
            /// <summary></summary>
            F21 = 0x84,
            /// <summary></summary>
            F22 = 0x85,
            /// <summary></summary>
            F23 = 0x86,
            /// <summary></summary>
            F24 = 0x87,
            /// <summary></summary>
            NumLock = 0x90,
            /// <summary></summary>
            ScrollLock = 0x91,
            /// <summary></summary>
            NEC_Equal = 0x92,
            /// <summary></summary>
            Fujitsu_Jisho = 0x92,
            /// <summary></summary>
            Fujitsu_Masshou = 0x93,
            /// <summary></summary>
            Fujitsu_Touroku = 0x94,
            /// <summary></summary>
            Fujitsu_Loya = 0x95,
            /// <summary></summary>
            Fujitsu_Roya = 0x96,
            /// <summary></summary>
            LeftShift = 0xA0,
            /// <summary></summary>
            RightShift = 0xA1,
            /// <summary></summary>
            LeftControl = 0xA2,
            /// <summary></summary>
            RightControl = 0xA3,
            /// <summary></summary>
            LeftMenu = 0xA4,
            /// <summary></summary>
            RightMenu = 0xA5,
            /// <summary></summary>
            BrowserBack = 0xA6,
            /// <summary></summary>
            BrowserForward = 0xA7,
            /// <summary></summary>
            BrowserRefresh = 0xA8,
            /// <summary></summary>
            BrowserStop = 0xA9,
            /// <summary></summary>
            BrowserSearch = 0xAA,
            /// <summary></summary>
            BrowserFavorites = 0xAB,
            /// <summary></summary>
            BrowserHome = 0xAC,
            /// <summary></summary>
            VolumeMute = 0xAD,
            /// <summary></summary>
            VolumeDown = 0xAE,
            /// <summary></summary>
            VolumeUp = 0xAF,
            /// <summary></summary>
            MediaNextTrack = 0xB0,
            /// <summary></summary>
            MediaPrevTrack = 0xB1,
            /// <summary></summary>
            MediaStop = 0xB2,
            /// <summary></summary>
            MediaPlayPause = 0xB3,
            /// <summary></summary>
            LaunchMail = 0xB4,
            /// <summary></summary>
            LaunchMediaSelect = 0xB5,
            /// <summary></summary>
            LaunchApplication1 = 0xB6,
            /// <summary></summary>
            LaunchApplication2 = 0xB7,
            /// <summary></summary>
            OEM1 = 0xBA,
            /// <summary></summary>
            OEMPlus = 0xBB,
            /// <summary></summary>
            OEMComma = 0xBC,
            /// <summary></summary>
            OEMMinus = 0xBD,
            /// <summary></summary>
            OEMPeriod = 0xBE,
            /// <summary></summary>
            OEM2 = 0xBF,
            /// <summary></summary>
            OEM3 = 0xC0,
            /// <summary></summary>
            OEM4 = 0xDB,
            /// <summary></summary>
            OEM5 = 0xDC,
            /// <summary></summary>
            OEM6 = 0xDD,
            /// <summary></summary>
            OEM7 = 0xDE,
            /// <summary></summary>
            OEM8 = 0xDF,
            /// <summary></summary>
            OEMAX = 0xE1,
            /// <summary></summary>
            OEM102 = 0xE2,
            /// <summary></summary>
            ICOHelp = 0xE3,
            /// <summary></summary>
            ICO00 = 0xE4,
            /// <summary></summary>
            ProcessKey = 0xE5,
            /// <summary></summary>
            ICOClear = 0xE6,
            /// <summary></summary>
            Packet = 0xE7,
            /// <summary></summary>
            OEMReset = 0xE9,
            /// <summary></summary>
            OEMJump = 0xEA,
            /// <summary></summary>
            OEMPA1 = 0xEB,
            /// <summary></summary>
            OEMPA2 = 0xEC,
            /// <summary></summary>
            OEMPA3 = 0xED,
            /// <summary></summary>
            OEMWSCtrl = 0xEE,
            /// <summary></summary>
            OEMCUSel = 0xEF,
            /// <summary></summary>
            OEMATTN = 0xF0,
            /// <summary></summary>
            OEMFinish = 0xF1,
            /// <summary></summary>
            OEMCopy = 0xF2,
            /// <summary></summary>
            OEMAuto = 0xF3,
            /// <summary></summary>
            OEMENLW = 0xF4,
            /// <summary></summary>
            OEMBackTab = 0xF5,
            /// <summary></summary>
            ATTN = 0xF6,
            /// <summary></summary>
            CRSel = 0xF7,
            /// <summary></summary>
            EXSel = 0xF8,
            /// <summary></summary>
            EREOF = 0xF9,
            /// <summary></summary>
            Play = 0xFA,
            /// <summary></summary>
            Zoom = 0xFB,
            /// <summary></summary>
            Noname = 0xFC,
            /// <summary></summary>
            PA1 = 0xFD,
            /// <summary></summary>
            OEMClear = 0xFE
        }

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
    }
}
