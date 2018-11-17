using bwoah_cli;
using bwoah_shared;
using bwoah_shared.DataClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MantainUserList : DataOnUpdateHandler
{
    [SerializeField] private Text userPrefab;
    [SerializeField] private Transform userContainer;
    [SerializeField] InputField _nicknameInput;
    [SerializeField] Color _unknownUserColor;

    private List<Text> _nicknameList = new List<Text>();
    private string _userNickname;

    new private void OnEnable()
    {
        typeToHandle = typeof(NicknameListData);
        base.OnEnable();
    }

    new private void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Return) && _nicknameInput.isFocused)
        {
            ClickSendButton();
        }
    }

    override protected void HandleData(RecievedState recievedState)
    {
        NicknameListData messageData = (NicknameListData)recievedState.RecievedData;

        foreach (string nickname in messageData.UserNicknames)
        {
            Text nicknameText = Instantiate(userPrefab, userContainer);
            string textToPut;
            if (nickname.Equals(string.Empty))
            {
                textToPut = "Unknown user";
                nicknameText.color = _unknownUserColor;
            }
            else if (nickname.Equals(_userNickname))
            {
                textToPut = string.Format("<b>{0}</b> (You)", nickname);
            }
            else
            {
                textToPut = nickname;
            }

            nicknameText.text = textToPut;
            _nicknameList.Add(nicknameText);
        }
    }

    public void ClickSendButton()
    {
        if (!_nicknameInput.text.Equals(string.Empty))
        {
            NicknameData nicknameData = new NicknameData();
            nicknameData.Nickname = _nicknameInput.text;
            _userNickname = _nicknameInput.text;
            ChatClient.I.SendMessageToServer(nicknameData);
        }
    }
}
