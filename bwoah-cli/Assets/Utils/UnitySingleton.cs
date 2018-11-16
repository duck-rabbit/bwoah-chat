using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T i;
    private static object lockObj = new Object();
    private static bool appIsQuitting = false;

    public static T I
    {
        get
        {
            if (appIsQuitting)
            {
                Debug.LogWarning("[UnitySingleton] Instance '" + typeof(T) +
                "' already destroyed on application quit." +
                " Won't create again - returning null.");
                return null;
            }

            lock (lockObj)
            {
                if (i == null)
                {
                    i = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[UnitySingleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the scene might fix it.");
                        return i;
                    }

                    if (i == null)
                    {
                        GameObject singleton = new GameObject();
                        i = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString();

                        Debug.Log("[UnitySingleton] An instance of " + typeof(T) +
                            " is needed in the scene, so '" + singleton +
                            "' was created.");
                    }
                }

                return i;
            }
        }
    }

    private void OnApplicationQuit()
    {
        appIsQuitting = true;
    }
}
