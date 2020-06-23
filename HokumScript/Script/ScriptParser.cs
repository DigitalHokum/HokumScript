using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HokumScript.Script
{
    public class ScriptParser
    {
        public static readonly List<TokenPattern> TokenPatterns = new List<TokenPattern>();

        public static List<ScriptToken> Tokenize(string code)
        {
            RegisterPatterns();
            List<ScriptToken> tokens = new List<ScriptToken>();
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

                        tokens.Add(new ScriptToken(pattern.Type, value));
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

            TokenPatterns.Add(new TokenPattern(EScriptTokenType.WHITESPACE, @"^[\s\n\r]+"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.BOOLEAN_LITERAL, @"^(true|false)"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.NULL_LITERAL, @"^null"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.RETURN, @"^return\s"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.OF, @"^of\s"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.FOR, @"^for\s"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.IF, @"^if\s"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.ELSE_IF, @"^else if\s"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.ELSE, @"^else\s"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.NAME, @"^[_a-zA-Z][_a-zA-Z0-9]*"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.NUMBER_LITERAL, @"^-?\d+(?:\.\d+)?(?:e[+\-]?\d+)?"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.L_BRACE, @"^{"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.R_BRACE, @"^}"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.L_BRACKET, @"^\["));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.R_BRACKET, @"^]"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.L_PAREN, @"^\("));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.R_PAREN, @"^\)"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.PERIOD, @"^\."));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.COMMA, @"^,"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.EQUALS, @"^=="));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.NOT_EQUALS, @"^!="));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.GREATER_THAN_EQUAL, @"^>="));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.LESS_THAN_EQUAL, @"^<="));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.GREATER_THAN, @"^>"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.LESS_THAN, @"^<"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.COLON, @"^:"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.SEMI_COLON, @"^;"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.STRING_LITERAL, "^\"(?<value>[^\"]*)\""));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.STRING_LITERAL, "^'(?<value>[^']*)'"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.AND, @"^&&"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.OR, @"^\|\|"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.ADD_ASSIGN, @"^\+="));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.SUBTRACT_ASSIGN, @"^-="));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.MULTIPLY_ASSIGN, @"^\*="));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.DIVIDE_ASSIGN, @"^\/="));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.ADD, @"^\+"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.SUBTRACT, @"^-"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.MULTIPLY, @"^\*"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.DIVIDE, @"^\/"));
            TokenPatterns.Add(new TokenPattern(EScriptTokenType.ASSIGN, @"^="));
        }
    }
}
