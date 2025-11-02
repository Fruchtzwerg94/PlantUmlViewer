using System;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace PlantUmlViewer.Windows
{
    /// <summary>
    /// Allows localization of Java by checking JAVA_HOME, Windows registry and PATH
    /// </summary>
    internal static class JavaLocator
    {
        /// <summary>
        /// Gets the path of the Java executable by checking JAVA_HOME, Windows registry and PATH
        /// </summary>
        /// <returns>The path of the Java executable</returns>
        /// <exception cref="InvalidOperationException">Failed to find Java checking JAVA_HOME, Windows registry and PATH</exception>
        /// <exception cref="FileNotFoundException">"Found Java but the executable does not exist</exception>
        public static string GetJavaExecutable()
        {
            string javaExecutable;

            string javaHome = GetJavaHome();
            if (!string.IsNullOrEmpty(javaHome))
            {
                javaExecutable = Path.Combine(javaHome, "bin", "java.exe");
            }
            else
            {
                javaExecutable = FindExecutableInPath("java.exe");
            }

            if (string.IsNullOrEmpty(javaExecutable))
            {
                throw new InvalidOperationException("Failed to find Java checking JAVA_HOME, Windows registry and PATH");
            }
            if (!File.Exists(javaExecutable))
            {
                throw new FileNotFoundException($"Found Java but the executable '{javaExecutable}' does not exist");
            }
            return javaExecutable;
        }

        private static string GetJavaHome()
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

            return null;
        }

        private static string FindExecutableInPath(string executableName)
        {
            string pathEnv = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrWhiteSpace(pathEnv))
            {
                return null;
            }

            string[] paths = pathEnv.Split(Path.PathSeparator);
            foreach (string path in paths)
            {
                try
                {
                    string fullPath = Path.Combine(path.Trim(), executableName);
                    if (File.Exists(fullPath))
                    {
                        return fullPath;
                    }
                }
                catch
                {
                    //Ignore invalid paths or access issues
                }
            }
            return null;
        }
    }
}