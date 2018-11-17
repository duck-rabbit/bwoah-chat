using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bwoah_shared;
using bwoah_shared.DataClasses;

public class GenerateChatMessages : DataOnUpdateHandler
{
    [SerializeField] private Message messagePrefab;
    [SerializeField] private Transform messageContainer;

    new private void OnEnable()
    {
        typeToHandle = typeof(ServerMessageData);
        base.OnEnable();
    }

    override protected void HandleData(RecievedState recievedState)
    {
        ServerMessageData messageData = (ServerMessageData)recievedState.RecievedData;
        Message message = Instantiate(messagePrefab, messageContainer);
        message._timeText.text = messageData.Time.ToLocalTime().ToLongTimeString();
        message._nicknameText.text = messageData.UserNickname;
        message._messageText.text = messageData.Message;
    }
}
