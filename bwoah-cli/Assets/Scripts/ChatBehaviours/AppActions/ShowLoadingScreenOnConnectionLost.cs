using bwoah_cli;
using bwoah_shared.DataClasses;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ShowLoadingScreenOnConnectionLost : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _manageChannelScreen;
    [SerializeField] private GameObject _manageChannelSpace;
    [SerializeField] private GameObject _channelListHolder;
    private bool _triggerShowLoading = false;
    private bool _triggerHideLoading = false;

    private List<ChannelData> channelDataCache = new List<ChannelData>();

    private void OnEnable()
    {
        ChatClient.I.OnConnectionLost += TriggerShowLoading;
        ChatClient.I.OnConnectionEstablished += TriggerHideLoading;
        ChatClient.I.OnUltimateConnectionLost += ClearCache;
    }

    private void Update()
    {
        if (_triggerShowLoading)
        {
            ShowLoadingScreen();
            _triggerShowLoading = false;
        }
        if (_triggerHideLoading)
        {
            HideLoadingScreen();
            _triggerHideLoading = false;
        }
    }

    private void TriggerShowLoading()
    {
        _triggerShowLoading = true;
    }

    private void TriggerHideLoading()
    {
        _triggerHideLoading = true;
    }

    private void ClearCache()
    {
        channelDataCache = new List<ChannelData>();
    }

    private void ShowLoadingScreen()
    {
        channelDataCache = new List<ChannelData>();

        _manageChannelScreen.SetActive(false);
        _loadingScreen.SetActive(true);

        ChatUser.I.CurrentChatChannel = 0;
        List<Channel> channelsToRemove = new List<Channel>();

        foreach (Channel channel in ChatUser.I.chatChannels.Values)
        {
            if (channel.ChannelData.ChannelId != 0)
            {
                channelDataCache.Add(channel.ChannelData);
                Destroy(channel.gameObject);

                channelsToRemove.Add(channel);
            }
        }

        foreach (Channel channel in channelsToRemove)
        {
            ChatUser.I.chatChannels.Remove(channel.ChannelData.ChannelId);
        }

        foreach (Transform channelLabel in _channelListHolder.transform)
        {
            if (channelLabel.GetComponent<ChannelButton>().channelId != 0)
            {
                Destroy(channelLabel.gameObject);
            }
        }
    }

    private void HideLoadingScreen()
    {
        NewUserData newUserData = new NewUserData();
        newUserData.Nickname = ChatUser.I.nickname;

        Thread newUserThread = new Thread(() => ChatClient.I.SendMessageToServer(newUserData));
        newUserThread.Start();

        _loadingScreen.SetActive(false);
        _manageChannelSpace.SetActive(true);

        foreach (ChannelData channelData in channelDataCache)
        {
            Debug.Log("GO!");
            
            ChatClient.I.SendMessageToServer(channelData);
        }
    }
}
