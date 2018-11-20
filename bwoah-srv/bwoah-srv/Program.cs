using System;
using bwoah_srv.Server;

namespace bwoah_srv
{
    class Program
    {
        static void Main(string[] args)
        {
            ChatServer chatServer = ChatServer.Instance;
            Chat chat = new Chat(chatServer);

            chatServer.StartServer();

            Console.ReadKey();
        }
    }
}
