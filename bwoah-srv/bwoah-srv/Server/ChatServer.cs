using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using bwoah_parser.Utils;
using bwoah_parser;

namespace bwoah_srv.Server
{
    /// <summary>
    /// Class responsible for managing connections
    /// </summary>
    class ChatServer : Singleton<ChatServer>
    {
        public static IPAddress INTERFACE_TO_LISTEN = IPAddress.Parse("192.168.0.59");
        public static int LOCAL_PORT = 13131;
        public static int MAX_PENDING_CONNECTIONS = 100;
        public static int SOCKET_FLAGS = 0;

        private IPEndPoint _localEndPoint;

        ConcurrentDictionary<IPEndPoint, String> _clientsNickDictionary = new ConcurrentDictionary<IPEndPoint, string>();

        ManualResetEvent _doneListening = new ManualResetEvent(false);

        /// <summary>
        /// Start chat server
        /// </summary>
        public void StartServer()
        {
            if (!IsIPCorrect())
            {
                return;
            }
            
            _localEndPoint = new IPEndPoint(INTERFACE_TO_LISTEN, LOCAL_PORT);

            Console.WriteLine(String.Format("Starting Bwoah! Server on {0}", _localEndPoint.ToString()));

            Socket socket = new Socket(_localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Bind(_localEndPoint);
                socket.Listen(MAX_PENDING_CONNECTIONS);

                while (true)
                {
                    _doneListening.Reset();

                    socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);

                    _doneListening.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected error. Closing Bwoah! server.");
                Console.WriteLine(e.ToString());
            }
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            _doneListening.Set();

            Socket listener = (Socket)asyncResult.AsyncState;
            Socket socket = listener.EndAccept(asyncResult);

            Console.WriteLine("User {0} connected from {1}.", ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(), socket.RemoteEndPoint.ToString());

            RecievedState socketState = new RecievedState(socket);
            socket.BeginReceive(socketState.buffer, 0, RecievedState.BUFFER_SIZE, 0, new AsyncCallback(ReadCallback), socketState);
        }

        private void ReadCallback(IAsyncResult asyncResult)
        {
            RecievedState state = (RecievedState)asyncResult.AsyncState;
            Socket handler = state.NetSocket;

            int dataLength = handler.EndReceive(asyncResult); 

            if (dataLength > 0)
            {
                state.HandleData();

                state = new RecievedState(handler);
 
                handler.BeginReceive(state.buffer, 0, RecievedState.BUFFER_SIZE, 0, new AsyncCallback(ReadCallback), state);
            }
            else
            {
                Console.WriteLine("User {0} disconnected from {1}.", ((IPEndPoint)handler.RemoteEndPoint).Address.ToString(), handler.RemoteEndPoint.ToString());

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }

        private bool IsIPCorrect()
        {
            List<String> AllIPs = new List<string>();
            IPHostEntry localHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress address in localHostInfo.AddressList)
            {
                if (address.Equals(INTERFACE_TO_LISTEN))
                    return true;

                AllIPs.Add(address.ToString());
            }

            Console.WriteLine("Your specified IP is none of those present on the machine interfaces!");
            Console.WriteLine("Your IP adresses:");

            foreach (String ip in AllIPs)
            {
                Console.WriteLine(ip);
            }

            return false;
        }
    }
}