using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using bwoah_shared.Utils;
using Newtonsoft.Json;

namespace bwoah_shared.DataClasses
{
    public abstract class AData
    {
        public byte DataId { get; set; }
        //public String _json;

        public string ParseToJson()
        {
            String json = JsonConvert.SerializeObject(this);
            Console.WriteLine(json);
            return json;
        }

        public AData ParseFromJson(string json)
        {
            Console.WriteLine(json);
            Console.WriteLine(GetType().ToString());
            return (AData)(JsonConvert.DeserializeObject(json, this.GetType()));
        }

        public byte[] ParseToByte()
        {
            DataId = DataTypeIds.GetIdByType(this.GetType());
            MemoryStream byteStream = new MemoryStream();

            byteStream.Append(DataId);
            byteStream.Append(Encoding.UTF8.GetBytes(ParseToJson()));

            return byteStream.ToArray();
        }

        public AData ParseFromByte(byte[] byteData)
        {
            return ParseFromJson(Encoding.UTF8.GetString(byteData.SubArray(1, byteData.Length - 1)));
        }
    }
}
