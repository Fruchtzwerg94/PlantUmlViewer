using System;
using System.Windows.Forms;

namespace PlantUmlViewer.Forms
{
    public static class ControlsExtensions
    {
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}
