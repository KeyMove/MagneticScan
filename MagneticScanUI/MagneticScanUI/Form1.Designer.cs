﻿namespace MagneticScanUI
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.MainTable = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.codeindex = new System.Windows.Forms.CheckBox();
            this.PathList = new System.Windows.Forms.ListBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.SpeedChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.Dvalue = new System.Windows.Forms.TextBox();
            this.Ivalue = new System.Windows.Forms.TextBox();
            this.Pvalue = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.trackI = new System.Windows.Forms.TrackBar();
            this.trackD = new System.Windows.Forms.TrackBar();
            this.trackP = new System.Windows.Forms.TrackBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.FLen = new System.Windows.Forms.TextBox();
            this.BLen = new System.Windows.Forms.TextBox();
            this.SendPath = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.turnB = new System.Windows.Forms.Button();
            this.turnF = new System.Windows.Forms.Button();
            this.turnR = new System.Windows.Forms.Button();
            this.send = new System.Windows.Forms.Button();
            this.turnL = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.StatsImg = new System.Windows.Forms.PictureBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gotoNodeButton = new System.Windows.Forms.Button();
            this.SearchButton = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.PathPoint = new System.Windows.Forms.ListBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.rightkey = new System.Windows.Forms.Button();
            this.resetKey = new System.Windows.Forms.Button();
            this.leftkey = new System.Windows.Forms.Button();
            this.downkey = new System.Windows.Forms.Button();
            this.upkey = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.MapBox = new System.Windows.Forms.PictureBox();
            this.ImgList = new System.Windows.Forms.ImageList(this.components);
            this.SaveMap = new System.Windows.Forms.Button();
            this.LoadMap = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.MainTable.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SpeedChart)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackP)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StatsImg)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapBox)).BeginInit();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTable
            // 
            this.MainTable.Controls.Add(this.tabPage1);
            this.MainTable.Controls.Add(this.tabPage2);
            this.MainTable.Location = new System.Drawing.Point(12, 12);
            this.MainTable.Name = "MainTable";
            this.MainTable.SelectedIndex = 0;
            this.MainTable.Size = new System.Drawing.Size(760, 527);
            this.MainTable.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox9);
            this.tabPage1.Controls.Add(this.groupBox8);
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(752, 501);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "状态";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // codeindex
            // 
            this.codeindex.AutoSize = true;
            this.codeindex.Location = new System.Drawing.Point(8, 49);
            this.codeindex.Name = "codeindex";
            this.codeindex.Size = new System.Drawing.Size(72, 16);
            this.codeindex.TabIndex = 8;
            this.codeindex.Text = "程序动态";
            this.codeindex.UseVisualStyleBackColor = true;
            // 
            // PathList
            // 
            this.PathList.FormattingEnabled = true;
            this.PathList.ItemHeight = 12;
            this.PathList.Location = new System.Drawing.Point(89, 19);
            this.PathList.Name = "PathList";
            this.PathList.Size = new System.Drawing.Size(120, 124);
            this.PathList.TabIndex = 7;
            this.PathList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PathList_KeyDown);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.SpeedChart);
            this.groupBox5.Location = new System.Drawing.Point(346, 190);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(400, 163);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "电机速度";
            // 
            // SpeedChart
            // 
            chartArea3.Name = "ChartArea1";
            this.SpeedChart.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.SpeedChart.Legends.Add(legend3);
            this.SpeedChart.Location = new System.Drawing.Point(6, 15);
            this.SpeedChart.Name = "SpeedChart";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.SpeedChart.Series.Add(series3);
            this.SpeedChart.Size = new System.Drawing.Size(388, 142);
            this.SpeedChart.TabIndex = 2;
            this.SpeedChart.Text = "chart1";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Dvalue);
            this.groupBox3.Controls.Add(this.Ivalue);
            this.groupBox3.Controls.Add(this.Pvalue);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.trackI);
            this.groupBox3.Controls.Add(this.trackD);
            this.groupBox3.Controls.Add(this.trackP);
            this.groupBox3.Location = new System.Drawing.Point(346, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(240, 180);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "电机控制";
            // 
            // Dvalue
            // 
            this.Dvalue.Location = new System.Drawing.Point(199, 134);
            this.Dvalue.Name = "Dvalue";
            this.Dvalue.Size = new System.Drawing.Size(29, 21);
            this.Dvalue.TabIndex = 3;
            this.Dvalue.Text = "30";
            // 
            // Ivalue
            // 
            this.Ivalue.Location = new System.Drawing.Point(199, 83);
            this.Ivalue.Name = "Ivalue";
            this.Ivalue.Size = new System.Drawing.Size(29, 21);
            this.Ivalue.TabIndex = 3;
            this.Ivalue.Text = "90";
            // 
            // Pvalue
            // 
            this.Pvalue.Location = new System.Drawing.Point(199, 30);
            this.Pvalue.Name = "Pvalue";
            this.Pvalue.Size = new System.Drawing.Size(29, 21);
            this.Pvalue.TabIndex = 3;
            this.Pvalue.Text = "100";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 137);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "D";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "I";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "P";
            // 
            // trackI
            // 
            this.trackI.BackColor = System.Drawing.Color.White;
            this.trackI.Location = new System.Drawing.Point(30, 68);
            this.trackI.Maximum = 100;
            this.trackI.Name = "trackI";
            this.trackI.Size = new System.Drawing.Size(163, 45);
            this.trackI.TabIndex = 0;
            this.trackI.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackI.Value = 90;
            this.trackI.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // trackD
            // 
            this.trackD.BackColor = System.Drawing.Color.White;
            this.trackD.Location = new System.Drawing.Point(30, 119);
            this.trackD.Maximum = 100;
            this.trackD.Name = "trackD";
            this.trackD.Size = new System.Drawing.Size(163, 45);
            this.trackD.TabIndex = 0;
            this.trackD.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackD.Value = 30;
            this.trackD.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // trackP
            // 
            this.trackP.BackColor = System.Drawing.Color.White;
            this.trackP.Location = new System.Drawing.Point(30, 17);
            this.trackP.Maximum = 100;
            this.trackP.Name = "trackP";
            this.trackP.Size = new System.Drawing.Size(163, 45);
            this.trackP.TabIndex = 0;
            this.trackP.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackP.Value = 100;
            this.trackP.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.FLen);
            this.groupBox2.Controls.Add(this.BLen);
            this.groupBox2.Location = new System.Drawing.Point(592, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(157, 180);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "超声波传感器";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "后方:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 152);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "探测距离:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "前方:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(79, 149);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(64, 21);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "5";
            // 
            // FLen
            // 
            this.FLen.Location = new System.Drawing.Point(59, 37);
            this.FLen.Name = "FLen";
            this.FLen.Size = new System.Drawing.Size(88, 21);
            this.FLen.TabIndex = 3;
            // 
            // BLen
            // 
            this.BLen.Location = new System.Drawing.Point(59, 64);
            this.BLen.Name = "BLen";
            this.BLen.Size = new System.Drawing.Size(88, 21);
            this.BLen.TabIndex = 3;
            // 
            // SendPath
            // 
            this.SendPath.Location = new System.Drawing.Point(8, 19);
            this.SendPath.Name = "SendPath";
            this.SendPath.Size = new System.Drawing.Size(75, 23);
            this.SendPath.TabIndex = 2;
            this.SendPath.Text = "发送";
            this.SendPath.UseVisualStyleBackColor = true;
            this.SendPath.Click += new System.EventHandler(this.SendPath_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 115);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "复位";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.turnB_Click);
            // 
            // turnB
            // 
            this.turnB.Location = new System.Drawing.Point(215, 105);
            this.turnB.Name = "turnB";
            this.turnB.Size = new System.Drawing.Size(75, 23);
            this.turnB.TabIndex = 2;
            this.turnB.Text = "尽头掉头";
            this.turnB.UseVisualStyleBackColor = true;
            this.turnB.Click += new System.EventHandler(this.turnB_Click);
            // 
            // turnF
            // 
            this.turnF.Location = new System.Drawing.Point(215, 76);
            this.turnF.Name = "turnF";
            this.turnF.Size = new System.Drawing.Size(75, 23);
            this.turnF.TabIndex = 2;
            this.turnF.Text = "路口直走";
            this.turnF.UseVisualStyleBackColor = true;
            this.turnF.Click += new System.EventHandler(this.turnB_Click);
            // 
            // turnR
            // 
            this.turnR.Location = new System.Drawing.Point(215, 47);
            this.turnR.Name = "turnR";
            this.turnR.Size = new System.Drawing.Size(75, 23);
            this.turnR.TabIndex = 2;
            this.turnR.Text = "路口右转";
            this.turnR.UseVisualStyleBackColor = true;
            this.turnR.Click += new System.EventHandler(this.turnB_Click);
            // 
            // send
            // 
            this.send.Location = new System.Drawing.Point(349, 14);
            this.send.Name = "send";
            this.send.Size = new System.Drawing.Size(85, 31);
            this.send.TabIndex = 2;
            this.send.Text = "刷新网络设备";
            this.send.UseVisualStyleBackColor = true;
            this.send.Click += new System.EventHandler(this.send_Click);
            // 
            // turnL
            // 
            this.turnL.Location = new System.Drawing.Point(215, 18);
            this.turnL.Name = "turnL";
            this.turnL.Size = new System.Drawing.Size(75, 23);
            this.turnL.TabIndex = 2;
            this.turnL.Text = "路口左转";
            this.turnL.UseVisualStyleBackColor = true;
            this.turnL.Click += new System.EventHandler(this.turnB_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.StatsImg);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(334, 347);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "传感器状态";
            // 
            // StatsImg
            // 
            this.StatsImg.Location = new System.Drawing.Point(6, 20);
            this.StatsImg.Name = "StatsImg";
            this.StatsImg.Size = new System.Drawing.Size(320, 320);
            this.StatsImg.TabIndex = 0;
            this.StatsImg.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.LoadMap);
            this.tabPage2.Controls.Add(this.SaveMap);
            this.tabPage2.Controls.Add(this.gotoNodeButton);
            this.tabPage2.Controls.Add(this.SearchButton);
            this.tabPage2.Controls.Add(this.groupBox7);
            this.tabPage2.Controls.Add(this.groupBox6);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(752, 501);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "路径规划";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // gotoNodeButton
            // 
            this.gotoNodeButton.Location = new System.Drawing.Point(87, 392);
            this.gotoNodeButton.Name = "gotoNodeButton";
            this.gotoNodeButton.Size = new System.Drawing.Size(75, 23);
            this.gotoNodeButton.TabIndex = 6;
            this.gotoNodeButton.Text = "前往节点";
            this.gotoNodeButton.UseVisualStyleBackColor = true;
            this.gotoNodeButton.Click += new System.EventHandler(this.gotoNodeButton_Click);
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(6, 392);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(75, 23);
            this.SearchButton.TabIndex = 6;
            this.SearchButton.Text = "搜索路径";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.PathPoint);
            this.groupBox7.Location = new System.Drawing.Point(515, 94);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(231, 292);
            this.groupBox7.TabIndex = 5;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "路径点";
            // 
            // PathPoint
            // 
            this.PathPoint.Font = new System.Drawing.Font("宋体", 16F);
            this.PathPoint.FormattingEnabled = true;
            this.PathPoint.ItemHeight = 21;
            this.PathPoint.Location = new System.Drawing.Point(6, 20);
            this.PathPoint.Name = "PathPoint";
            this.PathPoint.Size = new System.Drawing.Size(219, 256);
            this.PathPoint.TabIndex = 4;
            this.PathPoint.SelectedIndexChanged += new System.EventHandler(this.PathPoint_SelectedIndexChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.checkBox1);
            this.groupBox6.Controls.Add(this.rightkey);
            this.groupBox6.Controls.Add(this.resetKey);
            this.groupBox6.Controls.Add(this.leftkey);
            this.groupBox6.Controls.Add(this.downkey);
            this.groupBox6.Controls.Add(this.upkey);
            this.groupBox6.Location = new System.Drawing.Point(515, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(231, 82);
            this.groupBox6.TabIndex = 3;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "方向控制";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(6, 20);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(96, 16);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "启用方向控制";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // rightkey
            // 
            this.rightkey.Font = new System.Drawing.Font("宋体", 12F);
            this.rightkey.Location = new System.Drawing.Point(182, 29);
            this.rightkey.Name = "rightkey";
            this.rightkey.Size = new System.Drawing.Size(37, 23);
            this.rightkey.TabIndex = 2;
            this.rightkey.Text = "→";
            this.rightkey.UseVisualStyleBackColor = true;
            this.rightkey.Click += new System.EventHandler(this.rightkey_Click);
            // 
            // resetKey
            // 
            this.resetKey.Font = new System.Drawing.Font("宋体", 9F);
            this.resetKey.Location = new System.Drawing.Point(6, 53);
            this.resetKey.Name = "resetKey";
            this.resetKey.Size = new System.Drawing.Size(50, 23);
            this.resetKey.TabIndex = 2;
            this.resetKey.Text = "复位";
            this.resetKey.UseVisualStyleBackColor = true;
            this.resetKey.Click += new System.EventHandler(this.resetKey_Click);
            // 
            // leftkey
            // 
            this.leftkey.Font = new System.Drawing.Font("宋体", 12F);
            this.leftkey.Location = new System.Drawing.Point(108, 29);
            this.leftkey.Name = "leftkey";
            this.leftkey.Size = new System.Drawing.Size(37, 23);
            this.leftkey.TabIndex = 2;
            this.leftkey.Text = "←";
            this.leftkey.UseVisualStyleBackColor = true;
            this.leftkey.Click += new System.EventHandler(this.leftkey_Click);
            // 
            // downkey
            // 
            this.downkey.Font = new System.Drawing.Font("宋体", 12F);
            this.downkey.Location = new System.Drawing.Point(145, 42);
            this.downkey.Name = "downkey";
            this.downkey.Size = new System.Drawing.Size(37, 23);
            this.downkey.TabIndex = 2;
            this.downkey.Text = "↓";
            this.downkey.UseVisualStyleBackColor = true;
            this.downkey.Click += new System.EventHandler(this.downkey_Click);
            // 
            // upkey
            // 
            this.upkey.Font = new System.Drawing.Font("宋体", 12F);
            this.upkey.Location = new System.Drawing.Point(145, 13);
            this.upkey.Name = "upkey";
            this.upkey.Size = new System.Drawing.Size(37, 23);
            this.upkey.TabIndex = 2;
            this.upkey.Text = "↑";
            this.upkey.UseVisualStyleBackColor = true;
            this.upkey.Click += new System.EventHandler(this.upkey_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.MapBox);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(503, 380);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "地图";
            // 
            // MapBox
            // 
            this.MapBox.Location = new System.Drawing.Point(6, 20);
            this.MapBox.Name = "MapBox";
            this.MapBox.Size = new System.Drawing.Size(491, 353);
            this.MapBox.TabIndex = 0;
            this.MapBox.TabStop = false;
            // 
            // ImgList
            // 
            this.ImgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.ImgList.ImageSize = new System.Drawing.Size(32, 32);
            this.ImgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // SaveMap
            // 
            this.SaveMap.Location = new System.Drawing.Point(6, 472);
            this.SaveMap.Name = "SaveMap";
            this.SaveMap.Size = new System.Drawing.Size(75, 23);
            this.SaveMap.TabIndex = 7;
            this.SaveMap.Text = "储存地图";
            this.SaveMap.UseVisualStyleBackColor = true;
            this.SaveMap.Click += new System.EventHandler(this.SaveMap_Click);
            // 
            // LoadMap
            // 
            this.LoadMap.Location = new System.Drawing.Point(87, 472);
            this.LoadMap.Name = "LoadMap";
            this.LoadMap.Size = new System.Drawing.Size(75, 23);
            this.LoadMap.TabIndex = 7;
            this.LoadMap.Text = "读取地图";
            this.LoadMap.UseVisualStyleBackColor = true;
            this.LoadMap.Click += new System.EventHandler(this.LoadMap_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.listView1);
            this.groupBox8.Controls.Add(this.send);
            this.groupBox8.Location = new System.Drawing.Point(6, 355);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(440, 145);
            this.groupBox8.TabIndex = 9;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "设备列表";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.SendPath);
            this.groupBox9.Controls.Add(this.turnL);
            this.groupBox9.Controls.Add(this.codeindex);
            this.groupBox9.Controls.Add(this.turnR);
            this.groupBox9.Controls.Add(this.PathList);
            this.groupBox9.Controls.Add(this.turnF);
            this.groupBox9.Controls.Add(this.turnB);
            this.groupBox9.Controls.Add(this.button1);
            this.groupBox9.Location = new System.Drawing.Point(452, 355);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(297, 145);
            this.groupBox9.TabIndex = 10;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "固定线路";
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(1, 14);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(342, 126);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 546);
            this.Controls.Add(this.MainTable);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MainTable.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SpeedChart)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackP)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.StatsImg)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MapBox)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTable;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.PictureBox StatsImg;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ImageList ImgList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button send;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox FLen;
        private System.Windows.Forms.TextBox BLen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar trackI;
        private System.Windows.Forms.TrackBar trackD;
        private System.Windows.Forms.TrackBar trackP;
        private System.Windows.Forms.DataVisualization.Charting.Chart SpeedChart;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.PictureBox MapBox;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox Dvalue;
        private System.Windows.Forms.TextBox Ivalue;
        private System.Windows.Forms.TextBox Pvalue;
        private System.Windows.Forms.Button turnR;
        private System.Windows.Forms.Button turnL;
        private System.Windows.Forms.ListBox PathList;
        private System.Windows.Forms.Button SendPath;
        private System.Windows.Forms.Button turnB;
        private System.Windows.Forms.Button turnF;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox codeindex;
        private System.Windows.Forms.Button rightkey;
        private System.Windows.Forms.Button upkey;
        private System.Windows.Forms.Button leftkey;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.ListBox PathPoint;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button gotoNodeButton;
        private System.Windows.Forms.Button downkey;
        private System.Windows.Forms.Button resetKey;
        private System.Windows.Forms.Button LoadMap;
        private System.Windows.Forms.Button SaveMap;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.ListView listView1;
    }
}

