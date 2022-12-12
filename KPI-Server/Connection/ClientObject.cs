using KPI_Server.Classes;
using KPI_Server.dao;
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
                        connectionFactory.GetDataToLogIn();
                        User user = User.GetUser();
                        List<User> userarray = user.GetUsersArray();

                        for (int i = 0; i < userarray.Count; i++)
                        {
                            if (login == "root" && password == "root")
                            {
                                sendmsg = "root";
                                break;
                            }
                            if (login == userarray[i].login && password == userarray[i].password)
                            {
                                if (userarray[i].position == "meneger")
                                {
                                    sendmsg = "meneger";
                                    break;
                                }
                                if (userarray[i].position == "worker")
                                {
                                    sendmsg = "worker";
                                    break;
                                }
                                if (userarray[i].position == "NULL")
                                {
                                    sendmsg = "error";
                                    break;
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
                    if (recevmsg.StartsWith("select_persons"))
                    {
                        List<string> tosend;
                        ConnectionFactory connectionFactory = ConnectionFactory.GetConnection();
                        tosend = connectionFactory.SelectPersons();

                        Writer.WriteLine(tosend.Count());
                        Writer.Flush();

                        for (int i = 0; i < tosend.Count(); i++)
                        {
                            Writer.WriteLine(tosend[i]);
                            Writer.Flush();
                        }
                    }
                    if (recevmsg.StartsWith("select_all"))
                    {
                        await Writer.WriteLineAsync(sendmsg);
                        await Writer.FlushAsync();
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
