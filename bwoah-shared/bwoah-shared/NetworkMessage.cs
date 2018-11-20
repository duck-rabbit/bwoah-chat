using bwoah_shared.DataClasses;
using bwoah_shared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace bwoah_shared
{
    public class NetworkMessage
    {
        public byte MessageType { get; private set; }

        public byte[] Payload { get; private set; }

        public uint Size { get; private set; }

        public byte[] ByteMessage
        {
            get
            {
                Size = (UInt32)Payload.LongLength;

                MemoryStream memoryStream = new MemoryStream();

                memoryStream.Append(MessageType);
                memoryStream.Append(BitConverter.GetBytes(Size));
                memoryStream.Append(Payload);

                return memoryStream.ToArray();
            }
        }

        public AData GetPayloadData()
        {
            //get
            //{
                AData payloadData = DataFactory.Create(MessageType);
                String jsonData = Encoding.UTF8.GetString(Payload);
                payloadData = payloadData.ParseFromJson(jsonData);
                return payloadData;
            //}
        }

        public NetworkMessage(byte[] byteMessage)
        {
            MessageType = byteMessage[0];
            Size = BitConverter.ToUInt32(byteMessage, 1);
            Payload = byteMessage.SubArray(5, (int)Size);
        }

        public NetworkMessage(AData payloadData)
        {
            MessageType = DataTypeIds.GetIdByType(payloadData.GetType());

            String jsonData = JsonConvert.SerializeObject(payloadData);
            Payload = Encoding.UTF8.GetBytes(jsonData);
            Size = (UInt32)Payload.LongLength;
        }

        public void AddBytesToPayload(byte[] additionalData)
        {
            Payload.Add(additionalData);
        }
    }
}
