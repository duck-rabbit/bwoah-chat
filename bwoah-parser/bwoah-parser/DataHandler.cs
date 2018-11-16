using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using bwoah_parser.DataClasses;
using bwoah_parser.Utils;

namespace bwoah_parser
{
    class DataHandler : Singleton<DataHandler>
    {
        ConcurrentDictionary<Type, Action<IData>> _typeActionPairs = new ConcurrentDictionary<Type, Action<IData>>();

        public void RegisterAction(Type dataType, Action<IData> action)
        {
            if (!_typeActionPairs.ContainsKey(dataType))
            {
                _typeActionPairs.AddOrUpdate(dataType, action, (key, value) => _typeActionPairs[dataType]);
            }
            _typeActionPairs[dataType] += action;
        }

        public void UnregisterAction(Type dataType, Action<IData> action)
        {
            if (_typeActionPairs.ContainsKey(dataType))
            {
                _typeActionPairs[dataType] -= action;
            }
        }

        public void HandleData(IData data)
        {
            Type dataType = data.GetType();
            if (_typeActionPairs.ContainsKey(dataType))
            {
                _typeActionPairs[dataType]?.Invoke(data);
            }
        }
    }
}
