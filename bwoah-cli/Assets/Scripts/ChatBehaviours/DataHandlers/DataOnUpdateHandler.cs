using bwoah_shared;
using bwoah_shared.DataClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public abstract class DataOnUpdateHandler : MonoBehaviour
{
    protected Queue<AData> dataHandleQueue = new Queue<AData>();
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

    private void HandleDataOnUpdate(AData data, Socket socket)
    {
        Debug.Log("Received data asynchronically!");
        dataHandleQueue.Enqueue(data);
    }

    virtual protected void HandleData(AData data)
    {
        
    }
}
