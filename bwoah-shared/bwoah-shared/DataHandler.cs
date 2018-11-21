using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using bwoah_shared.DataClasses;
using bwoah_shared.Utils;

namespace bwoah_shared
{
    public class DataHandler : Singleton<DataHandler>
    {
        ConcurrentDictionary<Type, Action<AData, Socket>> _typeActionPairs = new ConcurrentDictionary<Type, Action<AData, Socket>>();

        public void RegisterAction(Type dataType, Action<AData, Socket> action)
        {
            if (!_typeActionPairs.ContainsKey(dataType))
            {
                _typeActionPairs.TryAdd(dataType, action);
            }
            else
            {
                _typeActionPairs[dataType] += action;
            }
        }

        public void UnregisterAction(Type dataType, Action<AData, Socket> action)
        {
            if (_typeActionPairs.ContainsKey(dataType))
            {
                _typeActionPairs[dataType] -= action;
            }
        }

        public void HandleData(AData data, Socket socket)
        {
            Type dataType = data.GetType();
            if (_typeActionPairs.ContainsKey(dataType))
            {
                _typeActionPairs[dataType]?.Invoke(data, socket);
            }
        }
    }
}
