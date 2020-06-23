namespace HokumScript.Template
{
    public class TemplateParser : Parser<TemplateToken, ETemplateTokenType>
    {
        

        public override void RegisterPatterns()
        {
            if (TokenPatterns.Count > 0)
                return;

            TokenPatterns.Add(new TemplateTokenPattern(ETemplateTokenType.WHITESPACE, @"^[\s\n\r]+"));
        }
    }
}
