using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using Kbg.NppPluginNET.PluginInfrastructure;

using PlantUmlViewer.Forms;
using PlantUmlViewer.Settings;

namespace PlantUmlViewer
{
    internal class PlantUmlViewer
    {
        public const string PLUGIN_NAME = "PlantUML Viewer";

        private const string PLANT_UML_BINARY = "plantuml-1.2022.5.jar";

        private enum CommandId
        {
            ShowPreview     = 0,
            Refresh         = 1,
            Separator1      = 2,
            ShowOptions     = 3,
            ShowAbout       = 4
        }

        private readonly string assemblyDirectory;

        private INotepadPPGateway notepadPp;
        private SettingsService settings;

        private readonly Icon icon;
        private PreviewWindow previewWindow;

        public PlantUmlViewer()
        {
            assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

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
            settings = new SettingsService(notepadPp);

            PluginBase.SetCommand((int)CommandId.ShowPreview, "Show preview", ShowPreview);
            PluginBase.SetCommand((int)CommandId.Refresh, "Refresh", Refresh);
            PluginBase.SetCommand((int)CommandId.Separator1, "---", null);
            PluginBase.SetCommand((int)CommandId.ShowOptions, "Options", ShowOptions);
            PluginBase.SetCommand((int)CommandId.ShowAbout, "About", ShowAbout);
        }

        public void SetToolBarIcon()
        {
            notepadPp.AddToolbarIcon((int)CommandId.ShowPreview, Properties.Resources.Icon);

            VisibilityChanged(false);
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
                    () =>
                    {
                        IScintillaGateway editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());
                        return editor.GetText(editor.GetLength() + 1);

                        //const int GET_TEXT_STEP_SIZE = 10000;
                        //StringBuilder textBuilder = new StringBuilder();
                        //int length = editor.GetLength();
                        //int position = 0;
                        //int rest = length;
                        //while (rest > 0)
                        //{
                        //    int step = Math.Min(rest, GET_TEXT_STEP_SIZE);
                        //    using (TextRange textRange = new TextRange(position, position + step, step + 1))
                        //    {
                        //        int ret = editor.GetTextRange(textRange);
                        //        if (ret != step)
                        //        {
                        //            throw new Exception("Failed to get text");
                        //        }
                        //        textBuilder.Append(textRange.lpstrText.TrimEnd('\0'));
                        //    }
                        //    position += step;
                        //    rest -= step;
                        //}
                        //return textBuilder.ToString();
                    }, settings);

                previewWindow.DockablePanelClose += (_, __) => VisibilityChanged(false);

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
                UpdateStyle();
            }
            else
            {
                if (!previewWindow.Visible)
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, previewWindow.Handle);
                    UpdateStyle();
                }
                else
                {
                    Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, previewWindow.Handle);
                }
            }
            VisibilityChanged(previewWindow.Visible);
        }

        private void UpdateStyle()
        {
            IntPtr editorBachgroundColorPtr = Win32.SendMessage(PluginBase.nppData._nppHandle,
                (uint)NppMsg.NPPM_GETEDITORDEFAULTBACKGROUNDCOLOR, 0, 0);
            int bbggrr = editorBachgroundColorPtr.ToInt32();
            Color editorBackgroundColor = Color.FromArgb(bbggrr & 0x0000FF, (bbggrr & 0x00FF00) >> 8, (bbggrr & 0xFF0000) >> 16);
            previewWindow.SetStyle(editorBackgroundColor);
        }

        private void VisibilityChanged(bool visible)
        {
            //Check / uncheck icon
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                PluginBase._funcItems.Items[(int)CommandId.ShowPreview]._cmdID, visible ? 1 : 0);

            //Enable / disable refresh menu entry
            IntPtr hMenu = Win32.SendMessage(PluginBase.nppData._nppHandle,
                (uint)NppMsg.NPPM_GETMENUHANDLE, (int)NppMsg.NPPPLUGINMENU, 0);
            Win32.EnableMenuItem(hMenu, PluginBase._funcItems.Items[(int)CommandId.Refresh]._cmdID,
                Win32.MF_BYCOMMAND | (visible ? Win32.MF_ENABLED : (Win32.MF_DISABLED | Win32.MF_GRAYED)));
        }

        private void Refresh()
        {
            if (previewWindow?.Visible == true)
            {
                previewWindow.Button_Refresh_Click(null, null);
            }
        }

        private void ShowOptions()
        {
            using (OptionsWindow optionsWindow = new OptionsWindow(settings))
            {
                optionsWindow.ShowDialog();
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
