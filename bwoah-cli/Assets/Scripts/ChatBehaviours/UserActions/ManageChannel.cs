using bwoah_cli;
using bwoah_shared.DataClasses;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ManageChannel : MonoBehaviour
{
    [SerializeField] private GameObject _channelManager;
    [SerializeField] private InputField _channelName;
    [SerializeField] private Transform _usersHolder;
    [SerializeField] private Text _incorrectDataInfo;

    private List<NicknameToggle> _nicknameToggles = new List<NicknameToggle>();
    private bool _createNewChannel;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OpenChannelManager(bool createNewChannel)
    {
        _incorrectDataInfo.gameObject.SetActive(false);

        _createNewChannel = createNewChannel;
        Channel activeChannel = ChatUser.I.chatChannels[ChatUser.I.CurrentChatChannel];

        _channelName.text = _createNewChannel ? string.Empty : activeChannel.ChannelData.ChannelName;

        foreach (NicknameToggle chatUser in ChatUser.I.chatChannels[0].NicknameList)
        {
            if (chatUser.text.text != ChatUser.I.nickname)
            {
                NicknameToggle newUserToggle = Instantiate(chatUser, _usersHolder);
                newUserToggle.toggle.interactable = true;
                if (!createNewChannel)
                {
                    NicknameToggle[] currentChannelUser = activeChannel.NicknameList.Where(toggle => toggle.text == newUserToggle.text).ToArray();
                    if (currentChannelUser.Length > 0)
                    {
                        Debug.Log("Go!");
                        currentChannelUser[0].toggle.isOn = true;
                    }
                }

                _nicknameToggles.Add(newUserToggle);
            }
        }

        _channelManager.SetActive(true);
    }

    public void ClickAcceptButton()
    {
        if (_channelName.text.Equals(string.Empty))
        {
            _incorrectDataInfo.gameObject.SetActive(true);
            return;
        }

        ChannelData channelData = new ChannelData();

        if (!_createNewChannel)
        {
            channelData.ChannelId = ChatUser.I.CurrentChatChannel;
        }
        channelData.ChannelName = _channelName.text;

        _nicknameToggles.Where(toggle => toggle.toggle.isOn);

        List<string> channelUsers = new List<string>();
        channelUsers.Add(ChatUser.I.nickname);
        channelUsers.AddRange(_nicknameToggles.Where(toggle => toggle.toggle.isOn).Select(toggle => toggle.text.text).ToList());

        channelData.UserNicknames = channelUsers.ToArray();

        ChatClient.I.SendMessageToServer(channelData);

        _channelManager.SetActive(false);
        ClearUserList();
    }

    public void ClickDeclineButton()
    {
        _channelManager.SetActive(false);
        ClearUserList();
    }

    public void ClearUserList()
    {
        foreach (NicknameToggle toggle in _nicknameToggles)
        {
            Destroy(toggle.gameObject);
        }
        _nicknameToggles = new List<NicknameToggle>();
    }
}
