using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Kbg.NppPluginNET.PluginInfrastructure;

using PlantUml.Net;
using PlantUml.Net.Java;

using PlantUmlViewer.Windows;
using PlantUmlViewer.Properties;
using PlantUmlViewer.Settings;

namespace PlantUmlViewer.Forms
{
    internal partial class PreviewWindow : Form
    {
        private readonly string plantUmlBinary;
        private readonly Func<string> getFilePath;
        private readonly Func<string> getText;
        private readonly SettingsService settings;

        private Color colorSuccess;
        private Color colorFailure;

        private CancellationTokenSource refreshCancellationTokenSource;

        public event EventHandler<EventArgs> DockablePanelClose;

        public PreviewWindow(string plantUmlBinary, Func<string> getFilePath, Func<string> getText, SettingsService settings)
        {
            this.plantUmlBinary = plantUmlBinary;
            this.getFilePath = getFilePath;
            this.getText = getText;
            this.settings = settings;

            InitializeComponent();

            imageBox_Diagram.ZoomChanged += ImageBox_ZoomChanged;
            ImageBox_ZoomChanged(this, null);
        }

        protected override void WndProc(ref Message m)
        {
            //Notify the dockable panel was closed
            if (m.Msg == (int)WindowsMessage.WM_NOTIFY)
            {
                NMHDR notification = (NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR));
                if (notification.code == (int)DockMgrMsg.DMN_CLOSE)
                {
                    Debug.WriteLine("Closed dockable panel", nameof(PreviewWindow));
                    DockablePanelClose?.Invoke(this, EventArgs.Empty);
                }
            }
            base.WndProc(ref m);
        }

        private void ImageBox_ZoomChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel_Zoom.Text = $"{imageBox_Diagram.Zoom}%";
        }

        public async void Button_Refresh_Click(object sender, EventArgs e)
        {
            //Cancel if already running
            if (refreshCancellationTokenSource != null)
            {
                refreshCancellationTokenSource.Cancel();
                return;
            }

            try
            {
                toolStripProgressBar_Refreshing.Style = ProgressBarStyle.Marquee;
                toolStripProgressBar_Refreshing.MarqueeAnimationSpeed = 30;
                button_Refresh.Text = "Cancel";

                RendererFactory factory = new RendererFactory();
                IPlantUmlRenderer renderer = factory.CreateRenderer(new PlantUmlSettings()
                {
                    LocalPlantUmlPath = plantUmlBinary,
                    JavaPath = settings.Settings.JavaPath,
                    RenderingMode = RenderingMode.Local
                });

                string text = getText();
                if (string.IsNullOrWhiteSpace(text))
                {
                    imageBox_Diagram.Image = Resources.Empty;
                }
                else
                {
                    refreshCancellationTokenSource = new CancellationTokenSource();
                    byte[] bytes = await renderer.RenderAsync(text, OutputFormat.Png,
                        refreshCancellationTokenSource.Token).ConfigureAwait(true);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(bytes, 0, bytes.Length);
                        imageBox_Diagram.Image = Image.FromStream(ms);
                    }
                }

                InvokeIfRequired(() =>
                {
                    toolStripStatusLabel_Time.Text = DateTime.Now.ToShortTimeString();
                    toolStripStatusLabel_Time.BackColor = colorSuccess;
                    button_Export.Enabled = true;
                });
            }
            catch (JavaNotFoundException jnfEx)
            {
                InvokeIfRequired(() => toolStripStatusLabel_Time.BackColor = colorFailure);
                MessageBox.Show($"{jnfEx.Message}{Environment.NewLine}Make sure Java can be found by setting the right path in the plugins options",
                    "Java not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (TaskCanceledException)
            {
                InvokeIfRequired(() => toolStripStatusLabel_Time.BackColor = colorFailure);
                MessageBox.Show("Refresh cancelled", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (RenderingException rEx)
            {
                InvokeIfRequired(() => toolStripStatusLabel_Time.BackColor = colorFailure);
                MessageBox.Show(rEx.Message, "Failed to render", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                InvokeIfRequired(() => toolStripStatusLabel_Time.BackColor = colorFailure);
                MessageBox.Show(ex.ToString(), "Failed to refresh", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                InvokeIfRequired(() =>
                {
                    toolStripProgressBar_Refreshing.Style = ProgressBarStyle.Continuous;
                    toolStripProgressBar_Refreshing.MarqueeAnimationSpeed = 0;
                    button_Refresh.Text = "Refresh";
                    refreshCancellationTokenSource = null;
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
