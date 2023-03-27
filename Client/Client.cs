using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// Use the temporary queue and will exist as long as
// the consumer exists, RabbitMQ will decide the name
// of the on their on

namespace ClientRabbitMQ
{
    public class Client
    {
        private readonly IConnection _conn;
        private readonly IModel _chn;
        private readonly string _replyQueueName;
        private readonly string _reqQueueName;

        public Client(Config conf, string requestQueue)
        {
            _conn = SetUpConnection(conf);
            _chn = _conn.CreateModel();
            _replyQueueName = ReplyQueueDecleration();
            RequestQueueDecleration(requestQueue);
            _reqQueueName =requestQueue;
        }

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
        #endregion

        // RebbitMQ declaring the random queue for the consumer
        public string ReplyQueueDecleration()
        {
            return _chn.QueueDeclare(queue: "", exclusive: true).QueueName;
        }
        public void RequestQueueDecleration(string requestQueue)
        {
            _chn.QueueDeclare(queue: requestQueue, exclusive: false);
        }

        // Receiving the in-comming messages
        public void MessageConsumer()
        {
            var consumer = new EventingBasicConsumer(_chn);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Reply received: {message}");
            };

            _chn.BasicConsume(queue: _replyQueueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Consuming");
        }

        public void PublishMessage(string Message )
        {
            var properties = _chn.CreateBasicProperties();
            properties.DeliveryMode = 2;
            properties.ReplyTo = _replyQueueName;
            properties.CorrelationId = Guid.NewGuid().ToString();

            _chn.BasicPublish("", _reqQueueName, properties, Encoding.UTF8.GetBytes(Message));
            Console.WriteLine($"sending Message: {Message} with coID {properties.CorrelationId}");
        }
    }
}
