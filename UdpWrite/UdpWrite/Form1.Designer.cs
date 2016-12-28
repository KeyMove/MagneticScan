namespace UdpWrite
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.FileData = new System.Windows.Forms.TextBox();
            this.SelectDevButton = new System.Windows.Forms.Button();
            this.DevSelect = new System.Windows.Forms.ComboBox();
            this.RecvText = new System.Windows.Forms.TextBox();
            this.FileList = new System.Windows.Forms.ListBox();
            this.FileControlMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.刷新ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFile = new System.Windows.Forms.Button();
            this.SaveFile = new System.Windows.Forms.Button();
            this.SendText = new System.Windows.Forms.TextBox();
            this.Downprogress = new System.Windows.Forms.ProgressBar();
            this.DownloadButton = new System.Windows.Forms.Button();
            this.SendCmd = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.FastSend = new System.Windows.Forms.GroupBox();
            this.SBOX = new System.Windows.Forms.TextBox();
            this.SButton = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.SendPanel = new System.Windows.Forms.Panel();
            this.FileControlMenuStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.FastSend.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SendPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileData
            // 
            this.FileData.Location = new System.Drawing.Point(132, 6);
            this.FileData.Multiline = true;
            this.FileData.Name = "FileData";
            this.FileData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.FileData.Size = new System.Drawing.Size(459, 457);
            this.FileData.TabIndex = 0;
            // 
            // SelectDevButton
            // 
            this.SelectDevButton.Location = new System.Drawing.Point(269, 10);
            this.SelectDevButton.Name = "SelectDevButton";
            this.SelectDevButton.Size = new System.Drawing.Size(75, 23);
            this.SelectDevButton.TabIndex = 1;
            this.SelectDevButton.Text = "连接";
            this.SelectDevButton.UseVisualStyleBackColor = true;
            this.SelectDevButton.Click += new System.EventHandler(this.SelectDevButton_Click);
            // 
            // DevSelect
            // 
            this.DevSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DevSelect.FormattingEnabled = true;
            this.DevSelect.Location = new System.Drawing.Point(65, 11);
            this.DevSelect.Name = "DevSelect";
            this.DevSelect.Size = new System.Drawing.Size(198, 20);
            this.DevSelect.TabIndex = 2;
            // 
            // RecvText
            // 
            this.RecvText.Location = new System.Drawing.Point(6, 20);
            this.RecvText.Multiline = true;
            this.RecvText.Name = "RecvText";
            this.RecvText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.RecvText.Size = new System.Drawing.Size(570, 232);
            this.RecvText.TabIndex = 3;
            // 
            // FileList
            // 
            this.FileList.ContextMenuStrip = this.FileControlMenuStrip;
            this.FileList.FormattingEnabled = true;
            this.FileList.ItemHeight = 12;
            this.FileList.Location = new System.Drawing.Point(6, 6);
            this.FileList.Name = "FileList";
            this.FileList.Size = new System.Drawing.Size(120, 412);
            this.FileList.TabIndex = 4;
            this.FileList.SelectedIndexChanged += new System.EventHandler(this.FileList_SelectedIndexChanged);
            // 
            // FileControlMenuStrip
            // 
            this.FileControlMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.刷新ToolStripMenuItem,
            this.删除ToolStripMenuItem});
            this.FileControlMenuStrip.Name = "FileControlMenuStrip";
            this.FileControlMenuStrip.Size = new System.Drawing.Size(101, 48);
            // 
            // 刷新ToolStripMenuItem
            // 
            this.刷新ToolStripMenuItem.Name = "刷新ToolStripMenuItem";
            this.刷新ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.刷新ToolStripMenuItem.Text = "刷新";
            this.刷新ToolStripMenuItem.Click += new System.EventHandler(this.刷新ToolStripMenuItem_Click);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.删除ToolStripMenuItem.Text = "删除";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.删除ToolStripMenuItem_Click);
            // 
            // OpenFile
            // 
            this.OpenFile.Location = new System.Drawing.Point(5, 443);
            this.OpenFile.Name = "OpenFile";
            this.OpenFile.Size = new System.Drawing.Size(60, 23);
            this.OpenFile.TabIndex = 5;
            this.OpenFile.Text = "打开";
            this.OpenFile.UseVisualStyleBackColor = true;
            this.OpenFile.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // SaveFile
            // 
            this.SaveFile.Location = new System.Drawing.Point(67, 443);
            this.SaveFile.Name = "SaveFile";
            this.SaveFile.Size = new System.Drawing.Size(60, 23);
            this.SaveFile.TabIndex = 6;
            this.SaveFile.Text = "保存";
            this.SaveFile.UseVisualStyleBackColor = true;
            // 
            // SendText
            // 
            this.SendText.Location = new System.Drawing.Point(9, 20);
            this.SendText.Multiline = true;
            this.SendText.Name = "SendText";
            this.SendText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.SendText.Size = new System.Drawing.Size(236, 144);
            this.SendText.TabIndex = 3;
            // 
            // Downprogress
            // 
            this.Downprogress.Location = new System.Drawing.Point(133, 441);
            this.Downprogress.Name = "Downprogress";
            this.Downprogress.Size = new System.Drawing.Size(457, 23);
            this.Downprogress.TabIndex = 7;
            this.Downprogress.Visible = false;
            // 
            // DownloadButton
            // 
            this.DownloadButton.Location = new System.Drawing.Point(67, 419);
            this.DownloadButton.Name = "DownloadButton";
            this.DownloadButton.Size = new System.Drawing.Size(60, 23);
            this.DownloadButton.TabIndex = 6;
            this.DownloadButton.Text = "下载";
            this.DownloadButton.UseVisualStyleBackColor = true;
            // 
            // SendCmd
            // 
            this.SendCmd.Location = new System.Drawing.Point(8, 165);
            this.SendCmd.Name = "SendCmd";
            this.SendCmd.Size = new System.Drawing.Size(74, 23);
            this.SendCmd.TabIndex = 6;
            this.SendCmd.Text = "发送";
            this.SendCmd.UseVisualStyleBackColor = true;
            this.SendCmd.Click += new System.EventHandler(this.SendCmd_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(7, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(602, 495);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.FastSend);
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(594, 469);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "数据收发";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.SendText);
            this.groupBox3.Controls.Add(this.SendCmd);
            this.groupBox3.Location = new System.Drawing.Point(3, 270);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(254, 193);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "发送区";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.RecvText);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(582, 258);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "接收区";
            // 
            // FastSend
            // 
            this.FastSend.Controls.Add(this.SendPanel);
            this.FastSend.Location = new System.Drawing.Point(263, 270);
            this.FastSend.Name = "FastSend";
            this.FastSend.Size = new System.Drawing.Size(325, 193);
            this.FastSend.TabIndex = 7;
            this.FastSend.TabStop = false;
            this.FastSend.Text = "快捷发送";
            // 
            // SBOX
            // 
            this.SBOX.Location = new System.Drawing.Point(3, 4);
            this.SBOX.Name = "SBOX";
            this.SBOX.Size = new System.Drawing.Size(259, 21);
            this.SBOX.TabIndex = 9;
            this.SBOX.Visible = false;
            // 
            // SButton
            // 
            this.SButton.Location = new System.Drawing.Point(268, 4);
            this.SButton.Name = "SButton";
            this.SButton.Size = new System.Drawing.Size(40, 21);
            this.SButton.TabIndex = 8;
            this.SButton.Text = "发送";
            this.SButton.UseVisualStyleBackColor = true;
            this.SButton.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.FileList);
            this.tabPage1.Controls.Add(this.Downprogress);
            this.tabPage1.Controls.Add(this.FileData);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.DownloadButton);
            this.tabPage1.Controls.Add(this.OpenFile);
            this.tabPage1.Controls.Add(this.SaveFile);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(594, 469);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "文件管理";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(5, 419);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(60, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "上传";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.DownloadButton_Click);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Location = new System.Drawing.Point(7, 500);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(598, 71);
            this.tabControl2.TabIndex = 10;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.DevSelect);
            this.tabPage3.Controls.Add(this.button3);
            this.tabPage3.Controls.Add(this.SelectDevButton);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(590, 45);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "网络连接";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "设备列表";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(350, 11);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 1;
            this.button3.Text = "连接";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.save);
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(590, 45);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "串口连接";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // SendPanel
            // 
            this.SendPanel.AutoScroll = true;
            this.SendPanel.Controls.Add(this.SBOX);
            this.SendPanel.Controls.Add(this.SButton);
            this.SendPanel.Location = new System.Drawing.Point(2, 16);
            this.SendPanel.Name = "SendPanel";
            this.SendPanel.Size = new System.Drawing.Size(326, 172);
            this.SendPanel.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 571);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FileControlMenuStrip.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.FastSend.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.SendPanel.ResumeLayout(false);
            this.SendPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox FileData;
        private System.Windows.Forms.Button SelectDevButton;
        private System.Windows.Forms.ComboBox DevSelect;
        private System.Windows.Forms.TextBox RecvText;
        private System.Windows.Forms.ListBox FileList;
        private System.Windows.Forms.Button OpenFile;
        private System.Windows.Forms.Button SaveFile;
        private System.Windows.Forms.TextBox SendText;
        private System.Windows.Forms.ProgressBar Downprogress;
        private System.Windows.Forms.Button DownloadButton;
        private System.Windows.Forms.Button SendCmd;
        private System.Windows.Forms.ContextMenuStrip FileControlMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem 刷新ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox FastSend;
        private System.Windows.Forms.TextBox SBOX;
        private System.Windows.Forms.Button SButton;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel SendPanel;
    }
}

