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
    /*
    public class Scope
    {
        private Dictionary<string, Types> _values = new Dictionary<string, Types>();
        private Dictionary<string, Scope> _scopeData = new Dictionary<string, Scope>();
        private Dictionary<string, bool> _booleanData = new Dictionary<string, bool>();
        private Dictionary<string, string> _stringData = new Dictionary<string, string>();
        private Dictionary<string, float> _numberData = new Dictionary<string, float>();
        private Dictionary<string, Function> _functionData = new Dictionary<string, Function>();

        public void Unset(string key)
        {
            if (_values.ContainsKey(key))
            {
                Types type = _values[key];
                _values.Remove(key);
                switch (type)
                {
                    case Types.Scope:
                        _scopeData.Remove(key);
                        break;
                    case Types.Boolean:
                        _booleanData.Remove(key);
                        break;
                    case Types.Number:
                        _numberData.Remove(key);
                        break;
                    case Types.Function:
                        _functionData.Remove(key);
                        break;
                    case Types.String:
                    default:
                        _stringData.Remove(key);
                        break;
                }
            }
        }

        public Types GetType(string key)
        {
            if (_values.ContainsKey(key))
                return _values[key];

            return Types.None;
        }
        
        public T Get<T>(string key)
        {
            if (!_values.ContainsKey(key))
                return default(T);
            
            Type type = typeof(T);
            if (type == typeof(Scope))
                return _scopeData[key];
            
            if (type == typeof(bool))
                    return _booleanData[key];
        
            if (type == typeof(float))
                return _numberData[key];
            
            if (type == typeof(Function))
                return _functionData[key];
        
            return _stringData[key];
        }

        public dynamic Get(string key)
        {
            if (!_values.ContainsKey(key))
                return null;
            
            Types type = _values[key];
            switch (type)
            {
                case Types.Scope:
                    return _scopeData[key];
                case Types.Boolean:
                    return _booleanData[key];
                case Types.Number:
                    return _numberData[key];
                case Types.Function:
                    return _functionData[key];
                case Types.String:
                default:
                    return _stringData[key];
            }
        }

        public Scope GetScope(string key)
        {
            Types type = _values[key];
            if (type != Types.Scope)
                return null;
            return _scopeData[key];
        }
        
        public void Set(string key, Scope value)
        {
            Unset(key);
            _values.Add(key, Types.Scope);
            _scopeData.Add(key, value);
        }

        public void Set(string key, bool value)
        {
            Unset(key);
            _values.Add(key, Types.Boolean);
            _booleanData.Add(key, value);
        }

        public void Set(string key, string value)
        {
            Unset(key);
            _values.Add(key, Types.String);
            _stringData.Add(key, value);
        }
        
        public void Set(string key, float value)
        {
            Unset(key);
            _values.Add(key, Types.Number);
            _numberData.Add(key, value);
        }
        
        public void Set(string key, int value)
        {
            Unset(key);
            _values.Add(key, Types.Number);
            _numberData.Add(key, value);
        }
        
        public void Set(string key, Function value)
        {
            Unset(key);
            _values.Add(key, Types.Function);
            _functionData.Add(key, value);
        }
    }
    */
}
