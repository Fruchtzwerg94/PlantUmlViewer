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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label_ProductName = new System.Windows.Forms.Label();
            this.label_Version = new System.Windows.Forms.Label();
            this.button_Ok = new System.Windows.Forms.Button();
            this.richTextBox_Text = new System.Windows.Forms.RichTextBox();
            this.linkLabel_Github = new System.Windows.Forms.LinkLabel();
            this.linkLabel_Mail = new System.Windows.Forms.LinkLabel();
            this.linkLabel_Donate = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label_Copyright = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.label_ProductName, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.label_Version, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.label_Copyright, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.button_Ok, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.richTextBox_Text, 0, 4);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(417, 265);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // label_ProductName
            // 
            this.label_ProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_ProductName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_ProductName.Location = new System.Drawing.Point(6, 0);
            this.label_ProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.label_ProductName.MaximumSize = new System.Drawing.Size(0, 17);
            this.label_ProductName.Name = "label_ProductName";
            this.label_ProductName.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.label_ProductName.Size = new System.Drawing.Size(408, 17);
            this.label_ProductName.TabIndex = 19;
            this.label_ProductName.Text = "Product name";
            this.label_ProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_Version
            // 
            this.label_Version.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Version.Location = new System.Drawing.Point(6, 17);
            this.label_Version.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.label_Version.MaximumSize = new System.Drawing.Size(0, 17);
            this.label_Version.Name = "label_Version";
            this.label_Version.Size = new System.Drawing.Size(408, 17);
            this.label_Version.TabIndex = 0;
            this.label_Version.Text = "Version";
            this.label_Version.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button_Ok
            // 
            this.button_Ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Ok.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Ok.Location = new System.Drawing.Point(3, 238);
            this.button_Ok.Name = "button_Ok";
            this.button_Ok.Size = new System.Drawing.Size(411, 24);
            this.button_Ok.TabIndex = 24;
            this.button_Ok.Text = "OK";
            // 
            // richTextBox_Text
            // 
            this.richTextBox_Text.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_Text.Location = new System.Drawing.Point(3, 74);
            this.richTextBox_Text.Name = "richTextBox_Text";
            this.richTextBox_Text.ReadOnly = true;
            this.richTextBox_Text.Size = new System.Drawing.Size(411, 158);
            this.richTextBox_Text.TabIndex = 27;
            this.richTextBox_Text.Text = "";
            // 
            // linkLabel_Github
            // 
            this.linkLabel_Github.AutoSize = true;
            this.linkLabel_Github.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel_Github.Location = new System.Drawing.Point(3, 0);
            this.linkLabel_Github.Name = "linkLabel_Github";
            this.linkLabel_Github.Size = new System.Drawing.Size(130, 14);
            this.linkLabel_Github.TabIndex = 25;
            this.linkLabel_Github.TabStop = true;
            this.linkLabel_Github.Text = "Visit project on GitHub";
            this.linkLabel_Github.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel_Github.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_Github_LinkClicked);
            // 
            // linkLabel_Mail
            // 
            this.linkLabel_Mail.AutoSize = true;
            this.linkLabel_Mail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel_Mail.Location = new System.Drawing.Point(139, 0);
            this.linkLabel_Mail.Name = "linkLabel_Mail";
            this.linkLabel_Mail.Size = new System.Drawing.Size(130, 14);
            this.linkLabel_Mail.TabIndex = 26;
            this.linkLabel_Mail.TabStop = true;
            this.linkLabel_Mail.Text = "Mail me";
            this.linkLabel_Mail.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel_Mail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_Mail_LinkClicked);
            // 
            // linkLabel_Donate
            // 
            this.linkLabel_Donate.AutoSize = true;
            this.linkLabel_Donate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel_Donate.Location = new System.Drawing.Point(275, 0);
            this.linkLabel_Donate.Name = "linkLabel_Donate";
            this.linkLabel_Donate.Size = new System.Drawing.Size(133, 14);
            this.linkLabel_Donate.TabIndex = 27;
            this.linkLabel_Donate.TabStop = true;
            this.linkLabel_Donate.Text = "Buy me a coffee :-)";
            this.linkLabel_Donate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel_Donate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_Donate_LinkClicked);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.linkLabel_Donate, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.linkLabel_Mail, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.linkLabel_Github, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 54);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(411, 14);
            this.tableLayoutPanel1.TabIndex = 26;
            // 
            // label_Copyright
            // 
            this.label_Copyright.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_Copyright.Location = new System.Drawing.Point(6, 34);
            this.label_Copyright.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
            this.label_Copyright.MaximumSize = new System.Drawing.Size(0, 17);
            this.label_Copyright.Name = "label_Copyright";
            this.label_Copyright.Size = new System.Drawing.Size(408, 17);
            this.label_Copyright.TabIndex = 21;
            this.label_Copyright.Text = "Copyright";
            this.label_Copyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AboutWindow
            // 
            this.AcceptButton = this.button_Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 283);
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutWindow";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutWindow";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label label_ProductName;
        private System.Windows.Forms.Label label_Version;
        private System.Windows.Forms.Button button_Ok;
        private System.Windows.Forms.RichTextBox richTextBox_Text;
        private System.Windows.Forms.Label label_Copyright;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.LinkLabel linkLabel_Donate;
        private System.Windows.Forms.LinkLabel linkLabel_Mail;
        private System.Windows.Forms.LinkLabel linkLabel_Github;
    }
}
