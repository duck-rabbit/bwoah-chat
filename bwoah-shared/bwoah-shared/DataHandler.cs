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
        ConcurrentDictionary<Type, Action<RecievedState>> _typeActionPairs = new ConcurrentDictionary<Type, Action<RecievedState>>();

        public void RegisterAction(Type dataType, Action<RecievedState> action)
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

        public void UnregisterAction(Type dataType, Action<RecievedState> action)
        {
            if (_typeActionPairs.ContainsKey(dataType))
            {
                _typeActionPairs[dataType] -= action;
            }
        }

        public void HandleData(RecievedState recievedState)
        {
            Type dataType = recievedState.RecievedData.GetType();
            if (_typeActionPairs.ContainsKey(dataType))
            {
                _typeActionPairs[dataType]?.Invoke(recievedState);
            }
        }
    }
}
