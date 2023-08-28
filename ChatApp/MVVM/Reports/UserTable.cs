using ChatApp.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.MVVM.Reports
{
    internal class UserTable : BaseReport
    {
        private string[] splitdata;

        public string[] SplitData
        {
            get => splitdata; set => splitdata = value;
        }

        public UserTable(string data) 
        {
            SplitData = data.Split("+");
            userlist = BuildUserTable();
        }

        public override ObservableCollection<DataTable> MakeReport()
        {
            ObservableCollection<DataTable> Table = new ObservableCollection<DataTable>();
            foreach(var user in userlist)
            {
                Table.Add(new DataTable
                {
                    Column1 = user.ID,
                    Column2 = user.UserName,
                    Column3 = user.Handle
                });
            }
            return Table;
        } 

        private ObservableCollection<UserModel> BuildUserTable()
        {
            ObservableCollection<UserModel> users = new ObservableCollection<UserModel>();
            string[] splitusers = SplitData[0].Split(",");
            foreach (string line in splitusers)
            {
                string[] userdata = line.Split(";");
                if (userdata.Length <= 1)
                {
                    return users;
                }
                users.Add(new UserModel
                {
                    ID = userdata[0],
                    UserName = userdata[1],
                    Handle = userdata[2]
                });
            }
            return users;
        }
    }
}
