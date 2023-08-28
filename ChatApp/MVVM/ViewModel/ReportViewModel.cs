using ChatApp.MVVM.Core;
using ChatApp.MVVM.Reports;
using ChatApp.MVVM.View;
using Google.Protobuf;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Asn1.Sec;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Shapes;

namespace ChatApp.MVVM.ViewModel
{
    internal enum ReportType
    {
        UserMessageCountReport,
        MessageReport,
        MessageTable,
        UserTable
    }

    internal class ReportViewModel
    {
        private BaseReport report;
        private ObservableCollection<DataTable> datatable;
        private string data;
        private ReportType reporttype;
        private string search;
        private List<string> changecommands;
        private ObservableCollection<DataTable> backuptable;
        private int selectedindex;

        public int SelectedIndex
        {
            get => selectedindex; set => selectedindex = value;
        }

        public List<string> ChangeCommands
        {
            get => changecommands; set => changecommands = value;
        }

        public ObservableCollection<DataTable> BackUpTable
        {
            get => backuptable; set => backuptable = value;
        }

        public RelayCommand ModifyDataCommand { get; set; }
        public RelayCommand ChangeToMessageReport { get; set; }
        public RelayCommand ChangeToUserMessageCount { get; set; }
        public RelayCommand SearchDataTableCommand { get; set; }
        public RelayCommand ChangeToMessageTable { get; set; }
        public RelayCommand ChangeToUserTable { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand SaveChangesCommand { get; set; }

        public string Search
        {
            get => search; set => search = value;
        }

        public ReportType Reporttype
        {
            get => reporttype; set => reporttype = value;
        }

        public string Data
        {
            get => data; set => data = value;
        }

        public ObservableCollection<DataTable> DataTable
        {
            get => datatable; set => datatable = value;
        }

        public BaseReport Report
        {
            get => report; set => report = value;
        }

        public Mainviewmodel ViewModel { get; set; }

        public ReportViewModel(string data, Mainviewmodel viewmodel) 
        {
            Data = data;
            ChangeToMessageReport = new RelayCommand(o => MessageReport());
            ChangeToUserMessageCount = new RelayCommand(o => UserMessageCount());
            SearchDataTableCommand = new RelayCommand(o => SearchDataTable(Search));
            ModifyDataCommand = new RelayCommand(o => ModifyData());
            ChangeToMessageTable = new RelayCommand(o => MessageTable());
            ChangeToUserTable = new RelayCommand(o => UserTable());
            DeleteCommand = new RelayCommand(o => DeleteData());
            SaveChangesCommand = new RelayCommand(o => SaveChanges());
            Report = new UserMessageCountReport(data);
            DataTable = Report.MakeReport();
            BackUpTable = DataTable;
            Reporttype = ReportType.UserMessageCountReport;
            ChangeCommands = new List<string>();
            ViewModel = viewmodel;
        }

        private void DeleteData()
        {
            if(SelectedIndex == -1)
            {
                return;
            }

            if (reporttype == ReportType.MessageTable)
            {
                ChangeCommands.Add($"delete messages {DataTable[SelectedIndex].Column1}");
                DeletefromDataString();
                DataTable.RemoveAt(SelectedIndex);
            }
            else if (reporttype == ReportType.UserTable)
            {
                ChangeCommands.Add($"delete Users {DataTable[SelectedIndex].Column1}");
                DeleteFromUserDataString();
                DataTable.RemoveAt(SelectedIndex);
            }

        }

        private string BuildChangeCommandsString()
        {
            string commands = "";
            foreach(string command in ChangeCommands)
            {
                commands += command+";";
            }
            return commands;
        } 

        private void SaveChanges()
        {
            string commands = BuildChangeCommandsString();
            ViewModel.SendChangesToServer(commands);
        }

        private void ModifyData()
        {
            if (reporttype == ReportType.MessageTable)
            {
                ChangeCommands.Add(ModifyMesageTable());
                modifymessagedatastring();
            }
            else if (reporttype == ReportType.UserTable)
            {
                ChangeCommands.Add(ModifyUserTable());
                modifyuserdatastring();
            }
        }

        private string ModifyMesageTable()
        {
            string modifystring = "";
            modifystring += $"modify,messages,{DataTable[SelectedIndex].Column1},{DataTable[SelectedIndex].Column3},{DataTable[SelectedIndex].Column4}";
            return modifystring;
        }

        private string ModifyUserTable()
        {
            string modifystring = "";
            modifystring += $"modify,Users,{DataTable[SelectedIndex].Column1},{DataTable[SelectedIndex].Column2},{DataTable[SelectedIndex].Column3}";         
            return modifystring;
        }

        private void SearchDataTable(string search)
        {
            ObservableCollection<DataTable> data = new ObservableCollection<DataTable>();
            if (Reporttype == ReportType.UserMessageCountReport)
            {
                DataTable isFound = SearchUserMessageReportTable(search);
                if (isFound.Column1 == null)
                {
                    return;
                }
                data.Add(isFound);
                foreach (var row in DataTable)
                {
                    data.Add(row);
                }
                DataTable.Clear();
                foreach (var row in data)
                {
                    DataTable.Add(row);
                }
            }
            else if (Reporttype != ReportType.UserMessageCountReport)
            {
                DataTable isFound = SearchMessageReport(search);
                if (isFound.Column1 == null)
                {
                    return;
                }
                data.Add(isFound);
                foreach (var row in DataTable)
                {
                    data.Add(row);
                }
                DataTable.Clear();
                foreach(var row in data)
                {
                    DataTable.Add(row);
                }
            }
        }

        private DataTable SearchUserMessageReportTable(string search)
        {
            DataTable result = null;
            foreach (var row in DataTable)
            {
                if (row.Column1 == search || row.Column2 == search)
                {
                    result = row;
                    DataTable.Remove(row);
                    return result;
                }
            }
            return result;
        }

        private DataTable SearchMessageReport(string search)
        {
            DataTable result = null;
            foreach(var row in DataTable)
            {
                if (row.Column1 == search || row.Column2 == search || row.Column3 == search)
                {
                    result = row;
                    DataTable.Remove(row);
                    return result;
                }
            }
            return result;
        }

        private void UpdateDataTable(ObservableCollection<DataTable> data)
        {
            DataTable.Clear();
            foreach(var line in data)
            {
                DataTable.Add(line);
            }
        }

        //This runs when a message is deleted
        private void DeletefromDataString()
        {
            string datastring = Data;
            string[] splitdata = datastring.Split('+');
            List<string> messages = MakeMessageList();
            messages.RemoveAt(SelectedIndex);
            datastring = splitdata[0] + '+' + BuildMessageDataString(messages);
            Data = datastring;
        }

        //This runs when a user is deleted
        private void DeletefromDataString(int userid)
        {
            string datastring = Data;
            string[] splitdata = datastring.Split('+');
            List<string> messages = MakeMessageList();
            foreach(var message in messages)
            {
                if(message.Length <=1) { break; }
                if (message.Split(';')[2] == Convert.ToString(userid))
                {
                    messages.Remove(message);
                }
            }
            datastring = splitdata[0] + '+' + BuildMessageDataString(messages);
            Data = datastring;
        }

        private void DeleteFromUserDataString()
        {
            string datastring = Data;
            string[] splitdata = datastring.Split('+');
            List<string> users = MakeUserList();
            DeletefromDataString(Convert.ToInt32(users[SelectedIndex].Split(';')[0]));
            users.RemoveAt(SelectedIndex);
            datastring = BuildUserDataString(users) + '+' + splitdata[1];
            Data = datastring;
        }

        private List<string> MakeUserList()
        {
            List<string> users = new List<string>();
            string datastring = Data;
            string[] splitdata = datastring.Split('+');
            string[] userdata = splitdata[0].Split(',');
            foreach (var user in userdata)
            {
                users.Add(user);
            }
            return users;
        }

        private string BuildUserDataString(List<string> users)
        {
            string userstring = "";
            foreach (string line in users)
            {
                userstring += line + ',';
            }
            return userstring;
        }

        private string BuildMessageDataString(List<string> Messages)
        {
            string Messagestring = "";
            foreach(string line in Messages)
            {
                Messagestring += line + ',';
            }
            return Messagestring;
        }

        private List<string> MakeMessageList()
        {
            List<string> messages = new List<string>();
            string datastring = Data;
            string[] splitdata = datastring.Split('+');
            string[] messagedata = splitdata[1].Split(",");
            foreach (var _Message in messagedata)
            {
                messages.Add(_Message);
            }
            return messages;
        }

        public void MessageReport()
        {
            Report = new MessageReport(Data);
            UpdateDataTable(Report.MakeReport());
            BackUpTable = cloneTable(DataTable);
            Reporttype = ReportType.MessageReport;
        }

        public void UserMessageCount()
        {
            Report = new UserMessageCountReport(data);
            UpdateDataTable(Report.MakeReport());
            BackUpTable = cloneTable(DataTable);
            Reporttype = ReportType.UserMessageCountReport;
        }

        public void MessageTable()
        {
            Report = new MessageTable(data);
            UpdateDataTable(Report.MakeReport());
            BackUpTable = cloneTable(DataTable);
            Reporttype = ReportType.MessageTable;
        }

        public void UserTable()
        {
            Report = new UserTable(data);
            UpdateDataTable(Report.MakeReport());
            BackUpTable = cloneTable(DataTable);
            Reporttype = ReportType.UserTable;
        }

        private ObservableCollection<DataTable> cloneTable(ObservableCollection<DataTable> table)
        {
            ObservableCollection<DataTable> clonedTable = new ObservableCollection<DataTable>();
            foreach(var row in table)
            {
                clonedTable.Add(row);
            }
            return clonedTable;
        }

        //Modifies message table string
        private void modifymessagedatastring()
        {
            string datastring = Data;
            string[] splitdata = datastring.Split('+');
            DataTable row = DataTable[SelectedIndex];
            string[] messages = splitdata[1].Split(",");
            for(int i = 0; i < messages.Length; i++)
            {
                if (messages[i].Split(';')[0] == row.Column1)
                {
                    messages[i] = $"{row.Column1};{row.Column2};{row.Column3};{row.Column4}";
                }
            }
            Data = splitdata[0] + '+' + makemodifydatastring(messages);
        }

        //Modifies user table string
        private void modifyuserdatastring()
        {
            string datastring = Data;
            string[] splitdata = datastring.Split('+');
            DataTable row = DataTable[SelectedIndex];
            string[] messages = splitdata[0].Split(",");
            for (int i = 0; i < messages.Length; i++)
            {
                if (messages[i].Split(';')[0] == row.Column1)
                {
                    messages[i] = $"{row.Column1};{row.Column2};{row.Column3};";
                }
            }
            Data = makemodifydatastring(messages) + '+' + splitdata[1];
        }

        private string makemodifydatastring(string[] msgs)
        {
            string datastring = "";
            foreach(string msg in msgs)
            {
                datastring += msg + ',';
            }
            return datastring;
        }
    }
}