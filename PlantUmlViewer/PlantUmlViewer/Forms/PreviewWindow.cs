using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
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

        private readonly Dictionary<Button, Bitmap> buttonImages = new Dictionary<Button, Bitmap>();

        private readonly DiagramGenerator diagramGenerator;

        private bool? isLight;
        private Color colorSuccess;
        private Color colorFailure;

        private CancellationTokenSource refreshCancellationTokenSource;

        #region Images
        private readonly object imagesLock = new object();
        private int selectedDiagramIndex;
        private int selectedPageIndex;
        private string generatedFile;
        private string generatedText;
        private DateTime generatedDateTime;
        private ReadOnlyCollection<GeneratedDiagram> images;

        private void UpdateImages(string file, string text, DateTime dateTime,
            List<GeneratedDiagram> newImages)
        {
            lock (imagesLock)
            {
                generatedFile = file;
                generatedText = text;
                generatedDateTime = dateTime;
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

        private Image GetSelectedImage(decimal sizeFactor)
        {
            return DiagramGenerator.SvgImageToBitmap(GetSelectedImage(), sizeFactor);
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

                imageBox_Diagram.Image = GetSelectedImage(1);
            }
        }
        #endregion Images

        public event EventHandler DockablePanelClose;

        public PreviewWindow(string plantUmlBinary, Func<string> getFilePath, Func<string> getText, SettingsService settings)
        {
            this.getFilePath = getFilePath;
            this.getText = getText;
            this.settings = settings;

            diagramGenerator = new DiagramGenerator(settings.Settings.JavaPath, plantUmlBinary);

            InitializeComponent();

            //Define the button images
            buttonImages[button_Refresh] = Resources.Refresh;
            buttonImages[button_Export] = Resources.Save;
            buttonImages[button_ZoomIn] = Resources.ZoomIn;
            buttonImages[button_ZoomOut] = Resources.ZoomOut;
            buttonImages[button_ZoomFit] = Resources.ZoomFit;
            buttonImages[button_ZoomReset] = Resources.ZoomReset;
            buttonImages[button_PreviousDiagram] = Resources.NavigateLeft;
            buttonImages[button_NextDiagram] = Resources.NavigateRight;
            buttonImages[button_PreviousPage] = Resources.NavigateUp;
            buttonImages[button_NextPage] = Resources.NavigateDown;

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

        protected override void OnHandleCreated(EventArgs e)
        {
            /*
             * Notepad++ may freezes if the plugins control is docked out and loosing focus
             *  - https://github.com/notepad-plus-plus/notepad-plus-plus/issues/8723
             *  - https://forums.codeguru.com/showthread.php?412421-endless-messages-WM_GETDLGCODE-when-focus-is-lost
             *  - http://robs-got-a-blog.blogspot.com/2011/09/net-custom-control-locks-up-when-losing.html
             *  - https://social.msdn.microsoft.com/Forums/en-US/dca98588-039a-44f8-b15c-86fd9ac415c6/how-can-i-resolve-datagridview-and-cdialog-bug?forum=winformsdatacontrols
             *  Fix: Set WS_EX_CONTROLPARENT to elemens which may are in focus
             */
            foreach (Button button in this.GetChildren<Button>(10))
            {
                NativeWindowApi.ModifyStyleEx(button.Handle, 0, NativeWindowApi.WS_EX_CONTROLPARENT);
            }
            base.OnHandleCreated(e);
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

        public void DocumentChanged()
        {
            button_Refresh.Enabled = true;
        }

        #region Styling
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

            if (isLight == true)
            {
                //Light
                colorSuccess = Color.LightGreen;
                colorFailure = Color.Tomato;
                BackColor = SystemColors.Control;
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
                label_SelectedDiagram.ForeColor = SystemColors.ControlLightLight;
                label_SelectedPage.ForeColor = SystemColors.ControlLightLight;
                loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Color = Color.LightGray;
                statusStrip_Bottom.BackColor = SystemColors.ControlDarkDark;
                statusStrip_Bottom.ForeColor = SystemColors.ControlLightLight;
            }

            foreach (KeyValuePair<Button, Bitmap> imageButton in buttonImages)
            {
                UpdateButtonStyle(imageButton.Key);
            }
        }

        private void Button_EnabledChanged(object sender, EventArgs e)
        {
            UpdateButtonStyle((Button)sender);
        }

        private void UpdateButtonStyle(Button button)
        {
            Color buttonBackColor = isLight == true ? SystemColors.Control : SystemColors.ControlDarkDark;
            Color buttonForeColor = isLight == true ? SystemColors.ControlText : SystemColors.ControlLightLight;
            if (!button.Enabled)
            {
                buttonForeColor = Color.FromArgb(50, buttonForeColor);
            }
            ColorMap[] buttonImageColorMap = new ColorMap[] {
                new ColorMap()
                {
                    OldColor = Color.Black,
                    NewColor = buttonForeColor
                }
            };
            button.BackColor = buttonBackColor;
            button.ForeColor = buttonForeColor;
            button.BackgroundImage = RemapImage(buttonImages[button], buttonImageColorMap);
        }
        #endregion Styling

        public async void Button_Refresh_Click(object sender, EventArgs e)
        #region ButtonClicks
        {
            //Cancel if already running
            if (refreshCancellationTokenSource != null)
            {
                refreshCancellationTokenSource.Cancel();
                return;
            }
            refreshCancellationTokenSource = new CancellationTokenSource();

            try
            {
                button_Refresh.Enabled = false;
                loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Active = true;
                loadingCircleToolStripMenuItem_Refreshing.Visible = true;

                List<GeneratedDiagram> images;
                string file = getFilePath();
                string text = getText();
                DateTime stamp = DateTime.Now;
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
                    diagramGenerator.JavaPath = settings.Settings.JavaPath;
                    images = await diagramGenerator.GenerateDocumentAsync(text, settings.Settings.Include,
                        refreshCancellationTokenSource).ConfigureAwait(true);
                }

                UpdateImages(file, text, stamp, images);
                this.InvokeIfRequired(() =>
                {
                    toolStripStatusLabel_Status.Text = $"{Path.GetFileName(file)} ({stamp.ToShortTimeString()})";
                    toolStripStatusLabel_Status.BackColor = colorSuccess;
                    toolStripStatusLabel_Status.ToolTipText = "";
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
                GenerationFailed("Failed to load file", ffEx.Message);
            }
            catch (JavaNotFoundException jnfEx)
            {
                GenerationFailed("Java not found",
                    $"{jnfEx.Message}{Environment.NewLine}Make sure Java can be found by setting the right path in the plugins options");
            }
            catch (TaskCanceledException)
            {
                GenerationFailed("Cancelled", "Refresh cancelled");
            }
            catch (RenderingException rEx)
            {
                GenerationFailed("Failed to render", rEx.Message);
            }
            catch (Exception ex)
            {
                GenerationFailed("Failed to refresh", ex.ToString());
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

        private void GenerationFailed(string title, string message)
        {
            this.InvokeIfRequired(() =>
            {
                toolStripStatusLabel_Status.BackColor = colorFailure;
                MessageBox.Show(this, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel_Status.ToolTipText = $"{title}:{Environment.NewLine}{message}";
            });
        }

        private void Button_Export_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Filter = "PNG file|*.png|SVG file|*.svg",
                    FileName = $"{Path.GetFileNameWithoutExtension(generatedFile)}{(images.Count > 1 ? $"_d{GetSelectedDiagramIndex() + 1}" : "")}{(images[GetSelectedDiagramIndex()].Pages.Count > 1 ? $"_p{GetSelectedPageIndex() + 1}" : "")}.png",
                    InitialDirectory = Path.GetDirectoryName(getFilePath())
                })
                {
                    if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        switch (Path.GetExtension(saveFileDialog.FileName))
                        {
                            case ".png":
                                GetSelectedImage(settings.Settings.ExportSizeFactor).Save(saveFileDialog.FileName);
                                break;
                            case ".svg":
                                //Clone, add metadata and save
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    GetSelectedImage().Write(ms);
                                    ms.Position = 0;
                                    SvgDocument svgToExport = SvgDocument.Open<SvgDocument>(ms);
                                    AddMetadata(svgToExport);
                                    svgToExport.Write(saveFileDialog.FileName);
                                }
                                break;
                            default:
                                throw new Exception("Invalid file extension");
                        }

                        if (settings.Settings.OpenExport == OpenExport.Always
                            || (settings.Settings.OpenExport == OpenExport.Ask
                                && MessageBox.Show(this, "Open the exported file?", "Open export", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo()
                            {
                                FileName = saveFileDialog.FileName,
                                UseShellExecute = true
                            };
                            Process.Start(startInfo);
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
        #endregion ButtonClicks

        private void ToolStripMenuItem_Diagram_CopyToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(GetSelectedImage(settings.Settings.ExportSizeFactor));
        }

        private void ImageBox_ZoomChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel_Zoom.Text = $"{imageBox_Diagram.Zoom}%";
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

        private const string NAMESPACE_RDF = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        private const string NAMESPACE_DC = "http://purl.org/dc/elements/1.1/";
        private const string NAMESPACE_PUV = "https://github.com/Fruchtzwerg94/PlantUmlViewer";
        private void AddMetadata(SvgDocument svg)
        {
            /*
             * Add some metadata to the SVG like described in
             *   - https://www.w3.org/TR/SVG11/metadata.html
             *   - https://developer.mozilla.org/en-US/docs/Web/SVG/Element/metadata
             *   - https://www.w3.org/TR/rdfa-syntax/
             *   - https://www.dublincore.org/specifications/dublin-core/dces/
             */

            //Add title
            svg.Children.Add(new SvgTitle()
            {
                Content = Path.GetFileNameWithoutExtension(generatedFile)
            });

            //Add metadata
            SvgUnknownElement metadata = new SvgUnknownElement("metadata");
            NonSvgElement rdfMetadata = new NonSvgElement("RDF", "rdf");
            rdfMetadata.Namespaces["rdf"] = NAMESPACE_RDF;
            rdfMetadata.Namespaces["dc"] = NAMESPACE_DC;
            rdfMetadata.Namespaces["puv"] = NAMESPACE_PUV;

            rdfMetadata.Children.Add(new NonSvgElement("product", NAMESPACE_PUV) { Content = AssemblyAttributes.Product });
            rdfMetadata.Children.Add(new NonSvgElement("version", NAMESPACE_PUV) { Content = AssemblyAttributes.Version });
            rdfMetadata.Children.Add(new NonSvgElement("plantuml", NAMESPACE_PUV) { Content = PlantUmlViewer.PLANT_UML_VERSION });

            NonSvgElement rdfMetadataDescription = new NonSvgElement("Description", NAMESPACE_RDF);
            rdfMetadataDescription.Children.Add(new NonSvgElement("creator", NAMESPACE_DC) { Content = WindowsIdentity.GetCurrent().Name });
            rdfMetadataDescription.Children.Add(new NonSvgElement("date", NAMESPACE_DC) { Content = generatedDateTime.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture) });
            rdfMetadataDescription.Children.Add(new NonSvgElement("source", NAMESPACE_DC) { Content = Path.GetFileName(generatedFile) });
            rdfMetadataDescription.Children.Add(new NonSvgElement("title", NAMESPACE_DC) { Content = Path.GetFileNameWithoutExtension(generatedFile) });
            rdfMetadataDescription.Children.Add(new NonSvgElement("format", NAMESPACE_DC) { Content = "image/svg" });
            rdfMetadataDescription.Children.Add(new NonSvgElement("page", NAMESPACE_PUV) { Content = (GetSelectedPageIndex() + 1).ToString() });
            rdfMetadataDescription.Children.Add(new NonSvgElement("diagram", NAMESPACE_PUV) { Content = (GetSelectedDiagramIndex() + 1).ToString() });
            rdfMetadata.Children.Add(rdfMetadataDescription);

            metadata.Children.Add(rdfMetadata);
            svg.Children.Add(metadata);

            //Add description
            if (settings.Settings.ExportDocument)
            {
                svg.Children.Add(new SvgDescription()
                {
                    Content = generatedText
                });
            }
        }
    }
}
