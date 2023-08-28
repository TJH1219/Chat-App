using ChatApp.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ChatApp.MVVM.Reports
{
    internal class UserMessageCountReport : BaseReport
    {
        private string[] splitdata;

        public string[] SplitData
        {
            get => splitdata; set => splitdata = value;
        }

        public UserMessageCountReport(string data)
        {
            SplitData = data.Split('+');
            base.messagelist = BuildMessageTable();
            base.userlist = BuildUserTable();
        }


        private ObservableCollection<MessageModel> BuildMessageTable()
        {
            ObservableCollection<MessageModel> messages = new ObservableCollection<MessageModel>();
            string[] splitmessages = SplitData[1].Split(',');
            foreach (string line in splitmessages)
            {
                string[] attributes = line.Split(";");
                if(attributes.Length <= 1) 
                {
                    return messages;
                }
                messages.Add(new MessageModel
                {
                    Id = Convert.ToInt32(attributes[0]),
                    PosterID = Convert.ToInt32(attributes[1]),
                    Text = attributes[2],
                    TimeStamp = Convert.ToDateTime(attributes[3]),
                }) ;
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
                if( userdata.Length <= 1)
                {
                    return users;
                }
                users.Add(new UserModel
                {
                    ID = userdata[0],
                    Handle = userdata[1],
                    UserName = userdata[2]
                });
            }
            return users;
        }

        public override ObservableCollection<DataTable> MakeReport()
        {
            ObservableCollection<DataTable> displayreport = new ObservableCollection<DataTable>();
            List<string> usermessagecountdata = CountUserMessages();
            for(int i = 0; i < usermessagecountdata.Count; i += 3)
            {
                displayreport.Add(new DataTable
                {
                    Column1 = usermessagecountdata[i],
                    Column2 = usermessagecountdata[i + 1],
                    Column3 = usermessagecountdata[i + 2]
                });
            }
            return displayreport;
        }
        
        private List<string> CountUserMessages()
        {
            int count = 0;
            List<string> usercount = new List<string>();
            foreach(var user in userlist)
            {
                usercount.Add(user.ID);
                usercount.Add(user.Handle);
                count = 0;
                foreach (var message in messagelist)
                {
                    if(message.PosterID == Convert.ToInt32(user.ID))
                    {
                        count += 1;
                    }
                }
                usercount.Add(Convert.ToString(count));
            }
            return usercount;
        }
    }
}
