using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using KPI_Server.dao;
using KPI_Server.Connection;
using KPI_Server.Classes;

namespace KPI_Server
{ 
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.Connect();
            ConnectionFactory.SetConnection(connectionFactory);
            ServerObject server = new ServerObject();
            await server.ListenAsync();
        }
    }
}