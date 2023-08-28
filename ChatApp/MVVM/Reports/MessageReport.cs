using ChatApp.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.MVVM.Reports
{
    internal class MessageReport : BaseReport
    {
        private string[] splitdata;

        public string[] SplitData
        {
            get => splitdata; set => splitdata = value;
        }

        public MessageReport(string data) 
        {
            SplitData = data.Split('+');
            messagelist = BuildMessageTable();
            userlist = BuildUserTable();
        }

        public override ObservableCollection<DataTable> MakeReport()
        {
            ObservableCollection<DataTable> data = new ObservableCollection<DataTable>();
            foreach (var message in messagelist)
            {
                foreach(var user in userlist)
                {
                    if (message.PosterID == Convert.ToInt32(user.ID))
                    {
                        data.Add(new DataTable
                        {
                            Column1 = message.Id.ToString(),
                            Column2 = user.Handle,
                            Column3 = message.Text,
                            Column4 = message.TimeStamp.ToString()
                        });
                    }
                }
            }
            return data;
        }

        private ObservableCollection<MessageModel> BuildMessageTable()
        {
            ObservableCollection<MessageModel> messages = new ObservableCollection<MessageModel>();
            string[] splitmessages = SplitData[1].Split(',');
            foreach (string line in splitmessages)
            {
                string[] attributes = line.Split(";");
                if (attributes.Length <= 1)
                {
                    return messages;
                }
                messages.Add(new MessageModel
                {
                    Id = Convert.ToInt32(attributes[0]),
                    PosterID = Convert.ToInt32(attributes[1]),
                    Text = attributes[2],
                    TimeStamp = Convert.ToDateTime(attributes[3]),
                });
            }
            return messages;
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
