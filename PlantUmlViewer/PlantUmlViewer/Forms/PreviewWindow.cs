﻿using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using PlantUmlViewer.Properties;

using PlantUml.Net;

namespace PlantUmlViewer.Forms
{
    internal partial class PreviewWindow : Form
    {
        private readonly string plantUmlBinary;
        private readonly Func<string> getFilePath;
        private readonly Func<string> getText;
        private readonly Func<string> getJavaPath;

        private Color colorSuccess;
        private Color colorFailure;

        public PreviewWindow(string plantUmlBinary, Func<string> getFilePath, Func<string> getText, Func<string> getJavaPath)
        {
            this.plantUmlBinary = plantUmlBinary;
            this.getFilePath = getFilePath;
            this.getText = getText;
            this.getJavaPath = getJavaPath;

            InitializeComponent();

            imageBox_Diagram.ZoomChanged += ImageBox_ZoomChanged;
            ImageBox_ZoomChanged(this, null);
        }

        private void ImageBox_ZoomChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel_Zoom.Text = $"{imageBox_Diagram.Zoom}%";
        }

        private async void Button_Refresh_Click(object sender, EventArgs e)
        {
            try
            {
                toolStripProgressBar_Refreshing.Style = ProgressBarStyle.Marquee;
                toolStripProgressBar_Refreshing.MarqueeAnimationSpeed = 30;
                tableLayoutPanel_Window.Enabled = false;

                RendererFactory factory = new RendererFactory();
                IPlantUmlRenderer renderer = factory.CreateRenderer(new PlantUmlSettings()
                {
                    LocalPlantUmlPath = plantUmlBinary,
                    JavaPath = getJavaPath(),
                    RenderingMode = RenderingMode.Local
                });

                string text = getText();
                if (string.IsNullOrWhiteSpace(text))
                {
                    imageBox_Diagram.Image = Resources.Empty;
                }
                else
                {
                    byte[] bytes = await Task.Run(() => renderer.Render(text, OutputFormat.Png)).ConfigureAwait(true);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(bytes, 0, bytes.Length);
                        imageBox_Diagram.Image = Image.FromStream(ms);
                    }
                }

                toolStripStatusLabel_Time.Text = DateTime.Now.ToShortTimeString();
                toolStripStatusLabel_Time.BackColor = colorSuccess;
            }
            catch (TaskCanceledException)
            {
                toolStripStatusLabel_Time.BackColor = colorFailure;
                MessageBox.Show("Refresh cancelled", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (RenderingException rEx)
            {
                toolStripStatusLabel_Time.BackColor = colorFailure;
                MessageBox.Show(rEx.Message, "Failed to render", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel_Time.BackColor = colorFailure;
                MessageBox.Show(ex.ToString(), "Failed to refresh", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                InvokeIfRequired(() =>
                {
                    toolStripProgressBar_Refreshing.Style = ProgressBarStyle.Continuous;
                    toolStripProgressBar_Refreshing.MarqueeAnimationSpeed = 0;
                    tableLayoutPanel_Window.Enabled = true;
                });
            }
        }

        private void Button_Export_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Filter = "PNG file | *.png",
                    FileName = $"{Path.GetFileNameWithoutExtension(getFilePath())}.png",
                    InitialDirectory = Path.GetDirectoryName(getFilePath())
                })
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        imageBox_Diagram.Image.Save(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failed to export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InvokeIfRequired(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        public void SetStyle(Color editorBackgroundColor)
        {
            imageBox_Diagram.BackColor = editorBackgroundColor;

            Color buttonBackColor;
            Color buttonForeColor;
            float brighness = editorBackgroundColor.GetBrightness();
            if (editorBackgroundColor.GetBrightness() > 0.4)
            {
                //Light
                colorSuccess = Color.LightGreen;
                colorFailure = Color.Tomato;
                BackColor = SystemColors.Control;
                buttonBackColor = SystemColors.Control;
                buttonForeColor = SystemColors.ControlText;
                statusStrip_Bottom.BackColor = SystemColors.Control;
                statusStrip_Bottom.ForeColor = SystemColors.ControlText;
            }
            else
            {
                //Dark
                colorSuccess = Color.DarkGreen;
                colorFailure = Color.DarkRed;
                BackColor = SystemColors.ControlDarkDark;
                buttonBackColor = SystemColors.ControlDarkDark;
                buttonForeColor = SystemColors.ControlLightLight;
                statusStrip_Bottom.BackColor = SystemColors.ControlDarkDark;
                statusStrip_Bottom.ForeColor = SystemColors.ControlLightLight;
            }

            button_Export.BackColor = buttonBackColor;
            button_Export.ForeColor = buttonForeColor;

            button_Refresh.BackColor = buttonBackColor;
            button_Refresh.ForeColor = buttonForeColor;
        }
    }
}
