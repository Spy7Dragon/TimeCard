using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace TimeCard
{
    public static class Logger
    {
        private const string SUB_PATH = "../Logs";
        private static string s_LogFile;

        private static LogForm s_Form;

        private static FileStream WriterStream
        {
            get
            {
                FileStream stream = new FileStream(s_LogFile, FileMode.Append, FileAccess.Write, FileShare.Read);
                return stream;
            }
        }

        private static FileStream ReaderStream
        {
            get
            {
                FileStream stream = new FileStream(s_LogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                return stream;
            }
        }

        private static LogForm InstanceForm
        {
            get
            {
                if (s_Form == null || s_Form.IsDisposed)
                {
                    s_Form = new LogForm();
                }
                return s_Form;
            }
        }

        static Logger()
        {
            CreateNewLogger();
        }

        public static void CreateNewLogger()
        {
            // Create Log file.
            try
            {
                string date_string = DateTime.Now.ToString("MM-dd-yyyy_hh-mm-ss_tt");
                string full_path = Path.Combine(Application.LocalUserAppDataPath, SUB_PATH);
                bool exists = Directory.Exists(full_path);
                if (!exists)
                {
                    Directory.CreateDirectory(full_path);
                }

                s_LogFile = Path.Combine(full_path, date_string);
                using (StreamWriter writer = new StreamWriter(WriterStream)
                {
                    AutoFlush = true
                })
                {
                    writer.WriteLine("Log file saved to: " + s_LogFile);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to create log file.\n"
                                + e.Message + "\n" + e.StackTrace);
            }
        }

        public static void Log(string message)
        {
            try
            {
                string date_string = DateTime.Now.ToString("MM-dd-yyyy_hh-mm-ss_tt");
                using (StreamWriter writer = new StreamWriter(WriterStream)
                {
                    AutoFlush = true
                })
                {
                    writer.WriteLine(date_string + ": " + message);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Unable to log message.\n" + e.Message + "\n" + e.StackTrace);
            }
            OnLog();
        }

        public static void LogException(Exception e)
        {
            Log(e.Message + "\n" + e.StackTrace);
        }

        public static void ViewCurrentLog(object sender, EventArgs e)
        {
            try
            {
                InstanceForm.Show();
                using (StreamReader reader = new StreamReader(ReaderStream))
                {
                    InstanceForm.lblText.Text = reader.ReadToEnd();
                }
            }
            catch (Exception exc)
            {
                string message = "Unable to open log file.\n"
                                 + exc.Message + "\n" + exc.StackTrace;
                Log(message);
                MessageBox.Show(message);
            }
        }

        private static void OnLog()
        {
            if (s_Form != null)
            {
                using (StreamReader reader = new StreamReader(ReaderStream))
                {
                    InstanceForm.lblText.Text = reader.ReadToEnd();
                }
            }
        }
    }
}
