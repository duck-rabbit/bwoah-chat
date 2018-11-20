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
        private IServer _server;

        public int ChannelId { get; private set; }
        public ConcurrentDictionary<Socket, ChatUser> UserList { get; private set; }

        public ChatChannel(IServer server, int id, ConcurrentDictionary<Socket, ChatUser> userList)
        {
            _server = server;
            ChannelId = id;
            UserList = userList;
        }

        public void SendDataToAllUsers(byte[] byteData)
        {
            foreach (Socket socket in UserList.Keys)
            {
                _server.SendData(byteData, socket);
            }
        }

        public ChannelData GetChannelData()
        {
            ChannelData channelData = new ChannelData();
            channelData.ChannelId = ChannelId;
            channelData.UserNicknames = UserList.Values.Select(chatUser => chatUser.Nickname).ToArray();

            return channelData;
        }
    }
}