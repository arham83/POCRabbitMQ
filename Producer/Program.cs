using System;
using System.Text;
using RabbitMQ.Client;
using System.Diagnostics;

// implementing Pub/Sub model 

namespace ProducerRabbitMQ
{
    public static class Producer
    {
        #region Methods: SetUpConnection, QueueDecleration and PublishMessage

        // To setup the connection with RabbitMQ Server
        public static IConnection SetUpConnection(Config conf)
        {
            ConnectionFactory factory =
                new()
                {
                    HostName = conf.HostName,
                    UserName = conf.UserName,
                    Password = conf.Password,
                    Port = Protocols.DefaultProtocol.DefaultPort
                };
            return factory.CreateConnection();
        }

        // To declare queue on the Server
        public static void ExchangeDecleration(ref IModel channel)
        {
            channel.ExchangeDeclare(
                exchange: "pubsub",
                type: ExchangeType.Fanout
            );
        }

        // Publish the message on the provided Queue
        public static void PublishMessage(string Message, IModel chn)
        {
            chn.BasicPublish("pubsub", "", null, Encoding.UTF8.GetBytes(Message));
            Console.WriteLine($"Publish Message: {Message}");
        }
        #endregion


        public static void Main()
        {
            // Reading the credentials/config params to connect to RabbitMQ Server
            Config conf = JsonFileReader.Read<Config>("./Config/config.json");
            // Printing credentials to set up the connection;
            Console.WriteLine("Setting up a connection with RabbitMQ Server Usiing following;");
            Console.WriteLine($"HostName: {conf.HostName}");
            Console.WriteLine($"UserName: {conf.UserName}");
            Console.WriteLine($"Password: {conf.Password}");

            var channel = SetUpConnection(conf).CreateModel();

            ExchangeDecleration(ref channel);

            var message = "Sample message 1";
            PublishMessage(message, channel);
        }
    }
}
