using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using bwoah_shared.DataClasses;
using bwoah_shared.Utils;

namespace bwoah_shared
{
    public class ReceivedState
    {
        public const int BUFFER_SIZE = 1024;

        public bool WaitForData { get; set; }
        public Socket NetSocket { get; set; }
        public byte[] Buffer { get; set; }

        private NetworkMessage _networkMessage;
        private int _recievedOperationIndex = 0;

        public ReceivedState(Socket socket)
        {
            NetSocket = socket;
            Buffer = new byte[BUFFER_SIZE];
            WaitForData = false;
        }

        public void HandleData()
        {
            _recievedOperationIndex++;

            if (WaitForData)
            {
                _networkMessage.AddBytesToPayload(Buffer);
            }
            else
            {
                _networkMessage = new NetworkMessage(Buffer);
            }

            if (_networkMessage.Size < BUFFER_SIZE * _recievedOperationIndex)
            {
                AData receivedData = _networkMessage.GetPayloadData();
                DataHandler.Instance.HandleData(receivedData, NetSocket);
                WaitForData = false;
            }
            else
            {
                WaitForData = true;
            }
        }
    }
}
