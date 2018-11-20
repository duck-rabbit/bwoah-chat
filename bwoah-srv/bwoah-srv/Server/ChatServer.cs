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
    class ChatServer : Singleton<ChatServer>, IServer
    {
        public static IPAddress INTERFACE_TO_LISTEN = IPAddress.Parse("192.168.0.59");
        public static int LOCAL_PORT = 13131;
        public static int MAX_PENDING_CONNECTIONS = 100;
        public static int SOCKET_FLAGS = 0;

        private IPEndPoint _localEndPoint;
        private Socket _serverSocket;

        public void StartServer()
        {
            if (!IsIPCorrect())
            {
                return;
            }

            _localEndPoint = new IPEndPoint(INTERFACE_TO_LISTEN, LOCAL_PORT);

            Console.WriteLine(String.Format("[Server] Starting Bwoah! Server on {0}", _localEndPoint.ToString()));

            _serverSocket = new Socket(_localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _serverSocket.Bind(_localEndPoint);
                _serverSocket.Listen(MAX_PENDING_CONNECTIONS);

                StartAccept();
            }
            catch (ArgumentNullException ae)
            {
                Console.WriteLine("[Server] ArgumentNullException : {0}", ae.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("[Server] SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("[Server] Unexpected exception : {0}", e.ToString());
            }
        }

        private void StartAccept()
        {
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), _serverSocket);
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            try
            {
                StartAccept();

                Socket listener = (Socket)asyncResult.AsyncState;
                Socket socket = listener.EndAccept(asyncResult);

                StartReceiveData(new ReceivedState(socket));
            }
            catch (ArgumentNullException ae)
            {
                Console.WriteLine("[Server] ArgumentNullException : {0}", ae.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("[Server] SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("[Server] Unexpected exception : {0}", e.ToString());
            }
        }

        private void StartReceiveData(ReceivedState receivedState)
        {
            receivedState.NetSocket.BeginReceive(receivedState.Buffer, 0, ReceivedState.BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), receivedState);
        }

        private void ReceiveCallback(IAsyncResult receivedSocket)
        {
            ReceivedState receivedState = (ReceivedState)receivedSocket.AsyncState;

            int dataLength = receivedState.NetSocket.EndReceive(receivedSocket);

            if (dataLength > 0)
            {
                receivedState.HandleData();

                if (receivedState.WaitForData)
                {
                    StartReceiveData(receivedState);
                }
                else
                {
                    StartReceiveData(new ReceivedState(receivedState.NetSocket));
                }
            }
            else
            {
                receivedState.NetSocket.Shutdown(SocketShutdown.Both);
                receivedState.NetSocket.Close();
            }
        }

        public void SendData(byte[] byteData, Socket socket)
        {
            StartSendData(byteData, socket);
        }

        private void StartSendData(byte[] byteData, Socket socket)
        {
            socket.BeginSend(byteData, 0, byteData.Length, (SocketFlags)ChatServer.SOCKET_FLAGS, new AsyncCallback(SendDataCallback), socket);
        }

        private void SendDataCallback(IAsyncResult connectionResult)
        {
            Socket socket = (Socket)connectionResult.AsyncState;
            try
            {
                socket.EndSend(connectionResult);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Server] {0}", e.ToString());
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

            Console.WriteLine("[Server] Your specified IP is none of those present on the machine interfaces!");
            Console.WriteLine("[Server] Your IP adresses:");

            foreach (String ip in AllIPs)
            {
                Console.WriteLine(ip);
            }

            return false;
        }
    }
}