using System;
using System.Text;
using System.Diagnostics;

// implementing Pub/Sub model

namespace ServerRabbitMQ
{
    public static class Excution
    {
        public static void Main()
        {
            // Reading the credentials/config params to connect to RabbitMQ Server
            Config conf = JsonFileReader.Read<Config>("../Config/config.json");
            // Printing credentials to set up the connection;
            Console.WriteLine("Setting up a connection with RabbitMQ Server Usiing following;");
            Console.WriteLine($"HostName: {conf.HostName}");
            Console.WriteLine($"UserName: {conf.UserName}");
            Console.WriteLine($"Password: {conf.Password}");

            var server = new Server(conf,"request-queue");
            server.MessageConsumer();
            Console.ReadKey();
        }
    }
}
