using System.Diagnostics;

using Kbg.NppPluginNET.PluginInfrastructure;

namespace Kbg.NppPluginNET
{
    internal static class Main
    {
        public const string PluginName = PlantUmlViewer.PlantUmlViewer.PLUGIN_NAME;

        private static readonly PlantUmlViewer.PlantUmlViewer plantUmlViewer = new PlantUmlViewer.PlantUmlViewer();

        public static void OnNotification(ScNotification notification)
        {
            Debug.WriteLine($"OnNotification: {notification.Header.Code}", nameof(Main));
            plantUmlViewer.OnNotification(notification);
        }

        internal static void CommandMenuInit()
        {
            Debug.WriteLine("CommandMenuInit", nameof(Main));
            plantUmlViewer.CommandMenuInit();
        }

        internal static void SetToolBarIcon()
        {
            Debug.WriteLine("SetToolBarIcon", nameof(Main));
            plantUmlViewer.SetToolBarIcon();
        }

        internal static void PluginCleanUp()
        {
            Debug.WriteLine("PluginCleanUp", nameof(Main));
            plantUmlViewer.PluginCleanUp();
        }
    }
}