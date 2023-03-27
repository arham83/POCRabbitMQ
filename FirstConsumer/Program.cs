using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Use the temporary queue and will exist as long as
// the consumer exists, RabbitMQ will decide the name
// of the on their on

namespace ConsumerRabbitMQ
{
    public static class Ececutor
    {
        public static void Main()
        {
            var consumer =new Consumer();
            // Reading the credentials/config params to connect to RabbitMQ Server
            Config conf = JsonFileReader.Read<Config>("./Config/config.json");

            // Printing credentials to set up the connection;
            Console.WriteLine("Setting up a connection with RabbitMQ Server Usiing following;");
            Console.WriteLine($"HostName: {conf.HostName}");
            Console.WriteLine($"UserName: {conf.UserName}");
            Console.WriteLine($"Password: {conf.Password}");

            var channel = consumer.SetUpConnection(conf).CreateModel();

            consumer.ExchangeDecleration(ref channel, "direct", "MyDirectExchange");
            var queueName = consumer.QueueDecleration(ref channel);
            consumer.Binding(ref channel, queueName, "MyDirectExchange", "FirstConsumerOnly");

            consumer.MessageConsumer(channel, queueName);

            Console.ReadKey();
        }
    }
}
