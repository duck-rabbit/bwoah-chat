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

        private ChatChannel _openWall;

        public Chat()
        {
            _openWall = new ChatChannel(0, _userList);

            DataHandler.Instance.RegisterAction(typeof(ClientMessageData), HandleClientMessageData);
            DataHandler.Instance.RegisterAction(typeof(NicknameOperationData), HandleNicknameData);
        }

        public void AddUser(Socket userSocket)
        {
            ChatUser newUser = new ChatUser(userSocket);

            NicknameOperationData nicknameOperationData = new NicknameOperationData();

            nicknameOperationData.NicknameOperation = NicknameOperation.Add;
            nicknameOperationData.NewNickname = "";
            nicknameOperationData.Time = DateTime.UtcNow;

            _openWall.SendDataToAllUsers(nicknameOperationData.ParseToByte());

            _userList.AddOrUpdate(userSocket, newUser, (key, value) => _userList[userSocket]);

            newUser.SendData(_openWall.GetChannelData().ParseToByte());
        }

        public void RemoveUser(Socket userSocket)
        {
            ChatUser userToRemove = GetUserBySocket(userSocket);

            NicknameOperationData nicknameOperationData = new NicknameOperationData();
            nicknameOperationData.NicknameOperation = NicknameOperation.Remove;
            nicknameOperationData.OldNickname = userToRemove.Nickname;
            nicknameOperationData.Time = DateTime.UtcNow;

            userToRemove.GenerateByeMessage();
            _userList.TryRemove(userSocket, out userToRemove);

            _openWall.SendDataToAllUsers(nicknameOperationData.ParseToByte());
        }

        public void HandleClientMessageData(ReceivedState receivedState)
        {
            Message message = new Message(GetUserBySocket(receivedState.NetSocket),
                                    ((ClientMessageData)receivedState.ReceivedData).Message);

            Console.WriteLine("[Open Wall] {0}", message.ToString());

            AData data = message.ServerMessageData;
            byte[] byteData = data.ParseToByte();

            _openWall.SendDataToAllUsers(byteData);
        }

        public void HandleNicknameData(ReceivedState recievedState)
        {
            NicknameOperationData nicknameOperationData = (NicknameOperationData)recievedState.ReceivedData;

            nicknameOperationData.OldNickname = GetUserBySocket(recievedState.NetSocket).Nickname;
            nicknameOperationData.Time = DateTime.UtcNow;

            GetUserBySocket(recievedState.NetSocket).Nickname = nicknameOperationData.NewNickname;

            _openWall.SendDataToAllUsers(nicknameOperationData.ParseToByte());
        }

        public ChatUser GetUserBySocket(Socket userSocket)
        {
            return _userList[userSocket];
        }
    }
}
