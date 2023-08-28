using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using ChatApp.Net.IO;
using MySqlX.XDevAPI;
using ChatApp.IO;
using System.Windows.Markup;
using ChatApp.MVVM.Core;

namespace ChatApp.Net
{
    internal class Server
    {
        public TcpClient _client;
        private PacketReader reader;
        public event Action connectedEvent;
        public event Action MSGReceivedEvent;
        public event Action UserDisconnectEvent;
        public event Action LoginEvent;
        public event Action RecievedAllMessages;
        public event Action RecievedUserMessageCount;
        public event Action RecievedModifiedMessages;

        public Server()
        {
            _client = new TcpClient();
        }

        public PacketReader Reader 
        {
            get => reader; set => reader = value;
        }

        public void ConnectToServer(string handle)
        {
            try
            {
                if (!_client.Connected)
                {
                    _client.Connect(Settings.IPAddress, Settings.Port);
                    Reader = new PacketReader(_client.GetStream());

                    if (!string.IsNullOrEmpty(handle))
                    {
                        var connectpacket = new PacketBuilder();
                        connectpacket.WriteOPCode(0);
                        connectpacket.WriteString(handle);
                        _client.Client.Send(connectpacket.GetPacketBytes());
                    }
                    ReadPackets();

                }
            }
            catch { }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        var opcode = Reader.ReadByte();
                        switch (opcode)
                        {
                            case 1:
                                connectedEvent?.Invoke();
                                break;

                            case 5:
                                MSGReceivedEvent?.Invoke();
                                break;

                            case 6:
                                LoginEvent?.Invoke();
                                break;

                            case 7:
                                RecievedAllMessages?.Invoke();
                                break;

                            case 10:
                                UserDisconnectEvent?.Invoke();
                                break;

                            case 20:
                                RecievedUserMessageCount?.Invoke();
                                break;
                            case 25:
                                RecievedModifiedMessages?.Invoke();
                                break;
                            default:
                                Console.WriteLine("ah yes..");
                                break;
                        }
                    }
                }
                catch
                {
                }
            });
        }

        public void SendMSGtoServer(string msg)
        {
            var msgpacket = new PacketBuilder();
            msgpacket.WriteOPCode(5);
            msgpacket.WriteString(msg);
            _client.Client.Send(msgpacket.GetPacketBytes());
        }

        public void SendLoginRequest(string data)
        {
            var datapacket = new PacketBuilder();
            datapacket.WriteOPCode(6);
            datapacket.WriteString(data);
            _client.Client.Send(datapacket.GetPacketBytes());
        }

        public void SendAllMessagesRequest(string data)
        {
            var datapacket = new PacketBuilder();
            datapacket.WriteOPCode(7);
            datapacket.WriteString(data);
            _client.Client.Send(datapacket.GetPacketBytes());
        }

        public void RequestUserMessageCount()
        {
            var datapacket = new PacketBuilder();
            datapacket.WriteOPCode(20);
            _client.Client.Send(datapacket.GetPacketBytes());
        }
    }
}
