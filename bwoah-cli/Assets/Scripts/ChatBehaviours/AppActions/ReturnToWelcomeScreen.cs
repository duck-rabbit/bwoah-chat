using bwoah_cli;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToWelcomeScreen : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private GameObject _welcomeScreen;
    [SerializeField] private Transform _messageContainer;
    [SerializeField] private Transform _userContainer;

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
        foreach (Transform userPrefab in _userContainer)
        {
            Destroy(userPrefab.gameObject);
        }
        foreach (Transform messagePrefab in _messageContainer)
        {
            Destroy(messagePrefab.gameObject);
        }
        _loadingScreen.SetActive(false);
        _welcomeScreen.SetActive(true);
    }
}
