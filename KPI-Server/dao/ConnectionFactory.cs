using Google.Protobuf.WellKnownTypes;
using KPI_Server.Classes;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KPI_Server.dao
{
    public class ConnectionFactory
    {
        public static ConnectionFactory connectionFactory;
        public static ConnectionFactory SetConnection(ConnectionFactory con) => connectionFactory = con;
        public static ConnectionFactory GetConnection() => connectionFactory;

        MySqlConnection? sqlConnection;
        MySqlCommand? sqlCommand;

        public void Connect()
        {
            try
            {
                sqlConnection = new MySqlConnection("server=localhost; uid=root; pwd=852654856; database=kpi");
                sqlConnection.Open();
                Console.WriteLine(" >> " + "Database connected");
                sqlCommand = sqlConnection.CreateCommand();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" >> " + "DATABASE EROR" + e);
            }
        }

        public void GetDataToLogIn() 
        {
            sqlCommand.CommandText = "SELECT * FROM user RIGHT JOIN employee ON user.uuid = employee.uuid;";
            MySqlDataReader reader = sqlCommand.ExecuteReader();

            List<User> users = new List<User>();
            User user = new User();
            User.SetUser(user);

            while (reader.Read())
            {
                user.uuid = (int)reader["uuid"];
                user.login = (string)reader["login"];
                user.password = (string)reader["password"];
                user.position = (string)reader["position"];
                users.Add(user);
            }
            user.SetUsersArray(users);
            reader.Close();
        }

        public List<string> SelectPersons() 
        { 
            List<string> persons = new List<string>();

            sqlCommand.CommandText = "SELECT * FROM person;";
            MySqlDataReader reader = sqlCommand.ExecuteReader();

            string tmp;

            while (reader.Read())
            {
                tmp = reader["upid"] + "&" + reader["uuid"] + "&" + reader["name"] + "&" + reader["surname"] + "&" + reader["patronymic"] + "&" + reader["email"] + "&" + reader["telephone"];
                persons.Add(tmp);
            }
            reader.Close();

            return persons;
        }

        public bool AddWorkerUserPerson(string login, string password, string name, string surname, string patronymic, string email, string telephone, string position, string salary) 
        {
            int uuid, upid, ueid;

            sqlCommand.CommandText = "INSERT INTO user (`login`, `password`) VALUES (@login, @password);";
            sqlCommand.Parameters.AddWithValue("@login", login);
            sqlCommand.Parameters.AddWithValue("@password", password);
            sqlCommand.ExecuteNonQuery();

            sqlCommand.CommandText = "select * from user where uuid=(SELECT LAST_INSERT_ID());";
            MySqlDataReader userreader = sqlCommand.ExecuteReader();
            userreader.Read();
            uuid = (int)userreader["uuid"];
            userreader.Close();

            sqlCommand.CommandText = "INSERT INTO person (`uuid`, `name`, `surname`, `patronymic`, `email`, `telephone`) VALUES (@uuid, @name, @surname, @patronymic, @email, @telephone);";
            sqlCommand.Parameters.AddWithValue("@uuid", uuid);
            sqlCommand.Parameters.AddWithValue("@name", name);
            sqlCommand.Parameters.AddWithValue("@surname", surname);
            sqlCommand.Parameters.AddWithValue("@patronymic", patronymic);
            sqlCommand.Parameters.AddWithValue("@email", email);
            sqlCommand.Parameters.AddWithValue("@telephone", telephone);
            sqlCommand.ExecuteNonQuery();

            sqlCommand.CommandText = "select * from person where upid=(SELECT LAST_INSERT_ID());";
            MySqlDataReader personreader = sqlCommand.ExecuteReader();
            personreader.Read();
            upid = (int)personreader["upid"];
            personreader.Close();

            sqlCommand.CommandText = "INSERT INTO `kpi`.`employee` (`uuid`, `upid`, `position`, `salary`) VALUES (@uuid, @upid, @position, @salary);";
            sqlCommand.Parameters.AddWithValue("@upid", upid);
            sqlCommand.Parameters.AddWithValue("@position", position);
            sqlCommand.Parameters.AddWithValue("@salary", salary);
            sqlCommand.ExecuteNonQuery();
        
            return true;
        }
    }
}
