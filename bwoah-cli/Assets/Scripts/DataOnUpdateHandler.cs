using bwoah_shared;
using bwoah_shared.DataClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataOnUpdateHandler : MonoBehaviour
{
    protected Queue<ReceivedState> dataHandleQueue = new Queue<ReceivedState>();
    protected Type typeToHandle;

    protected void OnEnable()
    {
        DataHandler.Instance.RegisterAction(typeToHandle, HandleDataOnUpdate);
    }

    protected void OnDisable()
    {
        DataHandler.Instance.UnregisterAction(typeToHandle, HandleDataOnUpdate);
    }

    protected void Update ()
    {
        while (dataHandleQueue.Count > 0)
        {
            HandleData(dataHandleQueue.Dequeue());
        }
    }

    private void HandleDataOnUpdate(ReceivedState ReceivedState)
    {
        dataHandleQueue.Enqueue(ReceivedState);
    }

    virtual protected void HandleData(ReceivedState ReceivedState)
    {
        
    }
}
