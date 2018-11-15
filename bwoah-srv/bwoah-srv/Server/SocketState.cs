using System;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Net;

namespace bwoah_srv.Server
{
    class SocketState
    {
        public const int BUFFER_SIZE = 1024;

        public StringBuilder StringBuilder { get; set; }
        public Socket CurrentSocket { get; set; }
        public byte[] buffer = new byte[BUFFER_SIZE];

        public SocketState(Socket socket)
        {
            CurrentSocket = socket;
            StringBuilder = new StringBuilder();
        }

        public Message AssembleMessage()
        {
            return new Message(((IPEndPoint)CurrentSocket.RemoteEndPoint).Address.ToString(), StringBuilder.ToString());
        }
    }
}
