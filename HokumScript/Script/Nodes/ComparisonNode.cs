using System.Collections.Generic;
using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    class ComparisonNode : AstTreeNode
    {
        public readonly AstTreeNode Left;
        public readonly AstTreeNode Right;
        public readonly EScriptTokenType Type;

        public ComparisonNode(
            AstTreeNode left,
            AstTreeNode right,
            EScriptTokenType type
        )
        {
            Left = left;
            Right = right;
            Type = type;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            DynamicReturnValue left = await Left.Evaluate(scope);
            DynamicReturnValue right = await Right.Evaluate(scope);

            switch (Type) {
                case EScriptTokenType.EQUALS:
                    return new DynamicReturnValue(left == right);
                case EScriptTokenType.NOT_EQUALS:
                    return new DynamicReturnValue(left != right);
                case EScriptTokenType.GREATER_THAN:
                    return new DynamicReturnValue(left > right);
                case EScriptTokenType.LESS_THAN:
                    return new DynamicReturnValue(left < right);
                case EScriptTokenType.GREATER_THAN_EQUAL:
                    return new DynamicReturnValue(left >= right);
                case EScriptTokenType.LESS_THAN_EQUAL:
                    return new DynamicReturnValue(left <= right);
            }

            return new DynamicReturnValue(false);
        }

        public static bool Matches(List<ScriptToken> tokens) {
            List<EScriptTokenType> types = new List<EScriptTokenType>();
            types.Add(EScriptTokenType.EQUALS);
            types.Add(EScriptTokenType.NOT_EQUALS);
            types.Add(EScriptTokenType.GREATER_THAN);
            types.Add(EScriptTokenType.LESS_THAN);
            types.Add(EScriptTokenType.GREATER_THAN_EQUAL);
            types.Add(EScriptTokenType.LESS_THAN_EQUAL);

            return types.Contains(tokens[0].Type);
        }

        public static ComparisonNode Parse(AstTreeNode lastNode, ScriptToken scriptToken, List<ScriptToken> tokens) {
            tokens.RemoveAt(0); // Remove comparison operator
            return new ComparisonNode(lastNode, ScriptTree.ProcessTokens(ScriptTree.GetNextStatementTokens(tokens)), scriptToken.Type);
        }
    }
}
