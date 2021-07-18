using System;
using System.Windows.Forms;

namespace Refterm
{
    class Program
    {
        [STAThread]
        static void Main()
        {
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
