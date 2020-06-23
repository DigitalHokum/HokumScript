namespace HokumScript.Script
{
    public class ScriptParser : Parser<ScriptToken, EScriptTokenType>
    {
        public override void RegisterPatterns()
        {
            if (TokenPatterns.Count > 0)
                return;

            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.WHITESPACE, @"^[\s\n\r]+"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.BOOLEAN_LITERAL, @"^(true|false)"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.NULL_LITERAL, @"^null"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.RETURN, @"^return\s"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.OF, @"^of\s"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.FOR, @"^for\s"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.IF, @"^if\s"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.ELSE_IF, @"^else if\s"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.ELSE, @"^else\s"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.NAME, @"^[_a-zA-Z][_a-zA-Z0-9]*"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.NUMBER_LITERAL, @"^-?\d+(?:\.\d+)?(?:e[+\-]?\d+)?"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.L_BRACE, @"^{"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.R_BRACE, @"^}"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.L_BRACKET, @"^\["));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.R_BRACKET, @"^]"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.L_PAREN, @"^\("));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.R_PAREN, @"^\)"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.PERIOD, @"^\."));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.COMMA, @"^,"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.EQUALS, @"^=="));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.NOT_EQUALS, @"^!="));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.GREATER_THAN_EQUAL, @"^>="));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.LESS_THAN_EQUAL, @"^<="));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.GREATER_THAN, @"^>"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.LESS_THAN, @"^<"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.COLON, @"^:"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.SEMI_COLON, @"^;"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.STRING_LITERAL, "^\"(?<value>[^\"]*)\""));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.STRING_LITERAL, "^'(?<value>[^']*)'"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.AND, @"^&&"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.OR, @"^\|\|"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.ADD_ASSIGN, @"^\+="));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.SUBTRACT_ASSIGN, @"^-="));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.MULTIPLY_ASSIGN, @"^\*="));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.DIVIDE_ASSIGN, @"^\/="));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.ADD, @"^\+"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.SUBTRACT, @"^-"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.MULTIPLY, @"^\*"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.DIVIDE, @"^\/"));
            TokenPatterns.Add(new ScriptTokenPattern(EScriptTokenType.ASSIGN, @"^="));
        }
    }
}
