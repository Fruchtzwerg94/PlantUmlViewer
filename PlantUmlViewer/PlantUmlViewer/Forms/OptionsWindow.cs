using System;
using System.Windows.Forms;

using PlantUmlViewer.Settings;

namespace PlantUmlViewer.Forms
{
    internal partial class OptionsWindow : Form
    {
        private readonly SettingsService settings;

        public OptionsWindow(SettingsService settings)
        {
            this.settings = settings;

            InitializeComponent();

            this.Text = PlantUmlViewer.PLUGIN_NAME + " options";

            textBox_JavaPath.Text = settings.Settings.JavaPath;
            textBox_Include.Text = settings.Settings.Include;
            numericUpDown_ExportSizeFactor.Value = settings.Settings.ExportSizeFactor;
        }

        private void Button_Ok_Click(object sender, EventArgs e)
        {
            settings.Settings.JavaPath = textBox_JavaPath.Text;
            settings.Settings.Include = textBox_Include.Text;
            settings.Settings.ExportSizeFactor = numericUpDown_ExportSizeFactor.Value;
            settings.Save();

            DialogResult = DialogResult.OK;
        }
    }
}
