using Google.Protobuf.WellKnownTypes;
using KPI_Server.Classes;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


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
                if ((string)reader["position"] == "worker")
                {
                    tmp = reader["upid"] + "&" + reader["uuid"] + "&" + reader["ueid"] + "&" + reader["name"] + "&" + reader["surname"] + "&" + reader["patronymic"] + "&" + reader["email"] + "&" + reader["telephone"] + "&" + reader["salary"] + "&" + reader["award"];
                    persons.Add(tmp);
                }
            }
            reader.Close();

            return persons;
        }

        public List<string> SelectTasks(int uuid)
        {
            List<string> tasks = new List<string>();

            sqlCommand.CommandText = "SELECT * FROM task WHERE task.uuid = " + uuid;
            MySqlDataReader reader = sqlCommand.ExecuteReader();

            string tmp;

            while (reader.Read())
            {
                DateTime start_date = DateTime.Parse(reader["task_start_date"].ToString());
                DateTime end_date = DateTime.Parse(reader["task_end_date"].ToString());
                tmp = reader["utid"] + "&" + reader["ueid"] + "&" + reader["uuid"] + "&" + reader["upid"] + "&" + reader["task_text"] + "&" + reader["task_count_to_do"] + "&" + start_date.ToShortDateString() + "&" + end_date.ToShortDateString() + "&" + reader["task_count_of_completed"];
                tasks.Add(tmp);
            }
            reader.Close();

            return tasks;
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
                    tmp = reader["upid"] + "&" + reader["uuid"] + "&" + reader["ueid"] + "&" + reader["name"] + "&" + reader["surname"] + "&" + reader["patronymic"] + "&" + reader["email"] + "&" + reader["telephone"] + "&" + reader["salary"] + "&" + reader["award"];
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
            tmp = reader["upid"] + "&" + reader["uuid"] + "&" + reader["ueid"] + "&" + reader["name"] + "&" + reader["surname"] + "&" + reader["patronymic"] + "&" + reader["salary"] + "&" + reader["award"] + "&" + reader["email"] + "&" + reader["telephone"];
            reader.Close();
            return tmp;
        }

        public void AddTask(int uuid, int upid, int ueid, string task_text, int task_count_to_do, int task_count_of_completed, string task_start_date, string task_end_date)
        {
            DateTime start_date = DateTime.ParseExact(task_start_date, "MM/dd/yyyy", null);
            sqlCommand.CommandText = " INSERT INTO task (ueid, uuid, upid, task_text, task_count_to_do, task_start_date, task_end_date, task_count_of_completed) VALUES ('" + ueid + "', '" + uuid + "', '" + upid + "', '" + task_text + "', '" + task_count_to_do + "', '" + start_date.ToString("yyyy-MM-dd") + "', '" + start_date.AddDays(30).ToString("yyyy-MM-dd") + "', '" + task_count_of_completed + "');";
            sqlCommand.ExecuteNonQuery();
        }

        public void UpdTaskCompleted(int utid, int uuid, int compl)
        {
            sqlCommand.CommandText = "UPDATE task SET task_count_of_completed = '"+ compl +"' WHERE(`utid` = '"+ utid +"') and (`uuid` = '"+ uuid +"')";
            sqlCommand.ExecuteNonQuery();
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

        public string KPIByUUID(int uuid, double salary)
        {
            string tmp;
            double kpi = 0;
            double award, k, weight;


            sqlCommand.CommandText = "SELECT * FROM kpi.task WHERE uuid = '"+ uuid +"';";
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            List<PersonTask> tasks = new List<PersonTask>();
            while (reader.Read())
            {
                PersonTask task = new PersonTask();
                task.utid = (int)reader["utid"];
                task.ueid = (int)reader["ueid"];
                task.uuid = (int)reader["uuid"];
                task.upid = (int)reader["upid"];
                task.task_text = (string)reader["task_text"];
                task.task_count_to_do = (int)reader["task_count_to_do"];
                task.task_count_of_completed = (int)reader["task_count_of_completed"];
                tasks.Add(task);
            }
            reader.Close();
            weight = (double)1 / (double)tasks.Count();
            for (int i = 0; i < tasks.Count(); i++)
            {
                k = (double)tasks[i].task_count_of_completed / (double)tasks[i].task_count_to_do;
                kpi += weight * k;
            }

            award = ((double)salary * (double)kpi) * (double)0.3;

            tmp = award + "&" + kpi;

            return tmp;
        }
    }
}
