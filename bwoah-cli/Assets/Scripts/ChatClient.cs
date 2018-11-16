using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ChatClient : UnitySingleton<ChatClient>
{
    public static IPAddress IP_ADDRESS = IPAddress.Parse("192.168.0.59");
    public static int PORT = 13131;
    public static int SOCKET_FLAGS = 0;
    ManualResetEvent _communicationDone;

    IPEndPoint _serverEndPoint = new IPEndPoint(IP_ADDRESS, 13131);
    Socket _clientSocket = new Socket(IP_ADDRESS.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    private void Awake()
    {
        ConnectToServer();
    }

    private void OnApplicationQuit()
    {
        DisconnectFromServer();
    }

    public void ConnectToServer()
    {
        _communicationDone = new ManualResetEvent(false);

        try
        {
            _communicationDone.Reset();

            _clientSocket.BeginConnect(_serverEndPoint, new AsyncCallback(ConnectCallback), _clientSocket);

            _communicationDone.WaitOne();
        }
        catch (ArgumentNullException ae)
        {
            Debug.LogError(String.Format("ArgumentNullException : {0}", ae.ToString()));
        }
        catch (SocketException se)
        {
            Debug.LogError(String.Format("SocketException : {0}", se.ToString()));
        }
        catch (Exception e)
        {
            Debug.LogError(String.Format("Unexpected exception : {0}", e.ToString()));
        }
    }

    public void ConnectCallback(IAsyncResult connectionResult)
    {
        try
        {
            _communicationDone.Set();

            _clientSocket.EndConnect(connectionResult); 

            Debug.Log("Connected to server.");
        }
        catch (Exception e)
        {
            Debug.LogError(String.Format("Unexpected exception : {0}", e.ToString()));
        }
    }

    public void DisconnectFromServer()
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

    public void SendMessageToServer(String message)
    {
        byte[] byteData = Encoding.ASCII.GetBytes(message);

        _clientSocket.BeginSend(byteData, 0, byteData.Length, (SocketFlags)SOCKET_FLAGS, new AsyncCallback(SendMessageToServerCallback), _clientSocket);
    }

    public void SendMessageToServerCallback(IAsyncResult connectionResult)
    {
        try
        {
            _clientSocket.EndSend(connectionResult);

            Debug.Log("Message sent.");
        }
        catch (Exception e)
        {
            Debug.LogError(String.Format("Unexpected exception : {0}", e.ToString()));
        }
    }
}
