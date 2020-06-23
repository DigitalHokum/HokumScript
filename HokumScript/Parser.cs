using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HokumScript
{
    public abstract class Parser<T, E> where T : Token<E>
    {
        public readonly List<TokenPattern<E>> TokenPatterns = new List<TokenPattern<E>>();
        
        public List<T> Tokenize(string code)
        {
            RegisterPatterns();
            List<T> tokens = new List<T>();
            bool foundToken;
            do {
                foundToken = false;
                foreach (TokenPattern<E> pattern in TokenPatterns)
                {
                    Match match = pattern.Pattern.Match(code);
                    if (match.Success)
                    {
                        string value = match.Value;
                        if (match.Groups["value"].Value != "")
                            value = match.Groups["value"].Value;

                        Type type = typeof(T);
                        tokens.Add((T) Activator.CreateInstance(type, pattern.Type, value));
                        code = code.Substring(match.Length);
                        foundToken = true;
                        break;
                    }
                }
            } while (code.Length > 0 && foundToken);

            return tokens;
        }

        public abstract void RegisterPatterns();
    }
}
