using bwoah_shared;
using bwoah_shared.DataClasses;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace bwoah_srv.Server
{
    class Chat
    {
        private ConcurrentDictionary<Socket, ChatUser> _userList = new ConcurrentDictionary<Socket, ChatUser>();

        private ChatRoom _openWall;

        public Chat()
        {
            _openWall = new ChatRoom(0, _userList);

            DataHandler.Instance.RegisterAction(typeof(ClientMessageData), HandleClientMessageData);
            DataHandler.Instance.RegisterAction(typeof(NicknameData), HandleNicknameData);
        }

        public void AddUser(Socket userSocket)
        {
            ChatUser newUser = new ChatUser(userSocket);

            _userList.AddOrUpdate(userSocket, newUser, (key, value) => _userList[userSocket]);

            //newUser.SendData(GetNicknameListData(_userList).ParseToByte());
        }

        public void RemoveUser(Socket userSocket)
        {
            ChatUser userToRemove = GetUserBySocket(userSocket);
            userToRemove.GenerateByeMessage();
            _userList.TryRemove(userSocket, out userToRemove);
        }

        public void HandleClientMessageData(ReceivedState receivedState)
        {
            Message message = new Message(GetUserBySocket(receivedState.NetSocket),
                                    ((ClientMessageData)receivedState.ReceivedData).Message);

            Console.WriteLine("[Open Wall] {0}", message.ToString());

            IData data = message.ServerMessageData;
            byte[] byteData = data.ParseToByte();

            _openWall.SendDataToAllUsers(byteData);
        }

        public void HandleNicknameData(ReceivedState recievedState)
        {
            GetUserBySocket(recievedState.NetSocket).Nickname = ((NicknameData)recievedState.ReceivedData).Nickname;
        }

        public ChatUser GetUserBySocket(Socket userSocket)
        {
            return _userList[userSocket];
        }

        private NicknameListData GetNicknameListData(ConcurrentDictionary<Socket, ChatUser> users)
        {
            NicknameListData nicknameData = new NicknameListData();
            nicknameData.UserNicknames = new List<String>();
            foreach (ChatUser user in users.Values)
            {
                nicknameData.UserNicknames.Add(user.Nickname);
            }
            return nicknameData;
        }
    }
}
