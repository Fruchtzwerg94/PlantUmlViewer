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

            textBox_JavaPath.Text = settings.Settings.JavaPath;
            numericUpDown_ExportSizeFactor.Value = settings.Settings.ExportSizeFactor;
        }

        private void Button_Ok_Click(object sender, EventArgs e)
        {
            settings.Settings.JavaPath = textBox_JavaPath.Text;
            settings.Settings.ExportSizeFactor = numericUpDown_ExportSizeFactor.Value;
            settings.Save();

            DialogResult = DialogResult.OK;
        }
    }
}
