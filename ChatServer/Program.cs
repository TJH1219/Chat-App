using ChatApp.MVVM.Core;
using ChatServer.DataBase;
using ChatServer.DataBase.Models;
using ChatServer.IO;
using ChatServer.Net.IO;
using System;
using System.Net;
using System.Net.Sockets;


namespace ChatServer
{
    class Program
    {
        private static TcpListener listener;
        private static List<Client> clientlist;

        public static List<Client> ClientList
        {
            get => clientlist; set => clientlist = value;
        }

        public static TcpListener Listener
        {
            get => listener; set => listener = value;
        }

        static void Main(string[] args)
        {
            ClientList = new List<Client>();
            Listener = new TcpListener(System.Net.IPAddress.Parse(Settings.IPAddress), Settings.Port);
            Listener.Start();
            //DataBaseHelper.InizializeDataBase();
            //DataBaseHelper.AddSampleData();

            while (true)
            {
                var client = new Client(Listener.AcceptTcpClient());
                ClientList.Add(client);
                BroadcastConnection();
            }
        }

        static void BroadcastConnection()
        {
            foreach (var user in ClientList)
            {
                foreach (var client in ClientList)
                {
                    var broadcastpacket = new PacketBuilder();
                    broadcastpacket.WriteOPCode(1);
                    broadcastpacket.WriteMSG(client.Handle);
                    broadcastpacket.WriteMSG(client.ID.ToString());
                    user.TCPClient.Client.Send(broadcastpacket.GetPacketBytes());
                }
            }
        }

        public static void SendAllMessages(string handle, string data)
        {
            foreach(var client in ClientList)
            {
                if(client.Handle == handle)
                {
                    var broadcastpacket = new PacketBuilder();
                    broadcastpacket.WriteOPCode(7);
                    broadcastpacket.WriteMSG(data);
                    client.TCPClient.Client.Send(broadcastpacket.GetPacketBytes());
                }
            }
        }

        public static void SendModifiedMessages(string handle, string data)
        {
            foreach (var client in ClientList)
            {
                if (client.Handle == handle)
                {
                    var broadcastpacket = new PacketBuilder();
                    broadcastpacket.WriteOPCode(25);
                    broadcastpacket.WriteMSG(data);
                    client.TCPClient.Client.Send(broadcastpacket.GetPacketBytes());
                }
            }
        }

        public static void SendUserMessageCount(string handle, string data)
        {
            foreach (var client in ClientList)
            {
                if (client.Handle == handle)
                {
                    var broadcastpacket = new PacketBuilder();
                    broadcastpacket.WriteOPCode(20);
                    broadcastpacket.WriteMSG(data);
                    client.TCPClient.Client.Send(broadcastpacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMSG(string msg)
        {
            try 
            { 
                foreach (var user in ClientList)
                {
                    var msgpacket = new PacketBuilder();
                    msgpacket.WriteOPCode(5);
                    msgpacket.WriteMSG(msg);
                    user.TCPClient.Client.Send(msgpacket.GetPacketBytes());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void BroadcastDisconnect(string id)
        {
            try
            {
                var DCuser = ClientList.Where(x => x.ID.ToString() == id).FirstOrDefault();
                ClientList.Remove(DCuser);
                foreach (var user in ClientList)
                {
                    var broadcastpacket = new PacketBuilder();
                    broadcastpacket.WriteOPCode(10);
                    broadcastpacket.WriteMSG(id);
                    user.TCPClient.Client.Send(broadcastpacket.GetPacketBytes());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
