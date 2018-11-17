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
    class ChatRoom
    {
        public int Id { get; private set; }
        public ConcurrentDictionary<Socket, ChatUser> UserList { get; private set; }

        public ChatRoom(int id, ConcurrentDictionary<Socket, ChatUser> userList)
        {
            Id = id;
            UserList = userList;
        }

        public void SendDataToAllUsers(byte[] byteData)
        {
            foreach (ChatUser user in UserList.Values)
            {
                user.SendData(byteData);
            }
        }
    }
}