using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace bwoah_srv.Server
{
    class Message
    {
        public DateTime ReceivedTime { get; private set; }
        public String User { get; private set; }
        public String Content { get; private set; }

        public Message(string user, string content)
        {
            ReceivedTime = DateTime.UtcNow;
            User = user;
            Content = content;
        }

        public override String ToString()
        {
            return String.Format("{0} {1}: {2}", ReceivedTime.ToShortTimeString(), User, Content);
        }
    }
}
