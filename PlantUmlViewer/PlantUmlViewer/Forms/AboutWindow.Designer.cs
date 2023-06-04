namespace PlantUmlViewer.Forms
{
    partial class AboutWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutWindow));
            this.tableLayoutPanel_Window = new System.Windows.Forms.TableLayoutPanel();
            this.label_Copyright = new System.Windows.Forms.Label();
            this.button_Ok = new System.Windows.Forms.Button();
            this.tableLayoutPanel_Links = new System.Windows.Forms.TableLayoutPanel();
            this.linkLabel_Donate = new System.Windows.Forms.LinkLabel();
            this.linkLabel_Mail = new System.Windows.Forms.LinkLabel();
            this.linkLabel_Github = new System.Windows.Forms.LinkLabel();
            this.richTextBox_Text = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel_Version = new System.Windows.Forms.TableLayoutPanel();
            this.label_PluginVersion = new System.Windows.Forms.Label();
            this.button_CheckForUpdate = new System.Windows.Forms.Button();
            this.loadingCircle_checkForUpdate = new MRG.Controls.UI.LoadingCircle();
            this.label_PlantUmlVersion = new System.Windows.Forms.Label();
            this.tableLayoutPanel_Header = new System.Windows.Forms.TableLayoutPanel();
            this.label_ProductName = new System.Windows.Forms.Label();
            this.pictureBox_Image = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel_Window.SuspendLayout();
            this.tableLayoutPanel_Links.SuspendLayout();
            this.tableLayoutPanel_Version.SuspendLayout();
            this.tableLayoutPanel_Header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Image)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_Window
            // 
            this.tableLayoutPanel_Window.ColumnCount = 1;
            this.tableLayoutPanel_Window.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Window.Controls.Add(this.label_Copyright, 0, 2);
            this.tableLayoutPanel_Window.Controls.Add(this.button_Ok, 0, 5);
            this.tableLayoutPanel_Window.Controls.Add(this.tableLayoutPanel_Links, 0, 3);
            this.tableLayoutPanel_Window.Controls.Add(this.richTextBox_Text, 0, 4);
            this.tableLayoutPanel_Window.Controls.Add(this.tableLayoutPanel_Version, 0, 1);
            this.tableLayoutPanel_Window.Controls.Add(this.tableLayoutPanel_Header, 0, 0);
            this.tableLayoutPanel_Window.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Window.Location = new System.Drawing.Point(9, 9);
            this.tableLayoutPanel_Window.Name = "tableLayoutPanel_Window";
            this.tableLayoutPanel_Window.RowCount = 6;
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel_Window.Size = new System.Drawing.Size(478, 309);
            this.tableLayoutPanel_Window.TabIndex = 0;
            // 
            // label_Copyright
            // 
            this.label_Copyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Copyright.Location = new System.Drawing.Point(6, 75);
            this.label_Copyright.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.label_Copyright.Name = "label_Copyright";
            this.label_Copyright.Size = new System.Drawing.Size(469, 17);
            this.label_Copyright.TabIndex = 21;
            this.label_Copyright.Text = "Copyright";
            this.label_Copyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button_Ok
            // 
            this.button_Ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Ok.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Ok.Location = new System.Drawing.Point(3, 282);
            this.button_Ok.Name = "button_Ok";
            this.button_Ok.Size = new System.Drawing.Size(472, 24);
            this.button_Ok.TabIndex = 24;
            this.button_Ok.Text = "OK";
            // 
            // tableLayoutPanel_Links
            // 
            this.tableLayoutPanel_Links.ColumnCount = 3;
            this.tableLayoutPanel_Links.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel_Links.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel_Links.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel_Links.Controls.Add(this.linkLabel_Donate, 2, 0);
            this.tableLayoutPanel_Links.Controls.Add(this.linkLabel_Mail, 1, 0);
            this.tableLayoutPanel_Links.Controls.Add(this.linkLabel_Github, 0, 0);
            this.tableLayoutPanel_Links.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Links.Location = new System.Drawing.Point(3, 95);
            this.tableLayoutPanel_Links.Name = "tableLayoutPanel_Links";
            this.tableLayoutPanel_Links.RowCount = 1;
            this.tableLayoutPanel_Links.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Links.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 14F));
            this.tableLayoutPanel_Links.Size = new System.Drawing.Size(472, 14);
            this.tableLayoutPanel_Links.TabIndex = 26;
            // 
            // linkLabel_Donate
            // 
            this.linkLabel_Donate.AutoSize = true;
            this.linkLabel_Donate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel_Donate.Location = new System.Drawing.Point(317, 0);
            this.linkLabel_Donate.Name = "linkLabel_Donate";
            this.linkLabel_Donate.Size = new System.Drawing.Size(152, 14);
            this.linkLabel_Donate.TabIndex = 27;
            this.linkLabel_Donate.TabStop = true;
            this.linkLabel_Donate.Text = "Buy me a coffee :-)";
            this.linkLabel_Donate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel_Donate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_Donate_LinkClicked);
            // 
            // linkLabel_Mail
            // 
            this.linkLabel_Mail.AutoSize = true;
            this.linkLabel_Mail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel_Mail.Location = new System.Drawing.Point(160, 0);
            this.linkLabel_Mail.Name = "linkLabel_Mail";
            this.linkLabel_Mail.Size = new System.Drawing.Size(151, 14);
            this.linkLabel_Mail.TabIndex = 26;
            this.linkLabel_Mail.TabStop = true;
            this.linkLabel_Mail.Text = "Mail me";
            this.linkLabel_Mail.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel_Mail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_Mail_LinkClicked);
            // 
            // linkLabel_Github
            // 
            this.linkLabel_Github.AutoSize = true;
            this.linkLabel_Github.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel_Github.Location = new System.Drawing.Point(3, 0);
            this.linkLabel_Github.Name = "linkLabel_Github";
            this.linkLabel_Github.Size = new System.Drawing.Size(151, 14);
            this.linkLabel_Github.TabIndex = 25;
            this.linkLabel_Github.TabStop = true;
            this.linkLabel_Github.Text = "Visit project on GitHub";
            this.linkLabel_Github.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel_Github.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_Github_LinkClicked);
            // 
            // richTextBox_Text
            // 
            this.richTextBox_Text.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_Text.Location = new System.Drawing.Point(3, 115);
            this.richTextBox_Text.Name = "richTextBox_Text";
            this.richTextBox_Text.ReadOnly = true;
            this.richTextBox_Text.Size = new System.Drawing.Size(472, 161);
            this.richTextBox_Text.TabIndex = 27;
            this.richTextBox_Text.Text = "";
            // 
            // tableLayoutPanel_Version
            // 
            this.tableLayoutPanel_Version.ColumnCount = 3;
            this.tableLayoutPanel_Version.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Version.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_Version.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_Version.Controls.Add(this.label_PluginVersion, 0, 0);
            this.tableLayoutPanel_Version.Controls.Add(this.button_CheckForUpdate, 2, 0);
            this.tableLayoutPanel_Version.Controls.Add(this.loadingCircle_checkForUpdate, 1, 0);
            this.tableLayoutPanel_Version.Controls.Add(this.label_PlantUmlVersion, 0, 1);
            this.tableLayoutPanel_Version.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Version.Location = new System.Drawing.Point(0, 35);
            this.tableLayoutPanel_Version.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_Version.Name = "tableLayoutPanel_Version";
            this.tableLayoutPanel_Version.RowCount = 2;
            this.tableLayoutPanel_Version.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Version.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Version.Size = new System.Drawing.Size(478, 40);
            this.tableLayoutPanel_Version.TabIndex = 28;
            // 
            // label_PluginVersion
            // 
            this.label_PluginVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_PluginVersion.Location = new System.Drawing.Point(6, 0);
            this.label_PluginVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.label_PluginVersion.Name = "label_PluginVersion";
            this.label_PluginVersion.Size = new System.Drawing.Size(259, 20);
            this.label_PluginVersion.TabIndex = 1;
            this.label_PluginVersion.Text = "PluginVersion";
            this.label_PluginVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button_CheckForUpdate
            // 
            this.button_CheckForUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_CheckForUpdate.Location = new System.Drawing.Point(352, 3);
            this.button_CheckForUpdate.Name = "button_CheckForUpdate";
            this.tableLayoutPanel_Version.SetRowSpan(this.button_CheckForUpdate, 2);
            this.button_CheckForUpdate.Size = new System.Drawing.Size(123, 34);
            this.button_CheckForUpdate.TabIndex = 2;
            this.button_CheckForUpdate.Text = "Check for update";
            this.button_CheckForUpdate.UseVisualStyleBackColor = true;
            this.button_CheckForUpdate.Click += new System.EventHandler(this.Button_CheckForUpdate_Click);
            // 
            // loadingCircle_checkForUpdate
            // 
            this.loadingCircle_checkForUpdate.Active = false;
            this.loadingCircle_checkForUpdate.Color = System.Drawing.Color.DarkGray;
            this.loadingCircle_checkForUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadingCircle_checkForUpdate.InnerCircleRadius = 6;
            this.loadingCircle_checkForUpdate.Location = new System.Drawing.Point(271, 3);
            this.loadingCircle_checkForUpdate.Name = "loadingCircle_checkForUpdate";
            this.loadingCircle_checkForUpdate.NumberSpoke = 9;
            this.loadingCircle_checkForUpdate.OuterCircleRadius = 7;
            this.loadingCircle_checkForUpdate.RotationSpeed = 100;
            this.tableLayoutPanel_Version.SetRowSpan(this.loadingCircle_checkForUpdate, 2);
            this.loadingCircle_checkForUpdate.Size = new System.Drawing.Size(75, 34);
            this.loadingCircle_checkForUpdate.SpokeThickness = 4;
            this.loadingCircle_checkForUpdate.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.Firefox;
            this.loadingCircle_checkForUpdate.TabIndex = 3;
            this.loadingCircle_checkForUpdate.Text = "Check for update";
            this.loadingCircle_checkForUpdate.Visible = false;
            // 
            // label_PlantUmlVersion
            // 
            this.label_PlantUmlVersion.Location = new System.Drawing.Point(6, 20);
            this.label_PlantUmlVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.label_PlantUmlVersion.Name = "label_PlantUmlVersion";
            this.label_PlantUmlVersion.Size = new System.Drawing.Size(198, 20);
            this.label_PlantUmlVersion.TabIndex = 30;
            this.label_PlantUmlVersion.Text = "PlantUmlVersion";
            this.label_PlantUmlVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel_Header
            // 
            this.tableLayoutPanel_Header.ColumnCount = 2;
            this.tableLayoutPanel_Header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Header.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_Header.Controls.Add(this.label_ProductName, 0, 0);
            this.tableLayoutPanel_Header.Controls.Add(this.pictureBox_Image, 1, 0);
            this.tableLayoutPanel_Header.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Header.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel_Header.Name = "tableLayoutPanel_Header";
            this.tableLayoutPanel_Header.RowCount = 1;
            this.tableLayoutPanel_Header.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Header.Size = new System.Drawing.Size(472, 29);
            this.tableLayoutPanel_Header.TabIndex = 29;
            // 
            // label_ProductName
            // 
            this.label_ProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_ProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ProductName.Location = new System.Drawing.Point(6, 0);
            this.label_ProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.label_ProductName.Name = "label_ProductName";
            this.label_ProductName.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.label_ProductName.Size = new System.Drawing.Size(357, 29);
            this.label_ProductName.TabIndex = 20;
            this.label_ProductName.Text = "Product name";
            this.label_ProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox_Image
            // 
            this.pictureBox_Image.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_Image.Image = global::PlantUmlViewer.Properties.Resources.Image;
            this.pictureBox_Image.Location = new System.Drawing.Point(369, 3);
            this.pictureBox_Image.Name = "pictureBox_Image";
            this.pictureBox_Image.Size = new System.Drawing.Size(100, 23);
            this.pictureBox_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox_Image.TabIndex = 21;
            this.pictureBox_Image.TabStop = false;
            // 
            // AboutWindow
            // 
            this.AcceptButton = this.button_Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 327);
            this.Controls.Add(this.tableLayoutPanel_Window);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutWindow";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutWindow";
            this.tableLayoutPanel_Window.ResumeLayout(false);
            this.tableLayoutPanel_Links.ResumeLayout(false);
            this.tableLayoutPanel_Links.PerformLayout();
            this.tableLayoutPanel_Version.ResumeLayout(false);
            this.tableLayoutPanel_Header.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Window;
        private System.Windows.Forms.Button button_Ok;
        private System.Windows.Forms.RichTextBox richTextBox_Text;
        private System.Windows.Forms.Label label_Copyright;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Links;
        private System.Windows.Forms.LinkLabel linkLabel_Donate;
        private System.Windows.Forms.LinkLabel linkLabel_Mail;
        private System.Windows.Forms.LinkLabel linkLabel_Github;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Version;
        private System.Windows.Forms.Label label_PluginVersion;
        private System.Windows.Forms.Button button_CheckForUpdate;
        private MRG.Controls.UI.LoadingCircle loadingCircle_checkForUpdate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Header;
        private System.Windows.Forms.Label label_ProductName;
        private System.Windows.Forms.PictureBox pictureBox_Image;
        private System.Windows.Forms.Label label_PlantUmlVersion;
    }
}
