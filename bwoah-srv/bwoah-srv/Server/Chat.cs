using bwoah_shared;
using bwoah_shared.DataClasses;
using bwoah_shared.Utils;
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
        private IServer _server;

        //private ConcurrentDictionary<Socket, ChatUser> _userList = new ConcurrentDictionary<Socket, ChatUser>();

        private ChatChannel _openWall; 

        public Chat(IServer server)
        {
            _server = server;

            _openWall = new ChatChannel(_server, 0, new ConcurrentDictionary<Socket, ChatUser>());

            _server.UserSocketException += UserDisconnected;

            DataHandler.Instance.RegisterAction(typeof(ChatMessageData), HandleChatMessageData);
            DataHandler.Instance.RegisterAction(typeof(NewUserData), HandleNewUserData);
            DataHandler.Instance.RegisterAction(typeof(DisconnectUserData), HandleDisconnectUserData);
            DataHandler.Instance.RegisterAction(typeof(NicknameChangeData), HandleNicknameChangeData);
        }

        private void HandleChatMessageData(AData receivedData, Socket socket)
        {
            ChatMessageData chatMessage = (ChatMessageData)receivedData;
            Message message = new Message(_openWall, GetUserBySocket(socket), chatMessage.Content);

            message.LogToConsole();

            AData dataToSend = message.GetChatMessage();

            NetworkMessage networkMessage = new NetworkMessage(dataToSend);

            _openWall.SendDataToAllUsers(networkMessage.ByteMessage);
        }

        private void HandleNewUserData(AData receivedData, Socket socket)
        {
            NewUserData data = (NewUserData)receivedData;

            if (_openWall.UserList.Keys.Any(userSocket => userSocket == socket))
            {
                return;
            }

            ChatUser newUser = new ChatUser();
            newUser.Nickname = data.Nickname;

            _openWall.UserList.AddOrUpdate(socket, newUser, (key, value) => _openWall.UserList[socket]);
            _openWall.UserList.OrderBy(pair => pair.Value.Nickname);

            Message userJoinedMessage = new Message(_openWall, newUser, string.Format("{0} joined the chat", data.Nickname), true);
            userJoinedMessage.LogToConsole();

            NetworkMessage networkMessage = new NetworkMessage(userJoinedMessage.GetChatMessage());
            _openWall.SendDataToAllUsers(networkMessage.ByteMessage);

            networkMessage = new NetworkMessage(_openWall.GetChannelData());
            _openWall.SendDataToAllUsers(networkMessage.ByteMessage);
        }

        private void HandleDisconnectUserData(AData receivedData, Socket socket)
        {
            UserDisconnected(socket);
        }

        private void UserDisconnected(Socket socket)
        {
            ChatUser userToRemove;

            _openWall.UserList.TryRemove(socket, out userToRemove);

            if (userToRemove != null)
            {
                NetworkMessage networkMessage = new NetworkMessage(_openWall.GetChannelData());
                _openWall.SendDataToAllUsers(networkMessage.ByteMessage);

                Message userLeftMessage = new Message(_openWall, userToRemove, string.Format("{0} left the chat", userToRemove.Nickname), true);
                userLeftMessage.LogToConsole();

                networkMessage = new NetworkMessage(userLeftMessage.GetChatMessage());
                _openWall.SendDataToAllUsers(networkMessage.ByteMessage);
            }
        }

        private void HandleNicknameChangeData(AData data, Socket socket)
        {
            NicknameChangeData nicknameOperationsData = (NicknameChangeData)data;
            ChatUser user = GetUserBySocket(socket);

            Message userChangedMessage = new Message(_openWall, user, string.Format("{0} changed their name to {1}", user.Nickname, nicknameOperationsData.NewNickname), true);
            userChangedMessage.LogToConsole();

            NetworkMessage networkMessage = new NetworkMessage(userChangedMessage.GetChatMessage());
            _openWall.SendDataToAllUsers(networkMessage.ByteMessage);

            user.Nickname = nicknameOperationsData.NewNickname;

            networkMessage = new NetworkMessage(_openWall.GetChannelData());
            _openWall.SendDataToAllUsers(networkMessage.ByteMessage);
        }

        private void HandleChannelData(AData data, Socket socket)
        {

        }

        private ChatUser GetUserBySocket(Socket userSocket)
        {
            return _openWall.UserList[userSocket];
        }
    }
}
