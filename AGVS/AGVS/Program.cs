using System;
using System.Windows.Forms;

namespace AGVS
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool ret;
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName, out ret);

            if (ret)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmMain());
            }
            else
            {
                MessageBox.Show("程序已启动！");
                Environment.Exit(1);
            }
        }
    }
}
