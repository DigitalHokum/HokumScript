using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class ArithmeticAssignmentNode : AstTreeNode
    {
        public readonly IScopeMemberNode Left;
        public readonly AstTreeNode Right;
        public readonly EScriptTokenType Type;

        public ArithmeticAssignmentNode(
            IScopeMemberNode left,
            AstTreeNode right,
            EScriptTokenType type
        )
        {
            Left = left;
            Right = right;
            Type = type;
        }

        public async Task<DynamicReturnValue> Evaluate(Scope scope) {
            string name = await Left.GetName(scope);
            DynamicReturnValue left = await Left.Evaluate(scope);
            DynamicReturnValue right = await Right.Evaluate(scope);
            float value = 0;

            switch (Type) {
                case EScriptTokenType.ADD_ASSIGN:
                    value = left.GetValue<float>() + right.GetValue<float>();
                    break;
                case EScriptTokenType.SUBTRACT_ASSIGN:
                    value = left.GetValue<float>() - right.GetValue<float>();
                    break;
                case EScriptTokenType.MULTIPLY_ASSIGN:
                    value = left.GetValue<float>() * right.GetValue<float>();
                    break;
                case EScriptTokenType.DIVIDE_ASSIGN:
                    value = left.GetValue<float>()/ right.GetValue<float>();
                    break;
            }

            Scope innerScope = await Left.GetScope(scope);
            innerScope.Set(name, value);

            if ((float) innerScope.Get(name) != value)
            {
                Console.WriteLine("System Error: Failed to assign ${name} to ${right}");
                return new DynamicReturnValue(null);
            }

            return new DynamicReturnValue(value);
        }

        public static bool Matches(List<ScriptToken> tokens) {
            List<EScriptTokenType> types = new List<EScriptTokenType>();
            types.Add(EScriptTokenType.ADD_ASSIGN);
            types.Add(EScriptTokenType.SUBTRACT_ASSIGN);
            types.Add(EScriptTokenType.MULTIPLY_ASSIGN);
            types.Add(EScriptTokenType.DIVIDE_ASSIGN);
            return types.Contains(tokens[0].Type);
        }

        public static ArithmeticAssignmentNode Parse(AstTreeNode lastNode, ScriptToken scriptToken, List<ScriptToken> tokens) {
            if (lastNode == null || !(lastNode is IScopeMemberNode)) {
                Console.WriteLine("Invalid assignment syntax.");
                return null;
            }
            tokens.RemoveAt(0); // consume +=
            List<ScriptToken> assignmentTokens = ScriptTree.GetNextStatementTokens(tokens);
            return new ArithmeticAssignmentNode(
                (IScopeMemberNode) Convert.ChangeType(lastNode, lastNode is RootScopeMemberNode ? typeof(RootScopeMemberNode) : typeof(ScopeMemberNode)),
                ScriptTree.ProcessTokens(assignmentTokens),
                scriptToken.Type
            );
        }
    }
}
