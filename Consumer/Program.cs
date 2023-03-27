using System;
using System.Diagnostics;
using System.Text;

namespace ConsumerRabbitMQ
{
    public static class Ececutor
    {
        public static void Main()
        {
            // Reading the credentials/config params to connect to RabbitMQ Server
            Config conf = JsonFileReader.Read<Config>("./Config/config.json");

            // Printing credentials to set up the connection;
            Console.WriteLine("Setting up a connection with RabbitMQ Server Usiing following;");
            Console.WriteLine($"HostName: {conf.HostName}");
            Console.WriteLine($"UserName: {conf.UserName}");
            Console.WriteLine($"Password: {conf.Password}");
            var consumer = new Consumer(conf, "MyDirectExchange", "sampleQueue");

            //consumer.MessageConsumer();
            consumer.CompileBatchReport();
            Console.ReadKey();
        }
    }
}
