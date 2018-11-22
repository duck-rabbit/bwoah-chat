using System;
using System.Collections.Generic;

namespace bwoah_shared.DataClasses
{
    public struct DataTypeId
    {
        public byte Id { get; set; }
        public Type Type { get; set; }

        public DataTypeId(byte id, Type type)
        {
            Id = id;
            Type = type;
        }
    }

    public static class DataTypeIds
    {
        public static readonly List<DataTypeId> dataTypeIds = new List<DataTypeId>
        {
            new DataTypeId(1, typeof(ChatMessageData)),
            new DataTypeId(2, typeof(NewUserData)),
            new DataTypeId(3, typeof(DisconnectUserData)),
            new DataTypeId(4, typeof(ChannelData)),
            new DataTypeId(5, typeof(NicknameChangeData)),
            new DataTypeId(255, typeof(PingData)),
        };

        public static byte GetIdByType(Type type)
        {
            return dataTypeIds.Find(x => x.Type.Equals(type)).Id;
        }

        public static Type GetTypeById(byte id)
        {
            return dataTypeIds.Find(x => x.Id.Equals(id)).Type;
        }
    }
}
