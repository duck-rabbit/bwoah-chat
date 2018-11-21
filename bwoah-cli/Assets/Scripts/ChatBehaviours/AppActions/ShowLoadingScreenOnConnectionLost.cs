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
    private bool _triggerShowLoading = false;
    private bool _triggerHideLoading = false;

    private void OnEnable()
    {
        ChatClient.I.OnConnectionLost += TriggerShowLoading;
        ChatClient.I.OnConnectionEstablished += TriggerHideLoading;
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

    private void ShowLoadingScreen()
    {
        _manageChannelScreen.SetActive(false);
        _loadingScreen.SetActive(true);
    }

    private void HideLoadingScreen()
    {
        NewUserData newUserData = new NewUserData();
        newUserData.Nickname = ChatUser.I.nickname;

        Thread newUserThread = new Thread(() => ChatClient.I.SendMessageToServer(newUserData));
        newUserThread.Start();

        _loadingScreen.SetActive(false);
        _manageChannelSpace.SetActive(true);
    }
}
