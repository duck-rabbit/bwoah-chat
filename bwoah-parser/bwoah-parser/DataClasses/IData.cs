using System;
using System.Collections.Generic;
using System.Text;

namespace bwoah_parser.DataClasses
{
    public interface IData
    { 
        IData ParseFromByte(byte[] byteStream);
        byte[] ParseToByte();
    }
}
