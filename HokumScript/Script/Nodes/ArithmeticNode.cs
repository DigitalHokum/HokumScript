using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HokumScript.Script.Nodes
{
    public class ArithmeticNode : AstTreeNode
    {
        public static readonly List<EScriptTokenType> ArithmeticTypes = new List<EScriptTokenType>
        {
            EScriptTokenType.ADD,
            EScriptTokenType.SUBTRACT,
            EScriptTokenType.MULTIPLY,
            EScriptTokenType.DIVIDE
        };

        public readonly AstTreeNode Left;
        public readonly AstTreeNode Right;
        public readonly EScriptTokenType Type;

        ArithmeticNode(
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

            Type leftType = left.Type;
            Type rightType = right.Type;

            if (leftType == typeof(int) && rightType == typeof(int))
            {
                switch (Type)
                {
                    case EScriptTokenType.ADD:
                        return new DynamicReturnValue(left.GetValue<int>() + right.GetValue<int>());
                    case EScriptTokenType.SUBTRACT:
                        return new DynamicReturnValue(left.GetValue<int>() - right.GetValue<int>());
                    case EScriptTokenType.MULTIPLY:
                        return new DynamicReturnValue(left.GetValue<int>() * right.GetValue<int>());
                    case EScriptTokenType.DIVIDE:
                        return new DynamicReturnValue(left.GetValue<int>() / right.GetValue<int>());
                }            
            }
            else
            {
                float leftValue = leftType == typeof(int)
                    ? left.GetValue<int>()
                    : left.GetValue<float>();
                float rightValue = rightType == typeof(int)
                    ? right.GetValue<int>()
                    : right.GetValue<float>();

                switch (Type)
                {
                    case EScriptTokenType.ADD:
                        return new DynamicReturnValue(leftValue + rightValue);
                    case EScriptTokenType.SUBTRACT:
                        return new DynamicReturnValue(leftValue - rightValue);
                    case EScriptTokenType.MULTIPLY:
                        return new DynamicReturnValue(leftValue * rightValue);
                    case EScriptTokenType.DIVIDE:
                        return new DynamicReturnValue(leftValue / rightValue);
                }
            }

            return new DynamicReturnValue(null);
        }

        public static bool Matches(List<ScriptToken> tokens) {
            return ArithmeticTypes.Contains(tokens[0].Type);
        }

        public static ArithmeticNode Parse(AstTreeNode lastNode, ScriptToken scriptToken, List<ScriptToken> tokens) {
            tokens.RemoveAt(0); // Remove arithmetic operator
            return new ArithmeticNode(lastNode, ScriptTree.ProcessTokens(ScriptTree.GetNextStatementTokens(tokens)), scriptToken.Type);
        }
    }
}
