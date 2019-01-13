using System;
using System.Windows.Forms;

namespace TimeCard
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!SingleInstance.Start())
            {
                SingleInstance.ShowFirstInstance();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start Application.
            try
            {
                Application.Run(FrmTimeCard.Instance);
            }
            catch (Exception e)
            {
                string message = "Application failed.\n" + e.Message + "\n" + e.StackTrace;
                Logger.Log(message);
                MessageBox.Show(message);
            }
            SingleInstance.Stop();
        }
    }
}
