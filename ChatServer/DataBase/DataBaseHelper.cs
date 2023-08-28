
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Data.Entity.Migrations.Infrastructure;
using System.Web;

namespace ChatServer.DataBase
{
    static class DataBaseHelper
    {
        private static string connectionString = @"Data Source=C:\Users\thadd\source\repos\ChatServer\DataBase\ChatDataBase.db;Version=3;";

        private static String HashPass(String Password)
        {
            var sha = SHA256.Create();
            var asByteArray = Encoding.Default.GetBytes(Password);
            var hashedpass = sha.ComputeHash(asByteArray);
            return Convert.ToBase64String(hashedpass);
        }

        public static void InizializeDataBase()
        {
            if (!File.Exists(@"C:\Users\thadd\source\repos\ChatServer\DataBase\ChatDataBase.db"))
            {
                System.Data.SQLite.SQLiteConnection.CreateFile(@"C:\Users\thadd\source\repos\ChatServer\DataBase\ChatDataBase.db");

                using (var connection = new System.Data.SQLite.SQLiteConnection(connectionString))
                {
                    connection.Open();

                    string createUserTable = @"Create Table if not exists Users(
                                               id Integer primary key autoincrement,
                                               UserName Text not null,
                                               PassWord Text not null,
                                               Handle Text Not Null)";

                    string createMessageTable = @"Create Table if not exists messages(
                                                id integer primary key autoincrement,
                                                PosterID integer not null,
                                                Text text not null,
                                                TimeStamp DateTime not null)";


                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = createUserTable;
                        command.ExecuteNonQuery();

                        command.CommandText = createMessageTable;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void AddSampleData()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string[] usernames = { "John Doe", "Merry Whether", "Rick James" };
                string[] passwords = { "JD93", "password", "password2" };
                string[] handles = { "JD", "MW2", "RJ1337" };

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    for (int i = 0; i < usernames.Length; i++)
                    {
                        command.CommandText = @"Insert into Users (UserName, PassWord, Handle)
                                                VALUES (@Username,@Password, @Handle)"; ;
                        command.Parameters.AddWithValue("@Username", usernames[i]);
                        command.Parameters.AddWithValue("@Password", HashPass(passwords[i]));
                        command.Parameters.AddWithValue("@Handle", handles[i]);
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                }
            }
        }

