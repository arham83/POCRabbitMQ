using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;

namespace ProducerRabbitMQ
{
    public class Miscellaneous
    {
        public static float checkSize(string message)
        {
            return message.Length * (sizeof(Char) / 2);
        }
    }

    public class SampleMessages
    {
        public string Message1 { get; set; }
        public string Message2 { get; set; }
        public string Message3 { get; set; }
        public string Message4 { get; set; }
        public string Message5 { get; set; }

        public SampleMessages()
        {
            Message1 = File.ReadAllText("../SampleMessages/sample5.json");
            Message2 = File.ReadAllText("../SampleMessages/sample6.json");
            Message3 = File.ReadAllText("../SampleMessages/sample7.json");
            Message4 = File.ReadAllText("../SampleMessages/sample8.json");
            Message5 = File.ReadAllText("../SampleMessages/sample9.json");
        }
    }

    public class ReportCompiler
    {
        public static void CompileReport(List<string> rows, string path)
        {
            var csv = new StringBuilder();
            foreach (var row in rows)
            {
                csv.AppendLine(row);
            }
            File.WriteAllText(path, csv.ToString());
        }
    }
}
