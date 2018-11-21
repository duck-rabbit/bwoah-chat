using bwoah_cli;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToWelcomeScreen : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _welcomeScreen;
    [SerializeField] private GameObject _manageChannelSpace;
    [SerializeField] private GameObject _channelListHolder;

    private bool _triggerAction = false;

    private void OnEnable()
    {
        ChatClient.I.OnUltimateConnectionLost += TriggerAction;
    }

    private void Update()
    {
        if (_triggerAction)
        {
            ShowWelcomeScreen();
            _triggerAction = false;
        }
    }

    private void TriggerAction()
    {
        _triggerAction = true;
    }

    private void ShowWelcomeScreen()
    {
        _loadingScreen.SetActive(false);
        _welcomeScreen.SetActive(true);

        foreach (Channel channel in ChatUser.I.chatChannels.Values)
        {
            Destroy(channel.gameObject);
        }
        ChatUser.I.chatChannels.Clear();

        foreach (Transform channelLabel in _channelListHolder.transform)
        {
            Destroy(channelLabel.gameObject);
        }

        _manageChannelSpace.SetActive(false);
    }
}
