using System;
using System.Collections.Generic;
using System.Text;

namespace bwoah_parser.DataClasses
{
    class NicknameData : IData
    {
        public String Nickname { get; set; }

        public IData ParseFromByte(byte[] byteStream)
        {
            StringBuilder builder = new StringBuilder();
            Nickname = Encoding.ASCII.GetString(byteStream, 1, byteStream.Length - 1);
            return this;
        }

        public byte[] ParseToByte()
        {
            throw new NotImplementedException();
        }
    }
}
