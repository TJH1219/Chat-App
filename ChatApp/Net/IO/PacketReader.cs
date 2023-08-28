using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.IO
{
    //Chatapp packet reader
    internal class PacketReader : BinaryReader
    {
        private NetworkStream _stream;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            Stream = ns;
        }
        public NetworkStream Stream
        {
            get => _stream; set => _stream = value;
        }

        public string ReadMSG()
        {
            byte[] msgbuffer;
            var length = ReadInt32();
            msgbuffer = new byte[length];
            Stream.Read(msgbuffer, 0, length);

            var msg = Encoding.ASCII.GetString(msgbuffer);

            return msg;
        }
    }
}
