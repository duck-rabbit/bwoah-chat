using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using bwoah_shared.Utils;
using bwoah_shared;
using bwoah_shared.DataClasses;

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
        private Socket _serverSocket;

        private string dafuq = String.Empty;

        ManualResetEvent _doneListening = new ManualResetEvent(false);

        Chat _chat = new Chat();

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

            Console.WriteLine(String.Format("[System] Starting Bwoah! Server on {0}", _localEndPoint.ToString()));

            _serverSocket = new Socket(_localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _serverSocket.Bind(_localEndPoint);
                _serverSocket.Listen(MAX_PENDING_CONNECTIONS);

                StartAccept();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected error. Closing Bwoah! server.");
                Console.WriteLine(e.ToString());
            }
        }

        private void StartAccept()
        {
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), _serverSocket);
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            StartAccept();

            Socket listener = (Socket)asyncResult.AsyncState;
            Socket socket = listener.EndAccept(asyncResult);

            _chat.AddUser(socket);

            StartReceiveData(socket);
        }

        private void StartReceiveData(Socket socket)
        {
            ReceivedState socketState = new ReceivedState(socket);
            socket.BeginReceive(socketState.buffer, 0, ReceivedState.BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), socketState);
        }

        private void ReceiveCallback(IAsyncResult receivedSocket)
        {
            ReceivedState socketState = (ReceivedState)receivedSocket.AsyncState;

            int dataLength = socketState.NetSocket.EndReceive(receivedSocket);

            if (dataLength > 0)
            {
                StartReceiveData(socketState.NetSocket);

                socketState.HandleData(dataLength);
            }
            else
            {
                _chat.RemoveUser(socketState.NetSocket);

                socketState.NetSocket.Shutdown(SocketShutdown.Both);
                socketState.NetSocket.Close();
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