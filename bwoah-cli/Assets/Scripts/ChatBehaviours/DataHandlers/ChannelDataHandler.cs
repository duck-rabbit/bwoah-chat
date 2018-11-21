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
        ChannelData messageData = (ChannelData)data;

        if (!_user.chatRooms.ContainsKey(messageData.ChannelId))
        {
            _user.chatRooms.Add(messageData.ChannelId, new ChatRoom());
        }

        foreach (Transform userPrefab in _userContainer)
        {
            Destroy(userPrefab.gameObject);
        }

        _user.chatRooms[messageData.ChannelId].nicknameList.Clear();

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

                _user.chatRooms[messageData.ChannelId].nicknameList.Add(nicknameText);
            }
        }
    }
}
