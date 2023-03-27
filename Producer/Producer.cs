using System;
using System.Text;
using RabbitMQ.Client;
using System.Diagnostics;

// implementing Pub/Sub model

namespace ProducerRabbitMQ
{
    public class Producer
    {
        private readonly IConnection _conn;
        private readonly IModel _chn;
        private readonly string _exchangeName;

        public Producer(Config conf, string exchangeName, string exchangeType)
        {
            _conn = SetUpConnection(conf);
            _chn = _conn.CreateModel();
            _exchangeName = exchangeName;
            ExchangeDecleration(exchangeType);
            ProducerQos();
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
                    Password = conf.Password
                };
            return factory.CreateConnection();
        }

        // To declare queue on the Server
        public void ExchangeDecleration(string exchangeType)
        {
            _chn.ExchangeDeclare(exchange: _exchangeName, type: exchangeType);
        }

        // QoS is a mechanism that allows the client to control how many messages are pre-fetched by the server and how
        // many unacknowledged messages are allowed.
        // Arguments:
        // prefetchSize: This specifies the maximum size of the content body of a message that the server will deliver,
        // regardless of the number of unacknowledged messages. In this case, it is set to 0, which means that the size
        // of the message is not limited.
        // prefetchCount: This specifies the maximum number of unacknowledged messages that the server will deliver to
        // the consumer before waiting for acknowledgments. In this case, it is set to 1, which means that the server
        // will only deliver one unacknowledged message at a time.
        // global: This specifies whether the QoS settings apply to the entire connection or to a specific channel.
        // In this case, it is set to false, which means that the QoS settings apply only to this specific channel.

        public void ProducerQos()
        {
            _chn.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        // Publish the message on the provided Queue
        public void PublishMessage(string Message, string routingKey)
        {
            var properties = _chn.CreateBasicProperties();
            properties.DeliveryMode = 2;
            _chn.BasicPublish(
                "MyDirectExchange",
                routingKey,
                properties,
                Encoding.UTF8.GetBytes(Message)
            );
            //Console.WriteLine($"Publish Message: {Message}");
        }
        #endregion
        public void PublishBatch(string message, string queueName, int batchSize)
        {
            _chn.ConfirmSelect(); // enable publisher confirms
            //var batch = new List<ulong>();

            foreach (int value in Enumerable.Range(0, batchSize))
            {
                var body = Encoding.UTF8.GetBytes(message + " : " + value.ToString());
                if (body.Length > 1048576) // check if payload exceeds 1MB
                {
                    Console.WriteLine(
                        $"Message '{message}' exceeds the maximum allowed length of 1MB"
                    );
                    continue;
                }
                var properties = _chn.CreateBasicProperties();
                properties.Persistent = true;
                properties.ContentEncoding = "utf-8";
                properties.DeliveryMode = 2;
                _chn.BasicPublish("", queueName, properties, body);
                //PublishMessage(message + ": " + value.ToString(), chn, queueName);
                //batch.Add(chn.NextPublishSeqNo);
            }
            if (_chn.WaitForConfirms(TimeSpan.FromSeconds(10)))
            {
                Console.WriteLine("All messages published successfully");
            }
            else
            {
                Console.WriteLine("Some messages failed to publish");
            }
        }

        public void BulkPublisher(string routingKey, int MessageCount, string message)
        {
            foreach (int value in Enumerable.Range(0, MessageCount))
            {
                PublishMessage(message + " : " + value.ToString(), routingKey);
            }
        }

        public void PerformanceCheckByVolume(string message, string routingKey)
        {
            Stopwatch stopwatch = new();
            int i = 100;
            var csv = new StringBuilder();
            var newLine = string.Format(
                "{0},{1}",
                "The Size of Message",
                (Miscellaneous.checkSize(message) / 1000).ToString()
            );
            csv.AppendLine(newLine);
            newLine = string.Format(
                "{0},{1},{2}",
                "No. of Messages(Nos)",
                "Times(ms)",
                "Average time per message(ms)"
            );
            csv.AppendLine(newLine);
            do
            {
                stopwatch.Start();
                BulkPublisher(routingKey, i, message);
                stopwatch.Stop();
                var time = stopwatch.ElapsedMilliseconds;
                var msgPerSec = (((float)stopwatch.ElapsedMilliseconds)) / ((float)i);
                newLine = string.Format(
                    "{0},{1},{2}",
                    i,
                    (((float)time)).ToString(),
                    msgPerSec.ToString()
                );
                csv.AppendLine(newLine);
                i *= 10;
            } while (i <= 1000000);
            string name =
                "VolumeTestReport- size - "
                + (Miscellaneous.checkSize(message) / 1000).ToString()
                + ".csv";
            File.WriteAllText("../Report/" + name, csv.ToString());
        }

        private void PublishAndComputeTime(ref List<string> rows, string msg, string routingKey)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            PublishMessage(msg, routingKey);
            stopwatch.Stop();
            float size = Miscellaneous.checkSize(msg);
            var newRow = string.Format("{0},{1}", size / 1000, stopwatch.ElapsedMilliseconds);
            rows.Add(newRow);
        }

        public void PerformanceCheckBySize(SampleMessages sm, string routingKey)
        {
            var rows = new List<string>();

            var newRow = string.Format("{0},{1}", "Size(KB)", "Times(ms)");
            rows.Add(newRow);

            PublishAndComputeTime(ref rows, sm.Message1, routingKey);
            PublishAndComputeTime(ref rows, sm.Message2, routingKey);
            PublishAndComputeTime(ref rows, sm.Message3, routingKey);
            PublishAndComputeTime(ref rows, sm.Message4, routingKey);

            ReportCompiler.CompileReport(rows, @"../Report/MessageSizeBased - Report.csv");
        }

        public void CompileBatchReport(string message)
        {
            Stopwatch stopwatch = new();
            var csv = new StringBuilder();
            var newLine = string.Format(
                "{0},{1}",
                "The Size of Message",
                (Miscellaneous.checkSize(message) / 1000).ToString()
            );
            csv.AppendLine(newLine);
            newLine = string.Format(
                "{0},{1},{2}",
                "No. of Messages(Nos)",
                "Times(ms)",
                "Average time per message(ms)"
            );
            csv.AppendLine(newLine);
            var i = 10;
            do
            {
                //Console.WriteLine(i);
                stopwatch.Start();
                PublishBatch(message, "sampleQueue", i);
                stopwatch.Stop();
                var time = stopwatch.ElapsedMilliseconds;
                var AvgTimeMsg = (((float)stopwatch.ElapsedMilliseconds)) / ((float)i);
                newLine = string.Format("{0},{1},{2}", i, time.ToString(), AvgTimeMsg.ToString());
                csv.AppendLine(newLine);
                i *= 10;
            } while (i <= 1000000);

            string name =
                "Publisher - VolumeTestReport(Batch) - size - "
                + (Miscellaneous.checkSize(message) / 1000).ToString()
                + ".csv";
            File.WriteAllText("../Report/" + name, csv.ToString());
        }
    }
}
