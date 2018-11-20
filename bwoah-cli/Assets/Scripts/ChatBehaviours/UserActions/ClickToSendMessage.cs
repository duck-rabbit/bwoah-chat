using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using bwoah_cli;
using bwoah_shared.DataClasses;

public class ClickToSendMessage : MonoBehaviour
{
    [SerializeField] private InputField _chatInput;
    private ChatUser _user;

    private void Start()
    {
        _user = ChatUser.I;
    }

    public void ClickSendButton()
    {
        if (!_chatInput.text.Equals(string.Empty))
        {
            ChatMessageData messageData = new ChatMessageData();
            messageData.Content = _chatInput.text;
            messageData.Channel = 0;
            messageData.Nickname = _user.nickname;
            ChatClient.I.SendMessageToServer(messageData);
            _chatInput.text = string.Empty;
        }
    }
}
