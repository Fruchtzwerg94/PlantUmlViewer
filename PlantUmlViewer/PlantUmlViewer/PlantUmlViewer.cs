using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using Kbg.NppPluginNET.PluginInfrastructure;

using PlantUmlViewer.Forms;

namespace PlantUmlViewer
{
    internal class PlantUmlViewer
    {
        public const string PLUGIN_NAME = "PlantUML Viewer";

        private const string PLANT_UML_BINARY = "plantuml-1.2022.5.jar";

        private enum CommandId
        {
            ShowPreview = 0,
            ShowSettings = 1,
            ShowAbout = 2
        }

        private readonly string assemblyDirectory;

        private INotepadPPGateway notepadPp;
        private IScintillaGateway editor;
        private Settings settings;

        private readonly Icon icon;
        private PreviewWindow previewWindow;

        public PlantUmlViewer()
        {
            assemblyDirectory = Uri.UnescapeDataString(Path.GetDirectoryName(
                new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath));

            using (Bitmap bmp = new Bitmap(16, 16))
            {
                Graphics g = Graphics.FromImage(bmp);
                ColorMap[] colorMap = new ColorMap[]
                {
                    new ColorMap
                    {
                        OldColor = Color.Transparent,
                        NewColor = Color.FromKnownColor(KnownColor.ButtonFace)
                    }
                };
                ImageAttributes attr = new ImageAttributes();
                attr.SetRemapTable(colorMap);
                g.DrawImage(Properties.Resources.Icon, new Rectangle(0, 0, 16, 16),
                    0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                icon = Icon.FromHandle(bmp.GetHicon());
            }
        }

        public void OnNotification(ScNotification notification)
        {

        }

        public void CommandMenuInit()
        {
            notepadPp = new NotepadPPGateway();
            editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            settings = new Settings();

            PluginBase.SetCommand((int)CommandId.ShowPreview, "Show preview", ShowPreview);
            PluginBase.SetCommand((int)CommandId.ShowSettings, "Settings", ShowSettings);
            PluginBase.SetCommand((int)CommandId.ShowAbout, "About", ShowAbout);
        }

        public void SetToolBarIcon()
        {
            toolbarIcons tbIcons = new toolbarIcons
            {
                hToolbarBmp = Properties.Resources.Icon.GetHbitmap()
            };
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON,
                PluginBase._funcItems.Items[(int)CommandId.ShowPreview]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        public void PluginCleanUp()
        {

        }

        private void ShowPreview()
        {
            if (previewWindow == null)
            {
                previewWindow = new PreviewWindow(
                    Path.Combine(assemblyDirectory, PLANT_UML_BINARY),
                    notepadPp.GetCurrentFilePath,
                    () => editor.GetText(editor.GetLength() + 1),
                    () => settings.GetSetting("JavaPath", ""));

                NppTbData nppTbData = new NppTbData
                {
                    hClient = previewWindow.Handle,
                    pszName = "PlantUML",
                    dlgID = (int)CommandId.ShowPreview,
                    uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR,
                    hIconTab = (uint)icon.Handle,
                    pszModuleName = PLUGIN_NAME
                };
                IntPtr nppTbDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf(nppTbData));
                Marshal.StructureToPtr(nppTbData, nppTbDataPtr, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, nppTbDataPtr);
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, previewWindow.Handle);
            }

            IntPtr editorBachgroundColorPtr = Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETEDITORDEFAULTBACKGROUNDCOLOR, 0, 0);
            int bbggrr = editorBachgroundColorPtr.ToInt32();
            Color editorBackgroundColor = Color.FromArgb(bbggrr & 0x0000FF, (bbggrr & 0x00FF00) >> 8, (bbggrr & 0xFF0000) >> 16);
            previewWindow.SetStyle(editorBackgroundColor);
        }

        private void ShowSettings()
        {
            using (SettingsWindow settingsWindow = new SettingsWindow(settings))
            {
                settingsWindow.ShowDialog();
            }
        }

        private void ShowAbout()
        {
            using (AboutWindow aboutWindow = new AboutWindow())
            {
                aboutWindow.ShowDialog();
            }
        }
    }
}
