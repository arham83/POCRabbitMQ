using System.Diagnostics;
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
        private readonly IConnection _conn;
        private readonly IModel _chn;
        private readonly string _queueName;

        #region Methods: SetUpConnection, QueueDecleration
        // To setup the connection with RabbitMQ Server
        public IConnection SetUpConnection(Config conf)
        {
            ConnectionFactory factory =
                new()
                {
                    HostName = conf.HostName,
                    UserName = conf.UserName,
                    Password = conf.Password
                };
            return factory.CreateConnection();
        }

        //Binding(queueName, "MyDirectExchange", "sampleQueue");
        public Consumer(Config conf, string exchangeName, string routingKey)
        {
            _conn = SetUpConnection(conf);
            _chn = _conn.CreateModel();
            _queueName = QueueDecleration();
            Binding(exchangeName, routingKey);
        }
        #endregion

        // RebbitMQ declaring the random queue for the consumer
        public string QueueDecleration()
        {
            return _chn.QueueDeclare().QueueName;
        }

        public void Binding(string exchangeName, string routingKey)
        {
            _chn.QueueBind(queue: _queueName, exchange: exchangeName, routingKey: routingKey);
        }

        // Receiving the in-comming messages
        public void MessageConsumer()
        {
            var consumer = new EventingBasicConsumer(_chn);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"ConsumerOne - Recieved new message: {message}");
            };

            _chn.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Consuming");
        }

        public void MessageBatchConsumer(int batchSize)
        {

            for (int i = 0; i < batchSize; i++)
            {
                var message = _chn.BasicGet(_queueName, false);

                if (message == null)
                {
                    break;
                }

                // Process the message here
                //Console.WriteLine($"Recieved new message: {message}");
            }
            //Console.WriteLine($"Time taken to pull {batchSize} messages: {timeTaken} ms");
        }

        public static float checkSize(string message)
        {
            return message.Length * (sizeof(Char) / 2);
        }

        public void CompileBatchReport()
        {
            Stopwatch stopwatch = new();
            var csv = new StringBuilder();
            var newLine = string.Format(
                "{0},{1},{2}",
                "No. of Messages(Nos)",
                "Times(ms)",
                "Average time per message(ms)"
            );
            csv.AppendLine(newLine);
            var i = 10;
            do
            {
                stopwatch.Start();
                MessageBatchConsumer(i);
                stopwatch.Stop();
                var time = stopwatch.ElapsedMilliseconds;
                var AvgTimeMsg = (((float)stopwatch.ElapsedMilliseconds)) / ((float)i);
                newLine = string.Format("{0},{1},{2}", i, time.ToString(), AvgTimeMsg.ToString());
                csv.AppendLine(newLine);
                i *= 10;
            } while (i <= 1000000);

            const string name = "Consumer - VolumeTestReport(Batch)- size.csv";
            File.WriteAllText("../Report/" + name, csv.ToString());
        }
    }
}
