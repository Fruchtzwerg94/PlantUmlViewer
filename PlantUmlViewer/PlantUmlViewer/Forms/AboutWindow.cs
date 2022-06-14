using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Windows.Forms;

using Newtonsoft.Json.Linq;

namespace PlantUmlViewer.Forms
{
    internal partial class AboutWindow : Form
    {
        #region Assembly Attribute Accessors
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion Assembly Attribute Accessors

        public AboutWindow()
        {
            InitializeComponent();

            this.Text = PlantUmlViewer.PLUGIN_NAME;
            this.label_ProductName.Text = AssemblyProduct;
            this.label_Version.Text = string.Format("Version {0}", AssemblyVersion);
            this.label_Copyright.Text = AssemblyCopyright;
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

        private void linkLabel_Github_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel_Github.LinkVisited = true;
            Process.Start("https://github.com/Fruchtzwerg94/PlantUmlViewer");
        }

        private void linkLabel_Mail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel_Mail.LinkVisited = true;
            Process.Start($"mailto:phi_dev@gmx.de?subject={PlantUmlViewer.PLUGIN_NAME}");
        }

        private void linkLabel_Donate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel_Donate.LinkVisited = true;
            Process.Start("https://www.paypal.me/schmidtph");
        }

        private async void button_CheckForUpdate_Click(object sender, EventArgs e)
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

                    if (latestReleaseVersion == AssemblyVersion)
                    {
                        MessageBox.Show(this, "You are using the latest release", "Congrats", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (MessageBox.Show(IsDisposed ? null : this, $"A newer version {latestReleaseVersion} is available, do you whish to proceed to the download site?", "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
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
