using System;

namespace bwoah_shared.Utils
{
    /// <summary>
    /// Parent class for singleton design pattern
    /// </summary>
    /// <typeparam name="T">Type of the singleton</typeparam>
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static object _lockObj = new Object();

        /// <summary>
        /// Returns instance of the singleton
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (_lockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }

                    return _instance;
                }
            }
        }
    }
}
