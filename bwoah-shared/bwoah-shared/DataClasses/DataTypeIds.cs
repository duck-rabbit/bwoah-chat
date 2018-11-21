using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            new DataTypeId(0, typeof(ChatMessageData)),
            new DataTypeId(1, typeof(NewUserData)),
            new DataTypeId(2, typeof(DisconnectUserData)),
            new DataTypeId(3, typeof(ChannelData)),
            new DataTypeId(4, typeof(NicknameChangeData)),
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
