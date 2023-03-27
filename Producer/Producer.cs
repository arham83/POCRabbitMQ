using System;
using System.Text;
using RabbitMQ.Client;
using System.Diagnostics;

// implementing Pub/Sub model

namespace ProducerRabbitMQ
{
    public class Producer
    {
        #region Methods: SetUpConnection, QueueDecleration and PublishMessage

        // To setup the connection with RabbitMQ Server
        public IConnection SetUpConnection(Config conf)
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
        public void ExchangeDecleration(ref IModel channel, string exchangeType, string exchangeName)
        {
            channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);
        }

        // Publish the message on the provided Queue
        public void PublishMessage(string Message, IModel chn, string routingKey)
        {
            chn.BasicPublish("MyDirectExchange", routingKey, null, Encoding.UTF8.GetBytes(Message));
            Console.WriteLine($"Publish Message: {Message}");
        }
        #endregion
    }
}
