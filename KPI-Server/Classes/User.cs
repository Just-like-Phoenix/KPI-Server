using KPI_Server.dao;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI_Server.Classes
{
    internal class User : Person
    {
        public static User user;
        public static User SetUser(User u) => user = u;
        public static User GetUser() => user;

        List<User> userarray = new List<User>();
        public int uuid { get; set; }
        public string login { get; set; }
        public string password { get; set; }

        public User()
        {
            uuid = 0;
            login = "root";
            password = "root";
        }

        public User(int uuid, string login, string password)
        {
            this.uuid = uuid;
            this.login = login;
            this.password = password;
        }

        public User(User user)
        {
            this.uuid = user.uuid;
            this.login = user.login;
            this.password = user.password;
        }

        public List<User> GetUsersArray() => this.userarray;
        public void SetUsersArray(List<User> users) => this.userarray = users;
    }
}
