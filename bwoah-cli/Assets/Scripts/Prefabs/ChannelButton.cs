using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChannelButton : MonoBehaviour
{
    public Text channelName;
    public int channelId;
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Image background;

    private void OnEnable()
    {
        ChatUser.I.OnChangeChannel += OnChangeChannel;
    }

    private void OnDisable()
    {
        ChatUser.I.OnChangeChannel -= OnChangeChannel;
    }

    private void Start()
    {
        background.color = channelId == ChatUser.I.CurrentChatChannel ? _activeColor : _inactiveColor;
    }

    public void SelectThisButtonChannel()
    {
        ChatUser.I.CurrentChatChannel = channelId;
    }

    public void OnChangeChannel(int currentChannel)
    {
        background.color = channelId == currentChannel ? _activeColor : _inactiveColor;
    }
}
