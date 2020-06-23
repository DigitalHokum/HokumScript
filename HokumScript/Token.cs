namespace HokumScript
{
    public class Token<T>
    {
        public readonly T Type;
        public readonly string Value;

        public Token(T type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
