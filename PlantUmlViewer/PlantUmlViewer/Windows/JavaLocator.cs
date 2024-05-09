using System;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace PlantUmlViewer.Windows
{
    /// <summary>
    /// Allows localization of the Java installation by checking the JAVA_HOME environment variable or optionally the Windows registry
    /// </summary>
    internal static class JavaLocator
    {
        /// <summary>
        /// Gets the path of the installed Java executable by checking the JAVA_HOME environment variable or optionally the Windows registry
        /// </summary>
        /// <returns>The path of the installed Java executable</returns>
        /// <exception cref="InvalidOperationException">Java executable does not exist</exception>
        /// <exception cref="InvalidOperationException">Failed to get Java installation checking JAVA_HOME and optionally Windows registry</exception>
        public static string GetJavaExecutable()
        {
            string javaHome = GetJavaHome();
            string javaExecutable= Path.Combine(javaHome, "bin", "java.exe");
            if (!File.Exists(javaExecutable))
            {
                throw new InvalidOperationException($"Java executable '{javaExecutable}' does not exist");
            }
            return javaExecutable;
        }

        /// <summary>
        /// Gets the installed Java home path by checking the JAVA_HOME environment variable or optionally the Windows registry
        /// </summary>
        /// <returns>The installed Java home path</returns>
        /// <exception cref="InvalidOperationException">Failed to get Java installation checking JAVA_HOME and optionally Windows registry</exception>
        public static string GetJavaHome()
        {
            //First use JAVA_HOME
            string javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrWhiteSpace(javaHome))
            {
                return javaHome.Trim('"');
            }

            //Else use Windows registry
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using (RegistryKey localMachineKey = Environment.Is64BitOperatingSystem ?
                    RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64) : Registry.LocalMachine)
                {
                    using (RegistryKey rk = localMachineKey.OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment\\"))
                    {
                        if (rk != null)
                        {
                            string currentVersion = rk.GetValue("CurrentVersion").ToString();
                            using (RegistryKey key = rk.OpenSubKey(currentVersion))
                            {
                                if (key != null)
                                {
                                    return key.GetValue("JavaHome").ToString();
                                }
                            }
                        }
                    }
                }
            }

            throw new InvalidOperationException("Failed to get Java installation checking JAVA_HOME and optionally Windows registry");
        }
    }
}