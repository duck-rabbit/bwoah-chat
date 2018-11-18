using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using bwoah_shared.Utils;
using Newtonsoft.Json;

namespace bwoah_shared.DataClasses
{
    public class ClientMessageData : AData
    {
        public int ChannelId { get; set; }
        public string Message { get; set; }
    }
}
