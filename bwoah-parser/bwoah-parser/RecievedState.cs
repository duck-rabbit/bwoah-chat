using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using bwoah_parser.DataClasses;

namespace bwoah_parser
{
    public class RecievedState
    {
        public const int BUFFER_SIZE = 1024;

        public Socket NetSocket { get; set; }
        public byte[] buffer = new byte[BUFFER_SIZE];

        public RecievedState(Socket socket)
        {
            NetSocket = socket;
        }

        public IData HandleData()
        {
            IData data = DataFactory.Create(buffer[0]);
            Console.WriteLine(data.GetType().ToString());
            //data.ParseFromByte(buffer);
            DataHandler.Instance.HandleData(data);
            return data;
        }
    }
}
