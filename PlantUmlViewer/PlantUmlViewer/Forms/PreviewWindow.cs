using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Kbg.NppPluginNET.PluginInfrastructure;

using PlantUml.Net;
using PlantUml.Net.Java;

using Svg;

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

        private SvgDocument svgImage;
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

            string text = "";
            try
            {
                loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Active = true;
                loadingCircleToolStripMenuItem_Refreshing.Visible = true;
                button_Refresh.Text = "Cancel";

                RendererFactory factory = new RendererFactory();
                IPlantUmlRenderer renderer = factory.CreateRenderer(new PlantUmlSettings()
                {
                    LocalPlantUmlPath = plantUmlBinary,
                    JavaPath = settings.Settings.JavaPath,
                    RenderingMode = RenderingMode.Local
                });

                text = getText();
                if (string.IsNullOrWhiteSpace(text))
                {
                    using (MemoryStream stream = new MemoryStream(Resources.Empty))
                    {
                        svgImage = SvgDocument.Open<SvgDocument>(stream);
                    }
                }
                else
                {
                    refreshCancellationTokenSource = new CancellationTokenSource();
                    byte[] bytes = await renderer.RenderAsync(text, OutputFormat.Svg,
                        refreshCancellationTokenSource.Token).ConfigureAwait(true);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        svgImage = SvgDocument.Open<SvgDocument>(ms);
                    }
                }

                this.InvokeIfRequired(() =>
                {
                    imageBox_Diagram.Image = GetDiagramImage(1);
                    toolStripStatusLabel_Time.Text = DateTime.Now.ToShortTimeString();
                    toolStripStatusLabel_Time.BackColor = colorSuccess;
                    button_Export.Enabled = true;
                    ToolStripMenuItem_Diagram_ExportFile.Enabled = true;
                    ToolStripMenuItem_Diagram_CopyToClipboard.Enabled = true;
                });
            }
            catch (JavaNotFoundException jnfEx)
            {
                this.InvokeIfRequired(() => toolStripStatusLabel_Time.BackColor = colorFailure);
                MessageBox.Show(this, $"{jnfEx.Message}{Environment.NewLine}Make sure Java can be found by setting the right path in the plugins options",
                    "Java not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (TaskCanceledException)
            {
                this.InvokeIfRequired(() => toolStripStatusLabel_Time.BackColor = colorFailure);
                MessageBox.Show(this, "Refresh cancelled", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (RenderingException rEx)
            {
                this.InvokeIfRequired(() => toolStripStatusLabel_Time.BackColor = colorFailure);

                //Try to get the line of a syntax error
                Match m = Regex.Match(rEx.Message, @"^ERROR\r\n(\d+)\r\nSyntax Error\?\r\nSome diagram description contains errors\r\n$");
                string errorMessage = rEx.Message;
                if (m.Success)
                {
                    int line = Convert.ToInt32(m.Groups[1].Value);
                    string syntaxErrorLineText = ReadLine(text, line);
                    if (!string.IsNullOrWhiteSpace(syntaxErrorLineText))
                    {
                        if (syntaxErrorLineText.Length > 150)
                        {
                            syntaxErrorLineText = syntaxErrorLineText.Substring(0, 150) + " ...";
                        }
                        errorMessage += $"{Environment.NewLine}{Environment.NewLine}This may is caused by line {line}:{Environment.NewLine}{syntaxErrorLineText}";
                    }
                }
                MessageBox.Show(this, errorMessage, "Failed to render", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                this.InvokeIfRequired(() => toolStripStatusLabel_Time.BackColor = colorFailure);
                MessageBox.Show(this, ex.ToString(), "Failed to refresh", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.InvokeIfRequired(() =>
                {
                    loadingCircleToolStripMenuItem_Refreshing.Visible = false;
                    loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Active = false;
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
                    Filter = "PNG file|*.png|SVG file|*.svg",
                    FileName = $"{Path.GetFileNameWithoutExtension(getFilePath())}.png",
                    InitialDirectory = Path.GetDirectoryName(getFilePath())
                })
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        switch (Path.GetExtension(saveFileDialog.FileName))
                        {
                            case ".png":
                                GetDiagramImage(settings.Settings.ExportSizeFactor).Save(saveFileDialog.FileName);
                                break;
                            case ".svg":
                                svgImage.Write(saveFileDialog.FileName);
                                break;
                            default:
                                throw new Exception("Invalid file extension");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Failed to export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToolStripMenuItem_Diagram_CopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(GetDiagramImage(settings.Settings.ExportSizeFactor));
        }

        private Image GetDiagramImage(decimal exportSizeFactor)
        {
            //Resize (See: https://github.com/svg-net/SVG/blob/master/Source/SvgDocument.Drawing.cs#L217)
            SizeF svgSize = svgImage.GetDimensions();
            SizeF imageSize = svgSize;
            svgImage.RasterizeDimensions(ref imageSize,
                (int)Math.Round((decimal)svgSize.Width * exportSizeFactor), (int)Math.Round((decimal)svgSize.Height * exportSizeFactor));
            Size bitmapSize = Size.Round(imageSize);
            Bitmap image = new Bitmap(bitmapSize.Width, bitmapSize.Height);

            //Set background if defined in SVG
            if (svgImage.TryGetAttribute("background", out string backgroundAttribute))
            {
                using (Graphics g = Graphics.FromImage(image))
                using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml(backgroundAttribute)))
                {
                    g.FillRectangle(brush, 0, 0, image.Width, image.Height);
                }
            }

            //Render
            using (var renderer = SvgRenderer.FromImage(image))
            {
                renderer.ScaleTransform(imageSize.Width / svgSize.Width, imageSize.Height / svgSize.Height);
                svgImage.Draw(renderer);
            }
            return image;
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
                loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Color = Color.DarkGray;
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
                loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Color = Color.LightGray;
                statusStrip_Bottom.BackColor = SystemColors.ControlDarkDark;
                statusStrip_Bottom.ForeColor = SystemColors.ControlLightLight;
            }

            button_Export.BackColor = buttonBackColor;
            button_Export.ForeColor = buttonForeColor;
            button_Refresh.BackColor = buttonBackColor;
            button_Refresh.ForeColor = buttonForeColor;
        }

        private static string ReadLine(string text, int lineNumber)
        {
            var reader = new StringReader(text);

            string line;
            int currentLineNumber = 0;
            do
            {
                currentLineNumber++;
                line = reader.ReadLine();
            }
            while (line != null && currentLineNumber < lineNumber);

            return (currentLineNumber == lineNumber) ? line : "";
        }
    }
}
