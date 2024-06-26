﻿using KPI_Server.Classes;
using KPI_Server.dao;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KPI_Server.Connection
{
    internal class ClientObject 
    {
        protected internal string Id { get; } = Guid.NewGuid().ToString();
        protected internal StreamWriter Writer { get; }
        protected internal StreamReader Reader { get; }

        TcpClient client;
        ServerObject server; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            client = tcpClient;
            server = serverObject;
            // получаем NetworkStream для взаимодействия с сервером
            var stream = client.GetStream();
            // создаем StreamReader для чтения данных
            Reader = new StreamReader(stream);
            // создаем StreamWriter для отправки данных
            Writer = new StreamWriter(stream);
        }

        public async System.Threading.Tasks.Task ProcessAsync()
        {
            try
            {
                string sendmsg = "";
                int uuid = 0;
                string login = "";
                string password = "";

                while (true)
                {
                    string? recevmsg = await Reader.ReadLineAsync();
                    if (recevmsg == null) continue;
                    if (recevmsg.StartsWith("login"))
                    {
                        string[] recevarr = recevmsg.Split("|&|");
                        login = recevarr[1];
                        password = recevarr[2];
                        ConnectionFactory connectionFactory = ConnectionFactory.GetConnection();
                        List<User> userarray = connectionFactory.GetDataToLogIn();


                        if (login == "root" && password == "root")
                        {
                            sendmsg = "root&000000";
                        }
                        else 
                        {
                            for (int i = 0; i < userarray.Count; i++)
                            {

                                if (login == userarray[i].login && password == userarray[i].password)
                                {
                                    if (userarray[i].position == "meneger")
                                    {
                                        sendmsg = "meneger&" + userarray[i].uuid;
                                        break;
                                    }
                                    if (userarray[i].position == "worker")
                                    {
                                        sendmsg = "worker&" + userarray[i].uuid;
                                        break;
                                    }
                                    if (userarray[i].position == "NULL")
                                    {
                                        sendmsg = "error&" + userarray[i].uuid;
                                        break;
                                    }
                                }
                            }
                        }
                        
                        Writer.WriteLine(sendmsg);
                        Writer.Flush();
                        Console.WriteLine(" >> " + Id + " connected!");
                    }
                    if (recevmsg.StartsWith("add_worker"))
                    {
                        string[] recevarr = recevmsg.Split("|&|");
                        ConnectionFactory connectionFactory = ConnectionFactory.GetConnection();
                        bool tmp = connectionFactory.AddWorkerUserPerson(recevarr[1], recevarr[2], recevarr[3], recevarr[4], recevarr[5], recevarr[6], recevarr[7], recevarr[8], recevarr[9]);

                        if (tmp == true) { sendmsg = "true"; }
                        else { sendmsg = "false"; }
                        
                        await Writer.WriteLineAsync(sendmsg);
                        await Writer.FlushAsync();
                    }
                    if (recevmsg.StartsWith("add_task")) 
                    {
                        string[] recevarr = recevmsg.Split("|&|");
                        ConnectionFactory connectionFactory = ConnectionFactory.GetConnection();
                        connectionFactory.AddTask(int.Parse(recevarr[1]), int.Parse(recevarr[2]), int.Parse(recevarr[3]), recevarr[4], int.Parse(recevarr[5]), int.Parse(recevarr[6]), recevarr[7], recevarr[8]);
                    }
                    if (recevmsg.StartsWith("update_task"))
                    {
                        string[] recevarr = recevmsg.Split("|&|");
                        ConnectionFactory connectionFactory = ConnectionFactory.GetConnection();
                        connectionFactory.UpdTaskCompleted(int.Parse(recevarr[1]), int.Parse(recevarr[2]), int.Parse(recevarr[3]));
                    }
                    if (recevmsg.StartsWith("kpi"))
                    {
                        string[] recevarr = recevmsg.Split("|&|");
                        string tosend;
                        ConnectionFactory connectionFactory = ConnectionFactory.GetConnection();
                        tosend = connectionFactory.KPIByUUID(int.Parse(recevarr[1]), double.Parse(recevarr[2]));

                        Writer.WriteLine(tosend);
                        Writer.Flush();
                    }
                    if (recevmsg.StartsWith("select_task_by_uuid"))
                    {
                        List<string> tosend; 
                        string[] recevarr = recevmsg.Split("|&|");
                        ConnectionFactory connectionFactory = ConnectionFactory.GetConnection();
                        tosend = connectionFactory.SelectTasks(int.Parse(recevarr[1]));

                        Writer.WriteLine(tosend.Count());
                        Writer.Flush();

                        for (int i = 0; i < tosend.Count(); i++)
                        {
                            Writer.WriteLine(tosend[i]);
                            Writer.Flush();
                        }
                    }
                    if (recevmsg.StartsWith("select_worker_persons"))
                    {
                        List<string> tosend;
                        ConnectionFactory connectionFactory = ConnectionFactory.GetConnection();
                        tosend = connectionFactory.SelectWorkerPersons();

                        Writer.WriteLine(tosend.Count());
                        Writer.Flush();

                        for (int i = 0; i < tosend.Count(); i++)
                        {
                            Writer.WriteLine(tosend[i]);
                            Writer.Flush();
                        }
                    }
                    if (recevmsg.StartsWith("select_meneger_persons"))
                    {
                        List<string> tosend;
                        ConnectionFactory connectionFactory = ConnectionFactory.GetConnection();
                        tosend = connectionFactory.SelectMenegerPersons();

                        Writer.WriteLine(tosend.Count());
                        Writer.Flush();

                        for (int i = 0; i < tosend.Count(); i++)
                        {
                            Writer.WriteLine(tosend[i]);
                            Writer.Flush();
                        }
                    }
                    if (recevmsg.StartsWith("select_person_by_uuid"))
                    {
                        string tosend;
                        string[] recevarr = recevmsg.Split("|&|");
                        ConnectionFactory connectionFactory = ConnectionFactory.GetConnection();
                        tosend = connectionFactory.SelectPersonByUUID(int.Parse(recevarr[1]));
                        Writer.WriteLine(tosend);
                        Writer.Flush();
                    }
                    if (recevmsg.StartsWith("delete_person_by_uuid"))
                    {
                        string[] recevarr = recevmsg.Split("|&|");
                        ConnectionFactory connectionFactory = ConnectionFactory.GetConnection();
                        connectionFactory.DeletePersonByUUID(int.Parse(recevarr[1]));
                    }
                    if (recevmsg.StartsWith("update_by_uuid")) 
                    {
                        string[] recevarr = recevmsg.Split("|&|");
                        ConnectionFactory connectionFactory = ConnectionFactory.GetConnection();
                        connectionFactory.UpdateUserPerson(int.Parse(recevarr[1]), recevarr[2], recevarr[3], recevarr[4], recevarr[5], recevarr[6], recevarr[7]);
                    }
                    if (recevmsg.StartsWith("exit")) break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine(" >> " + Id + " disconnected!");
                server.RemoveConnection(Id);
            }
        }
        // закрытие подключения
        protected internal void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
