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
    }
}
