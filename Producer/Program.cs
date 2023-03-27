using System;
using System.Text;
using RabbitMQ.Client;
using System.Diagnostics;

// implementing Pub/Sub model

namespace ProducerRabbitMQ
{
    public static class Excution
    {
        public static void Main()
        {
            var producer = new Producer();
            // Reading the credentials/config params to connect to RabbitMQ Server
            Config conf = JsonFileReader.Read<Config>("./Config/config.json");
            // Printing credentials to set up the connection;
            Console.WriteLine("Setting up a connection with RabbitMQ Server Usiing following;");
            Console.WriteLine($"HostName: {conf.HostName}");
            Console.WriteLine($"UserName: {conf.UserName}");
            Console.WriteLine($"Password: {conf.Password}");

            var channel = producer.SetUpConnection(conf).CreateModel();

            producer.ExchangeDecleration(ref channel, "direct", "MyDirectExchange");

            var message = "Sample message 1";
            producer.PublishMessage(message, channel, "FirstConsumerOnly");
            message = "Sample message 2";
            producer.PublishMessage(message, channel, "SecondConsumerOnly");
        }
    }
}
