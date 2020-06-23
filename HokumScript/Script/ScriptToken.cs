namespace HokumScript.Script
{
    public class ScriptTokenPattern : TokenPattern<EScriptTokenType>
    {
        public ScriptTokenPattern(EScriptTokenType type, string regex) : base(type, regex)
        {
        }
    }

    public class ScriptToken : Token<EScriptTokenType>
    {
        public ScriptToken(EScriptTokenType type, string value) : base(type, value)
        {
            
        }
    }

    public enum EBlockType {
        BRACE,
        BRACKET,
        PAREN
    }

    public enum EScriptTokenType {
        WHITESPACE,
        RETURN,
        OF,
        FOR,
        IF,
        ELSE_IF,
        ELSE,
        NAME,
        L_BRACE,
        R_BRACE,
        L_BRACKET,
        R_BRACKET,
        L_PAREN,
        R_PAREN,
        PERIOD,
        COMMA,
        COLON,
        SEMI_COLON,
        STRING_LITERAL,
        NUMBER_LITERAL,
        BOOLEAN_LITERAL,
        NULL_LITERAL,
        EQUALS,
        NOT_EQUALS,
        GREATER_THAN,
        LESS_THAN,
        GREATER_THAN_EQUAL,
        LESS_THAN_EQUAL,
        ASSIGN,
        AND,
        OR,
        ADD,
        SUBTRACT,
        MULTIPLY,
        DIVIDE,
        ADD_ASSIGN,
        SUBTRACT_ASSIGN,
        MULTIPLY_ASSIGN,
        DIVIDE_ASSIGN
    }
}
