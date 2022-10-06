using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Kbg.NppPluginNET.PluginInfrastructure;

using PlantUml.Net;
using PlantUml.Net.Java;

using Svg;

using PlantUmlViewer.DiagramGeneration;
using PlantUmlViewer.Properties;
using PlantUmlViewer.Settings;
using PlantUmlViewer.Windows;

namespace PlantUmlViewer.Forms
{
    internal partial class PreviewWindow : Form
    {
        private readonly Func<string> getFilePath;
        private readonly Func<string> getText;
        private readonly SettingsService settings;

        private readonly DiagramGenerator diagramGenerator;

        private bool? isLight;
        private Color colorSuccess;
        private Color colorFailure;

        private CancellationTokenSource refreshCancellationTokenSource;

        #region Images
        private readonly object imagesLock = new object();
        private int selectedDiagramIndex;
        private int selectedPageIndex;
        private ReadOnlyCollection<GeneratedDiagram> images;

        private void UpdateImages(List<GeneratedDiagram> newImages)
        {
            lock (imagesLock)
            {
                images = new ReadOnlyCollection<GeneratedDiagram>(newImages);

                //Update the text of the selected diagram and visibility
                SetSelectedImage(selectedDiagramIndex, selectedPageIndex);
            }
        }

        private int GetSelectedDiagramIndex()
        {
            lock (imagesLock)
            {
                return selectedDiagramIndex;
            }
        }

        private int GetSelectedPageIndex()
        {
            lock (imagesLock)
            {
                return selectedPageIndex;
            }
        }

        private SvgDocument GetSelectedImage()
        {
            lock (imagesLock)
            {
                return images[selectedDiagramIndex].Pages[selectedPageIndex];
            }
        }

        private void SetSelectedImage(int diagramIndex, int pageIndex)
        {
            Debug.WriteLine($"Selecting image for diagram index {diagramIndex}, page index {pageIndex}", nameof(PreviewWindow));
            lock (imagesLock)
            {
                selectedDiagramIndex = Math.Min(diagramIndex, images.Count - 1);
                label_SelectedDiagram.Text = (selectedDiagramIndex + 1).ToString();
                tableLayoutPanel_NavigationDiagram.Visible = images.Count > 1;
                button_NextDiagram.Enabled = selectedDiagramIndex < images.Count - 1;
                button_PreviousDiagram.Enabled = selectedDiagramIndex > 0;

                selectedPageIndex = Math.Min(pageIndex, images[selectedDiagramIndex].Pages.Count - 1);
                label_SelectedPage.Text = (selectedPageIndex + 1).ToString();
                tableLayoutPanel_NavigationPage.Visible = images.Any(i => i.Pages.Count > 1);
                button_NextPage.Enabled = selectedPageIndex < images[selectedDiagramIndex].Pages.Count - 1;
                button_PreviousPage.Enabled = selectedPageIndex > 0;

                imageBox_Diagram.Image = GetCurrentImage(1);
            }
        }
        #endregion Images

        public event EventHandler<EventArgs> DockablePanelClose;

