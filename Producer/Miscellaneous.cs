using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProducerRabbitMQ
{
    public class Miscellaneous
    {
        public static float checkSize(string message)
        {
            return message.Length * sizeof(Char);
        }
    }
}
