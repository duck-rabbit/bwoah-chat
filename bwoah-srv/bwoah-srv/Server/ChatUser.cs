using bwoah_shared.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace bwoah_srv.Server
{
    class ChatUser
    {
        public Socket UserSocket { get; private set; }

        private String _nickname = String.Empty;
        public String Nickname
        {
            get { return _nickname; }
            set
            {
                if (_nickname.Equals(String.Empty))
                {
                    Console.WriteLine("[System] {0} joined the chat", value);
                }
                else
                {
                    Console.WriteLine("[System] {0} changed their nickname to {1}", _nickname, value);
                }

                _nickname = value;

            }
        }

        public ChatUser(Socket userSocket)
        {
            UserSocket = userSocket;

            Console.WriteLine("[System] User connected from {0}.", userSocket.RemoteEndPoint.ToString());
        }

        public void GenerateByeMessage()
        {
            Console.WriteLine("[System] {0} left the chat", Nickname);
            Console.WriteLine("[System] User disconnected from {0}.", UserSocket.RemoteEndPoint.ToString());
        }

        public void SendData(IData data)
        {
            byte[] byteData = data.ParseToByte();

            SendData(byteData);
        }

        public void SendData(byte[] byteData)
        {
            UserSocket.BeginSend(byteData, 0, byteData.Length, (SocketFlags)ChatServer.SOCKET_FLAGS, new AsyncCallback(SendDataCallback), UserSocket);
        }

        public void SendDataCallback(IAsyncResult connectionResult)
        {
            try
            {
                UserSocket.EndSend(connectionResult);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Exception] {0}", e.ToString());
            }
        }
    }
}
