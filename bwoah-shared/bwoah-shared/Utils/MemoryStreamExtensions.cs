using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace bwoah_shared.Utils
{
    public static class MemoryStreamExtensions
    {
        public static void Append(this MemoryStream stream, byte value)
        {
            stream.Append(new[] { value });
        }

        public static void Append(this MemoryStream stream, byte[] values)
        {
            stream.Write(values, 0, values.Length);
        }
    }
}
