using System.Text.RegularExpressions;

namespace HokumScript.Template
{
    public struct TokenPattern
    {
        public readonly ETemplateTokenType Type;
        public readonly Regex Pattern;

        public TokenPattern(ETemplateTokenType type, string regex)
        {
            Type = type;
            Pattern = new Regex(regex);
        }
    }

    public class TemplateToken : Token<ETemplateTokenType>
    {
        public TemplateToken(ETemplateTokenType type, string value) : base(type, value)
        {
        }
    }

    public enum EBlockType {
        SCRIPT,
        TEXT
    }

    public enum ETemplateTokenType {
        WHITESPACE,
        START_SCRIPT,
        END_SCRIPT,
    }
}

