using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.MVVM.Reports
{
    //Represents a row in the reports ObservableCollection
    public class DataTable
    {
        #region fields
        private string column1;
        private string column2;
        private string column3;
        private string column4;
        private string column5;
        private string column6;
        #endregion

        #region properties
        public string Column1
        {
            get => column1; set => column1 = value;
        }

        public string Column2
        {
            get => column2; set => column2 = value;
        }

        public string Column3
        {
            get => column3; set => column3 = value;
        }

        public string Column4
        {
            get => column4; set => column4 = value;
        }

        public string Column5
        {
            get => column5; set => column5 = value;
        }

        public string Column6
        {
            get => column6; set => column6 = value;
        }
        #endregion


    }
}
