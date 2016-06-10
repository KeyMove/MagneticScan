using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MagneticScanUI
{
    class PipeSend
    {
        IPEndPoint endpoint;
        int sendID;
        UdpClient client;

        List<dataformat> dataformatList = new List<dataformat>();

        class dataformat
        {
            public int id;
            public object[] datalist;
            public dataformat(int id,object[] list)
            {
                this.id = id;
                datalist = list;
            }
        }
                
        void senddata(int id, params object[] list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("uartpacket(2,{");
            sb.Append(sendID);
            sb.Append(',');
            sb.Append(id);
            sb.Append(',');
            foreach (object obj in list)
            {
                sb.Append(obj.ToString());
                sb.Append(',');
            }
            sb.Append("0})");
            byte[] cmd = Encoding.Default.GetBytes(sb.ToString());
            client.Send(cmd, cmd.Length, new IPEndPoint(endpoint.Address.Address | 0xff000000, 2333));
        }

        public PipeSend(UdpClient cl, IPEndPoint ep)
        {
            client = cl;
            endpoint = ep;
        }

        public void AddSend(int id, params object[] list)
        {
            dataformatList.Add(new dataformat(id, list));
        }



    }
}
