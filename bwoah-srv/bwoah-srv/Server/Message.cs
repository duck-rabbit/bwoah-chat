using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using bwoah_shared.DataClasses;

namespace bwoah_srv.Server
{
    class Message
    {
        public DateTime ReceivedTime { get; private set; }
        public ChatUser User { get; private set; }
        public String Content { get; private set; }

        public ServerMessageData ServerMessageData
        {
            get
            {
                ServerMessageData smd = new ServerMessageData();
                smd.Time = ReceivedTime;
                smd.UserNickname = User.Nickname;
                smd.Message = Content;
                return smd;
            }
        }

        public Message(ChatUser user, string content)
        {
            ReceivedTime = DateTime.UtcNow;
            User = user;
            Content = content;
        }

        public override String ToString()
        {
            return String.Format("{0} {1}: {2}", ReceivedTime.ToLongTimeString(), User.Nickname, Content);
        }
    }
}
