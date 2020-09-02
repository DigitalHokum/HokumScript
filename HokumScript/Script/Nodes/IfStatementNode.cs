using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class IfStatementNode : AstTreeNode
    {
        protected List<ConditionalNode> Nodes;

        public IfStatementNode(
            List<ConditionalNode> nodes
        )
        {
            Nodes = nodes;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            foreach (ConditionalNode condition in Nodes)
            {
                DynamicReturnValue uno = await condition.Condition.Evaluate(scope);
                if (uno.Value != null && (bool) uno.Value) {
                    DynamicReturnValue dose = await condition.Block.Evaluate(scope);
                    return dose;
                }
            }

            return new DynamicReturnValue(null);
        }

        public static ConditionalNode ParseConditional(List<ScriptToken> tokens) {
            List<EScriptTokenType> ifTokens = new List<EScriptTokenType>();
            ifTokens.Add(EScriptTokenType.IF);
            ifTokens.Add(EScriptTokenType.ELSE_IF);

            if (!ifTokens.Contains(tokens[0].Type))  {
                Console.WriteLine("Invalid Syntax");
                return null;
            }

            tokens.RemoveAt(0); // consume if and else if
            return new ConditionalNode(
                ScriptTree.ProcessTokens(ScriptTree.GetBlockTokens(tokens, EBlockType.PAREN, false)[0]),
                ScriptTree.ProcessTokens(ScriptTree.GetBlockTokens(tokens, EBlockType.BRACE, false)[0])
            );
        }

        public static IfStatementNode Parse(AstTreeNode lastNode, ScriptToken scriptToken, List<ScriptToken> tokens) {
            if (tokens[1].Type != EScriptTokenType.L_PAREN) {
                Console.WriteLine("If statement needs to be followed by a condition encased in parenthesis.");
                return null;
            }
            List<ConditionalNode> nodes = new List<ConditionalNode>(); 
            nodes.Add(ParseConditional(tokens));

            while(tokens.Count > 0 && EScriptTokenType.ELSE_IF == tokens[0].Type) {
                nodes.Add(ParseConditional(tokens));
            }

            if (tokens.Count > 0 && EScriptTokenType.ELSE == tokens[0].Type) {
                tokens.RemoveAt(0); // Consume else
                nodes.Add(new ConditionalNode(
                    new LiteralNode<bool>(true),
                    ScriptTree.ProcessTokens(ScriptTree.GetBlockTokens(tokens, EBlockType.BRACE, false)[0])
                ));
            }

            return new IfStatementNode(nodes);
        }
    }
}
