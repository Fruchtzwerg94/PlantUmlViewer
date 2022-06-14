namespace PlantUmlViewer.Forms
{
    partial class PreviewWindow
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel_Window = new System.Windows.Forms.TableLayoutPanel();
            this.button_Export = new System.Windows.Forms.Button();
            this.statusStrip_Bottom = new System.Windows.Forms.StatusStrip();
            this.loadingCircleToolStripMenuItem_Refreshing = new MRG.Controls.UI.LoadingCircleToolStripMenuItem();
            this.toolStripStatusLabel_Time = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel_Zoom = new System.Windows.Forms.ToolStripStatusLabel();
            this.button_Refresh = new System.Windows.Forms.Button();
            this.imageBox_Diagram = new Cyotek.Windows.Forms.ImageBox();
            this.contextMenuStrip_Diagram = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_Diagram_CopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Diagram_ExportFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel_Window.SuspendLayout();
            this.statusStrip_Bottom.SuspendLayout();
            this.contextMenuStrip_Diagram.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_Window
            // 
            this.tableLayoutPanel_Window.ColumnCount = 2;
            this.tableLayoutPanel_Window.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Window.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel_Window.Controls.Add(this.button_Export, 1, 0);
            this.tableLayoutPanel_Window.Controls.Add(this.statusStrip_Bottom, 0, 2);
            this.tableLayoutPanel_Window.Controls.Add(this.button_Refresh, 0, 0);
            this.tableLayoutPanel_Window.Controls.Add(this.imageBox_Diagram, 0, 1);
            this.tableLayoutPanel_Window.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_Window.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_Window.Name = "tableLayoutPanel_Window";
            this.tableLayoutPanel_Window.RowCount = 3;
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_Window.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel_Window.Size = new System.Drawing.Size(284, 262);
            this.tableLayoutPanel_Window.TabIndex = 1;
            // 
            // button_Export
            // 
            this.button_Export.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Export.Enabled = false;
            this.button_Export.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Export.Location = new System.Drawing.Point(145, 3);
            this.button_Export.Name = "button_Export";
            this.button_Export.Size = new System.Drawing.Size(136, 23);
            this.button_Export.TabIndex = 2;
            this.button_Export.Text = "Export";
            this.button_Export.UseVisualStyleBackColor = true;
            this.button_Export.Click += new System.EventHandler(this.Button_Export_Click);
            // 
            // statusStrip_Bottom
            // 
            this.tableLayoutPanel_Window.SetColumnSpan(this.statusStrip_Bottom, 2);
            this.statusStrip_Bottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip_Bottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadingCircleToolStripMenuItem_Refreshing,
            this.toolStripStatusLabel_Time,
            this.toolStripStatusLabel_Zoom});
            this.statusStrip_Bottom.Location = new System.Drawing.Point(0, 240);
            this.statusStrip_Bottom.Name = "statusStrip_Bottom";
            this.statusStrip_Bottom.Size = new System.Drawing.Size(284, 22);
            this.statusStrip_Bottom.TabIndex = 2;
            // 
            // loadingCircleToolStripMenuItem_Refreshing
            // 
            // 
            // loadingCircleToolStripMenuItem_Refreshing
            // 
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.AccessibleName = "loadingCircleToolStripMenuItem_Refreshing";
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Active = false;
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Color = System.Drawing.Color.DarkGray;
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.InnerCircleRadius = 6;
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Location = new System.Drawing.Point(1, 2);
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Name = "loadingCircleToolStripMenuItem_Refreshing";
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.NumberSpoke = 9;
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.OuterCircleRadius = 7;
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.RotationSpeed = 100;
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Size = new System.Drawing.Size(22, 20);
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.SpokeThickness = 4;
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.Firefox;
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.TabIndex = 1;
            this.loadingCircleToolStripMenuItem_Refreshing.LoadingCircleControl.Text = "Refreshing";
            this.loadingCircleToolStripMenuItem_Refreshing.Name = "loadingCircleToolStripMenuItem_Refreshing";
            this.loadingCircleToolStripMenuItem_Refreshing.Size = new System.Drawing.Size(22, 20);
            this.loadingCircleToolStripMenuItem_Refreshing.Text = "Refreshing";
            this.loadingCircleToolStripMenuItem_Refreshing.Visible = false;
            // 
            // toolStripStatusLabel_Time
            // 
            this.toolStripStatusLabel_Time.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.toolStripStatusLabel_Time.Name = "toolStripStatusLabel_Time";
            this.toolStripStatusLabel_Time.Size = new System.Drawing.Size(181, 16);
            this.toolStripStatusLabel_Time.Spring = true;
            // 
            // toolStripStatusLabel_Zoom
            // 
            this.toolStripStatusLabel_Zoom.Name = "toolStripStatusLabel_Zoom";
            this.toolStripStatusLabel_Zoom.Size = new System.Drawing.Size(35, 17);
            this.toolStripStatusLabel_Zoom.Text = "100%";
            // 
            // button_Refresh
            // 
            this.button_Refresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_Refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Refresh.Location = new System.Drawing.Point(3, 3);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Size = new System.Drawing.Size(136, 23);
            this.button_Refresh.TabIndex = 1;
            this.button_Refresh.Text = "Refresh";
            this.button_Refresh.UseVisualStyleBackColor = true;
            this.button_Refresh.Click += new System.EventHandler(this.Button_Refresh_Click);
            // 
            // imageBox_Diagram
            // 
            this.tableLayoutPanel_Window.SetColumnSpan(this.imageBox_Diagram, 2);
            this.imageBox_Diagram.ContextMenuStrip = this.contextMenuStrip_Diagram;
            this.imageBox_Diagram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageBox_Diagram.GridDisplayMode = Cyotek.Windows.Forms.ImageBoxGridDisplayMode.None;
            this.imageBox_Diagram.Location = new System.Drawing.Point(3, 32);
            this.imageBox_Diagram.Name = "imageBox_Diagram";
            this.imageBox_Diagram.Size = new System.Drawing.Size(278, 205);
            this.imageBox_Diagram.TabIndex = 3;
            this.imageBox_Diagram.TabStop = false;
            // 
            // contextMenuStrip_Diagram
            // 
            this.contextMenuStrip_Diagram.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Diagram_CopyToClipboard,
            this.ToolStripMenuItem_Diagram_ExportFile});
            this.contextMenuStrip_Diagram.Name = "contextMenuStrip_Diagram";
            this.contextMenuStrip_Diagram.Size = new System.Drawing.Size(170, 48);
            // 
            // ToolStripMenuItem_Diagram_CopyToClipboard
            // 
            this.ToolStripMenuItem_Diagram_CopyToClipboard.Enabled = false;
            this.ToolStripMenuItem_Diagram_CopyToClipboard.Name = "ToolStripMenuItem_Diagram_CopyToClipboard";
            this.ToolStripMenuItem_Diagram_CopyToClipboard.Size = new System.Drawing.Size(169, 22);
            this.ToolStripMenuItem_Diagram_CopyToClipboard.Text = "Copy to clipboard";
            this.ToolStripMenuItem_Diagram_CopyToClipboard.Click += new System.EventHandler(this.ToolStripMenuItem_Diagram_CopyToClipboard_Click);
            // 
            // ToolStripMenuItem_Diagram_ExportFile
            // 
            this.ToolStripMenuItem_Diagram_ExportFile.Enabled = false;
            this.ToolStripMenuItem_Diagram_ExportFile.Name = "ToolStripMenuItem_Diagram_ExportFile";
            this.ToolStripMenuItem_Diagram_ExportFile.Size = new System.Drawing.Size(169, 22);
            this.ToolStripMenuItem_Diagram_ExportFile.Text = "Export";
            this.ToolStripMenuItem_Diagram_ExportFile.Click += new System.EventHandler(this.Button_Export_Click);
            // 
            // PreviewWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.tableLayoutPanel_Window);
            this.Name = "PreviewWindow";
            this.Text = "PlantUML";
            this.tableLayoutPanel_Window.ResumeLayout(false);
            this.tableLayoutPanel_Window.PerformLayout();
            this.statusStrip_Bottom.ResumeLayout(false);
            this.statusStrip_Bottom.PerformLayout();
            this.contextMenuStrip_Diagram.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_Window;
        private System.Windows.Forms.Button button_Refresh;
        private Cyotek.Windows.Forms.ImageBox imageBox_Diagram;
        private System.Windows.Forms.Button button_Export;
        private System.Windows.Forms.StatusStrip statusStrip_Bottom;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_Zoom;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_Time;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Diagram;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Diagram_CopyToClipboard;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Diagram_ExportFile;
        private MRG.Controls.UI.LoadingCircleToolStripMenuItem loadingCircleToolStripMenuItem_Refreshing;
    }
}