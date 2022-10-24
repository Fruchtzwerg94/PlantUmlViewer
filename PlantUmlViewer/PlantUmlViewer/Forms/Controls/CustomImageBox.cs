using System.Windows.Forms;

using Cyotek.Windows.Forms;

namespace PlantUmlViewer.Forms.Controls
{
    public class CustomImageBox : ImageBox
    {
        //Override the default mouse wheel action to allow custom zoom or scroll as discussed in https://github.com/cyotek/Cyotek.Windows.Forms.ImageBox/issues/18
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Control)
            {
                base.OnMouseWheel(e);
            }
            else if (VerticalScroll.Visible && VerticalScroll.Enabled && ModifierKeys == Keys.None)
            {
                int scrollDelta = SystemInformation.MouseWheelScrollLines * VerticalScroll.SmallChange;
                ScrollTo(HorizontalScroll.Value, VerticalScroll.Value + ((e.Delta > 0) ? -scrollDelta : scrollDelta));
            }
            else if (HorizontalScroll.Visible && HorizontalScroll.Enabled && ModifierKeys == Keys.Shift)
            {
                int scrollDelta = SystemInformation.MouseWheelScrollLines * HorizontalScroll.SmallChange;
                ScrollTo(HorizontalScroll.Value + ((e.Delta > 0) ? -scrollDelta : scrollDelta), VerticalScroll.Value);
            }
        }
    }
}
