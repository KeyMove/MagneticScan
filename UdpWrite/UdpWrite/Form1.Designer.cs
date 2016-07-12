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
            this.FileData = new System.Windows.Forms.TextBox();
            this.SelectDevButton = new System.Windows.Forms.Button();
            this.DevSelect = new System.Windows.Forms.ComboBox();
            this.RecvText = new System.Windows.Forms.TextBox();
            this.FileList = new System.Windows.Forms.ListBox();
            this.OpenFile = new System.Windows.Forms.Button();
            this.SaveFile = new System.Windows.Forms.Button();
            this.SendText = new System.Windows.Forms.TextBox();
            this.Downprogress = new System.Windows.Forms.ProgressBar();
            this.DownloadButton = new System.Windows.Forms.Button();
            this.SendCmd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FileData
            // 
            this.FileData.BackColor = System.Drawing.Color.DarkGray;
            this.FileData.Location = new System.Drawing.Point(138, 12);
            this.FileData.Multiline = true;
            this.FileData.Name = "FileData";
            this.FileData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.FileData.Size = new System.Drawing.Size(459, 436);
            this.FileData.TabIndex = 0;
            // 
            // SelectDevButton
            // 
            this.SelectDevButton.Location = new System.Drawing.Point(216, 495);
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
            this.DevSelect.Location = new System.Drawing.Point(12, 497);
            this.DevSelect.Name = "DevSelect";
            this.DevSelect.Size = new System.Drawing.Size(198, 20);
            this.DevSelect.TabIndex = 2;
            // 
            // RecvText
            // 
            this.RecvText.BackColor = System.Drawing.Color.DarkGray;
            this.RecvText.Location = new System.Drawing.Point(603, 12);
            this.RecvText.Multiline = true;
            this.RecvText.Name = "RecvText";
            this.RecvText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.RecvText.Size = new System.Drawing.Size(254, 209);
            this.RecvText.TabIndex = 3;
            // 
            // FileList
            // 
            this.FileList.BackColor = System.Drawing.Color.DarkGray;
            this.FileList.FormattingEnabled = true;
            this.FileList.ItemHeight = 12;
            this.FileList.Location = new System.Drawing.Point(12, 12);
            this.FileList.Name = "FileList";
            this.FileList.Size = new System.Drawing.Size(120, 436);
            this.FileList.TabIndex = 4;
            this.FileList.SelectedIndexChanged += new System.EventHandler(this.FileList_SelectedIndexChanged);
            // 
            // OpenFile
            // 
            this.OpenFile.Location = new System.Drawing.Point(11, 449);
            this.OpenFile.Name = "OpenFile";
            this.OpenFile.Size = new System.Drawing.Size(60, 23);
            this.OpenFile.TabIndex = 5;
            this.OpenFile.Text = "读取";
            this.OpenFile.UseVisualStyleBackColor = true;
            this.OpenFile.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // SaveFile
            // 
            this.SaveFile.Location = new System.Drawing.Point(73, 449);
            this.SaveFile.Name = "SaveFile";
            this.SaveFile.Size = new System.Drawing.Size(60, 23);
            this.SaveFile.TabIndex = 6;
            this.SaveFile.Text = "保存";
            this.SaveFile.UseVisualStyleBackColor = true;
            // 
            // SendText
            // 
            this.SendText.BackColor = System.Drawing.Color.DarkGray;
            this.SendText.Location = new System.Drawing.Point(603, 227);
            this.SendText.Multiline = true;
            this.SendText.Name = "SendText";
            this.SendText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.SendText.Size = new System.Drawing.Size(254, 79);
            this.SendText.TabIndex = 3;
            // 
            // Downprogress
            // 
            this.Downprogress.Location = new System.Drawing.Point(138, 449);
            this.Downprogress.Name = "Downprogress";
            this.Downprogress.Size = new System.Drawing.Size(393, 23);
            this.Downprogress.TabIndex = 7;
            // 
            // DownloadButton
            // 
            this.DownloadButton.Location = new System.Drawing.Point(537, 449);
            this.DownloadButton.Name = "DownloadButton";
            this.DownloadButton.Size = new System.Drawing.Size(60, 23);
            this.DownloadButton.TabIndex = 6;
            this.DownloadButton.Text = "下载";
            this.DownloadButton.UseVisualStyleBackColor = true;
            this.DownloadButton.Click += new System.EventHandler(this.DownloadButton_Click);
            // 
            // SendCmd
            // 
            this.SendCmd.Location = new System.Drawing.Point(797, 312);
            this.SendCmd.Name = "SendCmd";
            this.SendCmd.Size = new System.Drawing.Size(60, 23);
            this.SendCmd.TabIndex = 6;
            this.SendCmd.Text = "发送";
            this.SendCmd.UseVisualStyleBackColor = true;
            this.SendCmd.Click += new System.EventHandler(this.SendCmd_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(869, 529);
            this.Controls.Add(this.Downprogress);
            this.Controls.Add(this.SendCmd);
            this.Controls.Add(this.DownloadButton);
            this.Controls.Add(this.SaveFile);
            this.Controls.Add(this.OpenFile);
            this.Controls.Add(this.FileList);
            this.Controls.Add(this.SendText);
            this.Controls.Add(this.RecvText);
            this.Controls.Add(this.DevSelect);
            this.Controls.Add(this.SelectDevButton);
            this.Controls.Add(this.FileData);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

