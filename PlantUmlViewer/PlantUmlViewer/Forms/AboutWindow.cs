using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace PlantUmlViewer.Forms
{
    internal partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();
            this.Text = PlantUmlViewer.PLUGIN_NAME;
            this.label_ProductName.Text = AssemblyProduct;
            this.label_Version.Text = string.Format("Version {0}", AssemblyVersion);
            this.label_Copyright.Text = AssemblyCopyright;
            this.richTextBox_Text.SelectedRtf = Properties.Resources.AboutText;
        }

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
        #endregion

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
    }
}
