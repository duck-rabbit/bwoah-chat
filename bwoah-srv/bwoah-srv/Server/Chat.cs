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

            DataHandler.Instance.RegisterAction(typeof(ChatMessageData), HandleChatMessageData);
            DataHandler.Instance.RegisterAction(typeof(NewUserData), HandleNewUserData);
            DataHandler.Instance.RegisterAction(typeof(NicknameOperationsData), HandleNicknameOperationsData);
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
            ChatUser newUser = new ChatUser();
            newUser.Nickname = data.Nickname;

            NicknameOperationsData sendNicknameToClientTables = new NicknameOperationsData();
            sendNicknameToClientTables.OperationType = NicknameOperation.Add;
            sendNicknameToClientTables.NewNickname = data.Nickname;

            NetworkMessage networkMessage = new NetworkMessage(sendNicknameToClientTables);
            _openWall.SendDataToAllUsers(networkMessage.ByteMessage);

            _openWall.UserList.AddOrUpdate(socket, newUser, (key, value) => _openWall.UserList[socket]);

            Message userJoinedMessage = new Message(_openWall, newUser, string.Format("{0} joined the chat", data.Nickname), true);
            userJoinedMessage.LogToConsole();

            networkMessage = new NetworkMessage(userJoinedMessage.GetChatMessage());
            _openWall.SendDataToAllUsers(networkMessage.ByteMessage);

            networkMessage = new NetworkMessage(_openWall.GetChannelData());
            _server.SendData(networkMessage.ByteMessage, socket);
        }

        private void HandleNicknameOperationsData(AData data, Socket socket)
        {
            NicknameOperationsData nicknameOperationsData = (NicknameOperationsData)data;

            switch (nicknameOperationsData.OperationType)
            {
                case NicknameOperation.Add:
                    AddUser(nicknameOperationsData, socket);
                    break;
                case NicknameOperation.Change:
                    RenameUser(nicknameOperationsData, socket);
                    break;
                case NicknameOperation.Remove:
                    RemoveUser(nicknameOperationsData, socket);
                    break;
            }
        }

        private void HandleChannelData(AData data, Socket socket)
        {

        }

        public void AddUser(NicknameOperationsData data, Socket socket)
        {
            
        }

        public void RemoveUser(NicknameOperationsData data, Socket socket)
        {
            ChatUser userToRemove = GetUserBySocket(socket);

            _openWall.UserList.TryRemove(socket, out userToRemove);

            NetworkMessage networkMessage = new NetworkMessage(data);
            _openWall.SendDataToAllUsers(networkMessage.ByteMessage);

            Message userLeftMessage = new Message(_openWall, userToRemove, string.Format("{0} left the chat", data.OldNickname), true);
            userLeftMessage.LogToConsole();

            networkMessage = new NetworkMessage(userLeftMessage.GetChatMessage());
            _openWall.SendDataToAllUsers(networkMessage.ByteMessage);
        }

        public void RenameUser(NicknameOperationsData data, Socket socket)
        {
            ChatUser user = GetUserBySocket(socket);
            user.Nickname = data.NewNickname;

            NetworkMessage networkMessage = new NetworkMessage(data);
            _openWall.SendDataToAllUsers(networkMessage.ByteMessage);

            Message userChangedMessage = new Message(_openWall, user, string.Format("{0} changed their name to {1}", data.OldNickname, data.NewNickname), true);
            userChangedMessage.LogToConsole();

            networkMessage = new NetworkMessage(userChangedMessage.GetChatMessage());
            _openWall.SendDataToAllUsers(networkMessage.ByteMessage);
        }

        private ChatUser GetUserBySocket(Socket userSocket)
        {
            return _openWall.UserList[userSocket];
        }
    }
}
