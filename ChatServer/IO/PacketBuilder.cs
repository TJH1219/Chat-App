using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Net.IO
{
    class PacketBuilder
    {
        private MemoryStream stream;

        public PacketBuilder()
        {
            Stream = new MemoryStream();
        }

        public MemoryStream Stream
        {
            get => stream; set => stream = value;
        }

        public void WriteOPCode(byte opcode)
        {
            try
            {
                Stream.WriteByte(opcode);
            }
            catch { }
        }

        public void WriteMSG(string msg)
        {
            try
            {
                var msglength = msg.Length;
                Stream.Write(BitConverter.GetBytes(msglength));
                Stream.Write(Encoding.ASCII.GetBytes(msg));
            }
            catch { }
        }

        public byte[] GetPacketBytes()
        {
            return stream.ToArray();
        }
    }
}
