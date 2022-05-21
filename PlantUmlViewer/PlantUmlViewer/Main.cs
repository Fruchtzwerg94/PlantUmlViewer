using Kbg.NppPluginNET.PluginInfrastructure;

namespace Kbg.NppPluginNET
{
    internal static class Main
    {
        public const string PluginName = PlantUmlViewer.PlantUmlViewer.PLUGIN_NAME;

        private static readonly PlantUmlViewer.PlantUmlViewer plantUmlViewer = new PlantUmlViewer.PlantUmlViewer();

        public static void OnNotification(ScNotification notification)
        {
            plantUmlViewer.OnNotification(notification);
        }

        internal static void CommandMenuInit()
        {
            plantUmlViewer.CommandMenuInit();
        }

        internal static void SetToolBarIcon()
        {
            plantUmlViewer.SetToolBarIcon();
        }

        internal static void PluginCleanUp()
        {
            plantUmlViewer.PluginCleanUp();
        }
    }
}