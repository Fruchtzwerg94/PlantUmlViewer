using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using PlantUml.Net;

namespace PlantUmlViewer.Forms
{
    internal partial class PreviewWindow : Form
    {
        private readonly string plantUmlBinary;
        private readonly Func<string> getFilePath;
        private readonly Func<string> getText;
        private readonly Func<string> getJavaPath;

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
                byte[] bytes = await Task.Run(() => renderer.Render(getText(), OutputFormat.Png)).ConfigureAwait(true);
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(bytes, 0, bytes.Length);
                    imageBox_Diagram.Image = Image.FromStream(ms);
                }

                toolStripStatusLabel_Time.Text = DateTime.Now.ToShortTimeString();
                statusStrip_Bottom.BackColor = Color.LightGreen;
            }
            catch (TaskCanceledException)
            {
                statusStrip_Bottom.BackColor = Color.Tomato;
                MessageBox.Show("Refresh cancelled", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (RenderingException rex)
            {
                statusStrip_Bottom.BackColor = Color.Tomato;
                MessageBox.Show(rex.Message, "Failed to render", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                statusStrip_Bottom.BackColor = Color.Tomato;
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

        private void InvokeIfRequired(Action a)
        {
            if (InvokeRequired)
            {
                Invoke(a);
            }
            else
            {
                a();
            }
        }
    }
}
