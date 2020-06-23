using System.Text.RegularExpressions;

namespace HokumScript.Scripting
{
    public struct TokenPattern
    {
        public readonly ETokenType Type;
        public readonly Regex Pattern;

        public TokenPattern(ETokenType type, string regex)
        {
            Type = type;
            Pattern = new Regex(regex);
        }
    }

    public class Token
    {
        public readonly ETokenType Type;
        public readonly string Value;

        public Token(ETokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    public enum EBlockType {
        BRACE,
        BRACKET,
        PAREN
    }

    public enum ETokenType {
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