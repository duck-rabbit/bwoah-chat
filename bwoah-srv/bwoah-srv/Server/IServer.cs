using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace bwoah_srv.Server
{
    interface IServer
    {
        void StartServer();
        void SendData(byte[] byteData, Socket socket);
    }
}
