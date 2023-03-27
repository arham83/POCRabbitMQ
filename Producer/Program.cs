using System.Collections.Generic;
using System;
using System.Text;
using RabbitMQ.Client;
using System.Diagnostics;

namespace ProducerRabbitMQ
{
    public static class Ececutor
    {
        public static void Main()
        {
            var sm = new SampleMessages();

            // Reading the credentials/config params to connect to RabbitMQ Server
            Config conf = JsonFileReader.Read<Config>("./Config/config.json");
            // Printing credentials to set up the connection;
            Console.WriteLine("Setting up a connection with RabbitMQ Server Usiing following;");
            Console.WriteLine($"HostName: {conf.HostName}");
            Console.WriteLine($"UserName: {conf.UserName}");
            Console.WriteLine($"Password: {conf.Password}");
            var producer = new Producer(conf, "MyDirectExchange", "direct");

            producer.PerformanceCheckBySize(sm, "sampleQueue");
            producer.PerformanceCheckByVolume(sm.Message5, "sampleQueue");
            producer.CompileBatchReport(sm.Message5);
            //var message = "Sample message 1";
            //PublishMessage(message, channel, "sampleQueue");
        }
    }
}
