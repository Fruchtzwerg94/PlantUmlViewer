using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

using Kbg.NppPluginNET.PluginInfrastructure;

using PlantUmlViewer.Forms;
using PlantUmlViewer.Settings;

namespace PlantUmlViewer
{
    internal class PlantUmlViewer
    {
        public const string PLUGIN_NAME = "PlantUML Viewer";
        public const string PLANT_UML_VERSION = "1.2025.10";
        public const string PLANT_UML_JAR = "plantuml-" + PLANT_UML_VERSION + ".jar";

        private enum CommandId
        {
            ShowPreview,
            Refresh,
            Separator1,
            ShowOptions,
            ShowAbout,
            //Commands to map shortcuts to, hidden in the menu
            Export,
            ZoomIn,
            ZoomOut,
            ZoomFit,
            ZoomReset,
            PreviousDiagram,
            NextDiagram,
            PreviousPage,
            NextPage
        }

        private INotepadPPGateway notepadPp;
        private SettingsService settings;

        private PreviewWindow previewWindow;

        public PlantUmlViewer()
        { }

        public void OnNotification(ScNotification notification)
        {
            //NPPN_DARKMODECHANGED or NPPN_WORDSTYLESUPDATED
            if (   (notification.Header.Code == ((uint)NppMsg.NPPN_FIRST + 27))
                || (notification.Header.Code == (uint)NppMsg.NPPN_WORDSTYLESUPDATED))
            {
                UpdateStyle();
            }
            //Text modification or document changed
            else if((  (notification.Header.Code == (uint)SciMsg.SCN_MODIFIED)
                    && ((notification.ModificationType & ((int)SciMsg.SC_MOD_INSERTTEXT | (int)SciMsg.SC_MOD_DELETETEXT)) != 0))
                || (notification.Header.Code == (uint)NppMsg.NPPN_BUFFERACTIVATED))
            {
                previewWindow?.DocumentChanged();
            }
        }

        public void CommandMenuInit()
        {
            notepadPp = new NotepadPPGateway();
            settings = new SettingsService(notepadPp);

            PluginBase.SetCommand((int)CommandId.ShowPreview, "Preview PlantUML", ShowPreview);
            PluginBase.SetCommand((int)CommandId.Refresh, "Refresh", Refresh);
            PluginBase.SetCommand((int)CommandId.Separator1, "---", null);
            PluginBase.SetCommand((int)CommandId.ShowOptions, "Options", ShowOptions);
            PluginBase.SetCommand((int)CommandId.ShowAbout, "About", ShowAbout);
            PluginBase.SetCommand((int)CommandId.Export, "Export", () => previewWindow?.Button_Export_Click(null, null));
            PluginBase.SetCommand((int)CommandId.ZoomIn, "Zoom in", () => previewWindow?.Button_ZoomIn_Click(null, null));
            PluginBase.SetCommand((int)CommandId.ZoomOut, "Zoom out", () => previewWindow?.Button_ZoomOut_Click(null, null));
            PluginBase.SetCommand((int)CommandId.ZoomFit, "Zoom to fit", () => previewWindow?.Button_ZoomFit_Click(null, null));
            PluginBase.SetCommand((int)CommandId.ZoomReset, "Reset zoom", () => previewWindow?.Button_ZoomReset_Click(null, null));
            PluginBase.SetCommand((int)CommandId.PreviousDiagram, "Previous diagram", () => previewWindow?.Button_PreviousDiagram_Click(null, null));
            PluginBase.SetCommand((int)CommandId.NextDiagram, "Next diagram", () => previewWindow?.Button_NextDiagram_Click(null, null));
            PluginBase.SetCommand((int)CommandId.PreviousPage, "Previous page", () => previewWindow?.Button_PreviousPage_Click(null, null));
            PluginBase.SetCommand((int)CommandId.NextPage, "Next page", () => previewWindow?.Button_NextPage_Click(null, null));
        }

        public void SetToolBarIcon()
        {
            notepadPp.AddToolbarIcon((int)CommandId.ShowPreview, new toolbarIcons()
            {
                hToolbarBmp = Properties.Resources.Image.GetHbitmap(),
                hToolbarIcon = Properties.Resources.IconFluent.Handle,
                hToolbarIconDarkMode = Properties.Resources.IconFluentDark.Handle
            });

            //Hide some commands of the menu
            IntPtr hMenu = Win32.SendMessage(PluginBase.nppData._nppHandle,
                (uint)NppMsg.NPPM_GETMENUHANDLE, (int)NppMsg.NPPPLUGINMENU, 0);
            Win32.RemoveMenu(hMenu, PluginBase._funcItems.Items[(int)CommandId.Export]._cmdID, Win32.MF_BYCOMMAND);
            Win32.RemoveMenu(hMenu, PluginBase._funcItems.Items[(int)CommandId.ZoomIn]._cmdID, Win32.MF_BYCOMMAND);
            Win32.RemoveMenu(hMenu, PluginBase._funcItems.Items[(int)CommandId.ZoomOut]._cmdID, Win32.MF_BYCOMMAND);
            Win32.RemoveMenu(hMenu, PluginBase._funcItems.Items[(int)CommandId.ZoomFit]._cmdID, Win32.MF_BYCOMMAND);
            Win32.RemoveMenu(hMenu, PluginBase._funcItems.Items[(int)CommandId.ZoomReset]._cmdID, Win32.MF_BYCOMMAND);
            Win32.RemoveMenu(hMenu, PluginBase._funcItems.Items[(int)CommandId.PreviousDiagram]._cmdID, Win32.MF_BYCOMMAND);
            Win32.RemoveMenu(hMenu, PluginBase._funcItems.Items[(int)CommandId.NextDiagram]._cmdID, Win32.MF_BYCOMMAND);
            Win32.RemoveMenu(hMenu, PluginBase._funcItems.Items[(int)CommandId.PreviousPage]._cmdID, Win32.MF_BYCOMMAND);
            Win32.RemoveMenu(hMenu, PluginBase._funcItems.Items[(int)CommandId.NextPage]._cmdID, Win32.MF_BYCOMMAND);

            VisibilityChanged(false);
        }

        public void PluginCleanUp()
        { }

        private void ShowPreview()
        {
            if (previewWindow == null)
            {
                previewWindow = new PreviewWindow(notepadPp.GetCurrentFilePath, GetText, settings);

                previewWindow.DockablePanelClose += (_, __) => VisibilityChanged(false);

                NppTbData nppTbData = new NppTbData
                {
                    hClient = previewWindow.Handle,
                    pszName = "PlantUML",
                    dlgID = (int)CommandId.ShowPreview,
                    uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR,
                    hIconTab = (uint)Properties.Resources.Icon.Handle,
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

        private static string GetText()
        {
            IScintillaGateway editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            if (editor.GetCodePage() != (int)SciMsg.SC_CP_UTF8)
            {
                throw new FileFormatException("File encoding invalid, please use UTF-8 as encoding");
            }
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
        }

        private void UpdateStyle()
        {
            IntPtr editorBachgroundColorPtr = Win32.SendMessage(PluginBase.nppData._nppHandle,
                (uint)NppMsg.NPPM_GETEDITORDEFAULTBACKGROUNDCOLOR, 0, 0);
            int bbggrr = editorBachgroundColorPtr.ToInt32();
            Color editorBackgroundColor = Color.FromArgb(bbggrr & 0x0000FF, (bbggrr & 0x00FF00) >> 8, (bbggrr & 0xFF0000) >> 16);
            previewWindow?.SetStyle(editorBackgroundColor);
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
                previewWindow.Button_Refresh_Click(this, null);
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
