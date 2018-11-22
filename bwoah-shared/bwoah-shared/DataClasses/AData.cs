using Newtonsoft.Json;
using System;

namespace bwoah_shared.DataClasses
{
    public abstract class AData
    {
        public string ParseToJson()
        {
            String json = JsonConvert.SerializeObject(this);
            Console.WriteLine(json);
            return json;
        }

        public AData ParseFromJson(string json)
        {
            return (AData)(JsonConvert.DeserializeObject(json, this.GetType()));
        }
    }
}
