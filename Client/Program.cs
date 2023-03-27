using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Use the temporary queue and will exist as long as
// the consumer exists, RabbitMQ will decide the name
// of the on their on

namespace ClientRabbitMQ
{
    public static class Ececutor
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
            Console.WriteLine("Starting Client......");

            var client = new Client(conf, "request-queue");

            client.MessageConsumer();

            client.PublishMessage("if message received, kindly Reply");

            Console.ReadKey();
        }
    }
}
