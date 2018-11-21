﻿using bwoah_shared;
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
        public static int CACHE_MESSAGE_COUNT = 10;

        private IServer _server;

        public int ChannelId { get; private set; }
        public ConcurrentDictionary<Socket, ChatUser> UserList { get; private set; }
        public ConcurrentQueue<Message> MessageCache { get; private set; }

        public ChatChannel(IServer server, int id, ConcurrentDictionary<Socket, ChatUser> userList)
        {
            _server = server;
            ChannelId = id;
            UserList = userList;
            MessageCache = new ConcurrentQueue<Message>();
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

        public void CacheMessage(Message message)
        {
            if (MessageCache.Count >= CACHE_MESSAGE_COUNT)
            {
                Message messageToDequeue;
                MessageCache.TryDequeue(out messageToDequeue);
            }
            MessageCache.Enqueue(message);
        }

        public IEnumerable<Message> GetCachedMessages()
        {
            return MessageCache.ToArray();
        }
    }
}