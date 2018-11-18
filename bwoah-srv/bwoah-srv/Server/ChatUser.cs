using bwoah_shared.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bwoah_srv.Server
{
    class ChatUser
    {
        public Socket UserSocket { get; private set; }

        public String _nickname = String.Empty;
        ManualResetEvent _sendUserResetEvent = new ManualResetEvent(false);

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

            //Console.WriteLine("[System] User connected from {0}.", userSocket.RemoteEndPoint.ToString());
        }

        public void GenerateByeMessage()
        {
            Console.WriteLine("[System] {0} left the chat", Nickname);
            //Console.WriteLine("[System] User disconnected from {0}.", UserSocket.RemoteEndPoint.ToString());
        }

        public void SendData(AData data)
        {
            byte[] byteData = data.ParseToByte();

            SendData(byteData);
        }

        public void SendData(byte[] byteData)
        {
            //Console.WriteLine("FakeSend");

            //try
            //{
            //    UserSocket.Send(byteData);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("[Exception] {0}", e.ToString());
            //}

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
