using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TimeCard
{
    /// <summary>
    /// gets information about the program
    /// </summary>
    public static class ProgramInfo
    {
        public static string AssemblyGuid
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((GuidAttribute)attributes[0]).Value;
            }
        }

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute title_attribute = (AssemblyTitleAttribute)attributes[0];
                    if (title_attribute.Title != "")
                    {
                        return title_attribute.Title;
                    }
                }
                return Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
            }
        }
    }
}
