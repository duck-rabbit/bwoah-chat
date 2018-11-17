using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using bwoah_shared.Utils;

namespace bwoah_shared.DataClasses
{
    public class ServerMessageData : IData
    {
        public DateTime Time { get; set; }
        public String UserNickname { get; set; }
        public String Message { get; set; }

        public IData ParseFromByte(byte[] byteStream, int dataLength)
        {
            Time = DateTime.FromBinary(BitConverter.ToInt64(byteStream, 1));
            UserNickname = Encoding.UTF8.GetString(byteStream, 9, 40).TrimEnd('\0');
            Message = Encoding.UTF8.GetString(byteStream, 41, dataLength - 41);
            return this;
        }

        public byte[] ParseToByte()
        {
            byte idByte = DataTypeIds.GetIdByType(this.GetType());
            byte[] timeBytes = BitConverter.GetBytes(Time.ToBinary());
            byte[] nicknameBytes = Encoding.UTF8.GetBytes(UserNickname).ToArray();
            byte[] messageBytes = Encoding.UTF8.GetBytes(Message).ToArray();

            MemoryStream byteStream = new MemoryStream();
            byteStream.Append(idByte);
            byteStream.Append(timeBytes);
            byteStream.Position = 9;
            byteStream.Append(nicknameBytes);
            byteStream.Position = 41;
            byteStream.Append(messageBytes);

            return byteStream.ToArray();
        }
    }
}
