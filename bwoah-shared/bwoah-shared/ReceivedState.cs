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

        public Socket NetSocket { get; set; }
        public byte[] Buffer { get; set; }
        private byte[] _messageBuffer = new byte[0];

        public bool ReadCurrentBuffer
        {
            get
            {
                if (_networkMessage.Size == 0)
                {
                    return false;
                }
                else
                {
                    return _networkMessage.Size < _messageBuffer.Length;
                }
            }
        }

        public bool WaitForData
        {
            get
            {
                return _networkMessage.Size > _messageBuffer.Length;
            }
        }

        private NetworkMessage _networkMessage;

        public ReceivedState(Socket socket)
        {
            NetSocket = socket;
            Buffer = new byte[BUFFER_SIZE];
        }

        public void HandleData()
        {
            if (_messageBuffer.Length == 0)
            {
                _messageBuffer = Buffer;
            }
            else
            {
                byte[] newBuffer = _messageBuffer.Add(Buffer);
                _messageBuffer = newBuffer;
            }

            _networkMessage = new NetworkMessage(_messageBuffer);

            do
            {
                AData receivedData = _networkMessage.GetPayloadData;
                DataHandler.Instance.HandleData(receivedData, NetSocket);

                byte[] newBuffer = _messageBuffer.SubArray((int)_networkMessage.Size, _messageBuffer.Length - ((int)_networkMessage.Size));
                _messageBuffer = newBuffer;

                _networkMessage = new NetworkMessage(_messageBuffer);
            }
            while (ReadCurrentBuffer);

            Buffer = new byte[BUFFER_SIZE];
        }
    }
}
