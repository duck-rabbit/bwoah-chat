using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using bwoah_cli;
using bwoah_shared.DataClasses;
using Newtonsoft.Json;

public class ClickToSendNickname : MonoBehaviour
{
    [SerializeField] private InputField _nicknameInput;
    private ChatUser _user;

    private void Start()
    {
        _user = ChatUser.I;
        _nicknameInput.text = _user.nickname;
    }

    public void ClickSendButton()
    {
        if (!_nicknameInput.text.Equals(string.Empty))
        {
            NicknameChangeData nicknameData = new NicknameChangeData();
            nicknameData.NewNickname = _nicknameInput.text;

            Debug.Log(JsonConvert.SerializeObject(nicknameData));
            ChatClient.I.SendMessageToServer(nicknameData);

            _user.nickname = _nicknameInput.text;
        }
    }
}
