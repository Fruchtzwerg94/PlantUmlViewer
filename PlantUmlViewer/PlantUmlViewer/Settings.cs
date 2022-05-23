using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Kbg.NppPluginNET.PluginInfrastructure;

namespace PlantUmlViewer
{
    internal class Settings
    {
        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        private readonly string iniFilePath;

        private readonly Dictionary<string, string> settingsBuffer = new Dictionary<string, string>();

        public Settings()
        {
            StringBuilder sb = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sb);
            iniFilePath = sb.ToString();
            if (!Directory.Exists(iniFilePath))
            {
                Directory.CreateDirectory(iniFilePath);
            }
            iniFilePath = Path.Combine(iniFilePath, $"{nameof(PlantUmlViewer)}.ini");
            Debug.WriteLine($"Using settings file '{iniFilePath}'", nameof(Settings));
        }

        public string GetSetting(string key, string defaultValue)
        {
            if (settingsBuffer.TryGetValue(key, out string bufferedValue))
            {
                Debug.WriteLine($"Get buffered setting '{key}': '{bufferedValue}'", nameof(Settings));
                return bufferedValue;
            }
            else
            {
                StringBuilder sb = new StringBuilder(4096);
                string value;
                if (GetPrivateProfileString(nameof(PlantUmlViewer), key, "", sb, 4096, iniFilePath) < 1)
                {
                    value = defaultValue;
                }
                else
                {
                    value = sb.ToString();
                }
                settingsBuffer[key] = value;
                Debug.WriteLine($"Get setting '{key}': '{value}'", nameof(Settings));
                return value;
            }
        }

        public void SetSetting(string key, string value)
        {
            Debug.WriteLine($"Set setting '{key}': '{value}'", nameof(Settings));
            Win32.WritePrivateProfileString(nameof(PlantUmlViewer), key, value, iniFilePath);
            settingsBuffer[key] = value;
        }
    }
}
