using System;
using System.Collections.Generic;
using System.Linq;
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

        public static IEnumerable<T> GetChildren<T>(this Control control, int depth)
        {
            if (depth-- >= 1)
            {
                foreach (var childControl in control.Controls.Cast<Control>())
                {
                    if (childControl is T child)
                    {
                        yield return child;
                    }
                    else
                    {
                        foreach (T next in childControl.GetChildren<T>(depth))
                        {
                            yield return next;
                        }
                    }
                }
            }
        }
    }
}
