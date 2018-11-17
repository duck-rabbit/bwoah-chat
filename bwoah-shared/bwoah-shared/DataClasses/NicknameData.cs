using bwoah_shared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bwoah_shared.DataClasses
{
    public class NicknameData : IData
    {
        public String Nickname { get; set; }

        public IData ParseFromByte(byte[] byteStream, int dataLength)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Encoding.UTF8.GetString(byteStream, 1, dataLength - 1));
            Nickname = builder.ToString();
            return this;
        }

        public byte[] ParseToByte()
        {
            MemoryStream byteStream = new MemoryStream();
            byteStream.Append(DataTypeIds.GetIdByType(this.GetType()));
            byteStream.Append(Encoding.UTF8.GetBytes(Nickname));
            return byteStream.ToArray();
        }
    }
}
