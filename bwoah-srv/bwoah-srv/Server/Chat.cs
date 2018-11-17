using bwoah_shared;
using bwoah_shared.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace bwoah_srv.Server
{
    class Chat
    {
        private List<ChatUser> _userList = new List<ChatUser>();

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

            _userList.Add(new ChatUser(userSocket));

            newUser.SendData(GetNicknameListData(_userList).ParseToByte());
        }

        public void RemoveUser(Socket userSocket)
        {
            ChatUser userToRemove = GetUserBySocket(userSocket);
            userToRemove.GenerateByeMessage();
            _userList.Remove(userToRemove);
        }

        public void HandleClientMessageData(RecievedState recievedState)
        {
            Message message = new Message(GetUserBySocket(recievedState.NetSocket),
                                    ((ClientMessageData)recievedState.RecievedData).Message);

            Console.WriteLine("[Open Wall] {0}", message.ToString());

            IData data = message.ServerMessageData;
            byte[] byteData = data.ParseToByte(); 

            _openWall.SendDataToAllUsers(byteData);
        }

        public void HandleNicknameData(RecievedState recievedState)
        {
            GetUserBySocket(recievedState.NetSocket).Nickname = ((NicknameData)recievedState.RecievedData).Nickname;
        }

        public ChatUser GetUserBySocket(Socket userSocket)
        {
            return _userList.Find(x => x.UserSocket == userSocket);
        }

        private NicknameListData GetNicknameListData(List<ChatUser> users)
        {
            NicknameListData nicknameData = new NicknameListData();
            nicknameData.UserNicknames = new List<String>();
            foreach (ChatUser user in users)
            {
                nicknameData.UserNicknames.Add(user.Nickname);
            }
            return nicknameData;
        }
    }
}