        public static void InsertMessage(string MSG, int PosterID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string InsertQuery = "Insert into messages (PosterID, Text, TimeStamp)" +
                                     "Values (@PosterID, @MSG, @TimeStamp)";
                SQLiteCommand command = new SQLiteCommand(connection);
                command.CommandText = InsertQuery;
                command.Parameters.AddWithValue("@PosterID", PosterID);
                command.Parameters.AddWithValue("@MSG", MSG);
                command.Parameters.AddWithValue("@TimeStamp", DateTime.Now);
                command.ExecuteNonQuery();
            }
        }

        public static void MakeDataBaseChanges(string msg)
        {
            List<string> ChangeCommands = BuildChangeList(msg);
            foreach(string cmd in ChangeCommands)
            {
                if (cmd.Split(',')[0] == "modify")
                {
                    if(cmd.Split(',')[1] == "Users")
                    {
                        ModifyUserCommand(cmd);
                    }
                    else
                    {
                        ModifyMessageCommand(cmd);
                    }
                }
                if (cmd.Split(',')[0] == "delete")
                {
                    DeleteCommand(cmd);
                }
            }
        }

        private static List<string> BuildChangeList(string msg)
        {
            List<string> commands = new List<string>();
            foreach(string line in msg.Split(';'))
            {
                if(line.Length == 0) {break;}
                commands.Add(line);
            }
            return commands;
        }

        private static void DeleteCommand(string command)
        {
            if (command == "") { return; }
            string[] commanddata = command.Split(",");
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = $"delete from {GetTableName(commanddata[1])} where id = @ID";
                using(SQLiteCommand _Command = new SQLiteCommand(query, connection))
                {
                    _Command.Parameters.AddWithValue("@ID", commanddata[2]);
                    _Command.ExecuteNonQuery();
                }
            }
        }

        private static void ModifyUserCommand(string command)
        {
            if (command == "") { return; }
            string[] commanddata = command.Split(",");
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                if (commanddata[1] == "Users")
                {
                    string Query = $"update {commanddata[1]} set UserName=@username, Handle=@handle where id=@ID";
                    using (SQLiteCommand _Command = new SQLiteCommand(Query, connection))
                    {
                        _Command.Parameters.AddWithValue("@ID", commanddata[2]);
                        _Command.Parameters.AddWithValue("@username", commanddata[3]);
                        _Command.Parameters.AddWithValue("@handle", commanddata[4]);
                        _Command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void ModifyMessageCommand(string command)
        {
            if (command == "") { return; }
            string[] commanddata = command.Split(",");
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                if (commanddata[1] == "messages")
                {
                    string Query = $"update {commanddata[1]} set Text=@text, TimeStamp=@timestamp where id=@ID";
                    using (SQLiteCommand _Command = new SQLiteCommand(Query, connection))
                    {
                        _Command.Parameters.AddWithValue("@text", commanddata[3]);
                        _Command.Parameters.AddWithValue("@timestamp", Convert.ToDateTime(commanddata[4]));
                        _Command.Parameters.AddWithValue("@ID", commanddata[2]);
                        _Command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static string GetTableName(string table)
        {
            string tablename = "";
            if (table == "messagetable")
            {
                tablename = "messages";
            }
            else if (table == "usertable")
            {
                tablename = "Users";
            }
            return tablename;
        }

        private static string _BuildUserDataString(List<Models.Users> userlist)
        {
            string data = "";
            foreach (var user in userlist)
            {
                data += user.Id + ";";
                data += user.UserName + ";";
                data += user.Handle + ";";
                data += ",";
            }
            data += "+";
            return data;
        }

        private static string _BuildMessageDataString(List<Models.Chats> chatlist)
        {
            string data = "";
            foreach(var chat in chatlist)
            {
                data += chat.Id + ";";
                data += chat.PosterID + ";";
                data += chat.Text + ";";
                data += chat.TimeStamp + ";";
                data += ",";
            }
            return data;
        }

        public static string BuildAllDataString()
        {
            string data = "";
            List<Models.Users> userlist = GetAllUsers();
            List<Models.Chats> chatlist = GetMessages();
            data += _BuildUserDataString(userlist);
            data += _BuildMessageDataString(chatlist);
            return data;
        }

        public static Models.Users GetUser(string username)
        {
            Models.Users users = new Models.Users();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string Query = "Select * from Users where UserName = @username";
                using (SQLiteCommand command = new SQLiteCommand(Query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.UserName = reader.GetString(reader.GetOrdinal("UserName"));
                            users.Password = reader.GetString(reader.GetOrdinal("PassWord"));
                            users.Handle = reader.GetString(reader.GetOrdinal("Handle"));
                        }
                    }
                }
            }
            return users;
        }

        public static List<Models.Users> GetAllUsers()
        {
            List<Models.Users> userlist = new List<Models.Users>(0);
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string Query = "select * from Users";
                using (SQLiteCommand command = new SQLiteCommand(Query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userlist.Add(new Models.Users
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                Password = reader.GetString(reader.GetOrdinal("PassWord")),
                                Handle = reader.GetString(reader.GetOrdinal("Handle"))
                            }) ;
                        }
                    }
                }
            }
            return userlist;
        }

        public static List<Models.Chats> GetMessages()
        {
            List<Models.Chats> Messages = new List<Models.Chats>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string Query = "select * from messages";
                using (SQLiteCommand command = new SQLiteCommand(Query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Models.Chats chats = new Models.Chats
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                PosterID = reader.GetInt32(reader.GetOrdinal("PosterID")),
                                Text = reader.GetString(reader.GetOrdinal("Text")),
                                TimeStamp = reader.GetDateTime(reader.GetOrdinal("TimeStamp"))
                            };
                            Messages.Add(chats);
                        }
                    }
                }
            }
            return Messages;
        }

        public static void InsertUser(string[] data)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string Query = "insert into Users(UserName,PassWord,Handle) " +
                    "Values (@Username, @Password, @Handle)";
                SQLiteCommand command = new SQLiteCommand(Query, connection);
                
                command.Parameters.AddWithValue("@Username", data[0]);
                command.Parameters.AddWithValue("@Handle", data[1]);
                command.Parameters.AddWithValue("@Password", data[2]);
                command.ExecuteNonQuery();
            }
        }
    }
}
