using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bwoah_shared.DataClasses
{
    public class ChatMessageData : AData
    {
        public int Channel { get; set; }
        public bool IsAutoMessage { get; set; } = false;
        public DateTime Timestamp { get; set; }
        public string Nickname { get; set; }
        public string Content { get; set; }
    }
}
