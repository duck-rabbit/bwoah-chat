using bwoah_cli;
using bwoah_shared;
using bwoah_shared.DataClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChannelDataHandler : DataOnUpdateHandler
{
    [SerializeField] private Channel _channelPrefab;
    [SerializeField] private Transform _channelsHolder;
    [SerializeField] private ChannelButton _channelButtonPrefab;
    [SerializeField] private Transform _channelButtonsHolder;

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

    override protected void HandleData(AData data)
    {
        ChannelData messageData = (ChannelData)data;

        if (!_user.chatChannels.ContainsKey(messageData.ChannelId))
        {
            Channel newChannel = Instantiate(_channelPrefab, _channelsHolder);
            newChannel.gameObject.SetActive(_user.CurrentChatChannel == messageData.ChannelId);
            newChannel.ChannelData = messageData;
            
            ChannelButton newChannelButton = Instantiate(_channelButtonPrefab, _channelButtonsHolder);
            newChannelButton.channelId = messageData.ChannelId;
            newChannelButton.channelName.text = messageData.ChannelName;

            newChannel.ChannelButton = newChannelButton;
            _user.chatChannels.Add(messageData.ChannelId, newChannel);
        }

        _user.chatChannels[messageData.ChannelId].ChannelButton.channelName.text = messageData.ChannelName;
        _user.chatChannels[messageData.ChannelId].SetNicknameList(messageData.UserNicknames);
    }
}
