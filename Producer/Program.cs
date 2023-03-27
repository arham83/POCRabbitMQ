using System;
using System.Text;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Threading.Tasks;

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
        public static void QueueDecleration(ref IModel channel, string QueueName)
        {
            channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        // Publish the message on the provided Queue
        public static void PublishMessage(IModel chn, string QueueName)
        {
            var random = new Random();
            var Id = 1;
            while (true)
            {
                var publishingTime = random.Next(1, 4);
                var message = $"Message with ID: {Id}";
                chn.BasicPublish("", QueueName, null, Encoding.UTF8.GetBytes(message));
                Console.WriteLine($"Publish Message: {message}");
                Task.Delay(TimeSpan.FromSeconds(publishingTime)).Wait();
            }
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

            QueueDecleration(ref channel, "sampleQueue");

            //var message = "Sample message 1";
            //PublishMessage(message, channel, "sampleQueue");
        }
    }
}
