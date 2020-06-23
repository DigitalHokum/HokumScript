using System.Text.RegularExpressions;

namespace HokumScript.Template
{
    public class TemplateTokenPattern : TokenPattern<ETemplateTokenType>
    {
        public TemplateTokenPattern(ETemplateTokenType type, string regex) : base(type, regex)
        {
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
