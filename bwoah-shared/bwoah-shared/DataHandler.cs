using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using bwoah_shared.DataClasses;
using bwoah_shared.Utils;

namespace bwoah_shared
{
    public class DataHandler : Singleton<DataHandler>
    {
        ConcurrentDictionary<Type, Action<ReceivedState>> _typeActionPairs = new ConcurrentDictionary<Type, Action<ReceivedState>>();

        public void RegisterAction(Type dataType, Action<ReceivedState> action)
        {
            if (!_typeActionPairs.ContainsKey(dataType))
            {
                _typeActionPairs.AddOrUpdate(dataType, action, (key, value) => _typeActionPairs[dataType]);
            }
            else
            {
                _typeActionPairs[dataType] += action;
            }
        }

        public void UnregisterAction(Type dataType, Action<ReceivedState> action)
        {
            if (_typeActionPairs.ContainsKey(dataType))
            {
                _typeActionPairs[dataType] -= action;
            }
        }

        public void HandleData(ReceivedState recievedState)
        {
            Type dataType = recievedState.ReceivedData.GetType();
            if (_typeActionPairs.ContainsKey(dataType))
            {
                _typeActionPairs[dataType]?.Invoke(recievedState);
            }
        }
    }
}
