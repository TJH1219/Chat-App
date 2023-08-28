using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.MVVM.Model
{
    internal class MessageModel
    {
        public int Id { get; set; }
        public int PosterID { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}