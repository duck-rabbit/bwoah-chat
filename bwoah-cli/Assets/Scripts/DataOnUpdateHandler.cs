using bwoah_shared;
using bwoah_shared.DataClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataOnUpdateHandler : MonoBehaviour
{
    protected Queue<RecievedState> dataHandleQueue = new Queue<RecievedState>();
    protected Type typeToHandle;

    protected void OnEnable()
    {
        DataHandler.Instance.RegisterAction(typeToHandle, HandleDataOnUpdate);
    }

    protected void OnDisable()
    {
        DataHandler.Instance.UnregisterAction(typeToHandle, HandleDataOnUpdate);
    }

    // Update is called once per frame
    protected void Update ()
    {
        while (dataHandleQueue.Count > 0)
        {
            HandleData(dataHandleQueue.Dequeue());
        }
    }

    private void HandleDataOnUpdate(RecievedState recievedState)
    {
        dataHandleQueue.Enqueue(recievedState);
    }

    virtual protected void HandleData(RecievedState recievedState)
    {
        
    }
}
