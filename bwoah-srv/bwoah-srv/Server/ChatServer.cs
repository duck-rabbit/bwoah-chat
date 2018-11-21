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

        public Action<Socket> UserSocketException { get; set; }

        private ConcurrentDictionary<Socket, ManualResetEventSlim> _sendingToSocket = new ConcurrentDictionary<Socket, ManualResetEventSlim>();

        public void StartServer()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress localAddress in host.AddressList)
                {
                    IPEndPoint endPoint = new IPEndPoint(localAddress, LOCAL_PORT);
                    Socket serverSocket = new Socket(localAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    serverSocket.Bind(endPoint);

                    Console.WriteLine(String.Format("[Server] Bwoah! Server listening on {0}", endPoint.ToString()));

                    serverSocket.Listen(MAX_PENDING_CONNECTIONS);
                    StartAccept(serverSocket);
                }
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

        private void StartAccept(Socket socket)
        {
            socket.BeginAccept(new AsyncCallback(AcceptCallback), socket);
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket listener = (Socket)asyncResult.AsyncState;

                StartAccept(listener);
                
                Socket socket = listener.EndAccept(asyncResult);

                Console.WriteLine("[Server] Connection with {0} established", socket.RemoteEndPoint.ToString());

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

            try
            {
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
                    if (_sendingToSocket.ContainsKey(receivedState.NetSocket))
                    {
                        ManualResetEventSlim mres;
                        _sendingToSocket.TryRemove(receivedState.NetSocket, out mres);
                    }

                    receivedState.NetSocket.Shutdown(SocketShutdown.Both);
                    Console.WriteLine("[Server] Connection with {0} closed", receivedState.NetSocket.RemoteEndPoint.ToString());
                    receivedState.NetSocket.Close();
                }
            }
            catch (ArgumentNullException ae)
            {
                Console.WriteLine("[Server] ArgumentNullException : {0}", ae.ToString());
            }
            catch (SocketException se)
            {
                if (_sendingToSocket.ContainsKey(receivedState.NetSocket))
                {
                    ManualResetEventSlim mres;
                    _sendingToSocket.TryRemove(receivedState.NetSocket, out mres);
                }
                UserSocketException?.Invoke(receivedState.NetSocket);
                Console.WriteLine("[Server] SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("[Server] Unexpected exception : {0}", e.ToString());
            }
        }

        public void SendData(byte[] byteData, Socket socket)
        {
            StartSendData(byteData, socket);
        }

        private void StartSendData(byte[] byteData, Socket socket)
        {
            if (!_sendingToSocket.ContainsKey(socket))
            {
                _sendingToSocket.TryAdd(socket, new ManualResetEventSlim(true));
            }

            _sendingToSocket[socket].Wait();

            _sendingToSocket[socket].Reset();

            socket.BeginSend(byteData, 0, byteData.Length, (SocketFlags)ChatServer.SOCKET_FLAGS, new AsyncCallback(SendDataCallback), socket);
        }

        public void PrintByteArray(byte[] bytes)
        {
            var sb = new StringBuilder("new byte[] { ");
            foreach (var b in bytes)
            {
                sb.Append(b + ", ");
            }
            sb.Append("}");
            Console.WriteLine(sb.ToString());
        }

        private void SendDataCallback(IAsyncResult connectionResult)
        {
            Socket socket = (Socket)connectionResult.AsyncState;

            try
            {
                socket.EndSend(connectionResult);

                _sendingToSocket[socket].Set();
            }
            catch (ArgumentNullException ae)
            {
                Console.WriteLine("[Server] ArgumentNullException : {0}", ae.ToString());
            }
            catch (SocketException se)
            {
                if (_sendingToSocket.ContainsKey(socket))
                {
                    ManualResetEventSlim mres;
                    _sendingToSocket.TryRemove(socket, out mres);
                }
                UserSocketException?.Invoke(socket);
                Console.WriteLine("[Server] SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("[Server] Unexpected exception : {0}", e.ToString());
            }
        }
    }
}