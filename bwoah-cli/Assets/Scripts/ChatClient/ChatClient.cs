using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using bwoah_shared.DataClasses;
using bwoah_shared;

namespace bwoah_cli
{
    public class ChatClient : UnitySingleton<ChatClient>
    {
        public static int SOCKET_FLAGS = 0;
        public static int CONNECTION_CHECK_INTERVAL = 5;
        public static int RECONNECTION_INTERVAL = 1;
        public static int RECONNECTION_ATTEMPTS = 2;

        public Action OnConnectionLost;
        public Action OnUltimateConnectionLost;

        private Socket _clientSocket;
        private IPEndPoint _serverEndPoint;

        private ManualResetEvent _connectedToServer = new ManualResetEvent(false);
        private int _reconnectionAttempts = 0;

        private void OnApplicationQuit()
        {
            NicknameOperationsData uodh = new NicknameOperationsData();
            uodh.OperationType = NicknameOperation.Remove;
            uodh.OldNickname = ChatUser.I.nickname;

            NetworkMessage networkMessage = new NetworkMessage(uodh);
            _clientSocket.Send(networkMessage.ByteMessage);
            DisconnectFromServer();
        }

        public void ConnectToServer(IPAddress serverAddress, Int32 portNumber)
        {
            _serverEndPoint = new IPEndPoint(serverAddress, portNumber);

            ConnectToServer();
        }

        private void ConnectToServer()
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _connectedToServer.Reset();
            
            _clientSocket.BeginConnect(_serverEndPoint, new AsyncCallback(ConnectCallback), _clientSocket);
        }

        public void ConnectCallback(IAsyncResult connectionResult)
        {
            Socket connectedSocket = (Socket)connectionResult.AsyncState;

            try
            {
                connectedSocket.EndConnect(connectionResult);

                _connectedToServer.Set();

                _reconnectionAttempts = 0;
                //Thread checkConnectionThread = new Thread(CheckForConnection);
                //checkConnectionThread.Start();

                Debug.Log("Connected to server.");

                StartReceiveData(new ReceivedState(connectedSocket));
            }
            catch (ArgumentNullException ae)
            {
                Debug.LogError(String.Format("ArgumentNullException : {0}", ae.ToString()));
            }
            catch (SocketException se)
            {
                NoConnection();
                Debug.LogError(String.Format("SocketException : {0}", se.ToString()));
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format("Unexpected exception : {0}", e.ToString()));
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
                receivedState.NetSocket.EndReceive(receivedSocket);

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
            catch (ArgumentNullException ae)
            {
                Debug.LogError(String.Format("ArgumentNullException : {0}", ae.ToString()));
            }
            catch (SocketException se)
            {
                ConnectionLost();
                Debug.LogError(String.Format("SocketException : {0}", se.ToString()));
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format("Unexpected exception : {0}", e.ToString()));
            }
        }

        public void DisconnectFromServer(bool reuseSocket = false)
        {
            try
            {
                _clientSocket.Disconnect(false);

                Debug.Log("Disconnected from server.");
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format("Exception : {0}", e.ToString()));
            }
        }

        public void SendMessageToServer(AData message)
        {
            _connectedToServer.WaitOne();

            NetworkMessage networkMessage = new NetworkMessage(message);
            byte[] byteData = networkMessage.ByteMessage;

            _clientSocket.BeginSend(byteData, 0, byteData.Length, (SocketFlags)SOCKET_FLAGS, new AsyncCallback(SendMessageToServerCallback), _clientSocket);
        }

        public void SendMessageToServerCallback(IAsyncResult connectionResult)
        {
            try
            {
                _clientSocket.EndSend(connectionResult);

                Debug.Log("Message sent.");
            }
            catch (ArgumentNullException ae)
            {
                Debug.LogError(String.Format("ArgumentNullException : {0}", ae.ToString()));
            }
            catch (SocketException se)
            {
                ConnectionLost();
                Debug.LogError(String.Format("SocketException : {0}", se.ToString()));
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format("Unexpected exception : {0}", e.ToString()));
            }
        }

        private bool IsServerConnected()
        {
            return !((_clientSocket.Poll(1000, SelectMode.SelectRead) && (_clientSocket.Available == 0)) || !_clientSocket.Connected);
        }

        //private void CheckForConnection()
        //{
        //    Thread.Sleep(CONNECTION_CHECK_INTERVAL * 1000);
        //    if (!IsServerConnected())
        //    {
        //        ConnectionLost();
        //    }
        //    else
        //    {
        //        Thread checkConnectionThread = new Thread(CheckForConnection);
        //        checkConnectionThread.Start();
        //    }
        //}

        private void ConnectionLost()
        {
            if (OnUltimateConnectionLost != null)
            {
                _reconnectionAttempts = 0;
                OnConnectionLost();
            }
            DisconnectFromServer(true);
            NoConnection();
        }

        private void NoConnection()
        {
            Thread reconnectThread = new Thread(TryReconnect);
            reconnectThread.Start();
        }

        private void TryReconnect()
        {
            _reconnectionAttempts++;
            if (_reconnectionAttempts > RECONNECTION_ATTEMPTS)
            {
                if (OnUltimateConnectionLost != null)
                {
                    OnUltimateConnectionLost();
                }
                _reconnectionAttempts = 0;
            }
            else
            {
                Thread.Sleep(RECONNECTION_INTERVAL * 1000);
                ConnectToServer();
            }
        }
    }
}
