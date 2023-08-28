using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.MVVM.Model
{
    internal class UserModel
    {
        private string handle;
        private string id;
        private string username;


        public string ID
        {
            get => id; set => id = value;
        }

        public string UserName
        {
            get => username; set => username = value;
        }

        public string Handle
        {
            get => handle; set => handle = value;
        }
    }
}
