using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatUser : UnitySingleton<ChatUser>
{
    public string nickname = string.Empty;
    public Dictionary<int, ChatRoom> chatRooms = new Dictionary<int, ChatRoom>();

    private void Start()
    {
           
    }
}
