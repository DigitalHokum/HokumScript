using System;
using System.Collections.Generic;

namespace HokumScript
{
    public class Scope
    {
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public void Unset(string key)
        {
            data.Remove(key);
        }

        public void Set(string key, object value)
        {
            if (data.ContainsKey(key))
                data.Remove(key);
            data.Add(key, value);
        }

        public object Get(string key)
        {
            if (!data.ContainsKey(key))
                return null;
            return data[key];
        }

        public Type GetType(string key)
        {
            if (!data.ContainsKey(key))
                return null;

            return data[key].GetType();
        }
    }
}
