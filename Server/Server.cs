using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// implementing Pub/Sub model

namespace ServerRabbitMQ
{
    public class Server
    {
        private readonly IConnection _conn;
        private readonly IModel _chn;
        private readonly string _queueName;

        public Server(Config conf, string requestQueue)
        {
            _conn = SetUpConnection(conf);
            _chn = _conn.CreateModel();
            _queueName = requestQueue;
            RequestQueueDecleration(requestQueue);
        }

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

        public void RequestQueueDecleration(string requestQueue)
        {
            _chn.QueueDeclare(queue: requestQueue, exclusive: false);
        }

        public void MessageConsumer()
        {
            var consumer = new EventingBasicConsumer(_chn);
            consumer.Received += (model, ea) =>
            {
                Console.WriteLine($"Received Request: {ea.BasicProperties.CorrelationId}");
                var replyMessage =
                    $"Message Received By Server Against Following Co-relation ID: {ea.BasicProperties.CorrelationId}";
                var body = Encoding.UTF32.GetBytes(replyMessage);
                _chn.BasicPublish("", ea.BasicProperties.ReplyTo, null, body);
            };

            _chn.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }

        #endregion
    }
}
