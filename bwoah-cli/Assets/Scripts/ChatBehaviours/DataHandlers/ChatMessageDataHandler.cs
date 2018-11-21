using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bwoah_shared;
using bwoah_shared.DataClasses;

public class ChatMessageDataHandler : DataOnUpdateHandler
{
    private Dictionary<int, Queue<ChatMessageData>> _unreceivedMessages = new Dictionary<int, Queue<ChatMessageData>>();

    new private void OnEnable()
    {
        typeToHandle = typeof(ChatMessageData);
        base.OnEnable();
    }

    new private void Update()
    {
        base.Update();

        if (_unreceivedMessages.Keys.Count > 0)
        {
            foreach (int key in _unreceivedMessages.Keys)
            {
                if (ChatUser.I.chatChannels.ContainsKey(key))
                {
                    int queueCount = _unreceivedMessages[key].Count;
                    for (int i = 0; i < queueCount; i++)
                    {
                        ChatUser.I.chatChannels[key].AddMessage(_unreceivedMessages[key].Dequeue());
                    }
                }
            }
        }
    }

    override protected void HandleData(AData data)
    {
        ChatMessageData messageData = (ChatMessageData)data;

        if (ChatUser.I.chatChannels.ContainsKey(messageData.Channel))
        {
            ChatUser.I.chatChannels[messageData.Channel].AddMessage(messageData);
        }
        else
        {
            if (!_unreceivedMessages.ContainsKey(messageData.Channel))
            {
                _unreceivedMessages.Add(messageData.Channel, new Queue<ChatMessageData>());
            }
            _unreceivedMessages[messageData.Channel].Enqueue(messageData);
        }
    }
}
