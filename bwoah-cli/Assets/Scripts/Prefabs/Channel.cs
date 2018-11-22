using bwoah_shared.DataClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Channel : MonoBehaviour
{
    [SerializeField] private Transform _nicknameHolder;
    [SerializeField] private Transform _messageHolder;
    [SerializeField] private NicknameToggle _nicknamePrefab;
    [SerializeField] private Message _messagePrefab;
    [SerializeField] private SystemMessage _autoMessagePrefab;
    private List<NicknameToggle> _nicknameList = new List<NicknameToggle>();
    private List<GameObject> _messageList = new List<GameObject>();

    public List<NicknameToggle> NicknameList { get { return _nicknameList; } }
    public string ChannelName { get; set; }

    public ChannelData channelData { get; set; }

    public ChannelButton channelButton { get; set; }

    public void SetNicknameList(string[] userNicknames)
    {
        foreach (NicknameToggle userObject in _nicknameList)
        {
            Destroy(userObject.gameObject);
        }

        _nicknameList.Clear();


        if (userNicknames != null)
        {
            foreach (string nickname in userNicknames)
            {
                NicknameToggle nicknameText = Instantiate(_nicknamePrefab, _nicknameHolder);
                string textToPut;
                if (nickname.Equals(string.Empty))
                {
                    textToPut = "<i>Unknown user</i>";
                }
                else
                {
                    textToPut = nickname;
                }

                nicknameText.text.text = textToPut;

                _nicknameList.Add(nicknameText);
            }
        }
    }

    public void AddMessage(ChatMessageData messageData)
    {
        if (!messageData.IsAutoMessage)
        {
            Message message = Instantiate(_messagePrefab, _messageHolder);
            message._timeText.text = messageData.Timestamp.ToLocalTime().ToLongTimeString();
            message._nicknameText.text = messageData.Nickname;
            message._messageText.text = messageData.Content;

            _messageList.Add(message.gameObject);
        }
        else if (messageData.IsAutoMessage)
        {
            SystemMessage message = Instantiate(_autoMessagePrefab, _messageHolder);
            message._timeText.text = messageData.Timestamp.ToLocalTime().ToLongTimeString();
            message._messageText.text = messageData.Content;

            _messageList.Add(message.gameObject);
        }
    }
}
