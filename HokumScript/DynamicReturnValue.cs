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
    
    /*
    public class DynamicReturnValue
    {
        protected byte[] _value;
        public readonly Types Type;

        public DynamicReturnValue()
        {
            Type = Types.None;
        }
        
        public DynamicReturnValue(bool value)
        {
            Type = Types.Number;
            _value = BitConverter.GetBytes(value);
        }
        
        public DynamicReturnValue(int value)
        {
            Type = Types.Number;
            _value = BitConverter.GetBytes((float) value);
        }
        
        public DynamicReturnValue(float value)
        {
            Type = Types.Number;
            _value = BitConverter.GetBytes(value);
        }
        
        public DynamicReturnValue(double value)
        {
            Type = Types.Number;
            _value = BitConverter.GetBytes((float) value);
        }
        
        public DynamicReturnValue(string value)
        {
            Type = Types.String;
            _value = Encoding.Unicode.GetBytes(value);
        }
        
        public DynamicReturnValue(object value)
        {
            Type = Types.None;
            _value = null;
            Type t = typeof(value);
        }

        public bool IsNull()
        {
            return _value == null;
        }

        public T GetValue<T>()
        {
            Type type = typeof(T);
            if (type == typeof(float))
            {
                return (T) Convert.ChangeType(BitConverter.ToSingle(_value, 0), type);
            }
            if (type == typeof(bool))
            {
                return (T) Convert.ChangeType(BitConverter.ToBoolean(_value, 0), type);
            }
            else // (type == typeof(string))
            {
                return (T) Convert.ChangeType(Encoding.Unicode.GetString(_value), type);;
            }
                
        }
        
        public static bool operator ==(DynamicReturnValue obj1, DynamicReturnValue obj2)
        {
            if (obj1.Type != obj2.Type)
                return false;

            if (obj1.Type == Types.Number)
                return obj1.GetValue<float>() == obj2.GetValue<float>();
            
            if (obj1.Type == Types.Boolean)
                return obj1.GetValue<bool>() == obj2.GetValue<bool>();

            if (obj1.Type == Types.String)
                return obj1.GetValue<string>() == obj2.GetValue<string>();
            
            return false;
        }

        public static bool operator !=(DynamicReturnValue obj1, DynamicReturnValue obj2)
        {
            if (obj1.Type != obj2.Type)
                return false;

            if (obj1.Type == Types.Number)
                return obj1.GetValue<float>() != obj2.GetValue<float>();
            
            if (obj1.Type == Types.Boolean)
                return obj1.GetValue<bool>() != obj2.GetValue<bool>();

            if (obj1.Type == Types.String)
                return obj1.GetValue<string>() != obj2.GetValue<string>();

            return false;
        }
        
        public static bool operator >(DynamicReturnValue obj1, DynamicReturnValue obj2)
        {
            if (obj1.Type != obj2.Type)
                return false;

            if (obj1.Type == Types.Number)
                return obj1.GetValue<float>() > obj2.GetValue<float>();

            return false;
        }

        public static bool operator <(DynamicReturnValue obj1, DynamicReturnValue obj2)
        {
            if (obj1.Type != obj2.Type)
                return false;

            if (obj1.Type == Types.Number)
                return obj1.GetValue<float>() < obj2.GetValue<float>();

            return false;
        }
        
        public static bool operator >=(DynamicReturnValue obj1, DynamicReturnValue obj2)
        {
            if (obj1.Type != obj2.Type)
                return false;

            if (obj1.Type == Types.Number)
                return obj1.GetValue<float>() >= obj2.GetValue<float>();

            return false;
        }

        public static bool operator <=(DynamicReturnValue obj1, DynamicReturnValue obj2)
        {
            if (obj1.Type != obj2.Type)
                return false;

            if (obj1.Type == Types.Number)
                return obj1.GetValue<float>() <= obj2.GetValue<float>();

            return false;
        }
    }
    */
}
