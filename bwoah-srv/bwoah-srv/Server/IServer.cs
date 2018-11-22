using System;
using System.Net.Sockets;

namespace bwoah_srv.Server
{
    interface IServer
    {
        Action<Socket> UserSocketException { get; set; }
        void StartServer();
        void SendData(byte[] byteData, Socket socket);
    }
}
