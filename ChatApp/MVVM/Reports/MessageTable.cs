using ChatApp.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.MVVM.Reports
{
    internal class MessageTable : BaseReport
    {
        private string[] splitarray;

        public string[] SplitData
        {
            get => splitarray; set => splitarray = value;
        }
        public MessageTable(string data) 
        {
            SplitData = data.Split("+");
            messagelist = BuildMessageTable();
        }

        public override ObservableCollection<DataTable> MakeReport()
        {
            ObservableCollection<DataTable> DataTable = new ObservableCollection<DataTable>();
            foreach (var row in messagelist)
            {
                DataTable.Add(new Reports.DataTable
                {
                    Column1 = Convert.ToString(row.Id),
                    Column2 = Convert.ToString(row.PosterID),
                    Column3 = row.Text,
                    Column4 = Convert.ToString(row.TimeStamp)
                });
            }
            return DataTable;
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
    }
}
