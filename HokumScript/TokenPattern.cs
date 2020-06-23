using System.Text.RegularExpressions;

namespace HokumScript
{
    public class TokenPattern<T>
    {
        public readonly T Type;
        public readonly Regex Pattern;

        public TokenPattern(T type, string regex)
        {
            Type = type;
            Pattern = new Regex(regex);
        }
    }
}
