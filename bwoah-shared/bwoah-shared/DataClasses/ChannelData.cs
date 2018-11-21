using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using bwoah_shared.Utils;

namespace bwoah_shared.DataClasses
{
    public class ChannelData : AData
    {
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string[] UserNicknames { get; set; }
    }
}
