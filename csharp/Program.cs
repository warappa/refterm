using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Refterm
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //NativeWindows.AllocConsole();
            //var aa = NativeWindows.SetConsoleCP((uint)1200);
            //var bb = NativeWindows.SetConsoleOutputCP((uint)1200);
            //var res = NativeWindows.AttachConsole(Process.GetCurrentProcess().Id);
            //if (!res)
            //{
            //    var a = Marshal.GetLastWin32Error();
            //    var error = NativeWindows.GetLastError();
            //}
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = new MainForm();
            mainForm.Width = 612;
            mainForm.Height = 612;

            Application.Run(mainForm);
        }
    }
}
