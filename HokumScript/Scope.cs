using System;
using System.Collections.Generic;

namespace HokumScript
{
    public class Scope
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();
        private Dictionary<string, Type> types = new Dictionary<string, Type>();

        public void Unset(string key)
        {
            lock (data)
            {
                data.Remove(key);
            }
        }

        public void Set(string key, object value)
        {
            lock (data)
            {
                if (data.ContainsKey(key))
                    Unset(key);
                data.Add(key, value);
            }
        }

        public object Get(string key)
        {
            lock (data)
            {
                return !data.ContainsKey(key) ? null : data[key];
            }
        }

        public T Get<T>(string key)
        {
            lock (data)
            {
                return !data.ContainsKey(key) ? default(T) : (T) data[key];
            }
        }

        public Type GetType(string key)
        {
            lock (data)
            {
                return !data.ContainsKey(key) ? null : data[key].GetType();
            }
        }
    }
}
