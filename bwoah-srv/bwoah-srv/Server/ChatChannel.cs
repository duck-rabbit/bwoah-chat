using bwoah_shared;
using bwoah_shared.DataClasses;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace bwoah_srv.Server
{
    class ChatChannel
    {
        public int ChannelId { get; private set; }
        public ConcurrentDictionary<Socket, ChatUser> UserList { get; private set; }

        public ChatChannel(int id, ConcurrentDictionary<Socket, ChatUser> userList)
        {
            ChannelId = id;
            UserList = userList;
        }

        public void SendDataToAllUsers(byte[] byteData)
        {
            foreach (ChatUser user in UserList.Values)
            {
                user.SendData(byteData);
            }
        }

        public ChannelData GetChannelData()
        {
            ChannelData channelData = new ChannelData();
            channelData.ChannelId = ChannelId;

            List<String> userNicknames = new List<string>();

            foreach (ChatUser user in UserList.Values)
            {
                userNicknames.Add(user.Nickname);
            }

            channelData.UserNicknames = userNicknames.ToArray();

            return channelData;
        }
    }
}