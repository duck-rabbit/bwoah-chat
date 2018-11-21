using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickChangeNickButton : MonoBehaviour
{
    [SerializeField] private Button _changeNickButton;
    [SerializeField] private InputField _nicknameInput;

    public void OnClickChangeNickButton()
    {
        _changeNickButton.gameObject.SetActive(false);
    }
}
