using bwoah_shared.DataClasses;
using System;

namespace bwoah_srv.Server
{
    class Message
    {
        public ChatChannel Channel { get; private set; }
        public bool IsAutoMessage { get; private set; }
        public DateTime Timestamp { get; private set; }
        public ChatUser User { get; private set; }
        public String Content { get; private set; }

        public Message(ChatChannel channel, ChatUser user, string content, bool isAutoMessage = false)
        {
            Channel = channel;
            Timestamp = DateTime.UtcNow;
            User = user;
            Content = content;
            IsAutoMessage = isAutoMessage;
        }

        public ChatMessageData GetChatMessage()
        {
            ChatMessageData chatMessageData = new ChatMessageData();
            chatMessageData.Channel = Channel.ChannelId;
            chatMessageData.IsAutoMessage = IsAutoMessage;
            chatMessageData.Timestamp = Timestamp;
            chatMessageData.Nickname = User.Nickname;
            chatMessageData.Content = Content;
            return chatMessageData;
        }

        public override String ToString()
        {
            if (!IsAutoMessage)
            {
                return String.Format("[Channel {0}] @{1} {2}: {3}", Channel.ChannelId, Timestamp.ToLongTimeString(), User.Nickname, Content);
            }
            else
            {
                return String.Format("[Channel {0}] @{1}: {2}", Channel.ChannelId, Timestamp.ToLongTimeString(), Content);
            }
        }

        public void LogToConsole()
        {
            Console.WriteLine(ToString());
        }
    }
}
