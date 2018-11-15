using System;
using bwoah_srv.Server;

namespace bwoah_srv
{
    class Program
    {
        static void Main(string[] args)
        {
            ChatServer.Instance.StartServer();
        }
    }
}
