using KeyMove.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace LogisticManage
{
    public partial class Form1 : Form
    {
        Bitmap CarMap;
        Graphics CarMapDraw;
        MapDraw mapInfo;
        UdpClient Search = new UdpClient(0);
        IPAddress lasttarget;
        IPAddress[] ips;
        byte[] GetData;

        byte[] recvList=new byte[4096];
        TreeNode PathPoint;


        byte sendID;
        byte[][] sendarraydata = new byte[20][];
        byte[] sendarrayid = new byte[20];
        private byte[] GetIPID;
        private IPEndPoint OutPoint;
        private byte lastID;
        private int SensorStats=0xffff;
        private int WaveSensorL;
        private int WaveSensorR;
        private int LeftMotorSpeed;
        private int RightMotorSpeed;
        private int lastBattery;
        private int lastPathSelect;
        private int lastLenght;
        private int lastDir;
        private int lastNodeID;
        private CarData SelectCar;
        private int lastSendID;

        Series LSpeed = new Series("LeftMotor");
        Series RSpeed = new Series("RightMotor");

        enum CheckStatus:int
        {
            CheckWay = 0,
            ConfirmWay=1,
            ReturnWay=2,
            PreTurn=3,
            TurnWay=4,
            TurnDone=5,
            Stop=6,
        };

        int ReadInt16(Stream s)
        {
            int v;
            v = s.ReadByte();
            v <<= 8;
            v |= s.ReadByte();
            return v;
        }

        int ReadInt32(Stream s)
        {
            int v;
            v = s.ReadByte();
            v <<= 8;
            v |= s.ReadByte();
            v <<= 8;
            v |= s.ReadByte();
            v <<= 8;
            v |= s.ReadByte();
            return v;
        }

        void WriteInt16(Stream s,int data)
        {
            s.WriteByte((byte)((data >> 8) & 0xff));
            s.WriteByte((byte)((data) & 0xff));
        }

        void WriteInt32(Stream s, int data)
        {
            s.WriteByte((byte)((data >> 24) & 0xff));
            s.WriteByte((byte)((data >> 16) & 0xff));
            s.WriteByte((byte)((data >> 8) & 0xff));
            s.WriteByte((byte)((data) & 0xff));
        }

        enum SendCode : int
        {
            PID = 0,
            SetSensorInfo = 1,
            PathRun = 2,
            MotorControl = 3,
            Stop = 4,
            RecvMap = 5,
            SendMap = 6,
            AutoFindPath = 7,
            SetTargetNode = 8,            
        }
        bool checkdata(SendCode code)
        {
            return sendarraydata[(int)code] == null;
        }
        void senddata(SendCode code, params object[] list)
        {
            senddata((int)code, list);
        }
        void senddata(int id, object[] list)
        {
            StringBuilder sb = new StringBuilder();
            sendarrayid[id] = sendID;
            sb.Append("uartpacket(2,{");
            sb.Append(sendID++);
            sb.Append(',');
            sb.Append(id);
            sb.Append(',');
            foreach (object obj in list)
            {
                sb.Append(obj.ToString());
                sb.Append(',');
            }
            sb.Append("0})");

            sendarraydata[id] = Encoding.Default.GetBytes(sb.ToString());
            //Search.Send(cmd, cmd.Length, new IPEndPoint(lasttarget.Address | 0xff000000, 2333));
        }

        void SetNodePointList(List<PathNode> nodes)
        {
            PathPoint.Nodes.Clear();
            foreach (PathNode node in nodes)
            {
                TreeNode tn = new TreeNode(node.ToString());
                tn.Tag = node;
                PathPoint.Nodes.Add(tn);
                PathFlagTree.ExpandAll();
            }
        }

        void recvData(Stream s)
        {
            Invoke(new MethodInvoker(() =>
            {
                int size = ReadInt16(s);
                int pos = ReadInt16(s);
                int len = ReadInt16(s);
                int isData = s.ReadByte();
                if (size == 0)
                {
                    MapRecvFlag = false;
                    return;
                }
                for (int i = 0; i < len; i++)
                {
                    recvList[pos + i] = (byte)s.ReadByte();
                }
                if (isData != 0)
                {
                    senddata(SendCode.RecvMap, (pos + len) / 256, (pos + len) % 256);
                }
                else
                {
                    MapRecvFlag = false;
                    mapInfo.SearchInit();
                    mapInfo.setNode(mapInfo.toNode(new MemoryStream(recvList))[PathType.Forward]);
                    mapInfo.AutoOffset();
                    MapBox.Image = mapInfo.Update();
                    SetNodePointList(mapInfo.getEndPoint());
                }

                UDProgress.Value = (pos+len) * 100 / size;
            }));
        }

        void SendMapPos(int pos)
        {
            StringBuilder sb = new StringBuilder();
            int len = (SendLen - SendPos);
            len = len > 32 ? 32 : len;
            sb.Append(pos / 256);
            sb.Append(',');
            sb.Append(pos % 256);
            sb.Append(',');
            sb.Append(len);
            sb.Append(',');
            sb.Append((SendLen - SendPos) <= 32 ? 0 : 1);
            sb.Append(',');
            for (int i = 0; i < len; i++)
            {
                sb.Append(SendBuff[pos + i]);
                sb.Append(',');
            }
            sb.Append("0");
            if ((SendLen - SendPos) <= 32)
            {
                SendLen = 0;
                SendPos = 0;
                SendBuff = null;
            }
            SendPos += len;

            senddata(SendCode.SendMap, sb.ToString());
        }

        void BroadcastData(byte[] data)
        {
            for (int i = 0; i < ips.Length; i++)
            {
                Search.Send(data, data.Length, new IPEndPoint(ips[i].Address | 0xff000000, 2333));
            }
        }

        IPAddress[] getIPAddress()
        {
            List<IPAddress> iplist = new List<IPAddress>();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                //判断是否为以太网卡
                //Wireless80211         无线网卡    Ppp     宽带连接
                //Ethernet              以太网卡   
                //这里篇幅有限贴几个常用的，其他的返回值大家就自己百度吧！
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet || adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    if (adapter.Speed < 0) continue;
                    //获取以太网卡网络接口信息
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    //获取单播地址集
                    UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                    foreach (UnicastIPAddressInformation ipadd in ipCollection)
                    {
                        //InterNetwork    IPV4地址      InterNetworkV6        IPV6地址
                        //Max            MAX 位址
                        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                            //判断是否为ipv4
                            iplist.Add(ipadd.Address);//获取ip
                    }
                }
            }
            return iplist.ToArray();
        }

        public Form1()
        {
            InitializeComponent();
            mapInfo = new MapDraw(MapBox.Width, MapBox.Height);
            CarMap = new Bitmap(CarPicture.Width, CarPicture.Height);
            CarMapDraw = Graphics.FromImage(CarMap);
            CarMapDraw.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            MapBox.MouseWheel += (object sender, MouseEventArgs e) => {
                if (mapInfo.isNull()) return;
                if (e.Delta > 0)
                    mapInfo.DrawScale += .1;
                else if(mapInfo.DrawScale>0.3)
                    mapInfo.DrawScale -= .1;
                MapBox.Image = mapInfo.Update();
            };
        }
        

        class CarData
        {
            public IPAddress ip;
            public int ID;
            public CarData(IPAddress ip,int id)
            {
                this.ip = ip;
                this.ID = id;
            }

            public override string ToString()
            {
                return string.Format("[{0}] - 0x{1:X8}", ip.ToString(), ID);
            }

        }

        byte[] buildPacket(int id, byte[] data)
        {
            MemoryStream ms = new MemoryStream();
            byte sum = 0;
            ms.WriteByte(0xAA);
            WriteInt16(ms, data.Length);
            ms.WriteByte((byte)id);
            ms.Write(data, 0, data.Length);
            foreach (byte v in data)
                sum += v;
            ms.WriteByte(sum);
            ms.WriteByte(0x55);
            return ms.ToArray();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PathPoint = new TreeNode("路径点");
            PathFlagTree.Nodes.Add(PathPoint);
            GetData = buildPacket(1,new byte[]{ 0});
            GetIPID = Encoding.Default.GetBytes("print(\"ID:\"..node.chipid())");

            SpeedChart.Series.Clear();
            LSpeed.ChartType = RSpeed.ChartType = SeriesChartType.Spline;
            LSpeed.BorderWidth = RSpeed.BorderWidth = 2;
            LSpeed.ShadowOffset = RSpeed.ShadowOffset = 2;
            LSpeed.MarkerStyle = MarkerStyle.Diamond;
            RSpeed.MarkerStyle = MarkerStyle.Cross;

            SpeedChart.Series.Add(LSpeed);
            SpeedChart.Series.Add(RSpeed);
            SpeedChart.ChartAreas[0].AxisY.IsStartedFromZero = false;
            SpeedChart.ChartAreas[0].AxisY.Maximum = 100;
            SpeedChart.ChartAreas[0].AxisY.Minimum = -100;

            ips = getIPAddress();
            OutPoint = new IPEndPoint(IPAddress.Any, 0);
            new Task(() =>
            {
                while (!this.IsDisposed)
                    try
                    {
                        byte[] buff = Search.Receive(ref OutPoint);
                        int index = -1;
                        lasttarget = OutPoint.Address;
                        OutPoint.Address = IPAddress.Any;
                        OutPoint.Port = 0;
                        string value = Encoding.Default.GetString(buff);
                        if (buff[0] == 0xAA)
                        {
                            index = buff[3];
                            switch (buff[3])
                            {
                                case 1:
                                    MemoryStream ms = new MemoryStream(buff);
                                    ms.Seek(4, SeekOrigin.Begin);
                                    lastID = (byte)ms.ReadByte();
                                    SensorStats = ReadInt16(ms);
                                    WaveSensorL = ReadInt16(ms);
                                    WaveSensorR = ReadInt16(ms);
                                    LeftMotorSpeed = (int)ms.ReadByte();
                                    if (LeftMotorSpeed > 128) LeftMotorSpeed -= 256;
                                    RightMotorSpeed = (int)ms.ReadByte();
                                    if (RightMotorSpeed > 128) RightMotorSpeed -= 256;
                                    lastBattery = ms.ReadByte();
                                    switch (ms.ReadByte())
                                    {
                                        case 0:
                                            lastStatus = (CheckStatus)ms.ReadByte();
                                            lastPathSelect = ms.ReadByte();
                                            lastLenght = ms.ReadByte();
                                            lastNodeID = ms.ReadByte();
                                            nowNodeID = ms.ReadByte();
                                            int sid = ms.ReadByte();
                                            int tid = ms.ReadByte();
                                            
                                            mapInfo.WayLenght = false;
                                            Invoke(new MethodInvoker(() =>
                                            {
                                                
                                                if (sid != StartNodeID || tid != TargetNodeID)
                                                {
                                                    if(sid!=0xff&&tid!=0xff)
                                                        mapInfo.setTargetPoint(mapInfo[tid], mapInfo[sid]);
                                                    StartNodeID = sid;
                                                    TargetNodeID = tid;
                                                }
                                                if (!mapInfo.isNull())
                                                {
                                                    if (savelastNodeID != lastNodeID)
                                                    {
                                                        savelastNodeID = lastNodeID;
                                                        mapInfo.SetLastPathLenght(0);
                                                    }
                                                    mapInfo.setCarNode(mapInfo[nowNodeID], mapInfo[lastNodeID]);
                                                    if(nowNodeID!=lastNodeID)
                                                    if (!(lastStatus == CheckStatus.PreTurn || lastStatus == CheckStatus.TurnWay || lastStatus == CheckStatus.TurnDone))
                                                        mapInfo.SetLastPathLenght(mapInfo[lastNodeID].pathlenght[(int)mapInfo[lastNodeID][mapInfo[nowNodeID]]] * lastLenght / 100);
                                                    MapBox.Image = mapInfo.Update();
                                                }

                                            }));
                                            break;
                                        case 1:
                                            Invoke(new MethodInvoker(() =>
                                            {
                                                int isnewway = ms.ReadByte();
                                                lastStatus = (CheckStatus)ms.ReadByte();
                                                lastPathSelect = ms.ReadByte();
                                            lastLenght = ms.ReadByte();
                                            lastNodeID = ms.ReadByte();
                                            nowNodeID = ms.ReadByte();
                                            lastDir = ms.ReadByte();
                                            if (mapInfo.allPath.Count <= nowNodeID)
                                            {
                                                for (int i = mapInfo.NodeCount; i <= nowNodeID; i++)
                                                {
                                                    mapInfo.getNewPathNode();
                                                }
                                            }

                                            if (isnewway == 0)
                                            {
                                                mapInfo.WayLenght = true;
                                                if (mapInfo[lastNodeID][mapInfo[nowNodeID]] == PathType.nil)
                                                {
                                                    mapInfo.LinkNode(mapInfo[lastNodeID], mapInfo[nowNodeID], (PathType)lastDir);
                                                }
                                                if (!mapInfo.isNull())
                                                {
                                                    mapInfo.setCarNode(mapInfo[nowNodeID], mapInfo[lastNodeID]);
                                                    if (savelastNodeID != lastNodeID)
                                                    {
                                                        savelastNodeID = lastNodeID;
                                                        mapInfo.SetLastPathLenght(0);
                                                    }
                                                        if (nowNodeID != lastNodeID)
                                                            if (!(lastStatus == CheckStatus.PreTurn || lastStatus == CheckStatus.TurnWay || lastStatus == CheckStatus.TurnDone))
                                                        mapInfo.SetLastPathLenght(lastLenght);
                                                }
                                            }
                                            else
                                            {
                                                mapInfo.WayLenght = false;
                                                if (!mapInfo.isNull())
                                                {
                                                    if (savelastNodeID != lastNodeID)
                                                    {
                                                        savelastNodeID = lastNodeID;
                                                        mapInfo.SetLastPathLenght(0);
                                                    }
                                                    mapInfo.setCarNode(mapInfo[nowNodeID], mapInfo[lastNodeID]);
                                                        if (nowNodeID != lastNodeID)
                                                            if (!(lastStatus == CheckStatus.PreTurn || lastStatus == CheckStatus.TurnWay || lastStatus == CheckStatus.TurnDone))
                                                        mapInfo.SetLastPathLenght(mapInfo[lastNodeID].pathlenght[(int)mapInfo[lastNodeID][mapInfo[nowNodeID]]] * lastLenght / 100);
                                                }
                                            }
                                            
                                                if (!mapInfo.isNull())
                                                    MapBox.Image = mapInfo.Update();
                                            }));
                                            //mapInfo.SetCarPathPos(mapInfo[lastNodeID], lastLenght, (PathType)lastDir);
                                            break;
                                        case 2:
                                            if (MapRecvFlag)
                                                recvData(ms);
                                            break;
                                        case 3:
                                            if (MapSendFlag)
                                                if (!checkdata(SendCode.SendMap))
                                                {
                                                    if (SendLen != 0)
                                                    {
                                                        SendMapPos(SendPos);
                                                        Invoke(new MethodInvoker(() =>
                                                        {
                                                            if (SendLen != 0)
                                                                UDProgress.Value = SendPos * 100 / SendLen;
                                                        }));
                                                    }
                                                    else
                                                    {
                                                        MapSendFlag = false;
                                                        Invoke(new MethodInvoker(() =>
                                                        {
                                                            UDProgress.Value = 100;
                                                        }));
                                                    }
                                                }
                                            break;
                                    }
                                    Invoke(new MethodInvoker(() =>
                                    {
                                        UpdateInfo();
                                    }));
                                    break;
                            }
                        }
                        else
                        {
                            if (value.IndexOf("ID:") == 0)
                            {
                                IPAddress ip = OutPoint.Address;
                                int id = int.Parse(value.Substring(4));
                                bool isfind = false;
                                foreach (CarData car in CarList.Items)
                                {
                                    if (car.ID == id)
                                    {
                                        isfind = true;
                                        break;
                                    }
                                }
                                if (!isfind)
                                {
                                    CarData cd = new CarData(lasttarget, id);

                                    Invoke(new MethodInvoker(() =>
                                    {
                                        CarList.Items.Add(cd);
                                    }));
                                }
                            }
                        }
                    }
                    catch { }

            }).Start();
            int SearchCount=15;
            new Task(() => {
                while (!this.IsDisposed)
                {
                    try
                    {
                        if (++SearchCount >= 15)
                        {
                            SearchCount = 0;
                            BroadcastData(GetIPID);
                        }
                        if (SelectCar != null)
                            Search.Send(GetData, GetData.Length, new IPEndPoint(SelectCar.ip, 2333));

                        for (int i = 0; i < sendarraydata.Length; i++)
                        {
                            if (lastSendID != -1)
                                if (sendarrayid[lastSendID] == lastID)
                                {
                                    sendarraydata[lastSendID] = null;
                                    lastSendID = -1;
                                }

                            if (lastSendID != -1)
                            {
                                Search.Send(sendarraydata[lastSendID], sendarraydata[lastSendID].Length, new IPEndPoint(lasttarget.Address | 0xff000000, 2333));
                                break;
                            }
                            else
                            {
                                if (sendarraydata[i] != null)
                                {
                                    lastSendID = i;
                                    Search.Send(sendarraydata[lastSendID], sendarraydata[lastSendID].Length, new IPEndPoint(lasttarget.Address | 0xff000000, 2333));
                                    break;
                                }
                            }
                        }
                    }
                    catch { }
                    Thread.Sleep(100);
                }

            }).Start();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if(SelectCar!=null)
            IDText.Text = string.Format("0x{0:X8}", SelectCar.ID);
            PowerText.Text = lastBattery + "%";
            switch (lastStatus)
            {
                case CheckStatus.CheckWay:
                    StatusText.Text = "正常运行中";
                    break;
                case CheckStatus.ConfirmWay:
                    StatusText.Text = "确认路口中";
                    break;
                case CheckStatus.ReturnWay:
                    StatusText.Text = "路口导正";
                    break;
                case CheckStatus.PreTurn:
                    StatusText.Text = "转向预减速";
                    break;
                case CheckStatus.TurnWay:
                    StatusText.Text = "转向中";
                    break;
                case CheckStatus.TurnDone:
                    StatusText.Text = "转向完成导正";
                    break;
                case CheckStatus.Stop:
                    StatusText.Text = "待机";
                    break;
            }

            if(lastStatus==CheckStatus.PreTurn|| lastStatus == CheckStatus.TurnWay|| lastStatus == CheckStatus.TurnDone)
                NodePosText.Text = "节点" + lastNodeID;
            else if (lastLenght == 100||nowNodeID==lastNodeID)
                NodePosText.Text = "节点" + nowNodeID;
            else
                NodePosText.Text = "节点" + lastNodeID + " -> 节点" + nowNodeID;

            int temp = 1 << 7;
            double dy;
            double dx;
            int xpos = CarMap.Width / 2, ypos = CarMap.Height / 2;
            CarMapDraw.Clear(Color.White);
            //传感器指示
            CarMapDraw.DrawArc(new Pen(Color.Black, 10), xpos - 32, ypos - 32, 64, 64, -160, 140);
            for (int i = 0; i < MaxSensor; i++)
            {
                dy = Math.Sin((i * 15 + -150) * Math.PI / 180) * 32 + ypos;
                dx = Math.Cos((i * 15 + -150) * Math.PI / 180) * 32 + xpos;
                int x = (int)dx;
                int y = (int)dy;
                if ((SensorStats & temp) != 0)
                {
                    CarMapDraw.FillEllipse(Brushes.DarkRed, x - 4, y - 4, 7, 7);
                }
                else
                {
                    CarMapDraw.FillEllipse(Brushes.GreenYellow, x - 4, y - 4, 7, 7);
                }
                temp <<= 1;
            }
            //方向指示
            Point[] ps = new Point[4];
            dy = Math.Sin((accangle - 90 - 12) * Math.PI / 180) * 10 + ypos;
            dx = Math.Cos((accangle - 90 - 12) * Math.PI / 180) * 10 + xpos;
            ps[0].X = (int)dx;
            ps[0].Y = (int)dy;
            dy = Math.Sin((accangle - 90) * Math.PI / 180) * 13 + ypos;
            dx = Math.Cos((accangle - 90) * Math.PI / 180) * 13 + xpos;
            ps[1].X = (int)dx;
            ps[1].Y = (int)dy;
            dy = Math.Sin((accangle - 90 + 12) * Math.PI / 180) * 10 + ypos;
            dx = Math.Cos((accangle - 90 + 12) * Math.PI / 180) * 10 + xpos;
            ps[2].X = (int)dx;
            ps[2].Y = (int)dy;

            ps[3].X = xpos;
            ps[3].Y = ypos;
            CarMapDraw.FillEllipse(Brushes.Gray, xpos - 25, ypos - 25, 50, 50);
            CarMapDraw.FillEllipse(Brushes.Black, xpos - 15, ypos - 15, 30, 30);
            CarMapDraw.FillPolygon(Brushes.Wheat, ps);
            

            //路口指示
            if ((lastPathSelect & 2) != 0)
                CarMapDraw.DrawArc(new Pen(Color.GreenYellow, 4), xpos - 20, ypos - 20, 40, 40, -140, 30);
            else
                CarMapDraw.DrawArc(new Pen(Color.DarkRed, 4), xpos - 20, ypos - 20, 40, 40, -140, 30);

            if ((lastPathSelect & 1) != 0)
                CarMapDraw.DrawArc(new Pen(Color.GreenYellow, 4), xpos - 20, ypos - 20, 40, 40, -105, 30);
            else
                CarMapDraw.DrawArc(new Pen(Color.DarkRed, 4), xpos - 20, ypos - 20, 40, 40, -105, 30);

            if ((lastPathSelect & 4) != 0)
                CarMapDraw.DrawArc(new Pen(Color.GreenYellow, 4), xpos - 20, ypos - 20, 40, 40, -70, 30);
            else
                CarMapDraw.DrawArc(new Pen(Color.DarkRed, 4), xpos - 20, ypos - 20, 40, 40, -70, 30);

            //车轮
            CarMapDraw.FillRectangle(Brushes.Black, xpos - 28, ypos - 8, 7, 16);
            CarMapDraw.FillRectangle(Brushes.Black, xpos + 28-7, ypos - 8, 7, 16);
            for(int i=0;i<6;i++)
            {
                CarMapDraw.DrawLine(Pens.Gray, xpos - 26, ypos - 8 + i * 3, xpos - 23, ypos - 8 + i * 3);
                CarMapDraw.DrawLine(Pens.Gray, xpos + 23, ypos - 8 + i * 3, xpos + 26, ypos - 8 + i * 3);
            }

            CarMapDraw.FillRectangle(Brushes.Black, xpos - 19, ypos - 10, 2, 20);
            CarMapDraw.FillRectangle(Brushes.Black, xpos + 17, ypos - 10, 2, 20);
            if (LeftMotorSpeed > 0)
                CarMapDraw.DrawLine(Pens.LightGreen, xpos - 18, ypos, xpos - 18, ypos - LeftMotorSpeed / 10);
            else if (LeftMotorSpeed < 0)
                CarMapDraw.DrawLine(Pens.DarkRed, xpos - 18, ypos, xpos - 18, ypos - LeftMotorSpeed / 10);
            if (RightMotorSpeed > 0)
                CarMapDraw.DrawLine(Pens.LightGreen, xpos + 18, ypos, xpos + 18, ypos - RightMotorSpeed / 10);
            else if (RightMotorSpeed < 0)
                CarMapDraw.DrawLine(Pens.DarkRed, xpos + 18, ypos, xpos + 18, ypos - RightMotorSpeed / 10);

            LSpeed.Points.AddY(LeftMotorSpeed);
            RSpeed.Points.AddY(RightMotorSpeed);

            if (LSpeed.Points.Count >= 50)
            {
                LSpeed.Points.RemoveAt(0);
                RSpeed.Points.RemoveAt(0);
            }

            LeftWaveLenght.Text = WaveSensorL+"cm";
            RightWaveLenght.Text = WaveSensorR+"cm";

            CarInfoPicture.Image = CarPicture.Image = CarMap;

        }

        private void MoreInfoButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void BackMainButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void CarList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CarList.SelectedItem != null)
            {
                if (SelectCar == null)
                {
                    SelectCar = (CarData)CarList.SelectedItem;
                    DownloadMap_Click(null, null);
                }
                else if(SelectCar.ID!=((CarData)CarList.SelectedItem).ID)
                {
                    SelectCar = (CarData)CarList.SelectedItem;
                    DownloadMap_Click(null, null);
                }

            }
        }
        SaveFileDialog sf = new SaveFileDialog();
        private void SaveMapButton_Click(object sender, EventArgs e)
        {
            sf.Filter = "地图数据(*.map)|*.map";
            if (sf.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(sf.FileName, FileMode.OpenOrCreate);
                mapInfo.toBin(fs);
                fs.Flush();
                fs.Close();
            }
        }

        OpenFileDialog of = new OpenFileDialog();
        private int statusLR;
        private int AveSpeedL=10;
        private int AveSpeedR=10;
        private int SendLen;
        private int SendPos;
        private byte[] SendBuff;
        private bool MapSendFlag;
        private bool MapRecvFlag;
        private bool AutoFinderFlag;
        private int nowNodeID;
        private readonly int MaxSensor = 9;
        private readonly int accangle=0;
        private int RunTurnTime=30;
        private int DetectionLenghtValue=20;
        private int savelastNodeID;
        private CheckStatus lastStatus=CheckStatus.Stop;
        private int StartNodeID;
        private int TargetNodeID;

        private void LoadMapButton_Click(object sender, EventArgs e)
        {
            of.Filter = "地图数据(*.map)|*.map";
            if (of.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(of.FileName, FileMode.Open);
                mapInfo.SearchInit();
                mapInfo.setNode(mapInfo.toNode(fs)[PathType.Forward]);
                mapInfo.AutoOffset();
                fs.Close();
                SetNodePointList(mapInfo.getEndPoint());
                MapBox.Image = mapInfo.Update();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    statusLR |= 1 << (int)PathType.Forward;
                    //updataStatus = true;
                    senddata(SendCode.MotorControl, AveSpeedL, AveSpeedR, statusLR);
                    break;
                case Keys.S:
                    statusLR |=1<<(int)PathType.Back;
                    //updataStatus = true;
                    senddata(SendCode.MotorControl, AveSpeedL, AveSpeedR, statusLR);
                    break;
                case Keys.A:
                    statusLR |= 1 << (int)PathType.Left;
                    //updataStatus = true;
                    senddata(SendCode.MotorControl, AveSpeedL, AveSpeedR, statusLR);
                    break;
                case Keys.D:
                    statusLR |= 1 << (int)PathType.Right;
                    //updataStatus = true;
                    senddata(SendCode.MotorControl, AveSpeedL, AveSpeedR, statusLR);
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    statusLR &= ~(1 << (int)PathType.Forward);
                    //updataStatus = true;
                    senddata(SendCode.MotorControl, AveSpeedL, AveSpeedR, statusLR);
                    break;
                case Keys.S:
                    statusLR &= ~(1 << (int)PathType.Back);
                    //updataStatus = true;
                    senddata(SendCode.MotorControl, AveSpeedL, AveSpeedR, statusLR);
                    break;
                case Keys.A:
                    statusLR &= ~(1 << (int)PathType.Left);
                    //updataStatus = true;
                    senddata(SendCode.MotorControl, AveSpeedL, AveSpeedR, statusLR);
                    break;
                case Keys.D:
                    statusLR &= ~(1 << (int)PathType.Right);
                    //updataStatus = true;
                    senddata(SendCode.MotorControl, AveSpeedL, AveSpeedR, statusLR);
                    break;
            }
        }

        private void PathFlagTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            PathFlagTree.ExpandAll();
        }

        private void PathFlagTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (PathFlagTree.SelectedNode != null)
            {
                if (PathFlagTree.SelectedNode.Tag is PathNode)
                {
                    mapInfo.setTargetPoint((PathNode)PathFlagTree.SelectedNode.Tag);
                    MapBox.Image = mapInfo.Update();
                }
            }
        }

        private void upkey_Click(object sender, EventArgs e)
        {
            senddata(SendCode.PathRun, 0);
        }

        private void leftkey_Click(object sender, EventArgs e)
        {
            senddata(SendCode.PathRun, 1);
        }

        private void rightkey_Click(object sender, EventArgs e)
        {
            senddata(SendCode.PathRun, 2);
        }

        private void downkey_Click(object sender, EventArgs e)
        {
            senddata(SendCode.PathRun, 3);
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            senddata(SendCode.Stop);
        }

        private void UploadMap_Click(object sender, EventArgs e)
        {
            if (mapInfo.isNull())
            {
                MessageBox.Show("地图数据为空");
                return;
            }
            UDProgress.Value = 0;
            MemoryStream ms = new MemoryStream(recvList);
            mapInfo.toBin(ms);
            SendLen = (int)ms.Position;
            SendPos = 0;
            SendBuff = recvList;
            SendMapPos(0);
            MapSendFlag = true;
        }

        private void DownloadMap_Click(object sender, EventArgs e)
        {
            UDProgress.Value = 0;
            MapRecvFlag = true;
            senddata(SendCode.RecvMap, 255, 255);
        }

        private void gotoNodeButton_Click(object sender, EventArgs e)
        {
            if (PathFlagTree.SelectedNode != null)
                if(PathFlagTree.SelectedNode.Tag is PathNode)
                    senddata(SendCode.SetTargetNode, ((PathNode)PathFlagTree.SelectedNode.Tag).pathID);
        }

        private void SearchMapButton_Click(object sender, EventArgs e)
        {
            
            if (AutoFinderFlag)
            {
                senddata(SendCode.AutoFindPath, 0);
                SearchMapButton.BackColor = Color.DarkRed;
            }
            else
            {
                if (!mapInfo.isNull())
                {
                    if (MessageBox.Show("重新搜索会清除现有地图，是否继续", "警告", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                        return;
                }
                mapInfo.SearchInit();
                MapBox.Image = mapInfo.Update();
                senddata(SendCode.AutoFindPath, 1);
                SearchMapButton.BackColor = Color.LightGreen;
            }
            AutoFinderFlag = !AutoFinderFlag;
        }

        private void ResetScale(object sender, MouseEventArgs e)
        {
            if (mapInfo.isNull()) return;
            if (e.Button == MouseButtons.Middle)
                mapInfo.DrawScale = 1;
            MapBox.Image = mapInfo.Update();
        }

        private void PIDUpdate(object sender, EventArgs e)
        {
            Pvalue.Text = trackP.Value.ToString();
            Ivalue.Text = trackI.Value.ToString();
            Dvalue.Text = trackD.Value.ToString();
            senddata(SendCode.PID, trackP.Value, trackI.Value, trackD.Value);
        }

        private void SpeedSet(object sender, EventArgs e)
        {
            AveSpeedL = LeftSpeed.Value;
            AveSpeedR = RightSpeed.Value;
            Lspeedtex.Text = AveSpeedL + "";
            Rspeedtex.Text = AveSpeedR + "";
        }

        private void DetectionLenght_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int len = int.Parse(DetectionLenght.Text);
                DetectionLenghtValue = len;
                senddata(SendCode.SetSensorInfo, DetectionLenghtValue, RunTurnTime);
            }
            catch { }
        }

        private void returntimebar_Scroll(object sender, EventArgs e)
        {
            returntimetext.Text = returntimebar.Value.ToString();
            RunTurnTime = returntimebar.Value;
            senddata(SendCode.SetSensorInfo, DetectionLenghtValue, RunTurnTime);
        }
    }
}
