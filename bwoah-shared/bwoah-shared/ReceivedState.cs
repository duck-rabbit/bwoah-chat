using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using bwoah_shared.DataClasses;

namespace bwoah_shared
{
    public class ReceivedState
    {
        public const int BUFFER_SIZE = 1024;

        public Socket NetSocket { get; set; }
        public byte[] buffer = new byte[BUFFER_SIZE];
        public AData ReceivedData { get; private set; }

        public ReceivedState(Socket socket)
        {
            NetSocket = socket;
        }

        public void HandleData(int dataLength)
        {
            ReceivedData = DataFactory.Create(buffer[0]);
            ReceivedData = ReceivedData.ParseFromByte(buffer);
            DataHandler.Instance.HandleData(this);
        }
    }
}
