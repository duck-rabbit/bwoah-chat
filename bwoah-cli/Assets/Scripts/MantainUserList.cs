using bwoah_cli;
using bwoah_shared;
using bwoah_shared.DataClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MantainUserList : DataOnUpdateHandler
{
    [SerializeField] private Text userPrefab;
    [SerializeField] private SystemMessage systemMessagePrefab;
    [SerializeField] private Transform userContainer;
    [SerializeField] private Transform messageContainer;
    [SerializeField] InputField _nicknameInput;
    [SerializeField] Color _unknownUserColor;

    private List<Text> _nicknameList = new List<Text>();
    private string _userNickname = string.Empty;

    protected Queue<ReceivedState> dataAlterationHandleQueue = new Queue<ReceivedState>();

    new private void OnEnable()
    {
        typeToHandle = typeof(ChannelData);
        DataHandler.Instance.RegisterAction(typeof(NicknameOperationData), HandleAlterationOnUpdate);
        base.OnEnable();
    }

    new private void OnDisable()
    {
        DataHandler.Instance.UnregisterAction(typeof(NicknameOperationData), HandleAlterationOnUpdate);
        base.OnDisable();
    }

    new private void Update()
    {
        base.Update();

        while (dataAlterationHandleQueue.Count > 0)
        {
            HandleAlteration(dataAlterationHandleQueue.Dequeue());
        }

        if (Input.GetKeyDown(KeyCode.Return) && _nicknameInput.isFocused)
        {
            ClickSendButton();
        }
    }

    override protected void HandleData(ReceivedState receivedState)
    {
        ChannelData messageData = (ChannelData)receivedState.ReceivedData;

        foreach (string nickname in messageData.UserNicknames)
        {
            Text nicknameText = Instantiate(userPrefab, userContainer);
            string textToPut;
            if (nickname.Equals(string.Empty))
            {
                textToPut = "Unknown user";
                nicknameText.color = _unknownUserColor;
            }
            else if (nickname.Equals(_userNickname))
            {
                textToPut = string.Format("<b>{0}</b> (You)", nickname);
            }
            else
            {
                textToPut = nickname;
            }

            nicknameText.text = textToPut;
            _nicknameList.Add(nicknameText);
        }
    }

    private void HandleAlteration(ReceivedState receivedState)
    {
        NicknameOperationData nicknameOperationData = (NicknameOperationData)receivedState.ReceivedData;

        Text textToAlter;

        if (nicknameOperationData.NicknameOperation == NicknameOperation.Add)
        {
            if (nicknameOperationData.NewNickname == string.Empty)
            {
                Text nicknameText = Instantiate(userPrefab, userContainer);
                nicknameText.text = "Unknown user";
                nicknameText.color = _unknownUserColor;

                _nicknameList.Add(nicknameText);
            }
            else
            {

                textToAlter = _nicknameList.Find((text) => text.text == "Unknown user");
                textToAlter.color = Color.black;
                textToAlter.text = nicknameOperationData.NewNickname;

                var systemMessage = Instantiate(systemMessagePrefab, messageContainer);
                systemMessage._timeText.text = nicknameOperationData.Time.ToLocalTime().ToLongTimeString();
                systemMessage._messageText.text = string.Format("{0} joined channel", nicknameOperationData.NewNickname);
            }

        }
        else if (nicknameOperationData.NicknameOperation == NicknameOperation.Remove)
        {
            textToAlter = _nicknameList.Find((text) => text.text == nicknameOperationData.OldNickname);
            _nicknameList.Remove(textToAlter);
            Destroy(textToAlter);

            var systemMessage = Instantiate(systemMessagePrefab, messageContainer);
            systemMessage._timeText.text = nicknameOperationData.Time.ToLocalTime().ToLongTimeString();
            systemMessage._messageText.text = string.Format("{0} left channel", nicknameOperationData.OldNickname);
        }
        else if (nicknameOperationData.NicknameOperation == NicknameOperation.Alter)
        {
            textToAlter = _nicknameList.Find((text) => text.text == nicknameOperationData.OldNickname);
            textToAlter.text = nicknameOperationData.NewNickname;

            var systemMessage = Instantiate(systemMessagePrefab, messageContainer);
            systemMessage._timeText.text = nicknameOperationData.Time.ToLocalTime().ToLongTimeString();
            systemMessage._messageText.text = string.Format("{0} changed their nickname to {1}", nicknameOperationData.OldNickname, nicknameOperationData.NewNickname);
        }
    }

    private void HandleAlterationOnUpdate(ReceivedState ReceivedState)
    {
        dataAlterationHandleQueue.Enqueue(ReceivedState);
    }

    public void ClickSendButton()
    {
        if (!_nicknameInput.text.Equals(string.Empty))
        {
            NicknameOperationData nicknameData = new NicknameOperationData();
            nicknameData.NewNickname = _nicknameInput.text;

            if (_userNickname == string.Empty)
            {
                nicknameData.NicknameOperation = NicknameOperation.Add;
            }
            else
            {
                nicknameData.NicknameOperation = NicknameOperation.Alter;
                nicknameData.OldNickname = _userNickname;
            } 
            
            ChatClient.I.SendMessageToServer(nicknameData);

            _userNickname = _nicknameInput.text;
        }
    }
}
