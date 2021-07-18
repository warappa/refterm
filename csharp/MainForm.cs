using System;
using System.Threading;
using System.Windows.Forms;
using static Refterm.NativeWindows;

namespace Refterm
{
    public class MainForm : Form
    {
        private Thread terminalThread;
        uint terminalThreadId;


        [STAThread]
        private void ThreadMethod(object? handle)
        {
            terminalThreadId = NativeWindows.GetCurrentThreadId();
            var terminal = new Terminal((IntPtr)handle);
        }

        public MainForm()
        {
            terminalThread = new Thread(new ParameterizedThreadStart(ThreadMethod));
            terminalThread.Start(Handle);
        }
        protected override void WndProc(ref Message m)
        {
            var type = (WndMsg)m.Msg;
            if (terminalThreadId > 0 &&
                (type == WndMsg.WM_CHAR ||
                type == WndMsg.WM_KEYDOWN ||
                type == WndMsg.WM_QUIT ||
                type == WndMsg.WM_SIZE))
            {
                NativeWindows.PostThreadMessage(terminalThreadId, (uint)m.Msg, m.WParam, m.LParam);
            }
            else
            {
                if (type == WndMsg.WM_SYSCOMMAND && 
                    m.WParam.ToInt32() == (int)SysCommands.SC_CLOSE)
                {
                    NativeWindows.PostThreadMessage(terminalThreadId, (uint)WndMsg.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
                }

                base.WndProc(ref m);
            }
        }
    }
}
