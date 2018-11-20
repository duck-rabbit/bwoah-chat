using bwoah_cli;
using bwoah_shared;
using bwoah_shared.DataClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChannelDataHandler : DataOnUpdateHandler
{
    [SerializeField] private Text _userPrefab;
    [SerializeField] private Transform _userContainer;
    [SerializeField] private GameObject _loadingScreen;

    private ChatUser _user;

    new private void OnEnable()
    {
        typeToHandle = typeof(ChannelData);
        base.OnEnable();
    }

    new private void OnDisable()
    {
        base.OnDisable();
    }

    private void Start()
    {
        _user = ChatUser.I;
    }

    new private void Update()
    {
        base.Update();
    }

    override protected void HandleData(AData data)
    {
        _loadingScreen.SetActive(false);

        ChannelData messageData = (ChannelData)data;

        if (messageData.UserNicknames != null)
        {
            foreach (string nickname in messageData.UserNicknames)
            {
                Text nicknameText = Instantiate(_userPrefab, _userContainer);
                string textToPut;
                if (nickname.Equals(string.Empty))
                {
                    textToPut = "<i>Unknown user</i>";
                }
                else if (nickname.Equals(_user.nickname))
                {
                    textToPut = nickname;
                }
                else
                {
                    textToPut = nickname;
                }

                nicknameText.text = textToPut;

                _user.chatRooms.Add(0, new ChatRoom());
                _user.chatRooms[0].nicknameList.Add(nicknameText);
            }
        }
    }
}
