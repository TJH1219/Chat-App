using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ChatServer.DataBase.Models
{
    internal class Chats
    {

        public int Id { get; set; }
        public int PosterID { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
