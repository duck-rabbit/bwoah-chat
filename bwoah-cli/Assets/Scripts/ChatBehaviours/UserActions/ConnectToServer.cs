using bwoah_cli;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using bwoah_shared.DataClasses;
using System.Threading;

public class ConnectToServer : MonoBehaviour
{
    [SerializeField] private List<InputField> _addressByteTexts;
    [SerializeField] private InputField _nickname;
    [SerializeField] private Toggle _dataOnStart;
    [SerializeField] private Text _incorrectDataInfo;
    [SerializeField] private GameObject _welcomeScreenObject;
    [SerializeField] private GameObject _loadingScreenObject;

    private void Start()
    {
        if (PlayerPrefs.HasKey("nickname"))
        {
            for (int i = 0; i < _addressByteTexts.Count; i++)
            {
                _addressByteTexts[i].text = PlayerPrefs.GetString(string.Format("address{0}", i));
            }
            _nickname.text = PlayerPrefs.GetString("nickname");
        }
    }

    public void OnButtonPressed()
    {
        if (!IsDataCorrect())
        {
            _incorrectDataInfo.gameObject.SetActive(true);
            return;
        }
        else
        {
            _incorrectDataInfo.gameObject.SetActive(false);
        }

        string ipAddressString = string.Empty;
        foreach (InputField addressByteText in _addressByteTexts)
        {
            if (!ipAddressString.Equals(string.Empty))
            {
                ipAddressString += (".");
            }

            ipAddressString += addressByteText.text;
        }
        IPAddress serverAddress = IPAddress.Parse(ipAddressString);

        _welcomeScreenObject.SetActive(false);
        _loadingScreenObject.SetActive(true);

        ChatClient.I.ConnectToServer(serverAddress);

        NewUserData newUserData = new NewUserData();
        newUserData.Nickname = _nickname.text;

        ChatUser.I.nickname = _nickname.text;

        Thread newUserThread = new Thread(() => ChatClient.I.SendMessageToServer(newUserData));
        newUserThread.Start();

        if (_dataOnStart.isOn)
        {
            for (int i = 0; i < _addressByteTexts.Count; i++)
            {
                PlayerPrefs.SetString(string.Format("address{0}", i), _addressByteTexts[i].text);
            }
            PlayerPrefs.SetString("nickname", _nickname.text);
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }
    }

    private bool IsDataCorrect()
    {
        foreach (InputField addressByteText in _addressByteTexts)
        {
            if (addressByteText.text.Equals(string.Empty))
            {
                return false;
            }
            else if (int.Parse(addressByteText.text) < 0 || int.Parse(addressByteText.text) > 255)
            {
                return false;
            }
        }
        if (_nickname.text.Equals(string.Empty))
        {
            return false;
        }
        
        return true;
    }
}
