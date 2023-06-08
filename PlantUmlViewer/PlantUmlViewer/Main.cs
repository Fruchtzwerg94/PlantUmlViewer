using System.Diagnostics;

using Kbg.NppPluginNET.PluginInfrastructure;

namespace Kbg.NppPluginNET
{
    public static class Main
    {
        public const string PluginName = PlantUmlViewer.PlantUmlViewer.PLUGIN_NAME;

        private static readonly PlantUmlViewer.PlantUmlViewer plantUmlViewer = new PlantUmlViewer.PlantUmlViewer();

        public static void OnNotification(ScNotification notification)
        {
            Debug.WriteLine($"OnNotification: {notification.Header.Code}", nameof(Main));
            plantUmlViewer.OnNotification(notification);
        }

        public static void CommandMenuInit()
        {
            Debug.WriteLine("CommandMenuInit", nameof(Main));
            plantUmlViewer.CommandMenuInit();
        }

        public static void SetToolBarIcon()
        {
            Debug.WriteLine("SetToolBarIcon", nameof(Main));
            plantUmlViewer.SetToolBarIcon();
        }

        public static void PluginCleanUp()
        {
            Debug.WriteLine("PluginCleanUp", nameof(Main));
            plantUmlViewer.PluginCleanUp();
        }
    }
}