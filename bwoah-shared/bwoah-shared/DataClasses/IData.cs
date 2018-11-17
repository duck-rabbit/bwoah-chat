using System;
using System.Collections.Generic;
using System.Text;

namespace bwoah_shared.DataClasses
{
    public interface IData
    { 
        IData ParseFromByte(byte[] byteStream, int dataLength);
        byte[] ParseToByte();
    }
}
