using ChatApp.MVVM.Core;
using ChatApp.MVVM.Model;
using ChatApp.MVVM.View;
using ChatApp.Net;
using ChatApp.Net.IO;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp.MVVM.ViewModel
{
    internal class Mainviewmodel
    {
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMSGCommand { get; set; }
        public RelayCommand RequestUserMessageCount { get; set; }
        private ObservableCollection<UserModel> users;
        private Server _server;
        private string handle;
        private string msg;
        private ObservableCollection<string> msglist;

        public Mainviewmodel(string handle)
        {
            Handle = handle;
            Users = new ObservableCollection<UserModel>();
            MSGList = new ObservableCollection<string>();
            _Server = new Server();
            _Server.ConnectToServer(Handle);
            _Server.connectedEvent += UserConnected;
            _Server.MSGReceivedEvent += MessageReceived;
            _Server.UserDisconnectEvent += RemoveUser;
            _Server.RecievedAllMessages += AllMessagesRecieved;
            _Server.RecievedUserMessageCount += RecievedUserMessageCount;
            _Server.RecievedModifiedMessages += ModifiedMessagesRecieved;
            SendMSGCommand = new RelayCommand(o => _Server.SendMSGtoServer(MSG), o => !string.IsNullOrEmpty(MSG));
            RequestUserMessageCount = new RelayCommand(o => _Server.RequestUserMessageCount());
            _Server.SendAllMessagesRequest(Handle);
        }

        public ObservableCollection<string> MSGList
        {
            get => msglist; set => msglist = value;
        }

        public string MSG
        {
            get => msg; set => msg = value;
        }

        public ObservableCollection<UserModel> Users
        {
            get => users; set => users = value;
        }

        public string Handle
        {
            get => handle; set => handle = value;
        }

        public Server _Server
        {
            get => _server; set => _server = value;
        }

        private void RecievedUserMessageCount()
        {
            var data = _Server.Reader.ReadMSG();
            Application.Current.Dispatcher.Invoke(() => OpenReportWindow(data));      
        }

        public void SendChangesToServer(string msg)
        {
            var msgpacket = new PacketBuilder();
            msgpacket.WriteOPCode(25);
            msgpacket.WriteString(msg);
            _Server._client.Client.Send(msgpacket.GetPacketBytes());
        }

        private void OpenReportWindow(string data)
        {
            var window = new Reportwindow(data, this);
            window.Show();
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Handle = _Server.Reader.ReadMSG(),
                ID = _Server.Reader.ReadMSG(),
            };

            if (!Users.Any(x => x.ID == user.ID))
            {
                Application.Current.Dispatcher.Invoke(() => { Users.Add(user); }) ;
            }
        }

        private void AllMessagesRecieved()
        {
            var data = _server.Reader.ReadMSG();
            var splitdata = data.Split(',');
            foreach (var item in splitdata)
            {
                Application.Current.Dispatcher.Invoke(() =>  MSGList.Add(item));
            }
        }

        private void ModifiedMessagesRecieved()
        {
            Application.Current.Dispatcher.Invoke(() => MSGList.Clear());
            var data = _server.Reader.ReadMSG();
            var splitdata = data.Split(',');
            foreach (var item in splitdata)
            {
                Application.Current.Dispatcher.Invoke(() => MSGList.Add(item));
            }
        }

        private void MessageReceived()
        {
            var msg = _Server.Reader.ReadMSG();
            Application.Current.Dispatcher.Invoke(() => MSGList.Add(msg));
        }

        private void RemoveUser()
        {
            var id = _Server.Reader.ReadMSG();
            var user = Users.Where(x => x.ID == id).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }
    }
}
