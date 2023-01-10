using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;

using Newtonsoft.Json.Linq;

namespace PlantUmlViewer.Forms
{
    internal partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();

            this.Text = "About " + PlantUmlViewer.PLUGIN_NAME;
            this.label_ProductName.Text = AssemblyAttributes.Product;
            this.label_PluginVersion.Text = string.Format("Plugin version {0}", AssemblyAttributes.Version);
            this.label_PlantUmlVersion.Text = string.Format("PlantUML version {0}", PlantUmlViewer.PLANT_UML_VERSION);
            this.label_Copyright.Text = AssemblyAttributes.Copyright;
            this.richTextBox_Text.SelectedRtf = Properties.Resources.AboutText;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //Cancel closing if checking for updates ongoing
            if (loadingCircle_checkForUpdate.Active)
            {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }

        private void LinkLabel_Github_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel_Github.LinkVisited = true;
            Process.Start("https://github.com/Fruchtzwerg94/PlantUmlViewer");
        }

        private void LinkLabel_Mail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel_Mail.LinkVisited = true;
            Process.Start($"mailto:phi_dev@gmx.de?subject={PlantUmlViewer.PLUGIN_NAME}");
        }

        private void LinkLabel_Donate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel_Donate.LinkVisited = true;
            Process.Start("https://www.paypal.me/schmidtph");
        }

        private async void Button_CheckForUpdate_Click(object sender, EventArgs e)
        {
            button_CheckForUpdate.Enabled = false;
            loadingCircle_checkForUpdate.Active = true;
            loadingCircle_checkForUpdate.Visible = true;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (HttpClient client = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(3)
                })
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("request");
                    HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/Fruchtzwerg94/PlantUmlViewer/releases").ConfigureAwait(true);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                    JArray responseArray = JArray.Parse(responseBody);
                    JToken latestRelease = responseArray[0];
                    string latestReleaseVersion = latestRelease.Value<string>("name");
                    string latestReleaseUrl = latestRelease.Value<string>("html_url");

                    if (latestReleaseVersion == AssemblyAttributes.Version)
                    {
                        MessageBox.Show(IsDisposed ? null : this, "You are using the latest release", "Congrats", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (MessageBox.Show(IsDisposed ? null : this, $"A newer version {latestReleaseVersion} is available, do you wish to proceed to the download site?", "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Process.Start(latestReleaseUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(IsDisposed ? null : this, ex.Message, "Failed to get the latest release information", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.InvokeIfRequired(() =>
                {
                    loadingCircle_checkForUpdate.Visible = false;
                    loadingCircle_checkForUpdate.Active = false;
                    button_CheckForUpdate.Enabled = true;
                });
            }
        }
    }
}
