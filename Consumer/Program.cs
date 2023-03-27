using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// --------------------------------------------------------------------------// 
// In this project, we are going to implement the Competing Consumers Concept 
// --------------------------------------------------------------------------// 

namespace ConsumerRabbitMQ
{
    public static class Consumer
    {
        #region Methods: SetUpConnection, QueueDecleration and MessageConsumer
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
            // BasicQos (Basic Quality of Service) makes it possible to limit the 
            // number of unacknowledged messages on a channel (or connection) when 
            // consuming (aka "prefetch count").
            // global: false -> Specified to a single consumer
            channel.BasicQos(prefetchSize:0, prefetchCount:1, global: false);
        }
        #endregion

        // Receiving the in-comming messages
        public static void MessageConsumer(IModel chn, string QueueName)
        {
            var consumer = new EventingBasicConsumer(chn);
            var random =new Random();
            consumer.Received += (model, ea) =>
            {
                var processingTime = random.Next(1,6);
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Recieved message: {message} will take {processingTime} to process");
                Task.Delay(TimeSpan.FromSeconds(processingTime)).Wait();

                // Gives feedback that the message has been delivered
                chn.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            // Every message needs be be manually acknowledged
            chn.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

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

            QueueDecleration(ref channel, "sampleQueue");

            MessageConsumer(channel, "sampleQueue");

            Console.ReadKey();
        }
    }
}
