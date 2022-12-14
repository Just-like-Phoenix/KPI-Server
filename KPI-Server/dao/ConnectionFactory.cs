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

        public List<User> GetDataToLogIn() 
        {
            sqlCommand.CommandText = "SELECT * FROM user RIGHT JOIN employee ON user.uuid = employee.uuid;";
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            List<User> users = new List<User>();
            while (reader.Read())
            {
                User user = new User();
                user.uuid = (int)reader["uuid"];
                user.login = (string)reader["login"];
                user.password = (string)reader["password"];
                user.position = (string)reader["position"];
                users.Add(user);
            }
            reader.Close();
            return users;
        }

        public List<string> SelectWorkerPersons() 
        { 
            List<string> persons = new List<string>();

            sqlCommand.CommandText = "SELECT * FROM person RIGHT JOIN employee ON person.uuid = employee.uuid;";
            MySqlDataReader reader = sqlCommand.ExecuteReader();

            string tmp;

            while (reader.Read())
            {   
                if((string)reader["position"] == "worker")
                {
                    tmp = reader["upid"] + "&" + reader["uuid"] + "&" + reader["name"] + "&" + reader["surname"] + "&" + reader["patronymic"] + "&" + reader["email"] + "&" + reader["telephone"] + "&" + reader["salary"] + "&" + reader["award"];
                    persons.Add(tmp);
                }
            }
            reader.Close();

            return persons;
        }

        public List<string> SelectMenegerPersons()
        {
            List<string> persons = new List<string>();

            sqlCommand.CommandText = "SELECT * FROM person RIGHT JOIN employee ON person.uuid = employee.uuid;";
            MySqlDataReader reader = sqlCommand.ExecuteReader();

            string tmp;

            while (reader.Read())
            {
                if ((string)reader["position"] == "meneger")
                {
                    tmp = reader["upid"] + "&" + reader["uuid"] + "&" + reader["name"] + "&" + reader["surname"] + "&" + reader["patronymic"] + "&" + reader["email"] + "&" + reader["telephone"] + "&" + reader["salary"] + "&" + reader["award"];
                    persons.Add(tmp);
                }
            }
            reader.Close();

            return persons;
        }

        public string SelectPersonByUUID(int uuid)
        {
            string tmp;
            sqlCommand.CommandText = "SELECT * FROM person INNER JOIN employee ON person.uuid = employee.uuid and person.uuid = '" + uuid + "';";
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            reader.Read();
            tmp = reader["upid"] + "&" + reader["uuid"] + "&" + reader["name"] + "&" + reader["surname"] + "&" + reader["patronymic"] + "&" + reader["salary"] + "&" + reader["award"] + "&" + reader["email"] + "&" + reader["telephone"];
            reader.Close();
            return tmp;
        }

        public void AddTask() 
        {
        
        }

        public string SelectTasksByUUID(int uuid)
        {
            string tmp;
            sqlCommand.CommandText = "SELECT * FROM task WHERE uuid = '" + uuid + "';";
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            reader.Read();

            tmp = reader["utid"] + "&" + reader["ueid"] + "&" + reader["uuid"] + "&" + reader["upid"] + "&" + reader["task_text"] + "&" + reader["task_count_to_do"] + "&" + reader["task_count_of_completed"] + "&" + reader["task_start_date"] + "&" + reader["task_end_date"];
            reader.Close();
            return tmp;
        }

        public void DeletePersonByUUID(int uuid)
        {
            sqlCommand.CommandText = "DELETE FROM user WHERE (uuid = '" + uuid + "');";
            sqlCommand.ExecuteNonQuery();
        }

        public bool AddWorkerUserPerson(string login, string password, string name, string surname, string patronymic, string email, string telephone, string position, string salary) 
        {
            int uuid, upid, ueid;

            sqlCommand.CommandText = "INSERT INTO user (`login`, `password`) VALUES ('" + login + "', '" + password + "');";
            sqlCommand.ExecuteNonQuery();

            sqlCommand.CommandText = "select * from user where uuid=(SELECT LAST_INSERT_ID());";
            MySqlDataReader userreader = sqlCommand.ExecuteReader();
            userreader.Read();
            uuid = (int)userreader["uuid"];
            userreader.Close();

            sqlCommand.CommandText = "INSERT INTO person (`uuid`, `name`, `surname`, `patronymic`, `email`, `telephone`) VALUES ('" + uuid + "', '" + name + "', '" + surname + "', '" + patronymic + "', '" + email + "', '" + telephone + "');";
            sqlCommand.ExecuteNonQuery();

            sqlCommand.CommandText = "select * from person where upid=(SELECT LAST_INSERT_ID());";
            MySqlDataReader personreader = sqlCommand.ExecuteReader();
            personreader.Read();
            upid = (int)personreader["upid"];
            personreader.Close();

            sqlCommand.CommandText = "INSERT INTO `kpi`.`employee` (`uuid`, `upid`, `position`, `salary`) VALUES ('" + uuid + "', '" + upid + "', '" + position + "', '" + salary + "');";
            sqlCommand.ExecuteNonQuery();
        
            return true;
        }

        public void UpdateUserPerson(int uuid, string name, string surname, string patronymic, string email, string telephone, string salary)
        {
            sqlCommand.CommandText = "UPDATE person SET name = '" + name + "', surname = '" + surname + "', patronymic = '" + patronymic + "', email = '" + email + "', telephone = '" + telephone + "' WHERE (uuid = '" + uuid + "');";
            sqlCommand.ExecuteNonQuery();

            sqlCommand.CommandText = "UPDATE employee SET salary = '" + salary + "' WHERE (uuid = '" + uuid + "');";
            sqlCommand.ExecuteNonQuery();
        }

    }
}
