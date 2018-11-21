using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableManageChannelOnOpenWall : MonoBehaviour
{
    [SerializeField] Button _manageChannelButton;

    private void OnEnable()
    {
        ChatUser.I.OnChangeChannel += DisableButtonOnOpenWall;
    }

    private void OnDisable()
    {
        ChatUser.I.OnChangeChannel -= DisableButtonOnOpenWall;
    }

    private void DisableButtonOnOpenWall(int currentChannel)
    {
        _manageChannelButton.interactable = (currentChannel != 0);
    }
}
