using System;
using System.Collections.Generic;
using System.Text;

namespace bwoah_parser.DataClasses
{
    class DataFactory
    {
        public static readonly Dictionary<byte, Type> dataTypeIds = new Dictionary<byte, Type>
        {
            {0, typeof(ClientMessageData)},
            {9, typeof(NicknameData)}
        };

        public static IData Create(byte dataTypeId)
        {
            if (dataTypeIds.ContainsKey(dataTypeId))
            {
                return (IData)Activator.CreateInstance(dataTypeIds[dataTypeId]);
            }
            else
            {
                throw new ArgumentException(String.Format("Current stream data types do not contain {0} type.", dataTypeId), "dataTypeId");
            }
        }
    }
}
