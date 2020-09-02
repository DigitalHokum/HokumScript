using System.Collections.Generic;
using System.Threading.Tasks;
using HokumScript.Script;
using Xunit;
using Xunit.Abstractions;

namespace HokumScript.Test.Script.Nodes
{
    public class ScriptTreeTests
    {
        private readonly ITestOutputHelper output;

        public ScriptTreeTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task TestGetEnclosedStatementTokens()
        {
            ScriptParser parser = new ScriptParser();
            List<ScriptToken> tokens = parser.Tokenize("([1,2] + {1,2,3})");
            
            List<ScriptToken> enclosedTokens = ScriptTree.GetEnclosedTokens(tokens);
            Assert.Equal(EScriptTokenType.L_BRACKET, enclosedTokens[0].Type);
            Assert.Equal(EScriptTokenType.R_BRACKET, enclosedTokens[4].Type);
            
            Assert.Equal(EScriptTokenType.ADD, enclosedTokens[6].Type);

            Assert.Equal(EScriptTokenType.L_BRACE, enclosedTokens[8].Type);
            Assert.Equal(EScriptTokenType.R_BRACE, enclosedTokens[14].Type);
        }
    }
}
