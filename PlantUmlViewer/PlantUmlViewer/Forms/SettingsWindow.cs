using System;
using System.Windows.Forms;

namespace PlantUmlViewer.Forms
{
    internal partial class SettingsWindow : Form
    {
        private readonly Settings settings;

        public SettingsWindow(Settings settings)
        {
            this.settings = settings;

            InitializeComponent();

            textBox_JavaPath.Text = settings.GetSetting("JavaPath", "");
        }

        private void Button_Ok_Click(object sender, EventArgs e)
        {
            settings.SetSetting("JavaPath", textBox_JavaPath.Text);

            DialogResult = DialogResult.OK;
        }
    }
}
