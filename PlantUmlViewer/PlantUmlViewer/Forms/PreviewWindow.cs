using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
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
        private const string DIAGRAM_DELIMITOR = "|##|##|<PS>|##|##|";

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
        private int selectedDiagramIndex;
        private int selectedPageIndex;
        private ReadOnlyCollection<RenderedDiagram> svgImages;

        private void UpdateImages(List<List<SvgDocument>> newPages)
        {
            lock (imagesLock)
            {
                //Update the images
                int numberOfDiagrams = newPages.Max(p => p.Count);
                List<RenderedDiagram> newImagesList = Enumerable.Repeat<object>(null, numberOfDiagrams)
                    .Select(_ => new RenderedDiagram()).ToList();
                for (int diagramIndex = 0; diagramIndex < numberOfDiagrams; diagramIndex++)
                {
                    foreach (List<SvgDocument> newPage in newPages)
                    {
                        if (newPage[diagramIndex] != null)
                        {
                            newImagesList[diagramIndex].Pages.Add(newPage[diagramIndex]);
                        }
                    }
                }
                svgImages = new ReadOnlyCollection<RenderedDiagram>(newImagesList);

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
                return svgImages[selectedDiagramIndex].Pages[selectedPageIndex];
            }
        }

        private void SetSelectedImage(int diagramIndex, int pageIndex)
        {
            Debug.WriteLine($"Selecting image for diagram index {diagramIndex}, page index {pageIndex}", nameof(PreviewWindow));
            lock (imagesLock)
            {
                selectedDiagramIndex = Math.Min(diagramIndex, svgImages.Count - 1);
                label_SelectedDiagram.Text = (selectedDiagramIndex + 1).ToString();
                tableLayoutPanel_NavigationDiagram.Visible = svgImages.Count > 1;
                button_NextDiagram.Enabled = selectedDiagramIndex < svgImages.Count - 1;
                button_PreviousDiagram.Enabled = selectedDiagramIndex > 0;

                selectedPageIndex = Math.Min(pageIndex, svgImages[selectedDiagramIndex].Pages.Count - 1);
                label_SelectedPage.Text = (selectedPageIndex + 1).ToString();
                tableLayoutPanel_NavigationPage.Visible = svgImages.Any(i => i.Pages.Count > 1);
                button_NextPage.Enabled = selectedPageIndex < svgImages[selectedDiagramIndex].Pages.Count - 1;
                button_PreviousPage.Enabled = selectedPageIndex > 0;

                imageBox_Diagram.Image = GetCurrentImage(1);
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

            string text = null;
            try
            {
                loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Active = true;
                loadingCircleToolStripMenuItem_Refreshing.Visible = true;

                /*
                 * The responses could contain multiple pages, where each page could contain the images for multiple diagrams
                 * The list is build up like
                 *  - Page 1
                 *      - Diagram 1 image or null
                 *      - ...
                 *      - Diagram n image or null
                 *  - ...
                 *      - ...
                 *  - Diaggram n
                 *      - ...
                 */
                List<List<SvgDocument>> pages = new List<List<SvgDocument>>();

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
                        pages.Add(new List<SvgDocument>() { emptyImage });
                    }
                }
                else
                {
                    RendererFactory renderFactory = new RendererFactory();
                    refreshCancellationTokenSource = new CancellationTokenSource();

                    //Run through all pages
                    int pageIndex = 0;
                    while(true)
                    {
                        IPlantUmlRenderer renderer = renderFactory.CreateRenderer(new PlantUmlSettings()
                        {
                            ErrorReportMode = ErrorReportMode.Verbose,
                            LocalPlantUmlPath = plantUmlBinary,
                            JavaPath = settings.Settings.JavaPath,
                            RenderingMode = RenderingMode.Local,
                            Delimitor = DIAGRAM_DELIMITOR,
                            ImageIndex = pageIndex
                        });

                        byte[] bytes = await renderer.RenderAsync(text, OutputFormat.Svg,
                            refreshCancellationTokenSource.Token).ConfigureAwait(true);

                        //Find all delimitors to parse multiple diagram images
                        List<SvgDocument> imagesOfPage = new List<SvgDocument>();
                        List<int> delimitorIndices = new int[] { -(DIAGRAM_DELIMITOR.Length + 2) }
                            .Concat(PatternAt(bytes, Encoding.UTF8.GetBytes(DIAGRAM_DELIMITOR))).ToList();
                        for (int i = 0; i < delimitorIndices.Count - 1; i++)
                        {
                            int start = delimitorIndices[i] + DIAGRAM_DELIMITOR.Length + 2;
                            int end = delimitorIndices[i + 1];
                            using (MemoryStream memoryStream = new MemoryStream(bytes, start, end - start))
                            {
                                if (end - start > 0)
                                {
                                    Debug.WriteLine($"Generating image {i + 1} for page {pageIndex + 1}", nameof(PreviewWindow));
                                    imagesOfPage.Add(SvgDocument.Open<SvgDocument>(memoryStream));
                                }
                                else
                                {
                                    Debug.WriteLine($"No image {i + 1} for page {pageIndex + 1}", nameof(PreviewWindow));
                                    imagesOfPage.Add(null);
                                }
                            }
                        }
                        //No more pages available
                        if (imagesOfPage.All(i => i == null))
                        {
                            break;
                        }
                        pages.Add(imagesOfPage);
                        pageIndex++;
                    }
                }
                Debug.WriteLine($"{pages.SelectMany(p => p.Where(i => i != null)).Count()} images(s) at {pages.Count} page(s) generated",
                    nameof(PreviewWindow));

                this.InvokeIfRequired(() =>
                {
                    UpdateImages(pages);
                    toolStripStatusLabel_Time.Text = $"{Path.GetFileName(getFilePath())} ({DateTime.Now.ToShortTimeString()})";
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
            catch (FileFormatException ffEx)
            {
                this.InvokeIfRequired(() => toolStripStatusLabel_Time.BackColor = colorFailure);
                MessageBox.Show(this, ffEx.Message, "Failed to load file", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(this, rEx.Message, "Failed to render", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    FileName = $"{Path.GetFileNameWithoutExtension(getFilePath())}{(svgImages.Count > 1 ? $"_d{GetSelectedDiagramIndex() + 1}" : "")}{(svgImages[GetSelectedDiagramIndex()].Pages.Count > 1 ? $"_p{GetSelectedPageIndex() + 1}" : "")}.png",
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
