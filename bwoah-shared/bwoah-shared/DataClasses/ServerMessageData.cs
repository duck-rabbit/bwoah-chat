﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using bwoah_shared.Utils;

namespace bwoah_shared.DataClasses
{
    public class ServerMessageData : AData
    {
        public int ChannelId { get; set; }
        public DateTime Time { get; set; }
        public string UserNickname { get; set; }
        public string Message { get; set; }
    }
}
