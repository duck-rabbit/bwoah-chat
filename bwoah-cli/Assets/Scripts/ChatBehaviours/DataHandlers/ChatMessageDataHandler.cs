using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bwoah_shared;
using bwoah_shared.DataClasses;

public class ChatMessageDataHandler : DataOnUpdateHandler
{
    [SerializeField] private Message messagePrefab;
    [SerializeField] private SystemMessage autoMessage;
    [SerializeField] private Transform messageContainer;

    new private void OnEnable()
    {
        typeToHandle = typeof(ChatMessageData);
        base.OnEnable();
    }

    override protected void HandleData(AData data)
    {
        ChatMessageData messageData = (ChatMessageData)data;

        if (!messageData.IsAutoMessage)
        {
            Message message = Instantiate(messagePrefab, messageContainer);
            message._timeText.text = messageData.Timestamp.ToLocalTime().ToLongTimeString();
            message._nicknameText.text = messageData.Nickname;
            message._messageText.text = messageData.Content;
        }
        else if (messageData.IsAutoMessage)
        {
            SystemMessage message = Instantiate(autoMessage, messageContainer);
            message._timeText.text = messageData.Timestamp.ToLocalTime().ToLongTimeString();
            message._messageText.text = messageData.Content;
        }
    }
}
