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

            comboBox_OpenExport.DataSource = Enum.GetValues(typeof(OpenExport));

            textBox_JavaPath.Text = settings.Settings.JavaPath;
            textBox_PlantUmlPath.Text = settings.Settings.PlantUmlPath;
            textBox_Include.Text = settings.Settings.Include;
            numericUpDown_ExportSizeFactor.Value = settings.Settings.ExportSizeFactor;
            checkBox_ExportDocument.Checked = settings.Settings.ExportDocument;
            comboBox_OpenExport.SelectedItem = settings.Settings.OpenExport;
        }

        private void Button_Ok_Click(object sender, EventArgs e)
        {
            settings.Settings.JavaPath = textBox_JavaPath.Text;
            settings.Settings.PlantUmlPath = textBox_PlantUmlPath.Text;
            settings.Settings.Include = textBox_Include.Text;
            settings.Settings.ExportSizeFactor = numericUpDown_ExportSizeFactor.Value;
            settings.Settings.ExportDocument = checkBox_ExportDocument.Checked;
            settings.Settings.OpenExport = (OpenExport)comboBox_OpenExport.SelectedValue;
            settings.Save();

            DialogResult = DialogResult.OK;
        }
    }
}
