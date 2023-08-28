using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.MVVM.Reports
{
    internal class BaseReport
    {
        public virtual ObservableCollection<MVVM.Model.UserModel> userlist { get; set; }
        public virtual ObservableCollection<MVVM.Model.MessageModel> messagelist { get; set; }

        public virtual ObservableCollection<DataTable> MakeReport()
        {
            ObservableCollection<DataTable> collection = new ObservableCollection<DataTable>();

            return collection;
        }
    }
}   
    