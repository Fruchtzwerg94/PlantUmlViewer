namespace PlantUmlViewer.Forms
{
    partial class OptionsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            this.tableLayoutPanel_Window = new System.Windows.Forms.TableLayoutPanel();
            this.label_headerGeneral = new System.Windows.Forms.Label();
            this.tableLayoutPanel_Buttons = new System.Windows.Forms.TableLayoutPanel();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_ok = new System.Windows.Forms.Button();
            this.label_JavaPath = new System.Windows.Forms.Label();
            this.textBox_JavaPath = new System.Windows.Forms.TextBox();
            this.label_JavaPathDescription = new System.Windows.Forms.Label();
            this.tableLayoutPanel_Window.SuspendLayout();
            this.tableLayoutPanel_Buttons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_Window
            // 
            this.tableLayoutPanel_Window.ColumnCount = 2;
            this.tableLayoutPanel_Window.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel_Window.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Window.Controls.Add(this.label_headerGeneral, 0, 0);
            this.tableLayoutPanel_Window.Controls.Add(this.tableLayoutPanel_Buttons, 0, 4);
            this.tableLayoutPanel_Window.Controls.Add(this.label_JavaPath, 0, 1);
            this.tableLayoutPanel_Window.Controls.Add(this.textBox_JavaPath, 2, 1);
            this.tableLayoutPanel_Window.Controls.Add(this.label_JavaPathDescription, 1, 2);
            this.tableLayoutPanel_Window.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Window.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_Window.Name = "tableLayoutPanel_Window";
            this.tableLayoutPanel_Window.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel_Window.RowCount = 5;
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel_Window.Size = new System.Drawing.Size(566, 274);
            this.tableLayoutPanel_Window.TabIndex = 0;
            // 
            // label_headerGeneral
            // 
            this.label_headerGeneral.AutoSize = true;
            this.tableLayoutPanel_Window.SetColumnSpan(this.label_headerGeneral, 2);
            this.label_headerGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_headerGeneral.Location = new System.Drawing.Point(8, 5);
            this.label_headerGeneral.Name = "label_headerGeneral";
            this.label_headerGeneral.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.label_headerGeneral.Size = new System.Drawing.Size(66, 22);
            this.label_headerGeneral.TabIndex = 3;
            this.label_headerGeneral.Text = "General";
            // 
            // tableLayoutPanel_Buttons
            // 
            this.tableLayoutPanel_Buttons.ColumnCount = 2;
            this.tableLayoutPanel_Window.SetColumnSpan(this.tableLayoutPanel_Buttons, 2);
            this.tableLayoutPanel_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Buttons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel_Buttons.Controls.Add(this.button_Cancel, 0, 0);
            this.tableLayoutPanel_Buttons.Controls.Add(this.button_ok, 1, 0);
            this.tableLayoutPanel_Buttons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Buttons.Location = new System.Drawing.Point(8, 232);
            this.tableLayoutPanel_Buttons.Name = "tableLayoutPanel_Buttons";
            this.tableLayoutPanel_Buttons.RowCount = 1;
            this.tableLayoutPanel_Buttons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Buttons.Size = new System.Drawing.Size(550, 34);
            this.tableLayoutPanel_Buttons.TabIndex = 2;
            // 
            // button_Cancel
            // 
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Cancel.Location = new System.Drawing.Point(3, 3);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(269, 28);
            this.button_Cancel.TabIndex = 1;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            // 
            // button_ok
            // 
            this.button_ok.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_ok.Location = new System.Drawing.Point(278, 3);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(269, 28);
            this.button_ok.TabIndex = 0;
            this.button_ok.Text = "OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.Button_Ok_Click);
            // 
            // label_JavaPath
            // 
            this.label_JavaPath.AutoSize = true;
            this.label_JavaPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_JavaPath.Location = new System.Drawing.Point(8, 27);
            this.label_JavaPath.Name = "label_JavaPath";
            this.label_JavaPath.Size = new System.Drawing.Size(54, 26);
            this.label_JavaPath.TabIndex = 0;
            this.label_JavaPath.Text = "Java path";
            this.label_JavaPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_JavaPath
            // 
            this.textBox_JavaPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_JavaPath.Location = new System.Drawing.Point(68, 30);
            this.textBox_JavaPath.Name = "textBox_JavaPath";
            this.textBox_JavaPath.Size = new System.Drawing.Size(490, 20);
            this.textBox_JavaPath.TabIndex = 1;
            // 
            // label_JavaPathDescription
            // 
            this.label_JavaPathDescription.AutoSize = true;
            this.label_JavaPathDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_JavaPathDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_JavaPathDescription.Location = new System.Drawing.Point(68, 53);
            this.label_JavaPathDescription.Name = "label_JavaPathDescription";
            this.label_JavaPathDescription.Size = new System.Drawing.Size(490, 13);
            this.label_JavaPathDescription.TabIndex = 4;
            this.label_JavaPathDescription.Text = "Path of the Java executable. Leavy empty if JAVA_HOME environment variable is set" +
    " to auto detect.";
            this.label_JavaPathDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OptionsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(566, 274);
            this.Controls.Add(this.tableLayoutPanel_Window);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.tableLayoutPanel_Window.ResumeLayout(false);
            this.tableLayoutPanel_Window.PerformLayout();
            this.tableLayoutPanel_Buttons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Window;
        private System.Windows.Forms.Label label_headerGeneral;
        private System.Windows.Forms.Label label_JavaPath;
        private System.Windows.Forms.TextBox textBox_JavaPath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Buttons;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Label label_JavaPathDescription;
    }
}