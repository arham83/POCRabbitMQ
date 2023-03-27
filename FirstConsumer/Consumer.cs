using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Use the temporary queue and will exist as long as
// the consumer exists, RabbitMQ will decide the name
// of the on their on

namespace ConsumerRabbitMQ
{
    public class Consumer
    {
        #region Methods: SetUpConnection, QueueDecleration
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

        // We are re-declaring the exchange, to make sure that the exchange must exist
        // just in case if the consumer is started prior to the publisher
        public void ExchangeDecleration(ref IModel channel, string exchangeType, string exchangeName)
        {
            channel.ExchangeDeclare(exchange: exchangeName, type: exchangeType);
        }
        #endregion

        // RebbitMQ declaring the random queue for the consumer
        public  string QueueDecleration(ref IModel channel)
        {
            return channel.QueueDeclare().QueueName;
        }

        public void Binding(ref IModel channel, string QueueName,string exchangeName, string routingKey)
        {
            channel.QueueBind(
                queue: QueueName,
                exchange: exchangeName,
                routingKey: routingKey
            );
        }

        // Receiving the in-comming messages
        public void MessageConsumer(IModel chn, string QueueName)
        {
            var consumer = new EventingBasicConsumer(chn);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"ConsumerOne - Recieved new message: {message}");
            };

            chn.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Consuming");
        }
    }
}
