using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Kbg.NppPluginNET.PluginInfrastructure;

namespace PlantUmlViewer.Settings
{
    internal class SettingsService
    {
        private readonly string settingsFilePath;
        public PlantUmlViewerSettings Settings { get; private set; } = new PlantUmlViewerSettings();

        public SettingsService(INotepadPPGateway notepadPp)
        {
            settingsFilePath = notepadPp.GetPluginConfigPath();
            if (!Directory.Exists(settingsFilePath))
            {
                Directory.CreateDirectory(settingsFilePath);
            }
            settingsFilePath = Path.Combine(settingsFilePath, $"{nameof(PlantUmlViewer)}.xml");
            Debug.WriteLine($"Using settings file '{settingsFilePath}'", nameof(SettingsService));

            Load();
        }

        private void Load()
        {
            try
            {
                if (!File.Exists(settingsFilePath))
                {
                    Debug.WriteLine("Settings file does not exist, using defaults", nameof(SettingsService));
                    return;
                }
                XmlSerializer serializer = new XmlSerializer(typeof(PlantUmlViewerSettings));
                using (XmlReader xmlReader = XmlReader.Create(settingsFilePath, new XmlReaderSettings()
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null
                }))
                {
                    Settings = (PlantUmlViewerSettings)serializer.Deserialize(xmlReader);
                }
                Debug.WriteLine("Settings loaded", nameof(SettingsService));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load settings: {ex}", nameof(SettingsService));
            }
        }

        public void Save()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(Settings.GetType());
                using (StreamWriter settingsFileWriter = new StreamWriter(settingsFilePath))
                using (XmlWriter xmlWriter = XmlWriter.Create(settingsFileWriter, new XmlWriterSettings() { Indent = true }))
                {
                    serializer.Serialize(xmlWriter, Settings);
                }
                Debug.WriteLine("Settings saved", nameof(SettingsService));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save settings: {ex}", nameof(SettingsService));
            }
        }
    }
}