        public PreviewWindow(string plantUmlBinary, Func<string> getFilePath, Func<string> getText, SettingsService settings)
        {
            this.getFilePath = getFilePath;
            this.getText = getText;
            this.settings = settings;

            diagramGenerator = new DiagramGenerator(settings.Settings.JavaPath, plantUmlBinary);

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
            toolTip_Buttons.SetToolTip(button_PreviousPage, "Previous page");
            toolTip_Buttons.SetToolTip(button_NextPage, "Next page");

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
            //Set new background color
            imageBox_Diagram.BackColor = editorBackgroundColor;

            //Update light or dark colors if necessary
            bool newIsLight = editorBackgroundColor.GetBrightness() > 0.4;
            if (isLight == newIsLight)
            {
                return;
            }
            isLight = newIsLight;
            Debug.WriteLine("Setting style", nameof(PreviewWindow));

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
                label_SelectedPage.ForeColor = SystemColors.ControlText;
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
                label_SelectedPage.ForeColor = SystemColors.ControlLightLight;
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
            button_PreviousDiagram.BackgroundImage = RemapImage(Resources.NavigateLeft, buttonImageColorMap);
            button_NextDiagram.BackColor = buttonBackColor;
            button_NextDiagram.ForeColor = buttonForeColor;
            button_NextDiagram.BackgroundImage = RemapImage(Resources.NavigateRight, buttonImageColorMap);
            button_PreviousPage.BackColor = buttonBackColor;
            button_PreviousPage.ForeColor = buttonForeColor;
            button_PreviousPage.BackgroundImage = RemapImage(Resources.NavigateUp, buttonImageColorMap);
            button_NextPage.BackColor = buttonBackColor;
            button_NextPage.ForeColor = buttonForeColor;
            button_NextPage.BackgroundImage = RemapImage(Resources.NavigateDown, buttonImageColorMap);
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
            refreshCancellationTokenSource = new CancellationTokenSource();

            string text = null;
            try
            {
                loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Active = true;
                loadingCircleToolStripMenuItem_Refreshing.Visible = true;

                List<GeneratedDiagram> images;
                text = getText();
                if (string.IsNullOrWhiteSpace(text))
                {
                    //Empty input
                    using (MemoryStream memoryStream = new MemoryStream(Resources.Empty))
                    {
                        SvgDocument emptyImage = SvgDocument.Open<SvgDocument>(memoryStream);
                        void setTextColor(SvgElement element, SvgColourServer color)
                        {
                            if (element is SvgText textElement)
                            {
                                element.Fill = color;
                            }
                            else
                            {
                                foreach (SvgElement childElement in element.Children)
                                {
                                    setTextColor(childElement, color);
                                }
                            }
                        }
                        Random rnd = new Random();
                        setTextColor(emptyImage, new SvgColourServer(
                            Color.FromArgb(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255))));
                        images = new List<GeneratedDiagram>()
                        {
                            new GeneratedDiagram(emptyImage)
                        };
                    }
                }
                else
                {
                    images = await diagramGenerator.GenerateDocumentAsync(text, refreshCancellationTokenSource).ConfigureAwait(true);
                }

                UpdateImages(images);
                toolStripStatusLabel_Time.Text = $"{Path.GetFileName(getFilePath())} ({DateTime.Now.ToShortTimeString()})";
                toolStripStatusLabel_Time.BackColor = colorSuccess;
                button_Export.Enabled = true;
                button_ZoomIn.Enabled = true;
                button_ZoomOut.Enabled = true;
                button_ZoomFit.Enabled = true;
                button_ZoomReset.Enabled = true;
                ToolStripMenuItem_Diagram_ExportFile.Enabled = true;
                ToolStripMenuItem_Diagram_CopyToClipboard.Enabled = true;
            }
            catch (FileFormatException ffEx)
            {
                toolStripStatusLabel_Time.BackColor = colorFailure;
                MessageBox.Show(this, ffEx.Message, "Failed to load file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (JavaNotFoundException jnfEx)
            {
                toolStripStatusLabel_Time.BackColor = colorFailure;
                MessageBox.Show(this, $"{jnfEx.Message}{Environment.NewLine}Make sure Java can be found by setting the right path in the plugins options",
                    "Java not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (TaskCanceledException)
            {
                toolStripStatusLabel_Time.BackColor = colorFailure;
                MessageBox.Show(this, "Refresh cancelled", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (RenderingException rEx)
            {
                toolStripStatusLabel_Time.BackColor = colorFailure;
                MessageBox.Show(this, rEx.Message, "Failed to render", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                toolStripStatusLabel_Time.BackColor = colorFailure;
                MessageBox.Show(this, ex.ToString(), "Failed to refresh", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                loadingCircleToolStripMenuItem_Refreshing.Visible = false;
                loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Active = false;
                refreshCancellationTokenSource = null;
            }
        }

        private void Button_Export_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Filter = "PNG file|*.png|SVG file|*.svg",
                    FileName = $"{Path.GetFileNameWithoutExtension(getFilePath())}{(images.Count > 1 ? $"_d{GetSelectedDiagramIndex() + 1}" : "")}{(images[GetSelectedDiagramIndex()].Pages.Count > 1 ? $"_p{GetSelectedPageIndex() + 1}" : "")}.png",
                    InitialDirectory = Path.GetDirectoryName(getFilePath())
                })
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        switch (Path.GetExtension(saveFileDialog.FileName))
                        {
                            case ".png":
                                GetCurrentImage(settings.Settings.ExportSizeFactor).Save(saveFileDialog.FileName);
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
            SetSelectedImage(GetSelectedDiagramIndex() - 1, 0);
            if (!button_PreviousDiagram.Enabled)
            {
                button_NextDiagram.Focus();
            }
        }

        private void Button_NextDiagram_Click(object sender, EventArgs e)
        {
            SetSelectedImage(GetSelectedDiagramIndex() + 1, 0);
            if (!button_NextDiagram.Enabled)
            {
                button_PreviousDiagram.Focus();
            }
        }

        private void Button_PreviousPage_Click(object sender, EventArgs e)
        {
            SetSelectedImage(GetSelectedDiagramIndex(), GetSelectedPageIndex() - 1);
            if (!button_PreviousPage.Enabled)
            {
                button_NextPage.Focus();
            }
        }

        private void Button_NextPage_Click(object sender, EventArgs e)
        {
            SetSelectedImage(GetSelectedDiagramIndex(), GetSelectedPageIndex() + 1);
            if (!button_NextPage.Enabled)
            {
                button_PreviousPage.Focus();
            }
        }
        #endregion Button events

        private void ToolStripMenuItem_Diagram_CopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(GetCurrentImage(settings.Settings.ExportSizeFactor));
        }

        private void ImageBox_ZoomChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel_Zoom.Text = $"{imageBox_Diagram.Zoom}%";
        }

        private Image GetCurrentImage(decimal exportSizeFactor)
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
    }
}
