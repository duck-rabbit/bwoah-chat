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
    }

    public void ClickSendButton()
    {
        if (!_nicknameInput.text.Equals(string.Empty))
        {
            NicknameOperationsData nicknameData = new NicknameOperationsData();
            nicknameData.NewNickname = _nicknameInput.text;

            if (_user.nickname == string.Empty)
            {
                nicknameData.OperationType = NicknameOperation.Add;
            }
            else
            {
                nicknameData.OperationType = NicknameOperation.Change;
                nicknameData.OldNickname = _user.nickname;
            }

            Debug.Log(JsonConvert.SerializeObject(nicknameData));
            ChatClient.I.SendMessageToServer(nicknameData);

            _user.nickname = _nicknameInput.text;
        }
    }
}
