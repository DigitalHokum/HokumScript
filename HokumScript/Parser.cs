using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HokumScript
{
    public class Parser
    {
        public static readonly List<TokenPattern> TokenPatterns = new List<TokenPattern>();

        public static List<Token> Tokenize(string code)
        {
            RegisterPatterns();
            List<Token> tokens = new List<Token>();
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

                        tokens.Add(new Token(pattern.Type, value));
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

            TokenPatterns.Add(new TokenPattern(ETokenType.WHITESPACE, @"^[\s\n\r]+"));
            TokenPatterns.Add(new TokenPattern(ETokenType.BOOLEAN_LITERAL, @"^(true|false)"));
            TokenPatterns.Add(new TokenPattern(ETokenType.NULL_LITERAL, @"^null"));
            TokenPatterns.Add(new TokenPattern(ETokenType.RETURN, @"^return\s"));
            TokenPatterns.Add(new TokenPattern(ETokenType.OF, @"^of\s"));
            TokenPatterns.Add(new TokenPattern(ETokenType.FOR, @"^for\s"));
            TokenPatterns.Add(new TokenPattern(ETokenType.IF, @"^if\s"));
            TokenPatterns.Add(new TokenPattern(ETokenType.ELSE_IF, @"^else if\s"));
            TokenPatterns.Add(new TokenPattern(ETokenType.ELSE, @"^else\s"));
            TokenPatterns.Add(new TokenPattern(ETokenType.NAME, @"^[_a-zA-Z][_a-zA-Z0-9]*"));
            TokenPatterns.Add(new TokenPattern(ETokenType.NUMBER_LITERAL, @"^-?\d+(?:\.\d+)?(?:e[+\-]?\d+)?"));
            TokenPatterns.Add(new TokenPattern(ETokenType.L_BRACE, @"^{"));
            TokenPatterns.Add(new TokenPattern(ETokenType.R_BRACE, @"^}"));
            TokenPatterns.Add(new TokenPattern(ETokenType.L_BRACKET, @"^\["));
            TokenPatterns.Add(new TokenPattern(ETokenType.R_BRACKET, @"^]"));
            TokenPatterns.Add(new TokenPattern(ETokenType.L_PAREN, @"^\("));
            TokenPatterns.Add(new TokenPattern(ETokenType.R_PAREN, @"^\)"));
            TokenPatterns.Add(new TokenPattern(ETokenType.PERIOD, @"^\."));
            TokenPatterns.Add(new TokenPattern(ETokenType.COMMA, @"^,"));
            TokenPatterns.Add(new TokenPattern(ETokenType.EQUALS, @"^=="));
            TokenPatterns.Add(new TokenPattern(ETokenType.NOT_EQUALS, @"^!="));
            TokenPatterns.Add(new TokenPattern(ETokenType.GREATER_THAN_EQUAL, @"^>="));
            TokenPatterns.Add(new TokenPattern(ETokenType.LESS_THAN_EQUAL, @"^<="));
            TokenPatterns.Add(new TokenPattern(ETokenType.GREATER_THAN, @"^>"));
            TokenPatterns.Add(new TokenPattern(ETokenType.LESS_THAN, @"^<"));
            TokenPatterns.Add(new TokenPattern(ETokenType.COLON, @"^:"));
            TokenPatterns.Add(new TokenPattern(ETokenType.SEMI_COLON, @"^;"));
            TokenPatterns.Add(new TokenPattern(ETokenType.STRING_LITERAL, "^\"(?<value>[^\"]*)\""));
            TokenPatterns.Add(new TokenPattern(ETokenType.STRING_LITERAL, "^'(?<value>[^']*)'"));
            TokenPatterns.Add(new TokenPattern(ETokenType.AND, @"^&&"));
            TokenPatterns.Add(new TokenPattern(ETokenType.OR, @"^\|\|"));
            TokenPatterns.Add(new TokenPattern(ETokenType.ADD_ASSIGN, @"^\+="));
            TokenPatterns.Add(new TokenPattern(ETokenType.SUBTRACT_ASSIGN, @"^-="));
            TokenPatterns.Add(new TokenPattern(ETokenType.MULTIPLY_ASSIGN, @"^\*="));
            TokenPatterns.Add(new TokenPattern(ETokenType.DIVIDE_ASSIGN, @"^\/="));
            TokenPatterns.Add(new TokenPattern(ETokenType.ADD, @"^\+"));
            TokenPatterns.Add(new TokenPattern(ETokenType.SUBTRACT, @"^-"));
            TokenPatterns.Add(new TokenPattern(ETokenType.MULTIPLY, @"^\*"));
            TokenPatterns.Add(new TokenPattern(ETokenType.DIVIDE, @"^\/"));
            TokenPatterns.Add(new TokenPattern(ETokenType.ASSIGN, @"^="));
        }
    }
}
