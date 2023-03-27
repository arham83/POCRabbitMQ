using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProducerRabbitMQ
{
    public static class Miscellaneous
    {
        public static float CheckSize(string message)
        {
            return message.Length * sizeof(Char);
        }
    }
}
