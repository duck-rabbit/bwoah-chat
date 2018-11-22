using System;

namespace bwoah_shared.DataClasses
{
    class DataFactory
    {
        public static AData Create(byte dataTypeId)
        {
            return (AData)Activator.CreateInstance(DataTypeIds.GetTypeById(dataTypeId));
        }
    }
}
