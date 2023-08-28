using ChatApp.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for Reportwindow.xaml
    /// </summary>
    /// 
    public enum ReportType
    {
        UserMessageCountReport,
        MessageReport,
        MessageTable,
        UserTable
    }

    public partial class Reportwindow : Window
    {
        private ReportType reporttype;

        public ReportType Reporttype
        {
            get => reporttype; set => reporttype = value;
        }

        internal Reportwindow(string Data, Mainviewmodel model)
        {
            InitializeComponent();
            DataContext = new ViewModel.ReportViewModel(Data, model);
            Reporttype = ReportType.UserMessageCountReport;
            grid.Columns[0].Header = "ID";
            grid.Columns[1].Header = "User";
            grid.Columns[2].Header = "Count";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Reporttype = ReportType.UserMessageCountReport;
            grid.IsReadOnly = true;
            grid.Columns[0].Header = "ID";
            grid.Columns[1].Header = "User";
            grid.Columns[2].Header = "Count";
            grid.Columns[3].Header = "";
            grid.Columns[4].Header = "";
            grid.Columns[5].Header = "";
            ModifyButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }

        private void MessageReportButton_Click(object sender, RoutedEventArgs e)
        {
            Reporttype = ReportType.MessageReport;
            grid.IsReadOnly = true;
            grid.Columns[0].Header = "ID";
            grid.Columns[1].Header = "User";
            grid.Columns[2].Header = "Text";
            grid.Columns[3].Header = "Timestamp";
            grid.Columns[4].Header = "";
            grid.Columns[5].Header = "";
            ModifyButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
        }
        // Go Back Button
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MessageTable_Click(object sender, RoutedEventArgs e)
        {
            Reporttype = ReportType.MessageTable;
            grid.Columns[0].Header = "ID";
            grid.Columns[0].IsReadOnly = true;
            grid.Columns[1].Header = "PosterID";
            grid.Columns[1].IsReadOnly = true;
            grid.Columns[2].Header = "Text";
            grid.Columns[3].Header = "TimeStamp";
            grid.Columns[4].Header = "";
            grid.Columns[5].Header = "";
            ModifyButton.IsEnabled = true;
        }

        private void UserTable_Click(object sender, RoutedEventArgs e)
        {
            Reporttype = ReportType.UserTable;
            grid.Columns[0].Header = "ID";
            grid.Columns[0].IsReadOnly = true;
            grid.Columns[1].Header = "UserName";
            grid.Columns[1].IsReadOnly = false;
            grid.Columns[2].Header = "Handle";
            grid.Columns[3].Header = "";
            grid.Columns[4].Header = "";
            grid.Columns[5].Header = "";
            ModifyButton.IsEnabled = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
