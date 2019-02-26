using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SynapseTools
{
    public class TdtUdp
    {
        protected static readonly byte CMD_SEND_DATA = 0x00;
        protected static readonly byte CMD_GET_VERSION = 0x01;
        protected static readonly byte CMD_SET_REMOTE_IP = 0x02;
        protected static readonly byte CMD_FORGET_REMOTE_IP = 0x03;

        protected Type dataType;
        protected string hostname;
        protected int port;
        protected UdpClient client;

        public TdtUdp(Type dataType, string hostname, int port = 22022)
        {
            if(!(dataType.Equals(typeof(float)) || dataType.Equals(typeof(int))))
            {
                throw new InvalidOperationException("Only types int and float are supported, not " + dataType.Name);
            }
            this.dataType = dataType;
            this.hostname = hostname;
            this.port = port;

            this.client = new UdpClient();
            this.client.Connect(this.hostname, this.port);

            this.Send(new byte[] { 0x55, 0xAA, CMD_SET_REMOTE_IP, 0 });

        }


        public void Send(Array data)
        {
            int dataLength = Buffer.ByteLength(data);
            byte[] message = new byte[dataLength + 4];
            message[0] = 0x55;
            message[1] = 0xAA;
            message[2] = CMD_SEND_DATA;
            message[3] = (byte)((char)data.Length);
            Buffer.BlockCopy(data, 0, message, 4, dataLength);

            this.client.Send(message, message.Length);
        }

        public Array Receive()
        {
            var endpoint = new IPEndPoint(IPAddress.Any, this.port);
            byte[] receiveBytes = this.client.Receive(ref endpoint);

            if (BitConverter.ToInt16((byte[])receiveBytes.Take(2), 0) != 0x55AA)
            {
                throw new ApplicationException("Bad header!");
            }
            receiveBytes.Skip(2).Take(receiveBytes.Length - 4);
            return null;
        }
    }
}
