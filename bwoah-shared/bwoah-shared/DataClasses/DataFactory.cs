using System;
using System.Collections.Generic;
using System.Text;

namespace bwoah_shared.DataClasses
{
    class DataFactory
    {
        public static IData Create(byte dataTypeId)
        {
            return (IData)Activator.CreateInstance(DataTypeIds.GetTypeById(dataTypeId));
        }
    }
}
