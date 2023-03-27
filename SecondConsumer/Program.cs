using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Use the temporary queue and will exist as long as the consumer exists, RabbitMQ will decide the name of the on their on

namespace ConsumerRabbitMQ
{
    public static class Consumer
    {
        #region Methods: SetUpConnection, QueueDecleration
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

        // We are re-declaring the exchange, to make sure that the exchange must exist
        // just in case if the consumer is started prior to the publisher
        public static void ExchangeDecleration(ref IModel channel)
        {
            channel.ExchangeDeclare(exchange: "MyDirectExchange", type: ExchangeType.Direct);
        }
        #endregion

        // RebbitMQ declaring the random queue for the consumer
        public static string QueueDecleration(ref IModel channel)
        {
            return channel.QueueDeclare().QueueName;
        }

        public static void Binding(ref IModel channel,string QueueName)
        {
           channel.QueueBind(
                queue: QueueName,
                exchange: "MyDirectExchange",
                routingKey: "SecondConsumerOnly"
            );
        }

        // Receiving the in-comming messages
        public static void MessageConsumer(IModel chn, string QueueName)
        {
            var consumer = new EventingBasicConsumer(chn);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"ConsumerTwo - Recieved new message: {message}");
            };

            chn.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Consuming");
        }

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
            var queueName = QueueDecleration(ref channel);
            Binding(ref channel,queueName);

            MessageConsumer(channel, queueName);

            Console.ReadKey();
        }
    }
}
