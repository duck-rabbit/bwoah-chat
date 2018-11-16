using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickToSend : MonoBehaviour
{
    [SerializeField] InputField _chatInput;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ClickSendButton();
        }
    }

    public void ClickSendButton()
    {
        if (!_chatInput.text.Equals(string.Empty))
        {
            ChatClient.I.SendMessageToServer(_chatInput.text);
            _chatInput.text = string.Empty;
        }
    }
}
