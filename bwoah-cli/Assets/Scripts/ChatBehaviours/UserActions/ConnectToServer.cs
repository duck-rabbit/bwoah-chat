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
    [SerializeField] private InputField _portText;
    [SerializeField] private InputField _nickname;
    [SerializeField] private Toggle _dataOnStart;
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
            _portText.text = PlayerPrefs.GetString("port");
            _nickname.text = PlayerPrefs.GetString("nickname");
        }
    }

    public void OnButtonPressed()
    {
        string ipAddressString = string.Empty;
        foreach (InputField addressByteText in _addressByteTexts)
        {
            if (int.Parse(addressByteText.text) < 0 || int.Parse(addressByteText.text) > 255)
            {
                return;
            }
            if (!ipAddressString.Equals(string.Empty))
            {
                ipAddressString += (".");
            }

            ipAddressString += addressByteText.text;
        }
        IPAddress serverAddress = IPAddress.Parse(ipAddressString);

        Debug.Log(ipAddressString);

        int portNumber = int.Parse(_portText.text);

        if (portNumber < 0 && portNumber > 65535)
        {
            return;
        }

        _welcomeScreenObject.SetActive(false);
        _loadingScreenObject.SetActive(true);

        ChatClient.I.ConnectToServer(serverAddress, portNumber);

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
            PlayerPrefs.SetString("port", _portText.text);
            PlayerPrefs.SetString("nickname", _nickname.text);
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
