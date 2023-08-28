using ChatApp.IO;
using ChatApp.MVVM.Core;
using ChatApp.Net;
using ChatApp.Net.IO;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ChatApp.MVVM.ViewModel
{
    internal class LoginViewModel : INotifyPropertyChanged
    {
        TcpClient _client;
        private string username;
        private Server _server;
        private PacketBuilder packetBuilder;
        private PacketReader packetReader;
        private string errormessage;
        private string handle;
        private string password;

        public string Handle
        {
            get => handle; set => handle = value;
        }

        public string ErrorMessage
        {
            get => errormessage; set => SetField(ref errormessage, value, "ErrorMessage");
        }

        public Server _Server
        {
            get => _server; set => _server = value;
        }

        public PacketReader PacketReader
        {
            get => packetReader; set => packetReader = value;
        }

        public PacketBuilder PacketBuilder
        {
            get => packetBuilder; set => packetBuilder = value;
        }

        public string Username
        {
            get => username; set => username = value;
        }

        public string Password
        {
            get => password; set => password = value;
        }

        public RelayCommand LoginCommand { get; set; }

        public RelayCommand RegisterCommand { get; set; }

        public LoginViewModel()
        {
            _Server = new Server();
            PacketBuilder = new PacketBuilder();
            LoginCommand = new RelayCommand(o => Login(), o => !string.IsNullOrEmpty(Username));
            RegisterCommand = new RelayCommand(o => Register(), o => !string.IsNullOrEmpty(Username));
            _Server.LoginEvent += Login;
        }
        #region NotifyPropertyChange
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        private String HashPass(String Password)
        {
            try
            {
                var sha = SHA256.Create();
                var asByteArray = Encoding.Default.GetBytes(Password);
                var hashedpass = sha.ComputeHash(asByteArray);
                return Convert.ToBase64String(hashedpass);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }
         
        public void Login()
        {
            try
            {
                var _Client = new TcpClient();
                _Client.Connect("127.0.0.1", 7891);
                var packetreader = new PacketReader(_Client.GetStream());
                string hashedpass = HashPass(Password);
                string msg = $"{Username},{hashedpass}";
                var datapacket = new PacketBuilder();
                datapacket.WriteOPCode(6);
                datapacket.WriteString(msg);
                _Client.Client.Send(datapacket.GetPacketBytes());

                var opcode = packetreader.ReadByte();
                var data = packetreader.ReadMSG();
                string[] splitdata = data.Split(',');
                if (splitdata[0] == "Accepted")
                {
                    MainWindow window = new MainWindow(splitdata[1]);
                    window.Show();
                    _Client.Close();
                    App.Current.Windows[0].Close();
                }
                else if (data == "Denied")
                {
                    ErrorMessage = "Login Failed";
                    _Client.Close();
                }
            } 
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        public void Register()
        {
            try
            {
                var _Client = new TcpClient();
                _Client.Connect("127.0.0.1", 7891);
                var packetreader = new PacketReader(_Client.GetStream());
                string hashedpass = HashPass(Password);
                string msg = $"{Username},{Handle},{hashedpass}";
                var datapacket = new PacketBuilder();
                datapacket.WriteOPCode(11);
                datapacket.WriteString(msg);
                _Client.Client.Send(datapacket.GetPacketBytes());

                var opcode = packetreader.ReadByte();
                var data = packetreader.ReadMSG();
                if (data == "New User Created")
                {
                    ErrorMessage = data;
                    _Client.Close();
                }
                else
                {
                    ErrorMessage = "Register Failed";
                    _Client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
