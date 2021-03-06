using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;

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

        private bool? isLight;
        private Color colorSuccess;
        private Color colorFailure;

        private CancellationTokenSource refreshCancellationTokenSource;

        #region Images
        private object imagesLock = new object();
        private int selectedImageIndex = 0;
        private ReadOnlyCollection<SvgDocument> svgImages;

        private void UpdateImages(IEnumerable<SvgDocument> newImages)
        {
            lock (imagesLock)
            {
                //Update the images
                List<SvgDocument> newImagesList = newImages.ToList();
                svgImages = new ReadOnlyCollection<SvgDocument>(newImagesList);
                //Update the text of the selected diagram and visibility
                SetSelectedImage(Math.Min(selectedImageIndex, svgImages.Count - 1));
            }
        }

        private int GetSelectedImageIndex()
        {
            lock (imagesLock)
            {
                return selectedImageIndex;
            }
        }

        private SvgDocument GetSelectedImage()
        {
            lock (imagesLock)
            {
                return svgImages[selectedImageIndex];
            }
        }

        private void SetSelectedImage(int index)
        {
            Debug.WriteLine($"Selecting image at index {index}", nameof(PreviewWindow));
            lock (imagesLock)
            {
                selectedImageIndex = index;
                label_SelectedDiagram.Visible = svgImages.Count > 1;
                label_SelectedDiagram.Text = (selectedImageIndex + 1).ToString();
                tableLayoutPanel_Navigation.Visible = svgImages.Count > 1;
                button_NextDiagram.Enabled = selectedImageIndex < svgImages.Count - 1;
                button_PreviousDiagram.Enabled = selectedImageIndex > 0;
                imageBox_Diagram.Image = GetDiagramImage(1);
            }
        }
        #endregion Images

        public event EventHandler<EventArgs> DockablePanelClose;

        public PreviewWindow(string plantUmlBinary, Func<string> getFilePath, Func<string> getText, SettingsService settings)
        {
            this.plantUmlBinary = plantUmlBinary;
            this.getFilePath = getFilePath;
            this.getText = getText;
            this.settings = settings;

            InitializeComponent();

            //Add some tool tips
            toolTip_Buttons.SetToolTip(button_Refresh, "Refresh");
            toolTip_Buttons.SetToolTip(button_Export, "Export");
            toolTip_Buttons.SetToolTip(button_ZoomIn, "Zoom in");
            toolTip_Buttons.SetToolTip(button_ZoomOut, "Zoom out");
            toolTip_Buttons.SetToolTip(button_ZoomFit, "Zoom to fit");
            toolTip_Buttons.SetToolTip(button_ZoomReset, "Reset zoom");
            toolTip_Buttons.SetToolTip(button_PreviousDiagram, "Previous diagram");
            toolTip_Buttons.SetToolTip(button_NextDiagram, "Next diagram");

            //Handle zoom changes
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

        public void SetStyle(Color editorBackgroundColor)
        {
            //Check the current style and update if necessary
            bool newIsLight = editorBackgroundColor.GetBrightness() > 0.4;
            if (isLight == newIsLight)
            {
                return;
            }
            isLight = newIsLight;
            Debug.WriteLine("Setting style", nameof(PreviewWindow));

            imageBox_Diagram.BackColor = editorBackgroundColor;

            Color buttonBackColor;
            Color buttonForeColor;
            if (isLight == true)
            {
                //Light
                colorSuccess = Color.LightGreen;
                colorFailure = Color.Tomato;
                BackColor = SystemColors.Control;
                buttonBackColor = SystemColors.Control;
                buttonForeColor = SystemColors.ControlText;
                label_SelectedDiagram.ForeColor = SystemColors.ControlText;
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
                label_SelectedDiagram.ForeColor = SystemColors.ControlLightLight;
                loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Color = Color.LightGray;
                statusStrip_Bottom.BackColor = SystemColors.ControlDarkDark;
                statusStrip_Bottom.ForeColor = SystemColors.ControlLightLight;
            }
            ColorMap[] buttonImageColorMap = new ColorMap[] {
                new ColorMap()
                {
                    OldColor = Color.Black,
                    NewColor = buttonForeColor
                }
            };

            button_Refresh.BackColor = buttonBackColor;
            button_Refresh.ForeColor = buttonForeColor;
            button_Refresh.BackgroundImage = RemapImage(Resources.Refresh, buttonImageColorMap);
            button_Export.BackColor = buttonBackColor;
            button_Export.ForeColor = buttonForeColor;
            button_Export.BackgroundImage = RemapImage(Resources.Save, buttonImageColorMap);
            button_ZoomIn.BackColor = buttonBackColor;
            button_ZoomIn.ForeColor = buttonForeColor;
            button_ZoomIn.BackgroundImage = RemapImage(Resources.ZoomIn, buttonImageColorMap);
            button_ZoomOut.BackColor = buttonBackColor;
            button_ZoomOut.ForeColor = buttonForeColor;
            button_ZoomOut.BackgroundImage = RemapImage(Resources.ZoomOut, buttonImageColorMap);
            button_ZoomFit.BackColor = buttonBackColor;
            button_ZoomFit.ForeColor = buttonForeColor;
            button_ZoomFit.BackgroundImage = RemapImage(Resources.ZoomFit, buttonImageColorMap);
            button_ZoomReset.BackColor = buttonBackColor;
            button_ZoomReset.ForeColor = buttonForeColor;
            button_ZoomReset.BackgroundImage = RemapImage(Resources.ZoomReset, buttonImageColorMap);
            button_PreviousDiagram.BackColor = buttonBackColor;
            button_PreviousDiagram.ForeColor = buttonForeColor;
            button_PreviousDiagram.BackgroundImage = RemapImage(Resources.Previous, buttonImageColorMap);
            button_NextDiagram.BackColor = buttonBackColor;
            button_NextDiagram.ForeColor = buttonForeColor;
            button_NextDiagram.BackgroundImage = RemapImage(Resources.Next, buttonImageColorMap);
        }

        #region Button events
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

                RendererFactory factory = new RendererFactory();
                IPlantUmlRenderer renderer = factory.CreateRenderer(new PlantUmlSettings()
                {
                    LocalPlantUmlPath = plantUmlBinary,
                    JavaPath = settings.Settings.JavaPath,
                    RenderingMode = RenderingMode.Local
                });

                //The response could contain multiple XML documents / SVG images
                List<SvgDocument> images = new List<SvgDocument>();

                text = getText();
                if (string.IsNullOrWhiteSpace(text))
                {
                    using (MemoryStream memoryStream = new MemoryStream(Resources.Empty))
                    {
                        images.Add(SvgDocument.Open<SvgDocument>(memoryStream));
                    }
                }
                else
                {
                    refreshCancellationTokenSource = new CancellationTokenSource();
                    byte[] bytes = await renderer.RenderAsync(text, OutputFormat.Svg,
                        refreshCancellationTokenSource.Token).ConfigureAwait(true);

                    //Find all start XML declarations to parse multiple images
                    List<int> xmlStartDeclararationIndices = PatternAt(bytes, Encoding.UTF8.GetBytes("<?xml ")).ToList();
                    for (int i = 0; i < xmlStartDeclararationIndices.Count; i++)
                    {
                        int start = xmlStartDeclararationIndices[i];
                        int end = i == xmlStartDeclararationIndices.Count - 1 ? bytes.Length : xmlStartDeclararationIndices[i + 1];
                        using (MemoryStream memoryStream = new MemoryStream(bytes, start, end - start))
                        {
                            images.Add(SvgDocument.Open<SvgDocument>(memoryStream));
                        }
                    }
                }
                Debug.WriteLine($"{images.Count} image(s) generated", nameof(PreviewWindow));

                this.InvokeIfRequired(() =>
                {
                    UpdateImages(images);
                    toolStripStatusLabel_Time.Text = DateTime.Now.ToShortTimeString();
                    toolStripStatusLabel_Time.BackColor = colorSuccess;
                    button_Export.Enabled = true;
                    button_ZoomIn.Enabled = true;
                    button_ZoomOut.Enabled = true;
                    button_ZoomFit.Enabled = true;
                    button_ZoomReset.Enabled = true;
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
                    FileName = $"{Path.GetFileNameWithoutExtension(getFilePath())}{(svgImages.Count > 1 ? $"_{GetSelectedImageIndex() + 1}" : "")}.png",
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
                                GetSelectedImage().Write(saveFileDialog.FileName);
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

        private void Button_ZoomIn_Click(object sender, EventArgs e)
        {
            imageBox_Diagram.ZoomIn();
        }

        private void Button_ZoomOut_Click(object sender, EventArgs e)
        {
            imageBox_Diagram.ZoomOut();
        }

        private void Button_ZoomFit_Click(object sender, EventArgs e)
        {
            imageBox_Diagram.ZoomToFit();
        }

        private void Button_ZoomReset_Click(object sender, EventArgs e)
        {
            imageBox_Diagram.Zoom = 100;
        }

        private void Button_PreviousDiagram_Click(object sender, EventArgs e)
        {
            SetSelectedImage(GetSelectedImageIndex() - 1);
        }

        private void Button_NextDiagram_Click(object sender, EventArgs e)
        {
            SetSelectedImage(GetSelectedImageIndex() + 1);
        }
        #endregion Button events

        private void ToolStripMenuItem_Diagram_CopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(GetDiagramImage(settings.Settings.ExportSizeFactor));
        }

        private void ImageBox_ZoomChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel_Zoom.Text = $"{imageBox_Diagram.Zoom}%";
        }

        private Image GetDiagramImage(decimal exportSizeFactor)
        {
            SvgDocument selectedImage = GetSelectedImage();

            //Resize (See: https://github.com/svg-net/SVG/blob/master/Source/SvgDocument.Drawing.cs#L217)
            SizeF svgSize = selectedImage.GetDimensions();
            SizeF imageSize = svgSize;
            selectedImage.RasterizeDimensions(ref imageSize,
                (int)Math.Round((decimal)svgSize.Width * exportSizeFactor), (int)Math.Round((decimal)svgSize.Height * exportSizeFactor));
            Size bitmapSize = Size.Round(imageSize);
            Bitmap image = new Bitmap(bitmapSize.Width, bitmapSize.Height);

            //Set background if defined in SVG
            if (selectedImage.TryGetAttribute("background", out string backgroundAttribute))
            {
                using (Graphics g = Graphics.FromImage(image))
                using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml(backgroundAttribute)))
                {
                    g.FillRectangle(brush, 0, 0, image.Width, image.Height);
                }
            }

            //Render
            using (ISvgRenderer renderer = SvgRenderer.FromImage(image))
            {
                renderer.ScaleTransform(imageSize.Width / svgSize.Width, imageSize.Height / svgSize.Height);
                selectedImage.Draw(renderer);
            }
            return image;
        }

        private static Bitmap RemapImage(Bitmap image, ColorMap[] colorMap)
        {
            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                imageAttributes.SetRemapTable(colorMap);
                Bitmap newImage = new Bitmap(image.Width, image.Height);
                using (Graphics g = Graphics.FromImage(newImage))
                {
                    g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0,
                        image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
                }
                return newImage;
            }
        }

        private static string ReadLine(string text, int lineNumber)
        {
            using (StringReader reader = new StringReader(text))
            {
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

        private static IEnumerable<int> PatternAt(byte[] source, byte[] pattern)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    yield return i;
                }
            }
        }
    }
}
