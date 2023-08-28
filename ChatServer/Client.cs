using ChatServer.DataBase;
using ChatServer.IO;
using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    internal class Client
    {
        private string handle;
        private Guid id;
        private TcpClient tcpClient;
        private PacketReader packetReader;



        public Client(TcpClient Client)
        {
            TCPClient = Client;

            ID = Guid.NewGuid();
            PacketReader = new PacketReader(TCPClient.GetStream());

            var opcode = PacketReader.ReadByte();
            Handle = PacketReader.ReadMSG();

            if (opcode == 6)
            {
                string[] data = Handle.Split(',');
                DataBase.Models.Users user = DataBaseHelper.GetUser(data[0]);
                if(user.Password == data[1])
                {
                    Console.WriteLine($"[{DateTime.Now}]: Client {data[0]} has Joined");
                    PacketBuilder packet = new PacketBuilder();
                    packet.WriteOPCode(6);
                    packet.WriteMSG($"Accepted,{user.Handle}");
                    Client.Client.Send(packet.GetPacketBytes());
                }
                else
                {
                    PacketBuilder packet = new PacketBuilder();
                    packet.WriteOPCode(6);
                    packet.WriteMSG("Denied");
                    Client.Client.Send(packet.GetPacketBytes());
                }
            }
            else if(opcode == 11)
            {
                string[] data = Handle.Split(',');
                bool userexists = CheckIfUserExists(data);
                if (!userexists)
                {
                    DataBaseHelper.InsertUser(data);
                    PacketBuilder packet = new PacketBuilder();
                    packet.WriteOPCode(11);
                    packet.WriteMSG("New User Created");
                    Client.Client.Send(packet.GetPacketBytes());
                }
            }

            Task.Run(() => Process());
        }


        public TcpClient TCPClient
        {
            get => tcpClient; set => tcpClient = value;
        }

        public Guid ID
        {
            get => id; set => id = value;
        }

        public PacketReader PacketReader
        {
            get => packetReader; set => packetReader = value;   
        }

        public string Handle
        {
            get => handle; set => handle = value;
        }

        private int GetUserID(string handle)
        {
            List<DataBase.Models.Users> listusers = DataBaseHelper.GetAllUsers();
            int id = -1;
            foreach (var user in listusers)
            {
                if (user.Handle == Handle)
                {
                    id = user.Id;
                }
            }
            return id;
        }

        private bool CheckIfUserExists(string[] data)
        {
            bool check = false;
            List<DataBase.Models.Users> userlist = DataBaseHelper.GetAllUsers();
            foreach (var user in userlist)
            {
                if (user.UserName == data[0])
                {
                    check = true;
                }
            }
            return check;
        }

        private string GetUserHandle(int id)
        {
            string name = "";
            List<DataBase.Models.Users> listusers = DataBaseHelper.GetAllUsers();
            foreach(var user in listusers)
            {
                if (user.Id == id)
                {
                    name = user.Handle;
                }
            }
            return name;
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = PacketReader.ReadMSG();
                            int userid = GetUserID(Handle);
                            DataBase.Models.Chats chat = new DataBase.Models.Chats
                            {
                                PosterID = userid,
                                Text = msg,
                                TimeStamp = DateTime.Now,
                            };
                            DataBaseHelper.InsertMessage(chat.Text, chat.PosterID);
                            Console.WriteLine($"{DateTime.Now} Message recieved {msg}");
                            Program.BroadcastMSG($"{DateTime.Now} {Handle}: {msg} ");
                            break;
                        case 7:
                            string packetdata = "";
                            string handle = PacketReader.ReadMSG();
                            List<DataBase.Models.Chats> chatlist = DataBaseHelper.GetMessages();
                            foreach (var message in chatlist)
                            {
                                packetdata += $"{message.TimeStamp} {GetUserHandle(message.PosterID)} {message.Text},";
                            }
                            Program.SendAllMessages(handle, packetdata);
                            break;
                        case 20:
                            var _data = DataBaseHelper.BuildAllDataString();
                            Program.SendUserMessageCount(Handle, _data);
                            break;
                        case 25:
                            string messagedata = "";
                            var msgs = DataBaseHelper.GetMessages();
                            var changes = PacketReader.ReadMSG();
                            DataBaseHelper.MakeDataBaseChanges(changes);
                            List<DataBase.Models.Chats> messagelist = DataBaseHelper.GetMessages();
                            foreach (var message in messagelist)
                            {
                                messagedata += $"{message.TimeStamp} {GetUserHandle(message.PosterID)} {message.Text},";
                            }
                            Program.SendModifiedMessages(Handle, messagedata);
                            break;
                        default:
                            break;
                    }
                }
                catch(Exception)
                {
                    Console.WriteLine($"{ID.ToString()} has disconneted");
                    Program.BroadcastDisconnect(ID.ToString());
                    TCPClient.Close();
                    break;
                }
            }
        }
    }
}
