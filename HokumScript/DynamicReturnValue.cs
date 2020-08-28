using System;

namespace HokumScript
{
    public class DynamicReturnValue
    {
        private object _value;
        public object Value => _value;
        public readonly Type Type;

        public DynamicReturnValue(int value)
        {
            _value = value;
            Type = typeof(int);
        }
        
        public DynamicReturnValue(float value)
        {
            _value = value;
            Type = typeof(float);
        }
        
        public DynamicReturnValue(string value)
        {
            _value = value;
            Type = typeof(string);
        }
        
        public DynamicReturnValue(bool value)
        {
            _value = value;
            Type = typeof(bool);
        }
        
        public DynamicReturnValue(object value, Type type = null)
        {
            _value = value;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            return _value.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public T GetValue<T>()
        {
            return (T) _value;
        }

        public bool IsNull()
        {
            return _value == null;
        }

        public static bool operator ==(DynamicReturnValue obj1, DynamicReturnValue obj2)
        {
            if (obj1 == null || obj2 == null)
                return false;
            
            if (obj1.GetType() != obj2.GetType())
                return false;
            
            return obj1.Value == obj2.Value;
        }

        public static bool operator !=(DynamicReturnValue obj1, DynamicReturnValue obj2)
        {
            if (obj1 == null || obj2 == null)
                return false;
            
            if (obj1.GetType() != obj2.GetType())
                return false;
            
            return obj1.Value != obj2.Value;
        }
        
        public static bool operator >(DynamicReturnValue obj1, DynamicReturnValue obj2)
        {
            if (obj1.Value.GetType() != obj2.Value.GetType())
                return false;

            Type type = obj1.Value.GetType();
            if (type == typeof(float))
                return obj1.GetValue<float>() > obj2.GetValue<float>();
            
            if (type == typeof(int))
                return obj1.GetValue<int>() > obj2.GetValue<int>();

            return false;
        }

        public static bool operator <(DynamicReturnValue obj1, DynamicReturnValue obj2)
        {
            if (obj1.Value.GetType() != obj2.Value.GetType())
                return false;

            Type type = obj1.Value.GetType();
            if (type == typeof(float))
                return obj1.GetValue<float>() < obj2.GetValue<float>();
            if (type == typeof(int))
                return obj1.GetValue<int>() < obj2.GetValue<int>();

            return false;
        }
        
        public static bool operator >=(DynamicReturnValue obj1, DynamicReturnValue obj2)
        {
            if (obj1.Value.GetType() != obj2.Value.GetType())
                return false;

            Type type = obj1.Value.GetType();
            if (type == typeof(float))
                return obj1.GetValue<float>() >= obj2.GetValue<float>();
            
            if (type == typeof(int))
                return obj1.GetValue<int>() >= obj2.GetValue<int>();

            return false;
        }

        public static bool operator <=(DynamicReturnValue obj1, DynamicReturnValue obj2)
        {
            if (obj1.Value.GetType() != obj2.Value.GetType())
                return false;

            Type type = obj1.Value.GetType();
            if (type == typeof(float))
                return obj1.GetValue<float>() <= obj2.GetValue<float>();
            
            if (type == typeof(int))
                return obj1.GetValue<int>() <= obj2.GetValue<int>();

            return false;
        }
    }
}
