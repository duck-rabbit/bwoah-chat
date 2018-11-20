using bwoah_shared.DataClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserOperationsDataHandler : DataOnUpdateHandler
{
    [SerializeField] private Text userPrefab;
    [SerializeField] private Transform userContainer;
    private ChatUser _user;

    private void Start()
    {
        _user = ChatUser.I;
    }

    new private void OnEnable()
    {
        typeToHandle = typeof(NicknameOperationsData);
        base.OnEnable();
    }

    override protected void HandleData(AData data)
    {
        NicknameOperationsData nicknameOperationData = (NicknameOperationsData)data;

        Text textToAlter;

        if (nicknameOperationData.OperationType == NicknameOperation.Add)
        {
            Text nicknameText = Instantiate(userPrefab, userContainer);
            nicknameText.text = nicknameOperationData.NewNickname;

            _user.chatRooms[0].nicknameList.Add(nicknameText);
        }
        else if (nicknameOperationData.OperationType == NicknameOperation.Remove)
        {
            textToAlter = _user.chatRooms[0].nicknameList.Find((text) => text.text == nicknameOperationData.OldNickname);
            _user.chatRooms[0].nicknameList.Remove(textToAlter);
            Destroy(textToAlter);
        }
        else if (nicknameOperationData.OperationType == NicknameOperation.Change)
        {
            textToAlter = _user.chatRooms[0].nicknameList.Find((text) => text.text == nicknameOperationData.OldNickname);
            textToAlter.text = nicknameOperationData.NewNickname;
        }
    }
}
