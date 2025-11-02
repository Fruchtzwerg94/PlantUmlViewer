using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

using PlantUmlViewer.Helpers;
using PlantUmlViewer.Settings;
using PlantUmlViewer.Windows;

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

        private async void Button_JavaPathCheck_Click(object sender, EventArgs e)
        {
            string javaExecutable = textBox_JavaPath.Text;
            if (string.IsNullOrWhiteSpace(javaExecutable))
            {
                try
                {
                    javaExecutable = JavaLocator.GetJavaExecutable();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Failed to locate Java:{Environment.NewLine}{ex.Message}", "Java check",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            string javaVersionInformation;
            try
            {
                ProcessResult result = await ProcessHelper.RunProcessAsync(
                    javaExecutable, new string[] { "-version" }, null);
                if (result.ExitCode != 0)
                {
                    string message = Encoding.UTF8.GetString(result.Error);
                    throw new IOException(message);
                }
                //Get version information from stderr
                javaVersionInformation = Encoding.UTF8.GetString(result.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to run Java:{Environment.NewLine}{javaExecutable}{Environment.NewLine}{ex.Message}", "Java check",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show(this, $"Java executable found:{Environment.NewLine}{javaExecutable}{Environment.NewLine}{javaVersionInformation}",
                "Java check", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void Button_PlantUmlPathCheck_Click(object sender, EventArgs e)
        {
            string javaExecutable = textBox_JavaPath.Text;
            if (string.IsNullOrWhiteSpace(javaExecutable))
            {
                try
                {
                    javaExecutable = JavaLocator.GetJavaExecutable();
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show(this, $"Java not found:{Environment.NewLine}{ex.Message}",
                        "PlantUML check", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            javaExecutable = PathHelper.ResolvePathToAssembly(javaExecutable);

            string plantUmlJar = textBox_PlantUmlPath.Text;
            if (string.IsNullOrWhiteSpace(plantUmlJar))
            {
                plantUmlJar = PlantUmlViewer.PLANT_UML_JAR;
            }
            plantUmlJar = PathHelper.ResolvePathToAssembly(plantUmlJar);

            string plantUmlVersionInformation;
            try
            {
                ProcessResult result = await ProcessHelper.RunProcessAsync(javaExecutable,
                    new string[] { "-jar", $"\"{plantUmlJar}\"", "-version" }, null);
                if (result.ExitCode != 0)
                {
                    throw new IOException($"Code {result.ExitCode}: {Encoding.UTF8.GetString(result.Error)}");
                }
                //Get version information from stdout
                plantUmlVersionInformation = Encoding.UTF8.GetString(result.Output);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to run PlantUML:{Environment.NewLine}{plantUmlJar}{Environment.NewLine}{ex.Message}",
                    "PlantUML check", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show(this, $"PlantUML valid:{Environment.NewLine}{plantUmlJar}{Environment.NewLine}{plantUmlVersionInformation}",
                "PlantUML check", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
