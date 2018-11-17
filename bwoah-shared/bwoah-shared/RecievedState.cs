using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using bwoah_shared.DataClasses;

namespace bwoah_shared
{
    public class RecievedState
    {
        public const int BUFFER_SIZE = 1024;

        public Socket NetSocket { get; set; }
        public byte[] buffer = new byte[BUFFER_SIZE];
        public IData RecievedData { get; private set; }

        public RecievedState(Socket socket)
        {
            NetSocket = socket;
        }

        public void HandleData(int dataLength)
        {
            RecievedData = DataFactory.Create(buffer[0]);
            RecievedData.ParseFromByte(buffer, dataLength);
            DataHandler.Instance.HandleData(this);
        }
    }
}
