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

        private ConcurrentDictionary<int, ChatChannel> _channelList = new ConcurrentDictionary<int, ChatChannel>();

        private int _channelIndexValue = 0;

        private int NextChannelIndex
        {
            get
            {
                while (_channelList.ContainsKey(_channelIndexValue))
                {
                    _channelIndexValue++;
                }

                return _channelIndexValue;
            }
        }

        private ChatChannel OpenWall
        {
            get
            {
                if (!_channelList.ContainsKey(0))
                {
                    CreateOpenWall();
                }
                return _channelList[0];
            }
            set
            {
                if (!_channelList.ContainsKey(0))
                {
                    CreateOpenWall();
                }
                _channelList[0] = value;
            }
        }

        private void CreateOpenWall()
        {
            ChatUser mockUser = new ChatUser();
            mockUser.Nickname = "Automatic action";
            _channelList.TryAdd(0, new ChatChannel(_server, 0, "Open wall", new ConcurrentDictionary<Socket, ChatUser>(), mockUser));
        }

        public Chat(IServer server)
        {
            _server = server;

            _server.UserSocketException += UserDisconnected;

            DataHandler.Instance.RegisterAction(typeof(ChatMessageData), HandleChatMessageData);
            DataHandler.Instance.RegisterAction(typeof(NewUserData), HandleNewUserData);
            DataHandler.Instance.RegisterAction(typeof(DisconnectUserData), HandleDisconnectUserData);
            DataHandler.Instance.RegisterAction(typeof(ChannelData), HandleChannelData);
            DataHandler.Instance.RegisterAction(typeof(NicknameChangeData), HandleNicknameChangeData);
        }

        private void HandleChatMessageData(AData receivedData, Socket socket)
        {
            ChatMessageData chatMessage = (ChatMessageData)receivedData;

            ChatChannel channelToMessage;

            if (!_channelList.ContainsKey(chatMessage.Channel))
            {
                Console.WriteLine("[Server] Cannot log message for channel {0}. Channel does not exist.", chatMessage.Channel);
                return;
            }
            else
            {
                channelToMessage = _channelList[chatMessage.Channel];
            }

            Message message = new Message(channelToMessage, GetUserBySocket(socket), chatMessage.Content);

            message.LogToConsole();
            channelToMessage.CacheMessage(message);

            AData dataToSend = message.GetChatMessage();

            NetworkMessage networkMessage = new NetworkMessage(dataToSend);

            channelToMessage.SendDataToAllUsers(networkMessage.ByteMessage);
        }

        private void HandleNewUserData(AData receivedData, Socket socket)
        {
            NewUserData data = (NewUserData)receivedData;

            if (OpenWall.UserList.Keys.Any(userSocket => userSocket == socket))
            {
                return;
            }

            ChatUser newUser = new ChatUser();
            newUser.Nickname = data.Nickname;

            AddUserToChannel(socket, newUser, OpenWall);
        }

        private void AddUserToChannel(Socket userSocket, ChatUser user, ChatChannel channel)
        {
            channel.UserList.TryAdd(userSocket, user);
            channel.UserList.OrderBy(pair => pair.Value.Nickname);

            foreach (Message cachedMessage in channel.GetCachedMessages())
            {
                NetworkMessage catchUpMessage = new NetworkMessage(cachedMessage.GetChatMessage());

                _server.SendData(catchUpMessage.ByteMessage, userSocket);
            }

            Message userJoinedMessage = new Message(channel, user, string.Format("{0} joined {1}.", user.Nickname, channel.ChannelName), true);
            userJoinedMessage.LogToConsole();
            channel.CacheMessage(userJoinedMessage);

            NetworkMessage networkMessage = new NetworkMessage(userJoinedMessage.GetChatMessage());
            channel.SendDataToAllUsers(networkMessage.ByteMessage);

            networkMessage = new NetworkMessage(channel.GetChannelData());
            channel.SendDataToAllUsers(networkMessage.ByteMessage);
        }

        private void HandleDisconnectUserData(AData receivedData, Socket socket)
        {
            UserDisconnected(socket);
        }

        private void UserDisconnected(Socket socket)
        {
            foreach (ChatChannel channel in _channelList.Values)
            {
                if (channel.UserList.Keys.Contains(socket))
                {
                    RemoveUserFromChannel(socket, channel);
                }
            }
        }

        private void RemoveUserFromChannel(Socket userSocket, ChatChannel channel)
        {
            ChatUser userToRemove;
            channel.UserList.TryRemove(userSocket, out userToRemove);

            if (userToRemove != null)
            {
                if (channel.UserList.IsEmpty && channel != OpenWall)
                {
                    ChatChannel channelToRemove;
                    _channelList.TryRemove(channel.ChannelId, out channelToRemove);

                    Message channelClosedMessage = new Message(channel, userToRemove, string.Format("Channel {0} is closed", channel.ChannelName), true);
                    channelClosedMessage.LogToConsole();

                    return;
                }

                NetworkMessage networkMessage = new NetworkMessage(channel.GetChannelData());
                channel.SendDataToAllUsers(networkMessage.ByteMessage);

                Message userLeftMessage = new Message(channel, userToRemove, string.Format("{0} left {1}", userToRemove.Nickname, channel.ChannelName), true);
                userLeftMessage.LogToConsole();

                channel.CacheMessage(userLeftMessage);

                networkMessage = new NetworkMessage(userLeftMessage.GetChatMessage());
                channel.SendDataToAllUsers(networkMessage.ByteMessage);
            }
        }

        private void HandleChannelData(AData data, Socket socket)
        {
            ChannelData channelData = (ChannelData)data; 

            if (channelData.ChannelId == 0)
            {
                ConcurrentDictionary<Socket, ChatUser> newChannelUsers = new ConcurrentDictionary<Socket, ChatUser>();

                int channelIndex = NextChannelIndex;

                _channelList.TryAdd(channelIndex, new ChatChannel(_server, channelIndex,channelData.ChannelName, newChannelUsers, OpenWall.UserList[socket]));

                foreach (string nickname in channelData.UserNicknames)
                {
                    KeyValuePair<Socket, ChatUser> userPair = OpenWall.UserList.Where(keyValuePair => keyValuePair.Value.Nickname == nickname).ToArray()[0];
                    AddUserToChannel(userPair.Key, userPair.Value, _channelList[channelIndex]);
                }
            }
        }

        private void HandleNicknameChangeData(AData data, Socket socket)
        {
            NicknameChangeData nicknameOperationsData = (NicknameChangeData)data;

            foreach (ChatChannel channel in _channelList.Values)
            {
                if (channel.UserList.Keys.Contains(socket))
                {
                    ChatUser user = channel.UserList[socket];

                    Message userChangedMessage = new Message(channel, user, string.Format("{0} changed their name to {1}", user.Nickname, nicknameOperationsData.NewNickname), true);
                    userChangedMessage.LogToConsole();

                    channel.CacheMessage(userChangedMessage);

                    NetworkMessage networkMessage = new NetworkMessage(userChangedMessage.GetChatMessage());
                    channel.SendDataToAllUsers(networkMessage.ByteMessage);

                    user.Nickname = nicknameOperationsData.NewNickname;

                    networkMessage = new NetworkMessage(channel.GetChannelData());
                    channel.SendDataToAllUsers(networkMessage.ByteMessage);
                }
            }
        }

        private ChatUser GetUserBySocket(Socket userSocket)
        {
            return OpenWall.UserList[userSocket];
        }
    }
}
