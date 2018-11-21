using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatUser : UnitySingleton<ChatUser>
{
    public string nickname = string.Empty;
    public int _currentChatChannel = 0;
    public Action<int> OnChangeChannel;

    public int CurrentChatChannel
    {
        get { return _currentChatChannel; }
        set
        {
            _currentChatChannel = value;
            if (OnChangeChannel != null)
                OnChangeChannel(_currentChatChannel);
            foreach (KeyValuePair<int, Channel> intChannelPair in chatChannels)
            {
                if (intChannelPair.Key == _currentChatChannel)
                {
                    intChannelPair.Value.gameObject.SetActive(true);
                }
                else
                {
                    intChannelPair.Value.gameObject.SetActive(false);
                }
            }
        }
    }

    public Dictionary<int, Channel> chatChannels = new Dictionary<int, Channel>();
}
