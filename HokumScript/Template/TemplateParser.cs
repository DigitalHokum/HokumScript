using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HokumScript.Template
{
    public class TemplateParser
    {
        public static readonly List<TokenPattern> TokenPatterns = new List<TokenPattern>();

        public static List<TemplateToken> Tokenize(string code)
        {
            RegisterPatterns();
            List<TemplateToken> tokens = new List<TemplateToken>();
            bool foundToken;
            do {
                foundToken = false;
                foreach (TokenPattern pattern in TokenPatterns)
                {
                    Match match = pattern.Pattern.Match(code);
                    if (match.Success)
                    {
                        string value = match.Value;
                        if (match.Groups["value"].Value != "")
                            value = match.Groups["value"].Value;

                        tokens.Add(new TemplateToken(pattern.Type, value));
                        code = code.Substring(match.Length);
                        foundToken = true;
                        break;
                    }
                }
            } while (code.Length > 0 && foundToken);

            return tokens;
        }

        private static void RegisterPatterns()
        {
            if (TokenPatterns.Count > 0)
                return;

            TokenPatterns.Add(new TokenPattern(ETemplateTokenType.WHITESPACE, @"^[\s\n\r]+"));
        }
    }
}
