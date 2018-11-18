using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using bwoah_cli;
using bwoah_shared.DataClasses;

public class ClickToSendMessage : MonoBehaviour
{
    [SerializeField] InputField _chatInput;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && _chatInput.isFocused)
        {
            ClickSendButton();
        }
    }

    public void ClickSendButton()
    {
        if (!_chatInput.text.Equals(string.Empty))
        {
            ClientMessageData messageData = new ClientMessageData();
            messageData.Message = _chatInput.text;
            messageData.ChannelId = 0;
            Debug.Log(messageData.ParseToJson());
            ChatClient.I.SendMessageToServer(messageData);
            _chatInput.text = string.Empty;
        }
    }
}
