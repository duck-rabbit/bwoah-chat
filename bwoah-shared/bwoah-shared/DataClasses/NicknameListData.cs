using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using bwoah_shared.Utils;

namespace bwoah_shared.DataClasses
{
    public class NicknameListData : IData
    {
        public List<String> UserNicknames { get; set; }

        public IData ParseFromByte(byte[] byteStream, int dataLength)
        {
            UserNicknames = new List<String>();

            for (int i = 0; i < dataLength; i++)
            {
                UserNicknames.Add(Encoding.UTF8.GetString(byteStream, (i * 32) + 1, (i * 32) + 33).TrimEnd('\0'));
            }

            return this;
        }

        public byte[] ParseToByte()
        {
            MemoryStream byteStream = new MemoryStream();

            byte idByte = DataTypeIds.GetIdByType(this.GetType());
            byteStream.Append(idByte);

            for (int i = 0; i < UserNicknames.Count; i++)
            {
                byteStream.Append(Encoding.UTF8.GetBytes(UserNicknames[i]));
                byteStream.Position = (32 * (i + 1)) + 1;
            }

            return byteStream.ToArray();
        }
    }
}
