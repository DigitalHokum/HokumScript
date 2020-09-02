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
            Scope innerScope = await Left.GetScope(scope);
            
            DynamicReturnValue left = await Left.Evaluate(scope);
            DynamicReturnValue right = await Right.Evaluate(scope);
            
            Type leftType = left.Type;
            Type rightType = right.Type;
            
            if (leftType == typeof(int) && rightType == typeof(int))
            {
                int value = default;
                switch (Type) {
                    case EScriptTokenType.ADD_ASSIGN:
                        value = left.GetValue<int>() + right.GetValue<int>();
                        break;
                    case EScriptTokenType.SUBTRACT_ASSIGN:
                        value = left.GetValue<int>() - right.GetValue<int>();
                        break;
                    case EScriptTokenType.MULTIPLY_ASSIGN:
                        value = left.GetValue<int>() * right.GetValue<int>();
                        break;
                    case EScriptTokenType.DIVIDE_ASSIGN:
                        value = left.GetValue<int>() / right.GetValue<int>();
                        break;
                }

                innerScope.Set(name, value);
                if ((int) innerScope.Get(name) != value)
                {
                    Console.WriteLine("System Error: Failed to assign ${name} to ${right}");
                    return new DynamicReturnValue(null);
                }
                return new DynamicReturnValue(value);
            }
            else
            {
                float value = default;
                
                float leftValue = leftType == typeof(int)
                    ? left.GetValue<int>()
                    : left.GetValue<float>();
                float rightValue = rightType == typeof(int)
                    ? right.GetValue<int>()
                    : right.GetValue<float>();
                
                switch (Type) {
                    case EScriptTokenType.ADD_ASSIGN:
                        value = leftValue + rightValue;
                        break;
                    case EScriptTokenType.SUBTRACT_ASSIGN:
                        value = leftValue - rightValue;
                        break;
                    case EScriptTokenType.MULTIPLY_ASSIGN:
                        value = leftValue * rightValue;
                        break;
                    case EScriptTokenType.DIVIDE_ASSIGN:
                        value = leftValue / rightValue;
                        break;
                }
                
                innerScope.Set(name, value);
                if ((float) innerScope.Get(name) != value)
                {
                    Console.WriteLine("System Error: Failed to assign ${name} to ${right}");
                    return new DynamicReturnValue(null);
                }
                return new DynamicReturnValue(value);
            }
        }

        public static bool Matches(List<ScriptToken> tokens) {
            List<EScriptTokenType> types = new List<EScriptTokenType>
            {
                EScriptTokenType.ADD_ASSIGN,
                EScriptTokenType.SUBTRACT_ASSIGN,
                EScriptTokenType.MULTIPLY_ASSIGN,
                EScriptTokenType.DIVIDE_ASSIGN
            };
            return types.Contains(tokens[0].Type);
        }

        public static ArithmeticAssignmentNode Parse(AstTreeNode lastNode, ScriptToken scriptToken, List<ScriptToken> tokens) {
            if (lastNode == null || !(lastNode is IScopeMemberNode)) {
                Console.WriteLine("Invalid assignment syntax.");
                return null;
            }
            tokens.RemoveAt(0); // consume +=
            List<ScriptToken> assignmentTokens = ScriptTree.GetEnclosedTokens(tokens);
            return new ArithmeticAssignmentNode(
                (IScopeMemberNode) Convert.ChangeType(lastNode, lastNode is RootScopeMemberNode ? typeof(RootScopeMemberNode) : typeof(ScopeMemberNode)),
                ScriptTree.ProcessTokens(assignmentTokens),
                scriptToken.Type
            );
        }
    }
}
