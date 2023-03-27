using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace ClientRabbitMQ
{
    public class Config
    {
        public string? HostName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public static class JsonFileReader
    {
        public static T Read<T>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(text);
        }
    }
}